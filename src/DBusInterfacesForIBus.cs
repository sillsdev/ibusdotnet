using System;
using System.Collections.Generic;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace IBusDotNet
{
	// element type returned in array by ListEngines/ListActiveEngines
	// dbus type: (sa{sv}ssssssss/*u*/)
	public struct IBusEngineDesc
	{
		public string a;
		public IDictionary<string, object> b;
		public string longname;
		public string name;
		public string description;
		public string language;
		public string license;
		public string author;
		public string icon;
		public string layout;
		/* public UInt32 rank; rank isn't in lucid ibus yet */

		public override string ToString()
		{
			return string.Format("[IBusEngineDesc] '{0}' '{1}' \nname:'{2}' \nlongname:'{3}' \ndescription:'{4}' \nlanguage:'{5}' \nlicense:'{6}' \nauthor:'{7}' \nicon:'{8}' \nlayout:'{9}' ", a, b, name, longname, description, language, license, author, icon,
			layout);
		}
	}

	// TODO rename this interface to IBus
	[Interface("org.freedesktop.IBus")]
	[CLSCompliant(false)]
	public interface IIBus : Introspectable
	{
		[return: Argument("address")]
		string GetAddress();

		[return: Argument("context")]
		InputContext CreateInputContext(string name);

		[return: Argument("name")]
		string CurrentInputContext();

		void RegisterComponent(object components);

		[return: Argument("engines")]
		object[] ListEngines();

		[return: Argument("engines")]
		object[] ListActiveEngines();

		void Exit(bool restart);

		[return: Argument("data")]
		object Ping(object data);
	}

	public class Capabilities
	{
		public const uint PreeditText = 1;
		public const uint AuxText = 2;
		public const uint LookupTable = 4;
		public const uint Focus = 8;
		public const uint Property = 16;
		public const uint SurroundingText = 32;
	}


	public delegate void CommitTextHandler(object text);
	public delegate void EnabledHandler();
	public delegate void DisabledHandler();
	public delegate void ForwardKeyEventHandler(uint keyval, uint keycode, uint state);
	public delegate void UpdatePreeditTextHandler(object text, uint cursor_pos, bool visible);
	public delegate void ShowPreditTextHandler();
	public delegate void HidePreditTextHandler();
	public delegate void UpdateAuxiliaryTextHandler(object text, bool visible);
	public delegate void ShowAuxiliaryTextHandler();
	public delegate void HideAuxiliaryTextHandler();
	public delegate void UpdateLookupTableHandler(object text, bool visible);
	public delegate void ShowLookupTableHandler();
	public delegate void HideLookupTableHandler();
	public delegate void PageUpLookupTableHandler();
	public delegate void PageDownLookupTableHandler();
	public delegate void CursorUpLookupTableHandler();
	public delegate void CursorDownLookupTableHandler();
	public delegate void RegisterPropertiesHandler(object props);
	public delegate void UpdatePropertyPropertiesHandler(object props);

	// TODO: add more events for the rest of the signals
	[Interface("org.freedesktop.IBus.InputContext")]
	[CLSCompliant(false)]
	public interface InputContext : Introspectable
	{
		[return: Argument("handled")]
		bool ProcessKeyEvent(uint keyval, uint keycode, uint state);

		void SetCursorLocation(int x, int y, int w, int h);

		void FocusIn();

		void FocusOut();

		void Reset();

		void Enable();

		void Disable();

		[return: Argument("enabled")]
		bool IsEnabled();

		void SetCapabilities(UInt32 caps);

		void SetEngine(string name);

		[return: Argument("desc")]
		object GetEngine();

		void Destroy();

		event CommitTextHandler CommitText;
		event EnabledHandler Enabled;
		event DisabledHandler Disabled;
		event ForwardKeyEventHandler ForwardKeyEvent;
		event UpdatePreeditTextHandler UpdatePreeditText;
		event ShowPreditTextHandler ShowPreeditText;
		event HidePreditTextHandler HidePreeditText;
		event UpdateAuxiliaryTextHandler UpdateAuxiliaryText;
		event ShowAuxiliaryTextHandler ShowAuxiliaryText;
		event HideAuxiliaryTextHandler HideAuxiliaryText;
		event UpdateLookupTableHandler UpdateLookupTable;
		event ShowLookupTableHandler ShowLookupTable;
		event HideLookupTableHandler HideLookupTable;
		event PageUpLookupTableHandler PageUpLookupTable;
		event PageDownLookupTableHandler PageDownLookupTable;
		event CursorUpLookupTableHandler CursorUpLookupTable;
		event CursorDownLookupTableHandler CursorDownLookupTable;
		event RegisterPropertiesHandler RegisterProperties;
		event UpdatePropertyPropertiesHandler UpdatePropertyProperties;

	}

	[Interface("org.freedesktop.IBus.Panel")]
	[CLSCompliant(false)]
	public interface Panel : Introspectable
	{
		void UpdateLookupTable(object lookup_table, bool visible);
		void StartSetup();
		void SetCursorLocation(int x, int y, int w, int h);
		void UpdateAuxiliaryText(object text, bool visible);
		void FocusOut(object ic);
		void HideAuxiliaryText();
		void Destroy();
		void PageDownLookupTable();
		void StateChanged();
		void ShowAuxiliaryText();
		void ShowPreeditText();
		void Reset();
		void UpdateProperty(object prop);
		void HidePreeditText();
		void CursorUpLookupTable();
		void UpdatePreeditText(object text, uint cursor_pos, bool visible);
		void RegisterProperties(object props);
		void ShowLookupTable();
		void CursorDownLookupTable();
		void HideLookupTable();
		void HideLanguageBar();
		void FocusIn(object ic);
		void PageUpLookupTable();
		void ShowLanguageBar();
	}

	// dbus type: (sa{sv}sv)
	[CLSCompliant(false)]
	public struct IBusText
	{
		public string TypeName;
		public IDictionary<string, object> b;
		public string Text;
		public object d;
	}
}
