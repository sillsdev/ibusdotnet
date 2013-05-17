// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2011' company='SIL International'>
// 	Copyright (c) 2011, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using NDesk.DBus;

namespace IBusDotNet
{
	/// <summary>
	/// Dynamically constructed IBusEngineDesc struct based on the dbus signature
	/// </summary>
	/// <remarks>Limitations: we follow the YAGNI approach and support currently only the basic
	/// types. Also we assume that the order of properties of the same type stays the same (i.e.
	/// that the second string property we get is always the longname). It would be better and
	/// safer to get the IBusEngineDesc properties through the proper methods that ibus defines,
	/// but that looked way more complicated and isn't currently needed. Besides, the ibus
	/// code has a comment that the order of the serialized properties should not be changed
	/// (https://github.com/ibus/ibus/blob/master/src/ibusenginedesc.c).</remarks>
	public class IBusEngineDesc
	{
		private Type m_type;
		private object m_engine;

		private IBusEngineDesc(Type type, object engine)
		{
			m_type = type;
			m_engine = Convert.ChangeType(engine, type);
		}

		// copied (and slightly modified) from https://github.com/mono/dbus-sharp/blob/master/src/DType.cs
		private enum DType
		{
			Invalid = '\0',

			Byte = 'y',
			Boolean = 'b',
			Int16 = 'n',
			UInt16 = 'q',
			Int32 = 'i',
			UInt32 = 'u',
			Int64 = 'x',
			UInt64 = 't',
			Single = 'f', //This is not yet supported!
			Double = 'd',
			String = 's',
			ObjectPath = 'o',
			Signature = 'g',

			Array = 'a',
			[Obsolete ("Not in protocol")]
			Struct = 'r',
			[Obsolete ("Not in protocol")]
			DictEntry = 'e',
			Variant = 'v',

			StructBegin = '(',
			StructEnd = ')',
			DictEntryBegin = '{',
			DictEntryEnd = '}',
		}

		/// <summary>
		/// The field names we know (see https://github.com/ibus/ibus/blob/master/src/ibusenginedesc.c).
		/// </summary>
		private readonly static string[] StringFieldNames = { "a", "longname", "name",
			"description", "language", "license", "author", "icon", "layout",
			"hotkeys", "symbol", "setup", "layoutVariant", "layoutOption", "version",
			"textDomain"};
		private readonly static string[] IntFieldNames = { "rank" };

		private static Type GetTypeFromDType(string signature, out int advanceChars)
		{
			advanceChars = 0;
			for (int i = 0; i < signature.Length; i++)
			{
				advanceChars++;
				switch ((DType)signature[i])
				{
					case DType.Array: // 'a'
						// should be something like a{sv}
						if (signature.Length < 5 || signature[1] != '{' || signature[4] != '}')
						{
							advanceChars = 0;
							return null;
						}

						// see http://stackoverflow.com/a/1151470 on how to dynamically create a
						// generic type
						int advanceChars1, advanceChars2;
						var dictionaryType = typeof(IDictionary<,>);
						var genericType = dictionaryType.MakeGenericType(
							GetTypeFromDType(signature.Substring(2), out advanceChars1),
							GetTypeFromDType(signature.Substring(3), out advanceChars2));
						advanceChars = 3 + advanceChars1 + advanceChars2;
						return genericType;
					case DType.String: // 's'
						return typeof(string);
					case DType.Byte: // 'y'
						return typeof(Byte);
					case DType.Boolean: // 'b'
						return typeof(bool);
					case DType.Int16: // 'n'
						return typeof(Int16);
					case DType.UInt16: // 'q'
						return typeof(UInt16);
					case DType.Int32: // 'i'
						return typeof(Int32);
					case DType.UInt32: // 'u'
						return typeof(UInt32);
					case DType.Int64: // 'x'
						return typeof(Int64);
					case DType.UInt64: // 't'
						return typeof(UInt64);
					case DType.Single: // 'f'
						return typeof(Single);
					case DType.Double: // 'd'
						return typeof(Double);
					case DType.ObjectPath: // 'o'
						return typeof(ObjectPath);
					case DType.Signature: // 'g'
						return typeof(string);
					case DType.Variant: // 'v'
						return typeof(object);
					case DType.StructBegin:
					case DType.StructEnd:
						continue;
					default:
						advanceChars = 0;
						return null;
				}
			}
			return null;
		}

		/// <summary>
		/// Create an IBus Engine Description based on the DBus signature
		/// </summary>
		/// <returns>The ibus engine description struct.</returns>
		/// <param name="signature">DBus type signature.</param>
		internal static Type CreateEngineDesc(string signature)
		{
			var assemblyName = new AssemblyName();
			assemblyName.Name = "IBusEngineDescAssembly";

			var domain = Thread.GetDomain();
			var assemblyBuilder = domain.DefineDynamicAssembly(assemblyName,
				AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("IBusEngineDescModule");

			var typeBuilder = moduleBuilder.DefineType("IBusEngineDesc");

			int fieldCount = 0;
			int stringFieldCount = 0;
			int intFieldCount = 0;
			for (int i = 0; i < signature.Length; fieldCount++)
			{
				var fieldName = string.Format("field_{0}", fieldCount);
				int advanceChars;
				Type t = GetTypeFromDType(signature.Substring(i), out advanceChars);
				if (t == null && advanceChars == 0)
				{
					throw new NotSupportedException(string.Format("Unknown ibus version with signature {0}.",
						signature));
				}

				i += advanceChars;
				if (t == null)
					continue;

				switch ((DType)signature[i - 1])
				{
					case DType.String: // 's'
						if (stringFieldCount < StringFieldNames.Length)
							fieldName = StringFieldNames[stringFieldCount++];
						break;
					case DType.UInt32: // 'u'
						if (intFieldCount < IntFieldNames.Length)
							fieldName = IntFieldNames[intFieldCount++];
						break;
					default:
						break;
				}
				typeBuilder.DefineField(fieldName, t, FieldAttributes.Public);
			}
			return typeBuilder.CreateType();
		}

		private T GetProperty<T>(string fieldName)
		{
			var field = m_type.GetField(fieldName);
			if (field != null)
				return (T)field.GetValue(m_engine);
			return default(T);
		}

		public string Name { get { return GetProperty<string>("name"); } }
		public string LongName { get { return GetProperty<string>("longname"); } }
		public string Description { get { return GetProperty<string>("description"); } }
		public string Language { get { return GetProperty<string>("language"); } }
		public string License { get { return GetProperty<string>("license"); } }
		public string Author { get { return GetProperty<string>("author"); } }
		public string Icon { get { return GetProperty<string>("icon"); } }
		public string Layout { get { return GetProperty<string>("layout"); } }
		public string Hotkeys { get { return GetProperty<string>("hotkeys"); } }
		public UInt32 Rank { get { return GetProperty<UInt32>("rank"); } }
		public string Symbol { get { return GetProperty<string>("symbol"); } }
		public string Setup { get { return GetProperty<string>("setup"); } }
		public string LayoutVariant { get { return GetProperty<string>("layoutVariant"); } }
		public string LayoutOption { get { return GetProperty<string>("layoutOption"); } }
		public string Version { get { return GetProperty<string>("version"); } }
		public string TextDomain { get { return GetProperty<string>("textDomain"); } }

		/// <summary>Get IBusEngineDesc names in a way tolerant to IBusEngineDesc versions.</summary>
		public static IBusEngineDesc GetEngineDesc(object engine)
		{
			return new IBusEngineDesc(CreateEngineDesc(engine.ToString()), engine);
		}
	}
}