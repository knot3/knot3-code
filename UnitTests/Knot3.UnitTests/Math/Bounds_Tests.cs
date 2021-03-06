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

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Framework.Math;

using Knot3.MockObjects;

namespace Knot3.UnitTests.Math
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für Test_Bounds
    /// </summary>
    [TestFixture]
    public class Bounds_Tests
    {
        FakeScreen fakeScreen;
        ScreenPoint point;
        ScreenPoint size;
        ScreenPoint testPoint;
        Bounds bound;

        [SetUp]
        public void Init ()
        {
            fakeScreen = new FakeScreen ();
            point = new ScreenPoint (fakeScreen, 0, 0);
            size = new ScreenPoint (fakeScreen, 1, 1);
            testPoint = new ScreenPoint (fakeScreen, 0.5f, 0.5f);
            bound = new Bounds (point, size);
        }

        [Test]
        public void Bounds_Contains_Test ()
        {
            Assert.AreEqual (true, bound.Contains (testPoint));
            Assert.AreEqual (true, bound.Contains ((Point)testPoint));
        }

        [Test]
        public void Bounds_FromDirection_Test ()
        {
            //top
            Bounds compareBound = new Bounds (point, new ScreenPoint (fakeScreen, 1f, 0.9f));
            Assert.IsTrue (boundsEqual (compareBound, bound.FromTop (0.9f)), "top");
            //bottom
            compareBound = new Bounds (new ScreenPoint (fakeScreen, 0f, 0.1f), new ScreenPoint (fakeScreen, 1f, 0.9f));
            Assert.IsTrue (boundsEqual (bound.FromBottom (0.9f), compareBound), "bottom");
            //right
            compareBound = new Bounds (new ScreenPoint (fakeScreen, 0.1f, 0f), new ScreenPoint (fakeScreen, 0.9f, 1f));
            Assert.IsTrue (boundsEqual (bound.FromRight (0.9f), compareBound), "right");
            //left
            compareBound = new Bounds (point, new ScreenPoint (fakeScreen, 0.9f, 1f));
            Assert.IsTrue (boundsEqual (bound.FromLeft (0.9f), compareBound), "left");
        }

        [Test]
        public void Bounds_Grow_Test ()
        {
            FakeScreen screen2 = new FakeScreen (width: 1000, height: 1000);
            Bounds bounds = new Bounds (screen: screen2, relX: 0.100f, relY: 0.100f, relWidth: 0.300f, relHeight: 0.700f);
            Assert.IsTrue (boundsEqual (bounds.Grow (5), new Bounds (screen: screen2, relX: 0.095f, relY: 0.095f, relWidth: 0.310f, relHeight: 0.710f)));
            Assert.IsTrue (boundsEqual (bounds.Grow (5, 2), new Bounds (screen: screen2, relX: 0.095f, relY: 0.098f, relWidth: 0.310f, relHeight: 0.704f)));
            Assert.IsTrue (boundsEqual (bounds.Shrink (5), new Bounds (screen: screen2, relX: 0.105f, relY: 0.105f, relWidth: 0.290f, relHeight: 0.690f)));
            Assert.IsTrue (boundsEqual (bounds.Shrink (5, 2), new Bounds (screen: screen2, relX: 0.105f, relY: 0.102f, relWidth: 0.290f, relHeight: 0.696f)));
        }

        [Test]
        public void Bounds_Set_Test ()
        {
            ScreenPoint newSize = new ScreenPoint (fakeScreen, 1f, 0.9f);
            Bounds compareBound = new Bounds (point, newSize);
            bound.Size = newSize;
            Assert.AreEqual (true, boundsEqual (compareBound, bound));
        }

        [Test]
        public void Bounds_Get_Test ()
        {
            Assert.AreEqual (point, bound.Padding);
            Assert.AreEqual (point, bound.Position);
            Vector4 vector = bound.Vector4;
            Vector4 vector2 = new Vector4 (0,0,800,600);
            Assert.AreEqual (vector, vector2);
        }

        [Test]
        public void Bounds_In_Test ()
        {
            Bounds bound2 = new Bounds (point, size);
            Assert.AreEqual (true, boundsEqual (bound.In (bound),bound2));
        }

        private bool boundsEqual (Bounds a, Bounds b)
        {
            if (a.Position.Equals (b.Position) && a.Size.Equals (b.Size)) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
