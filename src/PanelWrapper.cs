using System;
using NDesk.DBus;

namespace IBusDotNet
{
	public class PanelWrapper
	{
		protected Panel _panel;

		/// <summary>
		/// Wraps a connection to the IBus Panel
		/// </summary>
		public PanelWrapper(IBusConnection connection)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			_panel = ((NDesk.DBus.Connection)connection).GetObject<Panel>("org.freedesktop.IBus.Panel", new ObjectPath("/org/freedesktop/IBus/Panel"));
		}

		/// <summary>
		/// Allow Access to the underlying Panel
		/// </summary>
		public Panel Panel {
			get { return _panel; }
		}
	}
}