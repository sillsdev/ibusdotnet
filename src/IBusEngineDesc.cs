// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2011' company='SIL International'>
// 	Copyright (c) 2011, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;

namespace IBusDotNet
{
	public class IBusEngineDesc
	{
		// enums of supported ibus versions
		// these numbers are NOT the same as the ibus version numbers.
		private enum IBusVersions
		{
			Unknown,
			Version1,
			// commit e2793f52bf3da7a22321 changed IBusEngineDesc to contain rank field.
			Version2,
			Version_1_3_7,
			Version_1_3_99,
			Version_1_4,
			Version_1_4_1,
		}

		private static IBusVersions s_ibusVersion = IBusVersions.Unknown;

		private IBusEngineDesc()
		{
		}

		public string Name { get; private set; }
		public string LongName { get; private set; }
		public string Description { get; private set; }
		public string Language { get; private set; }
		public string License { get; private set; }
		public string Author { get; private set; }
		public string Icon { get; private set; }
		public string Layout { get; private set; }
		public string Hotkeys { get; private set; }
		public UInt32 Rank { get; private set; }
		public string Symbol { get; private set; }
		public string Setup { get; private set; }

		private static T? ConvertToEngineDesc<T>(object engine)
		{
			try
			{
				System.Diagnostics.Debug.WriteLine(string.Format("Trying to convert '{0}' to {1}",
					engine, typeof(T)));
				return (T)Convert.ChangeType(engine, typeof(T));
			}
			catch (InvalidCastException)
			{
				return null;
			}
		}

		/// <summary>Get IBusEngineDesc names in a way tolerant to IBusEngineDesc versions.</summary>
		public static IBusEngineDesc GetEngineDesc(object engine)
		{
			switch (s_ibusVersion)
			{
				case IBusVersions.Unknown:
				case IBusVersions.Version1:
				{
					var engineDesc = ConvertToEngineDesc<IBusEngineDesc_v1>(engine);
					if (engineDesc.HasValue)
					{
						return new IBusEngineDesc { LongName = engineDesc.Value.longname,
							Name = engineDesc.Value.name, Description = engineDesc.Value.description,
							Language = engineDesc.Value.language, License = engineDesc.Value.license,
							Author = engineDesc.Value.author, Icon = engineDesc.Value.icon,
							Layout = engineDesc.Value.layout };
					}
					// try next version.
					s_ibusVersion = IBusVersions.Version2;
					break;
				}

				case IBusVersions.Version2:
				{
					var engineDesc = ConvertToEngineDesc<IBusEngineDesc_v2>(engine);
					if (engineDesc.HasValue)
					{
						return new IBusEngineDesc { LongName = engineDesc.Value.longname,
							Name = engineDesc.Value.name, Description = engineDesc.Value.description,
							Language = engineDesc.Value.language, License = engineDesc.Value.license,
							Author = engineDesc.Value.author, Icon = engineDesc.Value.icon,
							Layout = engineDesc.Value.layout, Rank = engineDesc.Value.rank };
					}
					// try next version.
					s_ibusVersion = IBusVersions.Version_1_3_7;
					break;
				}

				case IBusVersions.Version_1_3_7:
				{
					var engineDesc = ConvertToEngineDesc<IBusEngineDesc_v3>(engine);
					if (engineDesc.HasValue)
					{
						return new IBusEngineDesc { LongName = engineDesc.Value.longname,
							Name = engineDesc.Value.name, Description = engineDesc.Value.description,
							Language = engineDesc.Value.language, License = engineDesc.Value.license,
							Author = engineDesc.Value.author, Icon = engineDesc.Value.icon,
							Layout = engineDesc.Value.layout, Rank = engineDesc.Value.rank,
							Hotkeys = engineDesc.Value.hotkeys };
					}
					// try next version.
					s_ibusVersion = IBusVersions.Version_1_3_99;
					break;
				}

				case IBusVersions.Version_1_3_99:
					{
						var engineDesc = ConvertToEngineDesc<IBusEngineDesc_v4>(engine);
						if (engineDesc.HasValue)
						{
							return new IBusEngineDesc { LongName = engineDesc.Value.longname,
								Name = engineDesc.Value.name, Description = engineDesc.Value.description,
								Language = engineDesc.Value.language, License = engineDesc.Value.license,
								Author = engineDesc.Value.author, Icon = engineDesc.Value.icon,
								Layout = engineDesc.Value.layout, Rank = engineDesc.Value.rank,
								Hotkeys = engineDesc.Value.hotkeys };
						}
						// try next version.
						s_ibusVersion = IBusVersions.Version_1_4;
						break;
					}

				case IBusVersions.Version_1_4:
				{
					var engineDesc = ConvertToEngineDesc<IBusEngineDesc_v5>(engine);
					if (engineDesc.HasValue)
					{
						return new IBusEngineDesc { LongName = engineDesc.Value.longname,
							Name = engineDesc.Value.name, Description = engineDesc.Value.description,
							Language = engineDesc.Value.language, License = engineDesc.Value.license,
							Author = engineDesc.Value.author, Icon = engineDesc.Value.icon,
							Layout = engineDesc.Value.layout, Rank = engineDesc.Value.rank,
							Hotkeys = engineDesc.Value.hotkeys, Symbol = engineDesc.Value.symbol };
					}
					// try next version.
					s_ibusVersion = IBusVersions.Version_1_4_1;
					break;
				}

				default:
				case IBusVersions.Version_1_4_1:
				{
					var engineDesc = ConvertToEngineDesc<IBusEngineDesc_v6>(engine);
					if (engineDesc.HasValue)
					{
						return new IBusEngineDesc { LongName = engineDesc.Value.longname,
							Name = engineDesc.Value.name, Description = engineDesc.Value.description,
							Language = engineDesc.Value.language, License = engineDesc.Value.license,
							Author = engineDesc.Value.author, Icon = engineDesc.Value.icon,
							Layout = engineDesc.Value.layout, Rank = engineDesc.Value.rank,
							Hotkeys = engineDesc.Value.hotkeys, Symbol = engineDesc.Value.symbol,
							Setup = engineDesc.Value.setup };
					}

					Console.WriteLine("*** Unknown ibus version");
					throw new NotSupportedException("Unknown ibus version with signature " +
						engine.ToString());
				}
			}
			return GetEngineDesc(engine);
		}
	}
}