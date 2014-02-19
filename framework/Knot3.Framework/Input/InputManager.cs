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

using Knot3.Core;

using Knot3.Development;



using Knot3.Utilities;


#endregion

namespace Knot3.Input
{
	/// <summary>
	/// Stellt für jeden Frame die Maus- und Tastatureingaben bereit. Daraus werden die nicht von XNA bereitgestellten Mauseingaben berechnet. Zusätzlich wird die aktuelle Eingabeaktion berechnet.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class InputManager : GameScreenComponent
	{
		#region Properties

		/// <summary>
		/// Enthält den Klickzustand der rechten Maustaste.
		/// </summary>
		public static ClickState RightMouseButton { get; private set; }

		/// <summary>
		/// Enthält den Klickzustand der linken Maustaste.
		/// </summary>
		public static ClickState LeftMouseButton { get; private set; }

		/// <summary>
		/// Enthält den Mauszustand von XNA zum aktuellen Frames.
		/// </summary>
		public static MouseState CurrentMouseState { get; set; }

		/// <summary>
		/// Enthält den Tastaturzustand von XNA zum aktuellen Frames.
		/// </summary>
		public static KeyboardState CurrentKeyboardState { get; private set; }

		/// <summary>
		/// Enthält den Mauszustand von XNA zum vorherigen Frames.
		/// </summary>
		public static MouseState PreviousMouseState { get; private set; }

		/// <summary>
		/// Enthält den Tastaturzustand von XNA zum vorherigen Frames.
		/// </summary>
		public static KeyboardState PreviousKeyboardState { get; private set; }

		/// <summary>
		/// Gibt an, ob die Mausbewegung für Kameradrehungen verwendet werden soll.
		/// </summary>
		public Boolean GrabMouseMovement { get; set; }

		/// <summary>
		/// Gibt die aktuelle Eingabeaktion an, die von den verschiedenen Inputhandlern genutzt werden können.
		/// </summary>
		public InputAction CurrentInputAction { get; set; }

		private static double LeftButtonClickTimer;
		private static double RightButtonClickTimer;
		private static MouseState PreviousClickMouseState;

		public static bool FullscreenToggled { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt ein neues InputManager-Objekt, das an den übergebenen Spielzustand gebunden ist.
		/// </summary>
		public InputManager (IGameScreen screen)
		: base (screen, DisplayLayer.None)
		{
			CurrentInputAction = InputAction.FreeMouse;

			PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState ();
			PreviousMouseState = CurrentMouseState = Mouse.GetState ();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public override void Update (GameTime time)
		{
			PreviousKeyboardState = CurrentKeyboardState;
			PreviousMouseState = CurrentMouseState;
			CurrentKeyboardState = Keyboard.GetState ();
			CurrentMouseState = Mouse.GetState ();

			if (time != null) {
				bool mouseMoved;
				if (CurrentMouseState != PreviousMouseState) {
					// mouse movements
					Vector2 mouseMove = CurrentMouseState.ToVector2 () - PreviousClickMouseState.ToVector2 ();
					mouseMoved = mouseMove.Length () > 3;
				}
				else {
					mouseMoved = false;
				}

				LeftButtonClickTimer += time.ElapsedGameTime.TotalMilliseconds;
				if (CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton != ButtonState.Pressed) {
					LeftMouseButton = LeftButtonClickTimer < 500 && !mouseMoved
					                  ? ClickState.DoubleClick : ClickState.SingleClick;
					LeftButtonClickTimer = 0;
					PreviousClickMouseState = PreviousMouseState;
					Log.Debug ("LeftButton=", LeftMouseButton);
				}
				else {
					LeftMouseButton = ClickState.None;
				}
				RightButtonClickTimer += time.ElapsedGameTime.TotalMilliseconds;
				if (CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton != ButtonState.Pressed) {
					RightMouseButton = RightButtonClickTimer < 500 && !mouseMoved
					                   ? ClickState.DoubleClick : ClickState.SingleClick;
					RightButtonClickTimer = 0;
					PreviousClickMouseState = PreviousMouseState;
					Log.Debug ("RightButton=", RightMouseButton);
				}
				else {
					RightMouseButton = ClickState.None;
				}
			}

			// fullscreen
			if (Keys.F11.IsDown ()) {
				Screen.Game.IsFullScreen = !Screen.Game.IsFullScreen;
				FullscreenToggled = true;
			}
		}

		#endregion
	}
}
