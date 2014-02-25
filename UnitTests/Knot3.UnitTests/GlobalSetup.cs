using System;
using NUnit.Framework;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using System.IO;

namespace Knot3.UnitTests
{
	[SetUpFixture]
	public class GlobalSetup
	{
		[SetUp]
		public void RunBeforeAnyTests ()
		{
			Log.Message ("Start Unit Tests...");
			
			Directory.Delete (TestHelper.TempDirectory, true);
			Directory.CreateDirectory (TestHelper.TempDirectory);

			// Temporäre Verzeichnisse nur für die Tests zuweisen!
			SystemInfo.RelativeBaseDirectory = TestHelper.TempDirectory;
			SystemInfo.SettingsDirectory = TestHelper.TempDirectory;
			Config.Default = new ConfigFile (TestHelper.TempDirectory + "knot3.ini");
		}
		
		[TearDown]
		public void RunAfterAnyTests ()
		{
			Log.Message ("Stop Unit Tests...");
		}
	}
}

