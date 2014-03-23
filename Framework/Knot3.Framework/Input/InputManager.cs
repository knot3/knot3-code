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

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Input
{
    /// <summary>
    /// Stellt für jeden Frame die Maus- und Tastatureingaben bereit. Daraus werden die nicht von MonoGame bereitgestellten Mauseingaben berechnet. Zusätzlich wird die aktuelle Eingabeaktion berechnet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class InputManager : KeyBindingListener<InputManager>
    {
        /// <summary>
        /// Enthält den Klickzustand der rechten Maustaste.
        /// </summary>
        public ClickState RightMouseButton { get; private set; }

        /// <summary>
        /// Enthält den Klickzustand der linken Maustaste.
        /// </summary>
        public ClickState LeftMouseButton { get; private set; }

        /// <summary>
        /// Enthält den Mauszustand von MonoGame zum aktuellen Frame.
        /// </summary>
        public MouseState CurrentMouseState { get; set; }

        /// <summary>
        /// Enthält die Mausposition von MonoGame zum aktuellen Frame.
        /// </summary>
        public ScreenPoint CurrentMousePosition { get { return CurrentMouseState.ToScreenPoint (Screen); } }

        /// <summary>
        /// Enthält den Tastaturzustand von MonoGame zum aktuellen Frame.
        /// </summary>
        public KeyboardState CurrentKeyboardState { get; private set; }

        /// <summary>
        /// Enthält den Mauszustand von MonoGame zum vorherigen Frame.
        /// </summary>
        public MouseState PreviousMouseState { get; private set; }

        /// <summary>
        /// Enthält die Mausposition von MonoGame zum vorherigen Frame.
        /// </summary>
        public ScreenPoint PreviousMousePosition { get { return PreviousMouseState.ToScreenPoint (Screen); } }

        /// <summary>
        /// Enthält den Tastaturzustand von MonoGame zum vorherigen Frame.
        /// </summary>
        public KeyboardState PreviousKeyboardState { get; private set; }

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

        /// <summary>
        /// Gibt an, ob der Vollbildmodus in diesem (oder dem vorherigen?! irgendwie sowas!) Frame getoggelt wurde.
        /// </summary>
        public static bool FullscreenToggled { get; set; }

        /// <summary>
        /// Zeigt an, ob die Klasse zur Zeit auf Tastatureingaben reagiert.
        /// </summary>
        public override bool IsKeyEventEnabled { get { return true; } }

        /// <summary>
        /// Zeigt an, ob die Klasse modal ist.
        /// </summary>
        public override bool IsModal { get { return false; } }

        /// <summary>
        /// Der statische Initialisierer legt die Standard-Tastenbelegung fest.
        /// </summary>
        static InputManager ()
        {
            DefaultKeyAssignment [Keys.F11] = PlayerAction.ToggleFullscreen;
        }

        /// <summary>
        /// Erstellt ein neues InputManager-Objekt, das an den übergebenen Spielzustand gebunden ist.
        /// </summary>
        public InputManager (IScreen screen)
        : base (screen, DisplayLayer.None)
        {
            CurrentInputAction = InputAction.FreeMouse;

            PreviousKeyboardState = CurrentKeyboardState = Keyboard.GetState ();
            PreviousMouseState = CurrentMouseState = Mouse.GetState ();

            // Lege die Bedeutungen der Aktionen fest
            ActionBindings [PlayerAction.ToggleFullscreen] = (time) => toggleFullscreen (time);
            UpdateKeyBindings ();
        }

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
                    Vector2 mouseMove = (CurrentMousePosition - PreviousClickMouseState.ToScreenPoint (Screen)).AbsoluteVector;
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
        }

        private void toggleFullscreen (GameTime time)
        {
            if (KeyPressed (LookupKey (PlayerAction.ToggleFullscreen))) {
                Screen.Game.IsFullScreen = !Screen.Game.IsFullScreen;
                FullscreenToggled = true;
            }
        }

        public void ResetMouse (ScreenPoint position)
        {
            Point absolutePosition = position.Absolute;
            MouseState oldstate = CurrentMouseState;
            Mouse.SetPosition (absolutePosition.X, absolutePosition.Y);
            MouseState state = new MouseState (
                absolutePosition.X,
                absolutePosition.Y,
                oldstate.ScrollWheelValue,
                oldstate.LeftButton,
                oldstate.MiddleButton,
                oldstate.RightButton,
                oldstate.XButton1,
                oldstate.XButton2
            );
            CurrentMouseState = state;
        }

        /// <summary>
        /// Wurde die aktuelle Taste gedrückt und war sie im letzten Frame nicht gedrückt?
        /// </summary>
        public bool KeyPressed (Keys key)
        {
            // Is the key down?
            if (CurrentKeyboardState.IsKeyDown (key)) {
                // If not down last update, key has just been pressed.
                if (!PreviousKeyboardState.IsKeyDown (key)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Wurde die aktuelle Taste losgelassen und war sie im letzten Frame noch gedrückt?
        /// </summary>
        public bool KeyReleased (Keys key)
        {
            // Is the key up?
            if (!CurrentKeyboardState.IsKeyDown (key)) {
                // If down last update, key has just been released.
                if (PreviousKeyboardState.IsKeyDown (key)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Wird die aktuelle Taste gedrückt gehalten?
        /// </summary>
        public bool KeyHeldDown (Keys key)
        {
            return CurrentKeyboardState.IsKeyDown (key);
        }
    }
}
