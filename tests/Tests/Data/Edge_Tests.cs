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

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Core;
using Knot3.Data;
using Knot3.GameObjects;
using Knot3.RenderEffects;

#endregion

namespace Knot3.UnitTests
{
	[TestFixture]
	public class Edge_Tests
	{
		[Test]
		public void Edge_Equals_Tests ()
		{
			Assert.AreNotEqual (Edge.Left, Edge.Left);
			Assert.AreNotEqual (Edge.Right, Edge.Right);
			Assert.AreNotEqual (Edge.Down, Edge.Down);
			Assert.AreNotEqual (Edge.Up, Edge.Up);
			Assert.AreNotEqual (Edge.Forward, Edge.Forward);
			Assert.AreNotEqual (Edge.Backward, Edge.Backward);

			Assert.AreEqual ((Direction)Edge.Right, (Direction)Edge.UnitX);
			Assert.AreEqual ((Direction)Edge.Up, (Direction)Edge.UnitY);
			Assert.AreEqual ((Direction)Edge.Backward, (Direction)Edge.UnitZ);

			Assert.AreNotEqual (Edge.Zero, Edge.Zero);
			Assert.AreEqual ((Direction)Edge.Zero, Direction.Zero);
		}

		[Test]
		public void Edge_Nonsense_Tests ()
		{
			Assert.False (Edge.Zero.Equals (null));
			Assert.False (Edge.Zero.Equals (0f));
			Assert.False (Direction.Zero.Equals ((object)null));
			Assert.False (Direction.Zero.Equals ((object)0f));
			Assert.False (Direction.Zero.Equals ((object)new Quaternion (0, 0, 0, 0)));
			Assert.False (Direction.Zero.Equals ((object)Direction.Down));
			Assert.False (Direction.Zero.Equals ((object)Direction.Down.Vector));
			Assert.False (Direction.Zero.Equals ((object)Direction.Down.Description));
			Assert.False (Direction.Zero.Equals ((object)""));
		}

		[Test]
		public void Edge_ToString_Tests ()
		{
			Assert.IsNotEmpty (Edge.Zero.ToString ());
			Assert.IsNotEmpty (Edge.Zero.GetHashCode () + "");
		}

		[Test]
		public void Edge_FromString_Tests ()
		{
			Assert.AreEqual (Direction.FromString ("Left"), Edge.Left.Direction);
			Assert.AreEqual (Direction.FromString ("Right"), Edge.Right.Direction);
			Assert.AreEqual (Direction.FromString ("Down"), Edge.Down.Direction);
			Assert.AreEqual (Direction.FromString ("Up"), Edge.Up.Direction);
			Assert.AreEqual (Direction.FromString ("Forward"), Edge.Forward.Direction);
			Assert.AreEqual (Direction.FromString ("Backward"), Edge.Backward.Direction);
		}

		[Test]
		public void Edge_FromAxis_Tests ()
		{
			Assert.AreEqual (Direction.FromAxis (Axis.X).Axis, Axis.X);
			Assert.AreEqual (Direction.FromAxis (Axis.Y).Axis, Axis.Y);
			Assert.AreEqual (Direction.FromAxis (Axis.Z).Axis, Axis.Z);
		}

		[Test]
		public void Edge_Random_Tests ()
		{
			Assert.IsNotNull (Edge.RandomEdge ());
			Assert.IsNotNull (Edge.RandomColor (new GameTime (new TimeSpan (0), new TimeSpan (0))));
		}

		[Test]
		public void Edge_Math_Tests ()
		{
			Assert.AreEqual (Direction.Left.Vector * 3, Direction.Left * 3);
			Assert.AreEqual (Direction.Left.Vector / 3, Direction.Left / 3);
			Assert.AreEqual (Direction.Left.Vector + Direction.Forward.Vector, Direction.Left + Direction.Forward);
			Assert.AreEqual (Direction.Left.Vector - Direction.Forward.Vector, Direction.Left - Direction.Forward);
		}

		[Test]
		public void Edge_Clone_Tests ()
		{
			Edge orig = Edge.Left;
			Edge cloned = orig.Clone () as Edge;
			Assert.AreNotEqual (orig, cloned);
			Assert.AreEqual (orig.Direction, cloned.Direction);
			Assert.AreEqual ((Direction)orig, (Direction)cloned);
		}
	}
}
