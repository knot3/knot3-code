using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.KnotData;
using Knot3.RenderEffects;
using Knot3.UnitTests.MockObjects;

namespace Knot3.UnitTests.Knot_Tests
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
