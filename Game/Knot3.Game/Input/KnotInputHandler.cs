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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Data;
using Knot3.Game.Screens;

namespace Knot3.Game.Input
{
    /// <summary>
    /// Verarbeitet die Maus- und Tastatureingaben des Spielers und modifiziert die Kamera-Position
    /// und das Kamera-Ziel.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class KnotInputHandler : KeyBindingListener<KnotInputHandler>, IMouseMoveEventListener, IMouseScrollEventListener
    {
        /// <summary>
        /// Die Spielwelt.
        /// </summary>
        private World world;

        /// <summary>
        /// Die Kamera.
        /// </summary>
        private Camera camera { get { return world.Camera; } }

        /// <summary>
        /// Zeigt an, ob die Klasse zur Zeit auf Eingaben reagiert.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Zeigt an, ob die Klasse zur Zeit auf Tastatureingaben reagiert.
        /// </summary>
        public override bool IsKeyEventEnabled { get { return IsEnabled; } }

        /// <summary>
        /// Zeigt an, ob die Klasse zur Zeit auf Mausbewegungen reagiert.
        /// </summary>
        public bool IsMouseMoveEventEnabled { get { return IsEnabled; } }

        /// <summary>
        /// Zeigt an, ob die Klasse zur Zeit auf Mausrad-Bewegungen reagiert.
        /// </summary>
        public bool IsMouseScrollEventEnabled { get { return IsEnabled; } }

        /// <summary>
        /// Zeigt an, ob die Klasse modal ist.
        /// </summary>
        public override bool IsModal { get { return false; } }

        /// <summary>
        /// Der statische Initialisierer legt die Standard-Tastenbelegung fest.
        /// </summary>
        static KnotInputHandler ()
        {
            DefaultKeyAssignment [Keys.W] = Knot3PlayerAction.MoveForward;
            DefaultKeyAssignment [Keys.S] = Knot3PlayerAction.MoveBackward;
            DefaultKeyAssignment [Keys.A] = Knot3PlayerAction.MoveLeft;
            DefaultKeyAssignment [Keys.D] = Knot3PlayerAction.MoveRight;
            DefaultKeyAssignment [Keys.R] = Knot3PlayerAction.MoveUp;
            DefaultKeyAssignment [Keys.F] = Knot3PlayerAction.MoveDown;
            DefaultKeyAssignment [Keys.Up] = Knot3PlayerAction.RotateUp;
            DefaultKeyAssignment [Keys.Down] = Knot3PlayerAction.RotateDown;
            DefaultKeyAssignment [Keys.Left] = Knot3PlayerAction.RotateLeft;
            DefaultKeyAssignment [Keys.Right] = Knot3PlayerAction.RotateRight;
            DefaultKeyAssignment [Keys.Q] = Knot3PlayerAction.ZoomIn;
            DefaultKeyAssignment [Keys.E] = Knot3PlayerAction.ZoomOut;
            DefaultKeyAssignment [Keys.Enter] = Knot3PlayerAction.ResetCamera;
            DefaultKeyAssignment [Keys.Space] = Knot3PlayerAction.MoveToCenter;
            DefaultKeyAssignment [Keys.M] = Knot3PlayerAction.ToggleMouseLock;
            DefaultKeyAssignment [Keys.LeftControl] = Knot3PlayerAction.AddToEdgeSelection;
            DefaultKeyAssignment [Keys.LeftShift] = Knot3PlayerAction.AddRangeToEdgeSelection;
        }

        /// <summary>
        /// Erstellt einen neuen KnotInputHandler für den angegebenen Spielzustand und die angegebene Spielwelt.
        /// [base=screen]
        /// </summary>
        public KnotInputHandler (IScreen screen, World world)
        : base (screen, world.Index)
        {
            // Standardmäßig aktiviert
            IsEnabled = true;

            // Spielwelt
            this.world = world;

            // Setze die Standardwerte für die Mausposition
            screen.InputManager.GrabMouseMovement = false;
            ResetMousePosition ();

            // Lege die Bedeutungen der Aktionen fest
            ActionBindings [Knot3PlayerAction.MoveUp] = (time) => MoveCameraAndTarget (Vector3.Up, time);
            ActionBindings [Knot3PlayerAction.MoveDown] = (time) => MoveCameraAndTarget (Vector3.Down, time);
            ActionBindings [Knot3PlayerAction.MoveLeft] = (time) => MoveCameraAndTarget (Vector3.Left, time);
            ActionBindings [Knot3PlayerAction.MoveRight] = (time) => MoveCameraAndTarget (Vector3.Right, time);
            ActionBindings [Knot3PlayerAction.MoveForward] = (time) => MoveCameraAndTarget (Vector3.Forward, time);
            ActionBindings [Knot3PlayerAction.MoveBackward] = (time) => MoveCameraAndTarget (Vector3.Backward, time);
            ActionBindings [Knot3PlayerAction.RotateUp] = (time) => rotate (-Vector2.UnitY * 4, time);
            ActionBindings [Knot3PlayerAction.RotateDown] = (time) => rotate (Vector2.UnitY * 4, time);
            ActionBindings [Knot3PlayerAction.RotateLeft] = (time) => rotate (-Vector2.UnitX * 4, time);
            ActionBindings [Knot3PlayerAction.RotateRight] = (time) => rotate (Vector2.UnitX * 4, time);
            ActionBindings [Knot3PlayerAction.ZoomIn] = (time) => zoom (-1, time);
            ActionBindings [Knot3PlayerAction.ZoomOut] = (time) => zoom (+1, time);
            ActionBindings [Knot3PlayerAction.ResetCamera] = (time) => resetCamera (time);
            ActionBindings [Knot3PlayerAction.MoveToCenter] = (time) => moveToCenter (time);
            ActionBindings [Knot3PlayerAction.ToggleMouseLock] = (time) => toggleMouseLock (time);
            UpdateKeyBindings ();
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            // und die linke Maustaste gedrückt gehalten wird
            if (Screen.InputManager.CurrentMouseState.MiddleButton == ButtonState.Pressed && Screen.InputManager.PreviousMouseState.MiddleButton == ButtonState.Released) {
                Screen.InputManager.GrabMouseMovement = true;
            }
            else if (Screen.InputManager.CurrentMouseState.MiddleButton == ButtonState.Released && Screen.InputManager.PreviousMouseState.MiddleButton == ButtonState.Pressed) {
                Screen.InputManager.GrabMouseMovement = false;
            }

            if (!IsEnabled) {
                Screen.InputManager.CurrentInputAction = InputAction.FreeMouse;
            }

            if (Screen.InputManager.CurrentInputAction == InputAction.FreeMouse) {
                // automatische Kameraführung
                AutoCamera (time);
            }
        }

        public void OnLeftMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
            UpdateMouse (previousPosition, currentPosition, move, time);
            ResetMousePosition ();
        }

        public void OnRightMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
            UpdateMouse (previousPosition, currentPosition, move, time);
            ResetMousePosition ();
        }

        public void OnMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
            UpdateMouse (previousPosition, currentPosition, move, time);
            ResetMousePosition ();
        }

        public void OnNoMove (ScreenPoint currentPosition, GameTime time)
        {
        }

        private void UpdateMouse (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint _mouseMove, GameTime time)
        {
            // wurde im letzten Frame in den oder aus dem Vollbildmodus gewechselt?
            // dann überpringe einen frame
            if (InputManager.FullscreenToggled) {
                InputManager.FullscreenToggled = false;
                return;
            }

            // ist der MouseState gleich geblieben?
            if (Screen.InputManager.CurrentMouseState == Screen.InputManager.PreviousMouseState) {
                return;
            }

            // die aktuelle Mausbewegung
            Point mouseMove = _mouseMove.Absolute;

            InputAction action;
            // wenn die Maus in der Mitte des Bildschirms gelockt ist
            if (Screen.InputManager.GrabMouseMovement) {
                // und die linke Maustaste gedrückt gehalten wird
                if (Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                    action = InputAction.ArcballMove;
                }
                // und die rechte Maustaste gedrückt gehalten wird
                else if (Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Pressed) {
                    action = InputAction.ArcballMove;
                }
                // und alle Maustasten losgelassen sind
                else {
                    action = InputAction.CameraTargetMove;
                }
            }
            // wenn die Maus frei bewegbar ist
            else {
                // und die linke Maustaste gedrückt gehalten wird
                if (Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                    if (world.SelectedObject != null && world.SelectedObject.IsMovable) {
                        action = InputAction.SelectedObjectShadowMove;
                    }
                    else {
                        action = InputAction.FreeMouse;
                    }
                }
                // und die linke Maustaste gerade losgelassen wurde
                else if (Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Released && Screen.InputManager.PreviousMouseState.LeftButton == ButtonState.Pressed) {
                    if (world.SelectedObject != null && world.SelectedObject.IsMovable) {
                        action = InputAction.SelectedObjectMove;
                    }
                    else {
                        action = InputAction.FreeMouse;
                    }
                }
                // und die rechte Maustaste gedrückt gehalten wird
                else if (Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Pressed) {
                    action = InputAction.ArcballMove;
                }
                // und alle Maustasten losgelassen sind
                else {
                    action = InputAction.FreeMouse;
                }
            }
            Screen.InputManager.CurrentInputAction = action;

            //Log.Debug ("action=",action);

            switch (action) {
            case InputAction.ArcballMove:
                // rotieren
                rotate (new Vector2 (mouseMove.X, mouseMove.Y) * 1.5f, time);
                break;
            case InputAction.CameraTargetMove:
                // verschieben
                MoveTarget (new Vector3 (mouseMove.X, -mouseMove.Y, 0) * 0.3f, time);
                break;
            }
        }

        private void AutoCamera (GameTime time)
        {
            if (Config.Default ["video", "auto-camera-nomove", false]) {
                ScreenPoint currentPosition = Screen.InputManager.CurrentMouseState.ToScreenPoint (Screen);
                Bounds worldBounds = world.Bounds;
                var bounds = new[] {
                    new { Bounds = worldBounds.FromLeft (0.03f), Side = new Vector2 (-1, 0) },
                    new { Bounds = worldBounds.FromRight (0.03f), Side = new Vector2 (1, 0) },
                    new { Bounds = worldBounds.FromTop (0.03f), Side = new Vector2 (0, 1) },
                    new { Bounds = worldBounds.FromBottom (0.03f), Side = new Vector2 (0, -1) }
                };
                Vector2[] sides = bounds.Where (x => x.Bounds.Contains (currentPosition)).Select (x => x.Side).ToArray ();
                if (sides.Length == 1) {
                    //InputAction action = Screen.Input.CurrentInputAction;
                    MoveTarget (new Vector3 (sides [0].X, sides [0].Y, 0) * 0.5f, time);
                    world.Redraw = true;
                    Screen.InputManager.CurrentInputAction = InputAction.FreeMouse;
                }
            }
        }

        public void OnScroll (int scrollWheelValue, GameTime time)
        {
            // scroll wheel zoom
            if (Screen.InputManager.CurrentMouseState.ScrollWheelValue < Screen.InputManager.PreviousMouseState.ScrollWheelValue) {
                // camera.FoV += 1;
                zoom (8, time);
            }
            else if (Screen.InputManager.CurrentMouseState.ScrollWheelValue > Screen.InputManager.PreviousMouseState.ScrollWheelValue) {
                // camera.FoV -= 1

                zoom (-8, time);
            }
            world.Redraw = true;
        }

        private void ResetMousePosition ()
        {
            if (Screen.InputManager.GrabMouseMovement || (Screen.InputManager.CurrentInputAction == InputAction.ArcballMove)) {
                ScreenPoint center = world.Viewport.Center (Screen);
                if (Screen.InputManager.CurrentMousePosition != center) {
                    Screen.InputManager.ResetMouse (world.Viewport.Center (Screen));
                }
            }
        }

        /// <summary>
        /// Verschiebt die Kamera und das Target linear in die angegebene Richtung.
        /// </summary>
        public void MoveCameraAndTarget (Vector3 move, GameTime time)
        {
            Profiler.ProfileDelegate ["Move"] = () => {
                if (move.Length () > 0) {
                    move *= 5;
                    Vector3 targetDirection = camera.PositionToTargetDirection;
                    Vector3 up = camera.UpVector;
                    // Führe die lineare Verschiebung durch
                    camera.Target = camera.Target.MoveLinear (move, up, targetDirection);
                    camera.Position = camera.Position.MoveLinear (move, up, targetDirection);
                    Screen.InputManager.CurrentInputAction = InputAction.FirstPersonCameraMove;
                    world.Redraw = true;
                }
            };
        }

        /// <summary>
        /// Verschiebt das Target linear in die angegebene Richtung.
        /// </summary>
        public void MoveTarget (Vector3 move, GameTime time)
        {
            Profiler.ProfileDelegate ["Move"] = () => {
                if (move.Length () > 0) {
                    move *= 5;
                    Vector3 targetDirection = camera.PositionToTargetDirection;
                    Vector3 up = camera.UpVector;
                    // Führe die lineare Verschiebung durch
                    camera.Target = camera.Target.MoveLinear (move, up, targetDirection);
                    Screen.InputManager.CurrentInputAction = InputAction.FirstPersonCameraMove;
                    world.Redraw = true;
                }
            };
        }

        /// <summary>
        /// Rotiert die Kamera auf einem Arcball um das Target.
        /// </summary>
        private void rotate (Vector2 move, GameTime time)
        {
            Screen.InputManager.CurrentInputAction = InputAction.ArcballMove;

            rotateCenter (move, time);
        }

        private void rotateCenter (Vector2 move, GameTime time)
        {
            // Wenn kein 3D-Objekt selektiert ist...
            if (world.SelectedObject == null && world.Any ()) {
                // selektiere das Objekt, das der Mausposition am nächsten ist!
                IGameObject[] nearestObjects
                    = world.FindNearestObjects (nearTo: Screen.InputManager.CurrentMousePosition).ToArray ();
                if (nearestObjects.Length > 0) {
                    world.SelectedObject = nearestObjects [0];
                }
            }

            // Überprüfe, wie weit das Kamera-Target von dem Objekt, um das rotiert werden soll,
            // entfernt ist
            float arcballTargetDistance = Math.Abs (camera.Target.DistanceTo (camera.ArcballTarget));

            // Ist es mehr als 5 Pixel entfernt?
            if (arcballTargetDistance > 5) {
                // Falls noch kein SmoothMove gestartet ist, starte einen, um das Arcball-Target
                // in den Fokus der Kamera zu rücken
                if (!camera.InSmoothMove) {
                    camera.StartSmoothMove (target: camera.ArcballTarget, time: time);
                }
                Screen.InputManager.CurrentInputAction = InputAction.ArcballMove;
            }
            // Ist es weiter als 5 Pixel weg?
            else if (move.Length () > 0) {
                Screen.InputManager.CurrentInputAction = InputAction.ArcballMove;
                world.Redraw = true;

                // Berechne die Rotation
                camera.Target = camera.ArcballTarget;
                float oldDistance = camera.Position.DistanceTo (camera.Target);
                Vector3 targetDirection = camera.PositionToTargetDirection;
                Vector3 up = camera.UpVector;
                camera.Position = camera.Target
                                  + (camera.Position - camera.Target).ArcBallMove (move, up, targetDirection);
                camera.Position = camera.Position.SetDistanceTo (camera.Target, oldDistance);
            }
        }

        /// <summary>
        /// Führt einen Zoom durch, indem die Distanz von der Kamera zum Target erhöht oder verringert wird.
        /// </summary>
        private void zoom (int value, GameTime time)
        {
            if (camera.PositionToTargetDistance <= 80 && value < 0) {
                camera.PositionToTargetDistance = 40;
            }
            else {
                camera.PositionToTargetDistance += value * 5;
            }
        }

        public void OnStartEdgeChanged (Vector3 direction)
        {
            Log.Debug ("OnStartEdgeChanged: ", direction);
            camera.Position -= direction * Node.Scale;
            camera.Target -= direction * Node.Scale;
            Screen.InputManager.CurrentInputAction = InputAction.FreeMouse;
        }

        private void toggleMouseLock (GameTime time)
        {
            if (Screen.InputManager.KeyPressed (CurrentKeyAssignmentReversed [Knot3PlayerAction.ToggleMouseLock])) {
                Screen.InputManager.GrabMouseMovement = !Screen.InputManager.GrabMouseMovement;
            }
        }

        private void resetCamera (GameTime time)
        {
            if (Screen.InputManager.KeyPressed (CurrentKeyAssignmentReversed [Knot3PlayerAction.ResetCamera])) {
                camera.ResetCamera ();
            }
        }

        private void moveToCenter (GameTime time)
        {
            camera.StartSmoothMove (target: camera.ArcballTarget, time: time);
        }

        public override void OnKeyEvent (List<Keys> keys, KeyEvent keyEvent, GameTime time)
        {
            // bei einem Tastendruck wird die Spielwelt auf jeden Fall neu gezeichnet.
            world.Redraw = true;

            // rufe die Basismethode von KeyBindingListener auf
            base.OnKeyEvent (keys, keyEvent, time);
        }

        public Bounds MouseMoveBounds { get { return world.Bounds; } }

        public Bounds MouseScrollBounds { get { return world.Bounds; } }
    }
}
