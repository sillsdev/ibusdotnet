using System;
using System.Collections.Generic;
using NDesk.DBus;
using org.freedesktop.DBus;

namespace IBusDotNet
{
	[Interface("org.freedesktop.IBus")]
	internal interface IBusBus : Introspectable
	{
		[return: Argument("address")]
		string GetAddress();

		[return: Argument("context")]
		IBusInputContext CreateInputContext(string name);

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
		public const int PreeditText = 1;
		public const int AuxText = 2;
		public const int LookupTable = 4;
		public const int Focus = 8;
		public const int Property = 16;
		public const int SurroundingText = 32;
	}

	public delegate void CommitTextHandler(object text);
	public delegate void CursorDownLookupTableHandler();
	public delegate void CursorUpLookupTableHandler();
	public delegate void DeleteSurroundingTextHandler(int offset, uint nChars);
	public delegate void DisabledHandler();
	public delegate void EnabledHandler();
	public delegate void ForwardKeyEventHandler(uint keyval, uint keycode, uint state);
	public delegate void HideAuxiliaryTextHandler();
	public delegate void HideLookupTableHandler();
	public delegate void HidePreditTextHandler();
	public delegate void PageDownLookupTableHandler();
	public delegate void PageUpLookupTableHandler();
	public delegate void RegisterPropertiesHandler(object props);
	public delegate void ShowAuxiliaryTextHandler();
	public delegate void ShowLookupTableHandler();
	public delegate void ShowPreditTextHandler();
	public delegate void UpdateAuxiliaryTextHandler(object text, bool visible);
	public delegate void UpdateLookupTableHandler(object text, bool visible);
	public delegate void UpdatePreeditTextHandler(object text, uint cursorPos, bool visible);
	public delegate void UpdatePropertyHandler(object props);

	// TODO: add more events for the rest of the signals
	[Interface("org.freedesktop.IBus.InputContext")]
	internal interface IBusInputContext : Introspectable
	{
		[return: Argument("handled")]
		bool ProcessKeyEvent(uint keyval, uint keycode, uint state);

		void SetCursorLocation(int x, int y, int w, int h);

		void SetCapabilities(UInt32 caps);

		void PropertyActivate(object propName, Int32 state);

		void FocusIn();

		void FocusOut();

		void Reset();

		// the next three methods are no longer in the 1.5.x API
		void Enable();

		void Disable();

		[return: Argument("enabled")]
		bool IsEnabled();

		void SetEngine(string name);

		// the next two methods are not in the 1.3.7/1.3.9 API
		[return: Argument("desc")]
		object GetEngine();

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

	[Interface("org.freedesktop.IBus.Panel")]
	public interface Panel : Introspectable
	{
		void UpdateLookupTable(object lookupTable, bool visible);
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
		void UpdatePreeditText(object text, uint cursorPos, bool visible);
		void RegisterProperties(object props);
		void ShowLookupTable();
		void CursorDownLookupTable();
		void HideLookupTable();
		void HideLanguageBar();
		void FocusIn(object ic);
		void PageUpLookupTable();
		void ShowLanguageBar();
	}

	public class IBusText: IConvertible
	{
		// dbus type: (sa{sv}sv)
		protected internal struct DBusIBusText
		{
			public string TypeName;
			public IDictionary<string, object> b;
			public string Text;
			public object AttrList; // IBusAttrList
		}

		// dbus type: (sa{sv}av)
		protected internal struct DBusIBusAttrList
		{
			public string TypeName;
			public IDictionary<string, object> b;
			public object[] attributes;
		}

		private DBusIBusText TextObj;

		public IBusText(string text): this(text, new IBusAttribute[] {})
		{
		}

		public IBusText(string text, IBusAttribute[] attributes)
		{
			TextObj = new DBusIBusText
			{
				Text = text,
				AttrList = new DBusIBusAttrList { attributes = attributes }
			};
		}

		private IBusText(DBusIBusText textObj)
		{
			TextObj = textObj;
		}

		public static IBusText FromObject(object obj)
		{
			return new IBusText((DBusIBusText)Convert.ChangeType(obj, typeof(DBusIBusText)));
		}

		#region IConvertible implementation

		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this, provider);
		}

		public byte ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this, provider);
		}

		public char ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this, provider);
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this, provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this, provider);
		}

		public short ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this, provider);
		}

		public int ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this, provider);
		}

		public long ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this, provider);
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this, provider);
		}

		public float ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this, provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return Convert.ToString(this, provider);
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(DBusIBusText))
				return TextObj;
			return Convert.ChangeType(TextObj, conversionType, provider);
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this, provider);
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}
		#endregion

		public string Text { get { return TextObj.Text; }}

		public IEnumerable<IBusAttribute> Attributes
		{
			get
			{
				var attrList = (DBusIBusAttrList)Convert.ChangeType(TextObj.AttrList, typeof(DBusIBusAttrList));
				foreach (object obj in attrList.attributes)
				{
					yield return IBusAttribute.FromObject(obj);
				}
			}
		}
	}

	internal enum IBusAttrType
	{
		Underline = 1,
		Foreground = 2,
		Background = 3
	}

	public class IBusAttribute: IConvertible
	{
		// dbus type: (sa{sv}uuuu)
		protected internal struct DBusIBusAttribute
		{
			public string TypeName;
			public IDictionary<string, object> b;
			public uint type;
			public uint value;
			public uint start_index;
			public uint end_index;
		}

		protected DBusIBusAttribute Attribute;

		internal IBusAttribute()
		{
		}

		internal IBusAttribute(DBusIBusAttribute attr)
		{
			Attribute = attr;
		}

		internal static IBusAttribute FromObject(object obj)
		{
			var attr = (DBusIBusAttribute)Convert.ChangeType(obj, typeof(DBusIBusAttribute));
			switch ((IBusAttrType)attr.type)
			{
				case IBusAttrType.Underline:
					return new IBusUnderlineAttribute(attr);
				default:
					return new IBusColorAttribute(attr);
			}
		}

		#region IConvertible implementation

		public TypeCode GetTypeCode()
		{
			return TypeCode.Object;
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this, provider);
		}

		public byte ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this, provider);
		}

		public char ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this, provider);
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this, provider);
		}

		public double ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this, provider);
		}

		public short ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this, provider);
		}

		public int ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this, provider);
		}

		public long ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this, provider);
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this, provider);
		}

		public float ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this, provider);
		}

		public string ToString(IFormatProvider provider)
		{
			return Convert.ToString(this, provider);
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == typeof(DBusIBusAttribute))
				return Attribute;
			return Convert.ChangeType(Attribute, conversionType, provider);
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this, provider);
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt32(this, provider);
		}

		#endregion

		public int StartIndex { get { return (int)Attribute.start_index; }}
		public int EndIndex { get { return (int)Attribute.end_index; }}
	}

	// See http://ibus.googlecode.com/svn/docs/ibus-1.5/IBusAttribute.html
	public enum IBusAttrUnderline
	{
		None = 0,
		Single = 1,
		Double = 2,
		Low = 3,
		Error = 4
	}

	public class IBusUnderlineAttribute: IBusAttribute
	{
		public IBusUnderlineAttribute(IBusAttrUnderline underline, int startIndex, int endIndex)
		{
			Attribute = new DBusIBusAttribute
			{
				start_index = (uint)startIndex,
				end_index = (uint)endIndex,
				value = (uint)underline,
				type = (uint)IBusAttrType.Underline
			};
		}

		internal IBusUnderlineAttribute(DBusIBusAttribute attr): base(attr)
		{
		}

		public IBusAttrUnderline Underline { get { return (IBusAttrUnderline)Attribute.value; }}
	}

	public class IBusColorAttribute: IBusAttribute
	{
		public IBusColorAttribute(int color, bool isBackground, int startIndex, int endIndex)
		{
			Attribute = new DBusIBusAttribute
			{
				start_index = (uint)startIndex,
				end_index = (uint)endIndex,
				value = (uint)color,
				type = (uint)(isBackground ? IBusAttrType.Background : IBusAttrType.Foreground)
			};
		}

		internal IBusColorAttribute(DBusIBusAttribute attr): base(attr)
		{
		}

		public int Color { get { return (int)Attribute.value; }}
	}
}
