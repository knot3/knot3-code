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

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Utilities;

#endregion

namespace Knot3.UnitTests.Math
{
    /// <summary>
    /// Zusammenfassungsbeschreibung für Test_Angles3
    /// </summary>
    [TestFixture]
    public class Angles3_Tests
    {
        float X, Y, Z;
        float rX, rY, rZ;
        float redianX;
        float redianY;
        float redianZ;
        float scaleFactor;
        float divider;
        Angles3 angle1;
        Angles3 angle2;
        Angles3 angle3;
        Vector3 redian;
        object obj;

        [SetUp]
        public void Init ()
        {
            X = 120;
            Y = 50;
            Z = 280;

            rX = 2.0943951023931953f;
            rY = 0.8726646259971648f;
            rZ = 4.886921905584122f;

            redianX = X * ((float)System.Math.PI / 180);
            redianY = Y * ((float)System.Math.PI / 180);
            redianZ = Z * ((float)System.Math.PI / 180);

            redian = new Vector3 (redianX, redianY, redianZ);
            angle1 = new Angles3 (redian);
            angle3 = new Angles3 (rX, rY, rZ);

            obj = angle1;
            scaleFactor = 2.5f;
            divider = 1;
        }

        [Test]
        public void Angles3_FromDegrees_Test ()
        {
            angle2 = Angles3.FromDegrees (X, Y, Z);
            Assert.AreEqual (angle1, angle2);
        }

        [Test]
        public void Angles3_ToDegrees_Test ()
        {
            angle1.ToDegrees (out X, out Y, out Z);
            Assert.AreEqual (angle1, angle3);
        }

        [Test]
        public void Angles3_ToString_Test ()
        {
            Assert.IsNotEmpty (angle1.ToString ());
            Assert.AreNotEqual (angle1.GetHashCode (), (angle1 + angle2).GetHashCode ());
        }

        [Test]
        public void Angles3_Equals_Test ()
        {
            Assert.AreEqual (true, angle1.Equals (angle1));
            Assert.AreEqual (true, angle1.Equals (obj));
        }

        [Test]
        public void Angles3_Operator_Test ()
        {
            Angles3 angle2 = new Angles3 (1, 2, 3);
            Angles3 angle4 = new Angles3 (3, 2, 1);
            Angles3 sum = angle2 + angle4;
            Assert.AreEqual (sum, new Angles3 (4, 4, 4));
            Angles3 neg = -angle2;
            Assert.AreEqual (neg, new Angles3 (-1, -2, -3));
            Angles3 diff = angle2 - angle4;
            Assert.AreEqual (diff, new Angles3 (-2, 0, 2));
            Angles3 prod = angle2 * angle4;
            Assert.AreEqual (prod, new Angles3 (3, 4, 3));
            Angles3 scale1 = angle2 * scaleFactor;
            Angles3 scale2 = scaleFactor * angle2;
            Assert.AreEqual (scale1, new Angles3 (2.5f, 5, 7.5f));
            Assert.AreEqual (scale2, new Angles3 (2.5f, 5, 7.5f));
            Angles3 quot1 = angle2 / angle4;
            Assert.AreEqual (quot1, new Angles3 (0.33333333333f, 1, 3));
            Angles3 quot2 = angle2 / divider;
            Assert.AreEqual (quot2, angle2);
            Assert.AreEqual (angle2, angle2);
            Assert.AreNotEqual (angle2, angle4);
            Assert.IsTrue (angle1 != angle2);
        }
    }
}
