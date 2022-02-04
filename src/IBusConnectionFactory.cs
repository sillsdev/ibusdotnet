using System;
using System.IO;
using NDesk.DBus;
using System.Diagnostics;

namespace IBusDotNet
{
	// ReSharper disable once InconsistentNaming
	/// <summary>
	/// static class that allow creation of DBus connection to IBus's session DBus.
	/// </summary>
	public static class IBusConnectionFactory
	{
		// ReSharper disable InconsistentNaming
		private const string ENV_IBUS_ADDRESS = "IBUS_ADDRESS";
		private const string IBUS_ADDRESS = "IBUS_ADDRESS";
		// ReSharper restore InconsistentNaming

		private static IBusConnection singleConnection;

		/// <summary>
		/// Gets the local machine identifier.
		/// </summary>
		private static string LocalMachineId
		{
			get
			{
				// The path to the machine-id file is hardcoded in ibus sources as
				// /etc/machine-id or /var/lib/dbus/machine-id (src/ibusshare.c).
				// /etc/machine-id is in flatpak.
				using (var machineIdFile = new StreamReader("/etc/machine-id"))
				{
					return machineIdFile.ReadToEnd().TrimEnd('\n');
				}
			}
		}

		/// <summary>
		/// Gets the display number and the hostname (if any) from the DISPLAY environment
		/// variable.
		/// </summary>
		internal static int GetDisplayNumber(out string hostname)
		{
			// default to 0 if we can't find from DISPLAY ENV var
			var displayNumber = 0;
			hostname = null;
			var display = Environment.GetEnvironmentVariable("DISPLAY");
			if (!string.IsNullOrEmpty(display))
			{
				// DISPLAY is hostname:displayNumber.screenNumber
				// or more normally ':0.0"
				// so look for first number after :
				var start = display.IndexOf(':');
				var end = display.IndexOf('.', start >= 0 ? start : 0);
				if (end < 0)
					end = display.Length;
				if (start >= 0)
				{
					int.TryParse(display.Substring(start + 1, end - start - 1), out displayNumber);
					hostname = display.Substring(0, start);
				}
			}
			if (string.IsNullOrEmpty(hostname))
				hostname = "unix";

			if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLATPAK_ID")))
			{
				// Flatpak changes DISPLAY. Assume the host is display 0.
				displayNumber = 0;
			}

			return displayNumber;
		}

		/// <summary>
		/// Attempts to return the file name of the ibus server config file that contains the socket name.
		/// </summary>
		private static string GetIBusConfigFilename()
		{
			// Implementation Plan:
			// Read file in $XDG_CONFIG_HOME/ibus/bus/* if ($XDG_CONFIG_HOME) not set then $HOME/.config/ibus/bus/*
			// Actual file is called 'localMachineId'-'hostname'-'displayNumber'
			// eg: 5a2f89ae5421972c24f8a4414b0495d7-unix-0
			// could check $DISPLAY to see if we are running not on display 0 or not.
			// localMachineId comes from /etc/machine-id

			// We want to use the actual config home on the host, not an application-specific flatpak one. So
			// allow a running application to specify the location of the config home that is external to a flatpak
			// application, by setting USER_CONFIG_HOME.

			string configHome = Environment.GetEnvironmentVariable("USER_CONFIG_HOME") ??
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var directory = Path.Combine(configHome,"ibus", "bus");

			var displayNumber = GetDisplayNumber(out var hostname);

			var fileName = Path.Combine(directory, $"{LocalMachineId}-{hostname}-{displayNumber}");

			if (File.Exists(fileName))
				return fileName;

			Debug.Print("Unable to locate IBus Config file {0}", fileName);
			return null;

		}

		/// <summary>
		/// Read config file and return the socket name from it.
		/// </summary>
		private static string GetSocket(string configFilename)
		{
			// Look for line
			// Set Environment 'DBUS_SESSION_BUS_ADDRESS' so DBus Library actually connects to IBus' DBus.
			// IBUS_ADDRESS=unix:abstract=/tmp/dbus-DVpIKyfU9k,guid=f44265fa3b2781284d54c56a4b0d83f3

			using (var streamReader = new StreamReader(configFilename))
			{
				while (!streamReader.EndOfStream)
				{
					var line = streamReader.ReadLine();

					if (!string.IsNullOrEmpty(line) && !line.StartsWith("#") && line.Contains(IBUS_ADDRESS))
					{
						var tokens = line.Split("=".ToCharArray(), 2);
						if (tokens.Length != 2 || tokens[1] == string.Empty)
							throw new ApplicationException(
								$"IBus config file '{configFilename}' not as expected for line {line}. Expected IBUS_ADDRESS='some socket'");

						return tokens[1];
					}
				}
			}
			throw new ApplicationException($"IBus config file '{configFilename}' doesn't contain {IBUS_ADDRESS} token");
		}

		/// <summary>
		/// Create a DBus to connection to the IBus system in use.
		/// Returns null if it can't connect to ibus.
		/// </summary>
		public static IBusConnection Create()
		{
			if (singleConnection != null)
			{
				singleConnection.AddRef();
				return singleConnection;
			}

			try
			{
				// if Environment var IBUS_ADDRESS doesn't exist then attempt to read it from IBus server settings file.
				var socketName = Environment.GetEnvironmentVariable(ENV_IBUS_ADDRESS);
				if (string.IsNullOrEmpty(socketName))
				{
					var configFileName = GetIBusConfigFilename();
					if (!File.Exists(configFileName))
						return null;

					socketName = GetSocket(configFileName);
				}

				if (string.IsNullOrEmpty(socketName))
					return null;

				// Equivalent to having $DBUS_SESSION_BUS_ADDRESS set
				singleConnection = new IBusConnection(Bus.Open(socketName));
				singleConnection.Disposed += HandleSingleConnectionDisposed;
			}
			catch (Exception e)
			{
#if DEBUG
				Console.WriteLine("Got {0} exception trying to create IBusConnection: {1}", e.GetType(), e.Message);
#endif
			} // ignore - ibus may not be running.

			return singleConnection;
		}

		private static void HandleSingleConnectionDisposed (object sender, EventArgs e)
		{
			singleConnection = null;
		}

		public static void DestroyConnection()
		{
			singleConnection?.Close();
		}
	}
}
