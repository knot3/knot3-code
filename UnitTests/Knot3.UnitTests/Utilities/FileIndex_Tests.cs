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

using Knot3.Game.Utilities;

namespace Knot3.UnitTests.Utilities
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class FileIndex_Tests
    {
        private string teststring1;
        private string teststring2;
        private string testfilename;
        [SetUp]
        public void Init ()
        {
            teststring1 = "hallo";
            teststring2 = "tschüss";
            testfilename = "Test";
        }

        [Test]
        public void Test ()
        {
            FileIndex index1 = new FileIndex (testfilename);
            index1.Add (teststring1);
            index1.Add (teststring2);
            Assert.IsTrue (index1.Contains (teststring1), "contains teststring1");
            Assert.IsTrue (index1.Contains (teststring2), "contains teststring2");
            index1.Remove (teststring2);
            Assert.IsTrue (index1.Contains (teststring1), "still contains teststring1");
            Assert.IsFalse (index1.Contains (teststring2), "not containing teststring2 anymore");
            FileIndex index2 = new FileIndex (testfilename);
            Assert.IsTrue (index2.Contains (teststring1), "index loads right file");
        }
    }
}
