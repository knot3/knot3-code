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
using System.Linq;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.GameObjects;
using Knot3.MockObjects;
using Knot3.Game.RenderEffects;

#endregion

namespace Knot3.UnitTests.Core
{
	[TestFixture]
	public class World_Tests
	{
		private IGameScreen screen;
		private IRenderEffect effect;

		[Test, Description ("World Add/Remove")]
		public void AddRemoveTest ()
		{
			screen = screen ?? new FakeScreen ();
			effect = effect ?? new FakeEffect (screen);

			// Erstelle einen Knoten
			Knot knot = new Knot ();

			// Erstelle eine Rasterpunkt-Zuordnung
			NodeMap nodeMap = new NodeMap (knot);

			List<PipeModel> models = new List<PipeModel> ();

			// Erstelle ein paar Pipes
			foreach (Edge edge in knot) {
				PipeModelInfo pipeInfo = new PipeModelInfo (nodeMap: nodeMap, knot: knot, edge: edge);
				PipeModel pipe = new PipeModel (screen: screen, info: pipeInfo);
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
