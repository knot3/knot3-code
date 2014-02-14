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

using System;
using System.Collections;
using System.Collections.Generic;
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

using Knot3.Core;
using Knot3.Input;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Utilities;
using Knot3.Widgets;

namespace Knot3.Input
{
	/// <summary>
	/// Ein Inputhandler, der Tastatureingaben auf Widgets verarbeitet.
	/// </summary>
	public sealed class WidgetKeyHandler : GameScreenComponent
	{
		public WidgetKeyHandler (IGameScreen screen)
		: base (screen, DisplayLayer.None)
		{
		}

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		public override void Update (GameTime time)
		{
			foreach (IKeyEventListener component in Screen.Game.Components.OfType<IKeyEventListener>()
			         .Where (c => c.IsKeyEventEnabled).OrderByDescending (c => c.Index.Index)) {
				// keyboard input
				KeyEvent keyEvent = KeyEvent.None;
				List<Keys> keysInvolved = new List<Keys> ();

				foreach (Keys key in component.ValidKeys) {
					// Log.Debug ("receiver=",receiver,",validkeys=",key,", receiver.IsKeyEventEnabled=",((dynamic)receiver).IsVisible);

					if (key.IsDown ()) {
						keysInvolved.Add (key);
						keyEvent = KeyEvent.KeyDown;
					}
					else if (key.IsHeldDown ()) {
						keysInvolved.Add (key);
						keyEvent = KeyEvent.KeyHeldDown;
					}
				}

				if (keysInvolved.Count > 0) {
					component.OnKeyEvent (keysInvolved, keyEvent, time);
					break;
				}
				if (component.IsModal) {
					break;
				}
			}
		}
	}
}
