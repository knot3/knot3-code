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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;

using Knot3.Game.Data;
using Knot3.Game.Models;

using Knot3.MockObjects;

namespace Knot3.UnitTests.Core
{
    [TestFixture]
    public class World_Tests
    {
        private IScreen screen;
        private IRenderEffect effect;

        [Test, Description ("World Add/Remove")]
        public void AddRemoveTest ()
        {
            screen = screen ?? new FakeScreen ();
            effect = effect ?? new FakeEffect (screen);

            // Erstelle einen Knoten
            Knot knot = new Knot ();

            // Erstelle eine Rasterpunkt-Zuordnung
            NodeMap nodeMap = new NodeMap (screen: screen);
            nodeMap.Edges = knot;

            List<Pipe> models = new List<Pipe> ();

            // Erstelle ein paar Pipes
            foreach (Edge edge in knot) {
                Pipe pipe = new Pipe (screen: screen, nodeMap: nodeMap, knot: knot, edge: edge);
                models.Add (pipe);
            }
            Assert.AreEqual (knot.Count (), models.Count (), "Für jede Edge eine Pipe");

            return;
            // das hier zu sehr in XNA verwoben, macht als test wahrscheinlich keinen sinn!!

            /*

            World world = new World (screen: screen, effect: effect);

            foreach (PipeModel model in models) {
            	world.Add (model);
            }

            Assert.AreEqual (knot.Count (), world.Count (), "Anzahl GameObjects");

            foreach (PipeModel model in models) {
            	world.Add (model);
            }

            Assert.AreEqual (knot.Count (), world.Count (), "GameObjects sind Unique");

            foreach (PipeModel model in models) {
            	world.Remove (model);
            }

            Assert.AreEqual (0, world.Count (), "Leere World");

            */
        }
    }
}
