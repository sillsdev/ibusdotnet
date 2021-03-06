// Copyright (c) 2013 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)
using System;

namespace IBusDotNet
{
	// see http://ibus.github.io/docs/ibus-1.5/IBusInputContext.html
	public interface IInputContext
	{
		bool ProcessKeyEvent(int keyval, int keycode, int state);

		void SetCursorLocation(int x, int y, int w, int h);

		void SetCapabilities(int caps);

		void PropertyActivate(object propName, int state);

		void FocusIn();

		void FocusOut();

		void Reset();

		// The ibus documentation lists two methods SetSurroundingText and NeedsSurroundingText
		// which seem to be useful. However, the latter doesn't seem to available over DBus
		// and trying to use SetSurroundingText results in a strange error message.

		// the next three methods are no longer in the 1.5.x API
		void Enable();

		void Disable();

		bool IsEnabled();

		void SetEngine(string name);

		// the next two methods are not in the 1.3.7/1.3.9 API
		IBusEngineDesc GetEngine();

		void Destroy();

		event CommitTextHandler CommitText;
		event DeleteSurroundingTextHandler DeleteSurroundingText;
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
		event UpdatePropertyHandler UpdateProperty;
	}
}
