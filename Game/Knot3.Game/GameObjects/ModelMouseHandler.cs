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
using Knot3.Framework.Math;

#endregion

#region Using

using System;
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

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.GameObjects;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Development;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.GameObjects
{
	/// <summary>
	/// Ein Inputhandler, der Mauseingaben auf 3D-Modellen verarbeitet.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class ModelMouseHandler : GameScreenComponent
	{
		private World World;
		private double lastRayCheck = 0;
		private ScreenPoint lastMousePosition;

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines ModelMouseHandler-Objekts und ordnet dieser ein IGameScreen-Objekt screen zu,
		/// sowie eine Spielwelt world.
		/// </summary>
		public ModelMouseHandler (IGameScreen screen, World world)
		: base (screen, DisplayLayer.None)
		{
			World = world;
			lastMousePosition = ScreenPoint.Zero (screen);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public override void Update (GameTime time)
		{
			CheckMouseRay (time);
		}

		private void CheckMouseRay (GameTime time)
		{
			double millis = time.TotalGameTime.TotalMilliseconds;
			if (millis > lastRayCheck + 10
			        && (Screen.InputManager.CurrentInputAction == InputAction.CameraTargetMove
			            || Screen.InputManager.CurrentInputAction == InputAction.FreeMouse)
			        && Screen.InputManager.CurrentMousePosition != lastMousePosition) {
				//Log.Debug (Screen.Input.CurrentInputAction);
				lastRayCheck = millis;
				lastMousePosition = Screen.InputManager.CurrentMousePosition;

				Profiler.ProfileDelegate ["Ray"] = () => {
					UpdateMouseRay (time);
				};
			}
		}

		private void UpdateMouseRay (GameTime time)
		{
			Ray ray = World.Camera.GetMouseRay (Screen.InputManager.CurrentMousePosition);

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
