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

using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Framework.Math;

namespace Knot3.UnitTests.Math
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class BoundingCylinder_Tests
    {
        [SetUp]
        public void Init ()
        {
        }

        [Test]
        public void BoundingCylinder_Equals_Test ()
        {
            BoundingCylinder cyl1 = new BoundingCylinder (sideA: Vector3.Zero, sideB: Vector3.Up * 100, radius: 100f);
            BoundingCylinder cyl2 = new BoundingCylinder (sideA: Vector3.Up * 100, sideB: Vector3.Zero, radius: 100f);
            IsEqual (cyl1, cyl1);
            IsEqual (cyl1, cyl2);
            IsEqual (cyl2, cyl1);

            BoundingCylinder cyl3 = new BoundingCylinder (sideA: Vector3.Up * 100, sideB: Vector3.Zero, radius: 200f);
            IsNotEqual (cyl1, cyl3);
            IsNotEqual (cyl2, cyl3);
            IsNotEqual (cyl3, cyl1);
            IsNotEqual (cyl3, cyl2);

            BoundingCylinder cyl4 = new BoundingCylinder (sideA: Vector3.Up * 50, sideB: Vector3.Down * 50, radius: 100f);
            IsNotEqual (cyl1, cyl4);
            IsNotEqual (cyl2, cyl4);
            IsNotEqual (cyl4, cyl1);
            IsNotEqual (cyl4, cyl2);

            Assert.IsTrue (cyl1.Equals ((object)cyl1));
            Assert.IsFalse (cyl1.Equals ((object)cyl4));
            Assert.IsFalse (cyl1.Equals ((object)null));
        }

        [Test]
        public void BoundingCylinder_Ray_Test ()
        {
            BoundingCylinder cyl = new BoundingCylinder (sideA: Vector3.Zero, sideB: Vector3.Up * 100, radius: 100f);
            Ray ray;
            ray = new Ray (position: new Vector3 (0, 0, 0), direction: Vector3.Up);
            Assert.IsNotNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, 0, 0), direction: Vector3.Down);
            Assert.IsNotNull (ray.Intersects (cyl));

            ray = new Ray (position: new Vector3 (0, 0, 0), direction: Vector3.Forward);
            Assert.IsNotNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, 0, 0), direction: Vector3.Left);
            Assert.IsNotNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, -1, 0), direction: Vector3.Forward);
            Assert.IsNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, -1, 0), direction: Vector3.Left);
            Assert.IsNull (ray.Intersects (cyl));

            // hier sollte eigentlich (0,100,0) auch noch drin sein, nicht nur (0,99,0),
            // also wie bei SideA!
            ray = new Ray (position: new Vector3 (0, 99, 0), direction: Vector3.Forward);
            Assert.IsNotNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, 99, 0), direction: Vector3.Left);
            Assert.IsNotNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, 101, 0), direction: Vector3.Forward);
            Assert.IsNull (ray.Intersects (cyl));
            ray = new Ray (position: new Vector3 (0, 101, 0), direction: Vector3.Left);
            Assert.IsNull (ray.Intersects (cyl));
        }

        private void IsEqual (BoundingCylinder a, BoundingCylinder b)
        {
            Assert.IsTrue (a.Equals (b));
            Assert.IsTrue (a == b);
        }

        private void IsNotEqual (BoundingCylinder a, BoundingCylinder b)
        {
            Assert.IsTrue (!a.Equals (b));
            Assert.IsTrue (a != b);
        }
    }
}
