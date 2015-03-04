// Copyright (c) 2013 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;
using NDesk.DBus;

namespace IBusDotNet
{
	public class InputContext: IInputContext
	{
		private IBusInputContext m_inputContext;

		/// <summary>
		/// Wraps a connection to a specfic instance of an IBus InputContext
		/// inputContextName needs to be the name of specfic instance of the input context.
		/// For example "/org/freedesktop/IBus/InputContext_15"
		/// </summary>
		public InputContext(IBusConnection connection, string inputContextName)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			Init(((Connection)connection).GetObject<IBusInputContext>("org.freedesktop.DBus",
				new ObjectPath(inputContextName)));
		}

		internal InputContext(IBusInputContext inputContext)
		{
			Init(inputContext);
		}

		private void Init(IBusInputContext inputContext)
		{
			m_inputContext = inputContext;
			m_inputContext.CommitText += (object text) => {
				if (CommitText != null)
					CommitText(text);
			};
			m_inputContext.DeleteSurroundingText += (int offset, uint nChars) => {
				if (DeleteSurroundingText != null)
					DeleteSurroundingText(offset, nChars);
			};
			m_inputContext.Enabled += () => {
				if (Enabled != null)
					Enabled();
			};
			m_inputContext.Disabled += () => {
				if (Disabled != null)
					Disabled();
			};
			m_inputContext.ForwardKeyEvent += (uint keyval, uint keycode, uint state) => {
				if (ForwardKeyEvent != null)
					ForwardKeyEvent(keyval, keycode, state);
			};
			m_inputContext.HideAuxiliaryText += () => {
				if (HideAuxiliaryText != null)
					HideAuxiliaryText();
			};
			m_inputContext.HideLookupTable += () => {
				if (HideLookupTable != null)
					HideLookupTable();
			};
			m_inputContext.HidePreeditText += () => {
				if (HidePreeditText != null)
					HidePreeditText();
			};
			m_inputContext.PageDownLookupTable += () => {
				if (PageDownLookupTable != null)
					PageDownLookupTable();
			};
			m_inputContext.PageUpLookupTable += () => {
				if (PageUpLookupTable != null)
					PageUpLookupTable();
			};
			m_inputContext.RegisterProperties += (object props) => {
				if (RegisterProperties != null)
					RegisterProperties(props);
			};
			m_inputContext.ShowAuxiliaryText += () => {
				if (ShowAuxiliaryText != null)
					ShowAuxiliaryText();
			};
			m_inputContext.ShowLookupTable += () => {
				if (ShowLookupTable != null)
					ShowLookupTable();
			};
			m_inputContext.ShowPreeditText += () => {
				if (ShowPreeditText != null)
					ShowPreeditText();
			};
			m_inputContext.UpdateAuxiliaryText += (object text, bool visible) => {
				if (UpdateAuxiliaryText != null)
					UpdateAuxiliaryText(text, visible);
			};
			m_inputContext.UpdateLookupTable += (object text, bool visible) => {
				if (UpdateLookupTable != null)
					UpdateLookupTable(text, visible);
			};
			m_inputContext.UpdatePreeditText += (object text, uint cursorPos, bool visible) => {
				if (UpdatePreeditText != null)
					UpdatePreeditText(text, cursorPos, visible);
			};
			m_inputContext.UpdateProperty += (object props) => {
				if (UpdateProperty != null)
					UpdateProperty(props);
			};
			m_inputContext.CursorDownLookupTable += () => {
				if (CursorDownLookupTable != null)
					CursorDownLookupTable();
			};
			m_inputContext.CursorUpLookupTable += () => {
				if (CursorUpLookupTable != null)
					CursorUpLookupTable();
			};
		}

		#region IInputContext implementation

		public event CommitTextHandler CommitText;

		public event DeleteSurroundingTextHandler DeleteSurroundingText;

		public event EnabledHandler Enabled;

		public event DisabledHandler Disabled;

		public event ForwardKeyEventHandler ForwardKeyEvent;

		public event UpdatePreeditTextHandler UpdatePreeditText;

		public event ShowPreditTextHandler ShowPreeditText;

		public event HidePreditTextHandler HidePreeditText;

		public event UpdateAuxiliaryTextHandler UpdateAuxiliaryText;

		public event ShowAuxiliaryTextHandler ShowAuxiliaryText;

		public event HideAuxiliaryTextHandler HideAuxiliaryText;

		public event UpdateLookupTableHandler UpdateLookupTable;

		public event ShowLookupTableHandler ShowLookupTable;

		public event HideLookupTableHandler HideLookupTable;

		public event PageUpLookupTableHandler PageUpLookupTable;

		public event PageDownLookupTableHandler PageDownLookupTable;

		public event CursorUpLookupTableHandler CursorUpLookupTable;

		public event CursorDownLookupTableHandler CursorDownLookupTable;

		public event RegisterPropertiesHandler RegisterProperties;

		public event UpdatePropertyHandler UpdateProperty;

		public bool ProcessKeyEvent(int keyval, int keycode, int state)
		{
			return m_inputContext.ProcessKeyEvent((uint)keyval, (uint)keycode, (uint)state);
		}

		public void SetCursorLocation(int x, int y, int w, int h)
		{
			m_inputContext.SetCursorLocation(x, y, w, h);
		}

		public void SetCapabilities(int caps)
		{
			m_inputContext.SetCapabilities((uint)caps);
		}

		public void PropertyActivate(object propName, int state)
		{
			m_inputContext.PropertyActivate(propName, state);
		}

		public void FocusIn()
		{
			m_inputContext.FocusIn();
		}

		public void FocusOut()
		{
			m_inputContext.FocusOut();
		}

		public void Reset()
		{
			m_inputContext.Reset();
		}

		public void Enable()
		{
			// Enable is no longer available in IBus 1.5
			if (m_inputContext.Introspect().Contains("<method name=\"Enable\""))
				m_inputContext.Enable();
		}

		public void Disable()
		{
			// Disable is no longer available in IBus 1.5
			if (m_inputContext.Introspect().Contains("<method name=\"Disable\""))
				m_inputContext.Disable();
		}

		public bool IsEnabled()
		{
			// IsEnabled is no longer available in IBus 1.5
			var x = m_inputContext.Introspect();
			if (m_inputContext.Introspect().Contains("<method name=\"IsEnabled\""))
				return m_inputContext.IsEnabled();
			return true;
		}

		public void SetEngine(string name)
		{
			m_inputContext.SetEngine(name);
		}

		public IBusEngineDesc GetEngine()
		{
			var engine = m_inputContext.GetEngine();
			return new BusEngineDesc(BusEngineDesc.CreateEngineDesc(engine.ToString()), engine);
		}

		public void Destroy()
		{
			m_inputContext.Destroy();
		}

		#endregion
	}
}
