#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion

#region Using

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

#endregion

namespace Knot3.UnitTests.Utilities
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
			Console.WriteLine ("Environment.OSVersion.Platform=" + Environment.OSVersion.Platform);
			Console.WriteLine ("PlatformID.Unix=" + PlatformID.Unix);
			Assert.IsNotEmpty (SystemInfo.PathSeparator + "");
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