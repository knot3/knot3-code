using System;
using System.Linq;
using NUnit.Framework;
using Knot3.Platform;

namespace Knot3.UnitTests
{
	[TestFixture]
	public class SystemInfo_Tests
	{
		[Test]
		public void SystemInfo_Platform_Test ()
		{
			bool linux = SystemInfo.IsRunningOnLinux ();
			bool windows = SystemInfo.IsRunningOnWindows ();
			bool mono = SystemInfo.IsRunningOnMono ();
			bool monogame = SystemInfo.IsRunningOnMonogame ();

			Assert.False (linux && windows);
			Assert.False (mono && !monogame);
			Assert.False (linux && !mono);
		}
		
		[Test]
		public void SystemInfo_PathSep_Test ()
		{
			Assert.IsNotEmpty(SystemInfo.PathSeparator+"");
		}
		
		[Test]
		public void SystemInfo_Directory_Test ()
		{
			string[] directories = new string[] {
				SystemInfo.BaseDirectory,
				SystemInfo.DecodedMusicCache,
				SystemInfo.SavegameDirectory,
				SystemInfo.ScreenshotDirectory,
				SystemInfo.SettingsDirectory,
			};
			foreach (string directory in directories) {
				Assert.IsNotEmpty (directory);
			}
		}
	}
}

