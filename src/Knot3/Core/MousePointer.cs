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

using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Widgets;
using Knot3.Utilities;
using Knot3.Input;
using Knot3.Platform;

#endregion

namespace Knot3.Core
{
	/// <summary>
	/// Repräsentiert einen Mauszeiger.
	/// </summary>
	public sealed class MousePointer : DrawableGameScreenComponent
	{
		private SpriteBatch spriteBatch;
		private Texture2D cursorTex;

		#region Constructors

		/// <summary>
		/// Erstellt einen neuen Mauszeiger für den angegebenen Spielzustand.
		/// </summary>
		public MousePointer (IGameScreen screen)
		: base (screen, DisplayLayer.Cursor)
		{
			cursorTex = Screen.LoadTexture ("cursor");
			spriteBatch = new SpriteBatch (screen.Device);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Zeichnet den Mauszeiger.
		/// </summary>
		public override void Draw (GameTime time)
		{
			DrawCursor (time);
		}

		private void DrawCursor (GameTime time)
		{
			if (!SystemInfo.IsRunningOnLinux ()) {
				spriteBatch.Begin ();

				if (Screen.Input.GrabMouseMovement || Screen.Input.CurrentInputAction == InputAction.CameraTargetMove
				        || (Screen.Input.CurrentInputAction == InputAction.ArcballMove
				            && (InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed
				                || InputManager.CurrentMouseState.RightButton == ButtonState.Pressed))) {
					//spriteBatch.Draw (cursorTex, Screen.Device.Viewport.Center (), Color.White);
				}
				else {
					spriteBatch.Draw (
					    cursorTex,
					    new Vector2 (InputManager.CurrentMouseState.X, InputManager.CurrentMouseState.Y)* Options.Default["video","Supersamples",1],
					    null,
					    Color.White,
					    0f,
					    Vector2.Zero,
					    Vector2.One *Options.Default["video","Supersamples",1],
					    SpriteEffects.None,
					    1f
					);
				}

				spriteBatch.End ();
			}
		}

		#endregion
	}
}
