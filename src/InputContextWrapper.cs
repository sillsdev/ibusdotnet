using NDesk.DBus;
using org.freedesktop.IBus;

namespace IBusDotNet
{
	public class InputContextWrapper
	{
		protected org.freedesktop.IBus.InputContext _inputContext;

		/// <summary>
		/// Wraps a connection to a specfic instance of an IBus InputContext
		/// inputContextName needs to be the name of specfic instance of the input context.
		/// For example "/org/freedesktop/IBus/InputContext_15"
		/// </summary>
		public InputContextWrapper(NDesk.DBus.Connection connection, string inputContextName)
		{
			_inputContext = connection.GetObject<org.freedesktop.IBus.InputContext>("org.freedesktop.DBus", new ObjectPath(inputContextName));
		}

		/// <summary>
		/// Allow Access to the underlying org.freedesktop.IBus.IIBus
		/// </summary>
		public org.freedesktop.IBus.InputContext InputContext {
			get { return _inputContext; }
		}
	}
}
