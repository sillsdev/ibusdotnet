// --------------------------------------------------------------------------------------------
// <copyright from='2011' to='2011' company='SIL International'>
// 	Copyright (c) 2011, SIL International. All Rights Reserved.
//
// 	Distributable under the terms of either the Common Public License or the
// 	GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
// --------------------------------------------------------------------------------------------
using System;
using NUnit.Framework;

namespace IBusDotNet
{
	[TestFixture]
	public class IBusDotNetTests
	{
		private IBusConnection Connection;

		/// <summary>
		/// Close connection to ibus
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			Connection?.Dispose();
			Connection = null;
		}

		/// <summary>
		/// Tests that we can get the ibus engine. This will fail if the ibus API changed.
		/// </summary>
		[Test]
		public void CanGetEngineDesc()
		{
			Connection = IBusConnectionFactory.Create();
			if (Connection == null)
			{
				Assert.Ignore("Can't run this test without ibus running.");
				return;
			}

			var ibus = new InputBus(Connection);
			var engines = ibus.ListEngines();
			if (engines.Length == 0)
			{
				Assert.Ignore("Can't run this test without any ibus keyboards installed.");
				return;
			}

			Assert.IsNotNull(engines[0]);
		}
	}
}
