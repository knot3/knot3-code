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
using System.Linq;

using NUnit.Framework;

using Knot3.Framework.Platform;

using Knot3.Game.Data;

using Knot3.MockObjects;

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

            Assert.Catch (() => {
                KnotGenerator.generateInvalidKnot ();
            }, "invalid Knot construction");
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

            Log.Debug ("Selected: ", knot.SelectedEdges);

            bool success;

            success = knot.IsValidDirection (direction: Direction.Left);
            Assert.IsFalse (success);
            success = knot.IsValidDirection (direction: Direction.Right);
            Assert.IsFalse (success);
            success = knot.IsValidDirection (direction: Direction.Up);
            Assert.IsTrue (success);
            success = knot.IsValidDirection (direction: Direction.Down);
            Assert.IsTrue (success);
            success = knot.IsValidDirection (direction: Direction.Forward);
            Assert.IsTrue (success);
            success = knot.IsValidDirection (direction: Direction.Backward);
            Assert.IsTrue (success);

            success = knot.Move (direction: Direction.Down, distance: 1);
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

            success = knot.Move (direction: Direction.Zero, distance: 3);
            Assert.AreEqual (knot.Count (), edges.Length, "Null-Move");
            success = knot.Move (direction: Direction.Left, distance: 0);
            Assert.AreEqual (knot.Count (), edges.Length, "Null-Move");
            success = knot.Move (direction: Direction.Zero, distance: 0);
            Assert.AreEqual (knot.Count (), edges.Length, "Null-Move");
        }

        [Test]
        public void Knot_Clone_Test ()
        {
            Knot knot = DefaultKnot;

            Knot cloned = knot.Clone () as Knot;
            Assert.AreEqual (knot, cloned);
            Assert.AreEqual (knot.Name, cloned.Name);

            knot.Name = "test2";
            Assert.AreNotEqual (knot.Name, cloned.Name);

            cloned = knot.Clone () as Knot;
            Assert.AreEqual (knot.Name, cloned.Name);
        }

        [Test]
        public void Knot_Selection_Test ()
        {
            Knot knot = DefaultKnot;
            Edge[] edges = knot.ToArray ();

            knot.AddRangeToSelection (edges [2]);
            Assert.AreEqual (1, knot.SelectedEdges.Count ());
            knot.ClearSelection ();
            Assert.AreEqual (0, knot.SelectedEdges.Count ());
            knot.AddToSelection (edges [0]);
            knot.RemoveFromSelection (edges [0]);
            knot.AddToSelection (edges [0]);
            Assert.AreEqual (1, knot.SelectedEdges.Count ());
            knot.AddRangeToSelection (edges [2]);
            Assert.AreEqual (3, knot.SelectedEdges.Count ());
            knot.RemoveFromSelection (edges [1]);
            Assert.AreEqual (2, knot.SelectedEdges.Count ());
            knot.ClearSelection ();
            Assert.AreEqual (0, knot.SelectedEdges.Count ());
            knot.AddToSelection (edges [0]);
            knot.AddRangeToSelection (edges [5]);
            Assert.AreEqual (2, knot.SelectedEdges.Count ());
        }

        [Test]
        public void Knot_ToString_Test ()
        {
            Knot knot = DefaultKnot;
            Assert.IsNotEmpty (knot.ToString ());
        }

        [Test]
        public void Knot_MoveCenterToZero_Test ()
        {
            Knot knot = DefaultKnot;

            knot.MoveCenterToZero ();
        }

        public Knot DefaultKnot
        {
            get {
                Edge[] edges = new Edge[] {
                    Edge.Up, Edge.Left, Edge.Left, Edge.Down, Edge.Right, Edge.Right
                };
                string name = "test";

                KnotMetaData metaData = new KnotMetaData (name: name, countEdges: () => edges.Length);
                Knot knot = new Knot (metaData, edges);
                return knot;
            }
        }

        [Test]
        public void Knot_Equals_Test ()
        {
            CircleEntry<Edge> start = new CircleEntry<Edge> (new Edge[] {
                Edge.Up,
                Edge.Left,
                Edge.Backward,
                Edge.Down,
                Edge.Right,
                Edge.Forward
            }
                                                            );
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
