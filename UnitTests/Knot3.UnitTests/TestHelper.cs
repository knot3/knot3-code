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
using System.IO;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Utilities;

#endregion

namespace Knot3.UnitTests
{
	[TestFixture]
	public static class TestHelper
	{
		private static Random random = new Random ();

		[SetUp]
		public static void TestHelper_SetUp ()
		{
			// Vorrang beim statischen Initialisieren!
			var bullshit = Options.Default;
			Options.Default = new ConfigFile (TempDirectory + "knot3.ini");
		}

		public static string TestResourcesDirectory
		{
			get {
				return SystemInfo.RelativeBaseDirectory + "Resources" + SystemInfo.PathSeparator;
			}
		}

		public static string TempDirectory
		{
			get {
				string directory;
				/*
				if (SystemInfo.IsRunningOnLinux ()) {
					directory = "/var/tmp/knot3-tests/";
				}
				else {
					directory = Path.GetTempPath () + "\\Knot3-Tests\\";
				}
				*/
				directory = "tmp" + SystemInfo.PathSeparator;
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		public static string RandomFilename (string extension)
		{
			string filename = TestHelper.TempDirectory + "test" + (random.Next () % 100) + "." + extension;

			try {
				File.Delete (filename);
			}
			catch (IOException) {
			}

			return filename;
		}
	}
}
