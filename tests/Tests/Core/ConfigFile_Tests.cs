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
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Knot3.Core;
using Knot3.Data;
using Knot3.GameObjects;
using Knot3.RenderEffects;
using Knot3.Utilities;

#endregion

namespace Knot3.UnitTests.Core
{
	[TestFixture]
	public class ConfigFile_Tests
	{
		private static Random random = new Random ();

		[Test]
		public void ConfigFile_RandomAccess_Test ()
		{
			int countSections = 10;
			int countKeysPerSection = 10;
			string filename = TestHelper.TempDirectory + "test.ini";

			try {
				File.Delete (filename);
			}
			catch (IOException ex) {
			}
			ConfigFile cfg = new ConfigFile (filename);
			Console.WriteLine (filename);

			string[] sections = RandomStrings (count: countSections).ToArray ();
			Dictionary<string, string[]> keyMap = new Dictionary<string, string[]> ();
			sections.ForEach (section => keyMap [section] = RandomStrings (count: countKeysPerSection).ToArray ());

			Dictionary<string, string[]> stringValueMap = new Dictionary<string, string[]> ();
			sections.ForEach (section => stringValueMap [section] = RandomStrings (count: countKeysPerSection).ToArray ());
			string stringDefaultValue = "default";

			foreach (string section in sections) {
				string[] keys = keyMap [section];
				string[] values = stringValueMap [section];

				keys.Length.Repeat (i => Assert.AreEqual (stringDefaultValue, cfg [section, keys [i], stringDefaultValue]));
				keys.Length.Repeat (i => cfg [section, keys [i], stringDefaultValue] = values [i]);
				keys.Length.Repeat (i => Assert.AreEqual (values [i], cfg [section, keys [i], stringDefaultValue]));
			}

			Dictionary<string, float[]> floatValueMap = new Dictionary<string, float[]> ();
			sections.ForEach (section => floatValueMap [section] = RandomFloats (count: countKeysPerSection).ToArray ());
			float floatDefaultValue = -1;

			foreach (string section in sections) {
				string[] keys = keyMap [section];
				float[] values = floatValueMap [section];

				keys.Length.Repeat (i => Assert.Greater (Math.Abs (values [i] - cfg [section, keys [i], floatDefaultValue]), 0.00001f));
				keys.Length.Repeat (i => cfg [section, keys [i], floatDefaultValue] = values [i]);
				keys.Length.Repeat (i => Assert.Less (Math.Abs (values [i] - cfg [section, keys [i], floatDefaultValue]), 0.001f));
			}

			Dictionary<string, bool[]> boolValueMap = new Dictionary<string, bool[]> ();
			sections.ForEach (section => boolValueMap [section] = RandomBooleans (count: countKeysPerSection).ToArray ());
			bool boolDefaultValue = false;

			foreach (string section in sections) {
				string[] keys = keyMap [section];
				bool[] values = boolValueMap [section];

				keys.Length.Repeat (i => cfg [section, keys [i], boolDefaultValue] = values [i]);
				keys.Length.Repeat (i => Assert.AreEqual (values [i], cfg [section, keys [i], boolDefaultValue]));
			}
		}

		public static string RandomString (int length)
		{
			var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string (Enumerable.Repeat (chars, length).Select (s => s [random.Next (s.Length)]).ToArray ());
		}

		public static IEnumerable<string> RandomStrings (int count, int length = 30)
		{
			return count.Repeat (i => RandomString (length: length));
		}

		public static IEnumerable<float> RandomFloats (int count)
		{
			return count.Repeat (i => (float)random.NextDouble ());
		}

		public static IEnumerable<bool> RandomBooleans (int count)
		{
			return count.Repeat (i => random.Next (0, 100) < 50);
		}

		[Test]
		public void Test_2 ()
		{
		}
	}
}
