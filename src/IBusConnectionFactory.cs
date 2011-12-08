using System;
using System.IO;
using NDesk.DBus;

namespace IBusDotNet
{
	/// <summary>
	/// static class that allow creation of DBus connection to IBus's session DBus.
	/// </summary>
	public static class IBusConnectionFactory
	{
		const string ENV_IBUS_ADDRESS = "IBUS_ADDRESS";
		const string IBUS_ADDRESS = "IBUS_ADDRESS";

		/// <summary>
		/// Gets the local machine identifier.
		/// </summary>
		/// <remarks>The path to the machine-id file is hardcoded in ibus sources as
		/// /var/lib/dbus/machine-id (src/ibusshare.c).</remarks>
		private static string LocalMachineId
		{
			get
			{
				using (var machineIdFile = new StreamReader("/var/lib/dbus/machine-id"))
				{
					return machineIdFile.ReadToEnd().TrimEnd('\n');
				}
			}
		}

		/// <summary>
		/// Attempts to return the file name of the ibus server config file that contains the socket name.
		/// </summary>
		static string IBusConfigFilename()
		{
			// Implementation Plan:
			// Read file in $XDG_CONFIG_HOME/ibus/bus/* if ($XDG_CONFIG_HOME) not set then $HOME/.config/ibus/bus/*
			// Actual file is called 'localmachineid'-'hostname'-'displaynumber'
			// eg: 5a2f89ae5421972c24f8a4414b0495d7-unix-0
			// could check $DISPLAY to see if we are running not on display 0 or not.
			// localmachineid comes from /var/lib/dbus/machine-id

			string directory = System.Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
			if (String.IsNullOrEmpty(directory))
			{
				directory = System.Environment.GetEnvironmentVariable("HOME");

				if (String.IsNullOrEmpty(directory))
					throw new ApplicationException("$XDG_CONFIG_HOME or $HOME Environment not set");

				directory = Path.Combine(directory, ".config");
			}
			directory = Path.Combine(directory, "ibus");
			directory = Path.Combine(directory, "bus");

			// default to 0 if we can't find from DISPLAY ENV var
			int displayNumber = 0;

			string hostname = null;
			string display = System.Environment.GetEnvironmentVariable("DISPLAY");
			if (display != String.Empty)
			{
				// DISPLAY is hostname:displaynumber.screennumber
				// or more nomally ':0.0"
				// so look for first number after :
				int start = display.IndexOf(':');
				int end = display.IndexOf('.');
				if (start > 0 && end > 0)
					int.TryParse(display.Substring(start, end - start), out displayNumber);
				hostname = display.Substring(0, start);
			}

			if (string.IsNullOrEmpty(hostname))
				hostname = "unix";

			string fileName = Path.Combine(directory,
				string.Format("{0}-{1}-{2}", LocalMachineId, hostname, displayNumber));

			if (!File.Exists(fileName))
			{
				throw new ApplicationException(string.Format("Unable to locate IBus Config file {0}",
					fileName));
			}

			return fileName;
		}

		/// <summary>
		/// Read config file and return the socket name from it.
		/// </summary>
		static string GetSocket(string filename)
		{
			// Look for line
			// Set Enviroment 'DBUS_SESSION_BUS_ADDRESS' so DBus Library actually connects to IBus' DBus.
			// IBUS_ADDRESS=unix:abstract=/tmp/dbus-DVpIKyfU9k,guid=f44265fa3b2781284d54c56a4b0d83f3

			using (StreamReader s = new StreamReader(filename))
			{
				string line = String.Empty;
				while (line != null)
				{
					line = s.ReadLine();

					if (line.Contains(IBUS_ADDRESS))
					{
						string[] toks = line.Split("=".ToCharArray(), 2);
						if (toks.Length != 2 || toks[1] == String.Empty)
							throw new ApplicationException(String.Format("IBUS config file : {0} not as expected for line {1}. Expected IBUS_ADDRESS='some socket'", filename, line));

						return toks[1];
					}
				}
			}
			throw new ApplicationException(String.Format("IBUS config file : {0} doesn't contain {1} token", filename, IBUS_ADDRESS));
		}

		static IBusConnection singleConnection = null;
		/// <summary>
		/// Create a DBus to connection to the IBus system in use.
		/// Returns null if it can't conenct to ibus.
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
				// if Enviroment var IBUS_ADDRESS doesn't exist then attempt to read it from IBus server settings file.
				string socketName = System.Environment.GetEnvironmentVariable(ENV_IBUS_ADDRESS);
				if (String.IsNullOrEmpty(socketName))
					socketName = GetSocket(IBusConfigFilename());

				// Equivalent to having $DBUS_SESSION_BUS_ADDRESS set
				singleConnection = new IBusConnection(Bus.Open(socketName));
				singleConnection.Disposed += HandleSingleConnectionDisposed;
			}
			catch(System.Exception) { } // ignore - ibus may not be running.

			return singleConnection;
		}

		private static void HandleSingleConnectionDisposed (object sender, EventArgs e)
		{
			singleConnection = null;
		}

		public static void DestroyConnection()
		{
			if (singleConnection != null)
				singleConnection.Close();
		}
	}
}
