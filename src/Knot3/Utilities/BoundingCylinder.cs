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
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

#endregion

namespace Knot3.Utilities
{
	public struct BoundingCylinder : IEquatable<BoundingCylinder> {
		public Vector3 SideA;
		public Vector3 SideB;
		public float Radius;

		public BoundingCylinder (Vector3 sideA, Vector3 sideB, float radius)
		{
			this.SideA = sideA;
			this.SideB = sideB;
			this.Radius = radius;
		}

		public static bool operator == (BoundingCylinder a, BoundingCylinder b)
		{
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return a.Equals (b);
		}

		public static bool operator != (BoundingCylinder a, BoundingCylinder b)
		{
			return !(a == b);
		}

		public bool Equals (BoundingCylinder other)
		{
			return SideA == other.SideA && SideB == other.SideB && Radius == other.Radius;
		}

		public override bool Equals (object other)
		{
			return other != null && Equals ((BoundingCylinder)other);
		}

		[ExcludeFromCodeCoverageAttribute]
public override int GetHashCode ()
		{
			// irgendwas möglichst eindeutiges
			return (Radius * (SideA + SideB)).GetHashCode ();
		}
	}
}
