using System;
using NDesk.DBus;

namespace IBusDotNet
{
	public class InputContextWrapper
	{
		protected InputContext _inputContext;

		/// <summary>
		/// Wraps a connection to a specfic instance of an IBus InputContext
		/// inputContextName needs to be the name of specfic instance of the input context.
		/// For example "/org/freedesktop/IBus/InputContext_15"
		/// </summary>
		public InputContextWrapper(IBusConnection connection, string inputContextName)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			_inputContext = ((NDesk.DBus.Connection)connection).GetObject<InputContext>("org.freedesktop.DBus", new ObjectPath(inputContextName));
		}

		/// <summary>
		/// Allow Access to the underlying InputContext
		/// </summary>
		public InputContext InputContext {
			get { return _inputContext; }
		}
	}
}
