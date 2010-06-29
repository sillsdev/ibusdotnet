using NDesk.DBus;
using org.freedesktop.IBus;

namespace IBusDotNet
{
	public class InputBusWrapper
	{
		org.freedesktop.IBus.IIBus _inputBus;

		public InputBusWrapper(NDesk.DBus.Connection connection)
		{
			_inputBus = connection.GetObject<org.freedesktop.IBus.IIBus>("org.freedesktop.IBus", new ObjectPath("/org/freedesktop/IBus"));
		}

		/// <summary>
		/// Allow Access to the underlying org.freedesktop.IBus.IIBus
		/// </summary>
		public org.freedesktop.IBus.IIBus InputBus {
			get { return _inputBus; }
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
	}
}
