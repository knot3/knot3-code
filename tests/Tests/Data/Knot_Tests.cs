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

using NUnit.Framework;

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.Data;
using Knot3.RenderEffects;
using Knot3.MockObjects;

#endregion

namespace Knot3.UnitTests.Data
{
	[TestFixture]
	public class Knot_Tests
	{
		public Knot_Tests ()
		{
		}

		[Test, Description ("Knot Contruction")]
		public void Knot_Construct_Test ()
		{
			Edge[] edges = new Edge[] {
				Edge.Up, Edge.Left, Edge.Down, Edge.Right
			};
			string name = "test";

			KnotMetaData metaData = new KnotMetaData (name: name, countEdges: () => edges.Length);
			Knot knot = new Knot (metaData, edges);

			Assert.AreEqual (knot.Count (), edges.Length, "Knotenlänge #1");
			Assert.AreEqual (knot.MetaData.CountEdges, edges.Length, "Knotenlänge #2");

			Assert.AreEqual (knot.Name, name, "Knotenname #1");
			Assert.AreEqual (knot.MetaData.Name, name, "Knotenname #2");

			Assert.Catch (() => { KnotGenerator.generateInvalidKnot (); }, "invalid Knot construction");
		}

		[Test, Description ("Knot Move")]
		public void Knot_Move_Test ()
		{
			Edge[] edges = new Edge[] {
				Edge.Up, Edge.Left, Edge.Down, Edge.Right
			};
			string name = "test";

			KnotMetaData metaData = new KnotMetaData (name: name, countEdges: () => edges.Length);
			Knot knot = new Knot (metaData, edges);

			knot.AddToSelection (edges [1]); // Edge.Left

			bool success = knot.Move (direction: Direction.Down, distance: 1);
			Assert.IsFalse (success, "Nicht möglich! Knoten würde zu zwei Kanten zusammenfallen!");

			success = knot.Move (direction: Direction.Left, distance: 1);
			Assert.IsFalse (success, "Ungültige Richtung!");

			success = knot.Move (direction: Direction.Right, distance: 1);
			Assert.IsFalse (success, "Ungültige Richtung!");

			// nach oben schieben (1x)
			success = knot.Move (direction: Direction.Up, distance: 1);
			Assert.IsTrue (success, "Gültige Richtung!");

			Assert.AreEqual (knot.Count (), edges.Length + 2, "Knotenlänge nach Verschiebung #1");

			// noch mal nach oben schieben (2x)
			success = knot.Move (direction: Direction.Up, distance: 2);

			Assert.AreEqual (knot.Count (), edges.Length + 2 * 3, "Knotenlänge nach Verschiebung #2");

			// wieder nach unten schieben (3x)
			success = knot.Move (direction: Direction.Down, distance: 3);
			Assert.IsTrue (success, "Gültige Richtung!");

			Assert.AreEqual (knot.Count (), edges.Length, "Knotenlänge nach Verschiebung #3");
		}

		[Test]
		public void Knot_Equals_Test ()
		{
			CircleEntry<Edge> start = new CircleEntry<Edge>(new Edge[] { Edge.Up, Edge.Left, Edge.Backward, Edge.Down, Edge.Right, Edge.Forward });
			KnotMetaData metaData = new KnotMetaData (name: "test", countEdges: () => start.Count ());
			Knot knot = new Knot (metaData, start);
			for (int i = 0; i < 6; i++) {
				Assert.IsTrue (knot.Equals (new Knot (metaData, start)));
				start = start.Previous;
			}
			start.InsertBefore (Edge.Forward);
			start.InsertAfter (Edge.Backward);
			for (int i = 0; i < 6; i++) {
				Assert.IsFalse (knot.Equals (new Knot (metaData, start)));
				start = start.Previous;
			}
		}
	}
}
