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

using Knot3.Framework.Core;

using Knot3.MockObjects;

namespace Knot3.UnitTests
{
    [TestFixture]
    public class DisplayLayer_Tests
    {
        IScreen screen;

        [SetUp]
        public void DisplayLayer_Setup ()
        {
            screen = new FakeScreen ();
        }

        [Test]
        public void DisplayLayer_Equals_Tests ()
        {
            foreach (DisplayLayer lay1 in DisplayLayer.Values) {
                Assert.AreEqual (lay1, lay1);
                Assert.IsTrue (lay1.Equals (lay1));
                Assert.IsTrue (lay1.Equals (lay1));
                Assert.IsFalse (lay1.Equals ((object)null));
                Assert.IsFalse (lay1.Equals ((DisplayLayer)null));
                foreach (DisplayLayer lay2 in DisplayLayer.Values) {
                    if (lay1.Index == lay2.Index) {
                        Assert.AreEqual (lay1, lay2);
                    }
                    else {
                        Assert.AreNotEqual (lay1, lay2);
                    }
                }
            }
        }

        [Test]
        public void DisplayLayer_Implicit_Tests ()
        {
            foreach (DisplayLayer lay in DisplayLayer.Values) {
                int index = lay;
                Assert.AreEqual (lay.Index, index);
            }
        }

        [Test]
        public void DisplayLayer_Math_Tests ()
        {
            foreach (DisplayLayer lay1 in DisplayLayer.Values) {
                foreach (DisplayLayer lay2 in DisplayLayer.Values) {
                    Assert.AreEqual (lay1 + lay2, lay2 + lay1);
                    Assert.AreEqual (lay1 + lay2, lay1 + new FakeWidget (screen, lay2));
                }
                Assert.AreEqual ((lay1 * 99).Index, lay1.Index * 99);
            }
        }

        [Test]
        public void DisplayLayer_ToString_Tests ()
        {
            Assert.IsNotEmpty (DisplayLayer.Background.ToString ());
            Assert.IsNotEmpty (DisplayLayer.Background.GetHashCode () + "");
        }
    }
}
