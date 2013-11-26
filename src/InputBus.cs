// Copyright (c) 2013 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NDesk.DBus;
using System.Collections.Generic;

namespace IBusDotNet
{
	public class InputBus
	{
		private readonly IBusBus _inputBus;

		public InputBus(IBusConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			_inputBus = ((Connection)connection).GetObject<IBusBus>("org.freedesktop.IBus",
				new ObjectPath("/org/freedesktop/IBus"));
		}

		/// <summary>
		/// Return the DBUS 'path' name for the currently focused InputContext
		/// Throws: System.Exception with message 'org.freedesktop.DBus.Error.Failed: No input context focused'
		/// if nothing is currently focused.
		/// </summary>
		public string GetFocusedInputContextPath()
		{
			return _inputBus.CurrentInputContext();
		}

		private static IBusEngineDesc[] ConvertEngines(object[] rawEngines)
		{
			var engines = new List<IBusEngineDesc>();
			foreach (var engine in rawEngines)
			{
				engines.Add(new BusEngineDesc(BusEngineDesc.CreateEngineDesc(engine.ToString()), engine));
			}
			return engines.ToArray();
		}

		#region IIBus implementation

		public string GetAddress()
		{
			return _inputBus.GetAddress();
		}

		public IInputContext CreateInputContext(string name)
		{
			return new InputContext(_inputBus.CreateInputContext(name));
		}

		public string CurrentInputContext()
		{
			return _inputBus.CurrentInputContext();
		}

		public void RegisterComponent(object components)
		{
			_inputBus.RegisterComponent(components);
		}

		public IBusEngineDesc[] ListEngines()
		{
			return ConvertEngines(_inputBus.ListEngines());
		}

		public IBusEngineDesc[] ListActiveEngines()
		{
			return ConvertEngines(_inputBus.ListActiveEngines());
		}

		public void Exit(bool restart)
		{
			_inputBus.Exit(restart);
		}

		public object Ping(object data)
		{
			return _inputBus.Ping(data);
		}
		#endregion

	}
}
