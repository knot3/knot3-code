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
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;

#endregion

namespace Knot3.UnitTests.Core
{
	/// <summary>
	///
	/// </summary>
	[TestFixture]
	public class KeyOptionInfo_Tests
	{
		[SetUp]
		public void Init ()
		{
		}

		[Test]
		public void Test ()
		{
			string name = "test-option";
			string section = "test-section";
			Keys defaultValue = Keys.Escape;

			ConfigFile configFile = new ConfigFile (TestHelper.RandomFilename (extension: "ini"));

			KeyOptionInfo option = new KeyOptionInfo (section, name, defaultValue, configFile);

			Assert.AreEqual (option.Value, defaultValue);
			string defaultStr = option.DisplayValue;
			Assert.IsTrue (option.DisplayValidValues.ContainsKey (defaultStr));

			option.Value = Keys.LeftShift;
			Assert.AreEqual (option.Value, Keys.LeftShift);
			string tenStr = option.DisplayValue;
			Assert.IsTrue (option.DisplayValidValues.ContainsKey (tenStr));

			Assert.AreNotEqual (defaultStr, tenStr);

			(option as DistinctOptionInfo).Value = "invalid!!";
			Assert.AreEqual (option.Value, defaultValue);
			Assert.AreEqual (defaultStr, option.DisplayValue);
		}
	}
}
