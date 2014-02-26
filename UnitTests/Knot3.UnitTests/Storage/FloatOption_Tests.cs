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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;

namespace Knot3.UnitTests.Storage
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class FloatOption_Tests
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
            float defaultValue = 5f;
            float[] validValues = new float[] { 0f, 5f, 10f, 15f, 20f };

            ConfigFile configFile = new ConfigFile (TestHelper.RandomFilename (extension: "ini"));

            FloatOption option = new FloatOption (section, name, defaultValue, validValues, configFile);

            Assert.AreEqual (option.Value, defaultValue);
            string defaultStr = option.DisplayValue;
            Assert.IsTrue (option.DisplayValidValues.ContainsKey (defaultStr));

            option.Value = 10f;
            Assert.AreEqual (option.Value, 10f);
            string tenStr = option.DisplayValue;
            Assert.IsTrue (option.DisplayValidValues.ContainsKey (tenStr));

            Assert.AreNotEqual (defaultStr, tenStr);

            option.Value = 99f;
            Assert.AreEqual (option.Value, defaultValue);
            Assert.AreEqual (defaultStr, option.DisplayValue);

            (option as DistinctOption).Value = "invalid!!";
            Assert.AreEqual (option.Value, defaultValue);
            Assert.AreEqual (defaultStr, option.DisplayValue);
        }
    }
}
