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

		[OneTimeSetUp]
		public void FixtureSetup()
		{
			m_OriginalDisplay = Environment.GetEnvironmentVariable("DISPLAY");
		}

		[OneTimeTearDown]
		public void FixtureTearDown()
		{
			Environment.SetEnvironmentVariable("DISPLAY", m_OriginalDisplay);
		}

		[Test]
		public void GetDisplayNumber_NoEnv()
		{
			Environment.SetEnvironmentVariable("DISPLAY", null);
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			Assert.That(displayNumber, Is.EqualTo(0));
			Assert.That(hostname, Is.EqualTo("unix"));
		}

		[Test]
		public void GetDisplayNumber_Flatpak()
		{
			string displayInFlatpak = ":99.0";
			Environment.SetEnvironmentVariable("DISPLAY", displayInFlatpak);
			Environment.SetEnvironmentVariable("FLATPAK_ID", "org.foo.ClientApp");
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			int assumedDisplayWhenFlatpak = 0;
			Assert.That(displayNumber, Is.EqualTo(assumedDisplayWhenFlatpak));
			Assert.That(hostname, Is.EqualTo("unix"));
		}

		[Test]
		public void GetDisplayNumber_DisplayOnly()
		{
			Environment.SetEnvironmentVariable("DISPLAY", ":5");
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			Assert.That(displayNumber, Is.EqualTo(5));
			Assert.That(hostname, Is.EqualTo("unix"));
		}

		[Test]
		public void GetDisplayNumber_DisplayWithScreen()
		{
			Environment.SetEnvironmentVariable("DISPLAY", ":6.7");
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			Assert.That(displayNumber, Is.EqualTo(6));
			Assert.That(hostname, Is.EqualTo("unix"));
		}

		[Test]
		public void GetDisplayNumber_DisplayWithHostname()
		{
			Environment.SetEnvironmentVariable("DISPLAY", "foo:8.0");
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			Assert.That(displayNumber, Is.EqualTo(8));
			Assert.That(hostname, Is.EqualTo("foo"));
		}

		[Test]
		public void GetDisplayNumber_DisplayWithHostnameWithPeriod()
		{
			Environment.SetEnvironmentVariable("DISPLAY", "foo.local:8.0");
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			Assert.That(displayNumber, Is.EqualTo(8));
			Assert.That(hostname, Is.EqualTo("foo.local"));
		}

		[Test]
		public void GetDisplayNumber_Invalid()
		{
			Environment.SetEnvironmentVariable("DISPLAY", "f.1");
			var displayNumber = IBusConnectionFactory.GetDisplayNumber(out var hostname);
			Assert.That(displayNumber, Is.EqualTo(0));
			Assert.That(hostname, Is.EqualTo("unix"));
		}
	}
}
