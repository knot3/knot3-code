/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 *
 * See the LICENSE file for full license details of the Knot3 project.
 */

using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using Knot3.Framework.Storage;

namespace Knot3.UnitTests.Storage
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class BooleanOption_Tests
    {
        [SetUp]
        public void Init ()
        {
        }

        [Test]
        public void BooleanOptionInfo_Constructor_Test ()
        {
            string name = "test-option";
            string section = "test-section";
            bool defaultValue = false;

            ConfigFile configFile = new ConfigFile (TestHelper.RandomFilename (extension: "ini"));

            BooleanOption option = new BooleanOption (section, name, defaultValue, configFile);

            Assert.IsFalse (option.Value);
            string falseStr = option.DisplayValue;
            Assert.IsTrue (option.DisplayValidValues.ContainsKey (falseStr));

            option.Value = true;
            Assert.IsTrue (option.Value);
            string trueStr = option.DisplayValue;
            Assert.IsTrue (option.DisplayValidValues.ContainsKey (trueStr));

            Assert.AreNotEqual (falseStr, trueStr);

            option.Value = !option.Value;
            Assert.IsFalse (option.Value);
            Assert.AreEqual (falseStr, option.DisplayValue);

            option.Value = !option.Value;
            Assert.IsTrue (option.Value);
            Assert.AreEqual (trueStr, option.DisplayValue);

            (option as DistinctOption).Value = "invalid!!";
            Assert.AreEqual (option.Value, defaultValue);
        }
    }
}
