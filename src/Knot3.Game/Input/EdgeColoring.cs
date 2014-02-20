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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Input
{
	[ExcludeFromCodeCoverageAttribute]
	public class EdgeColoring : GameScreenComponent, IKeyEventListener
	{
		public Knot Knot { get; set; }

		public EdgeColoring (GameScreen screen)
		: base (screen, DisplayLayer.None)
		{
			ValidKeys = new List<Keys> ();
			ValidKeys.Add (Keys.C);
		}

		[ExcludeFromCodeCoverageAttribute]
		public override void Update (GameTime time)
		{
		}

		public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			// Soll die Farbe geändert wurde?
			if (   Knot.SelectedEdges.Any ()
			        && Keys.C.IsDown ()) {
				Color currentColor = Knot.SelectedEdges.ElementAt (0);
				ColorPickDialog picker = new ColorPickDialog (
				    screen: Screen,
				    drawOrder: DisplayLayer.Dialog,
				    selectedColor: currentColor
				);
				foreach (Edge edge in Knot.SelectedEdges) {
					picker.Close += (t) => {
						edge.Color = picker.SelectedColor;
					};
				}
				Knot.EdgesChanged ();
				Screen.AddGameComponents (time, picker);
			}
		}

		public List<Keys> ValidKeys { get; private set; }

		public bool IsKeyEventEnabled { get { return true; } }

		public bool IsModal { get { return false; } }
	}
}
