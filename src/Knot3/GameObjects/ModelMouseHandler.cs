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
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Widgets;
using Knot3.Utilities;
using Knot3.Development;

#endregion

namespace Knot3.GameObjects
{
	/// <summary>
	/// Ein Inputhandler, der Mauseingaben auf 3D-Modellen verarbeitet.
	/// </summary>
	public sealed class ModelMouseHandler : GameScreenComponent
	{
		private World World;
		private double lastRayCheck = 0;
		private Vector2 lastMousePosition = Vector2.Zero;

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines ModelMouseHandler-Objekts und ordnet dieser ein IGameScreen-Objekt screen zu,
		/// sowie eine Spielwelt world.
		/// </summary>
		public ModelMouseHandler (IGameScreen screen, World world)
		: base (screen, DisplayLayer.None)
		{
			World = world;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		public override void Update (GameTime time)
		{
			CheckMouseRay (time);
		}

		private void CheckMouseRay (GameTime time)
		{
			double millis = time.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 10
			        && (Screen.Input.CurrentInputAction == InputAction.CameraTargetMove
			            || Screen.Input.CurrentInputAction == InputAction.FreeMouse)
			        && InputManager.CurrentMouseState.ToVector2 () != lastMousePosition) {
				//Log.Debug (Screen.Input.CurrentInputAction);
				lastRayCheck = millis;
				lastMousePosition = InputManager.CurrentMouseState.ToVector2 ();

				Profiler.ProfileDelegate ["Ray"] = () => {
					UpdateMouseRay (time);
				};
			}
		}

		private void UpdateMouseRay (GameTime time)
		{
			Ray ray = World.Camera.GetMouseRay (InputManager.CurrentMouseState.ToVector2 ());

			GameObjectDistance nearest = null;
			foreach (IGameObject obj in World.Objects) {
				if (obj.Info.IsVisible) {
					GameObjectDistance intersection = obj.Intersects (ray);
					if (intersection != null) {
						if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
							nearest = intersection;
						}
					}
				}
			}
			if (nearest != null) {
				World.SelectedObject = nearest.Object;
			}
			else {
				World.SelectedObject = null;
			}
		}

		#endregion
	}
}
