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
	public class IBusConnectionFactoryTests
	{
		private string m_OriginalDisplay;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			m_OriginalDisplay = Environment.GetEnvironmentVariable("DISPLAY");
		}

		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			Environment.SetEnvironmentVariable("DISPLAY", m_OriginalDisplay);
		}

		[Test]
		public void GetDisplayNumber_NoEnv()
		{
			Environment.SetEnvironmentVariable("DISPLAY", null);
			string hostname;
			int displayNumber = IBusConnectionFactory.GetDisplayNumber(out hostname);
			Assert.AreEqual(0, displayNumber);
			Assert.AreEqual("unix", hostname);
		}

		[Test]
		public void GetDisplayNumber_DisplayOnly()
		{
			Environment.SetEnvironmentVariable("DISPLAY", ":5");
			string hostname;
			int displayNumber = IBusConnectionFactory.GetDisplayNumber(out hostname);
			Assert.AreEqual(5, displayNumber);
			Assert.AreEqual("unix", hostname);
		}

		[Test]
		public void GetDisplayNumber_DisplayWithScreen()
		{
			Environment.SetEnvironmentVariable("DISPLAY", ":6.7");
			string hostname;
			int displayNumber = IBusConnectionFactory.GetDisplayNumber(out hostname);
			Assert.AreEqual(6, displayNumber);
			Assert.AreEqual("unix", hostname);
		}

		[Test]
		public void GetDisplayNumber_DisplayWithHostname()
		{
			Environment.SetEnvironmentVariable("DISPLAY", "foo:8.0");
			string hostname;
			int displayNumber = IBusConnectionFactory.GetDisplayNumber(out hostname);
			Assert.AreEqual(8, displayNumber);
			Assert.AreEqual("foo", hostname);
		}

		[Test]
		public void GetDisplayNumber_DisplayWithHostnameWithPeriod()
		{
			Environment.SetEnvironmentVariable("DISPLAY", "foo.local:8.0");
			string hostname;
			int displayNumber = IBusConnectionFactory.GetDisplayNumber(out hostname);
			Assert.AreEqual(8, displayNumber);
			Assert.AreEqual("foo.local", hostname);
		}

		[Test]
		public void GetDisplayNumber_Invalid()
		{
			Environment.SetEnvironmentVariable("DISPLAY", "f.1");
			string hostname;
			int displayNumber = IBusConnectionFactory.GetDisplayNumber(out hostname);
			Assert.AreEqual(0, displayNumber);
			Assert.AreEqual("unix", hostname);
		}
	}
}
