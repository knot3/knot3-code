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

using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Math;

namespace Knot3.Framework.Input
{
    /// <summary>
    /// Ein Inputhandler, der Mauseingaben auf Widgets verarbeitet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class WidgetMouseHandler : ScreenComponent
    {
        public WidgetMouseHandler (IScreen screen)
        : base (screen, DisplayLayer.None)
        {
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            UpdateMouseClick (time);
            UpdateMouseScroll (time);
            UpdateMouseMove (time);
        }

        private void UpdateMouseClick (GameTime time)
        {
            // Mausklicks
            foreach (IMouseClickEventListener component in Screen.Game.Components.OfType<IMouseClickEventListener>()
                     .Where (c => c.IsMouseClickEventEnabled).OrderByDescending (c => c.Index.Index)) {
                Bounds bounds = component.MouseClickBounds;
                bool hovered = bounds.Contains (Screen.InputManager.CurrentMousePosition);
                component.SetHovered (hovered, time);
                if (hovered) {
                    ScreenPoint relativePosition = Screen.InputManager.CurrentMousePosition - bounds.Position;
                    if (Screen.InputManager.LeftMouseButton != ClickState.None) {
                        component.OnLeftClick (relativePosition, Screen.InputManager.LeftMouseButton, time);
                        break;
                    }
                    else if (Screen.InputManager.RightMouseButton != ClickState.None) {
                        component.OnRightClick (relativePosition, Screen.InputManager.RightMouseButton, time);
                        break;
                    }
                }
            }
        }

        private void UpdateMouseScroll (GameTime time)
        {
            foreach (IMouseScrollEventListener component in Screen.Game.Components.OfType<IMouseScrollEventListener>()
                     .Where (c => c.IsMouseScrollEventEnabled).OrderByDescending (c => c.Index.Index)) {
                Bounds bounds = component.MouseScrollBounds;
                bool hovered = bounds.Contains (Screen.InputManager.CurrentMousePosition);

                if (hovered) {
                    if (Screen.InputManager.CurrentMouseState.ScrollWheelValue > Screen.InputManager.PreviousMouseState.ScrollWheelValue) {
                        component.OnScroll (-1, time);
                    }
                    else if (Screen.InputManager.CurrentMouseState.ScrollWheelValue < Screen.InputManager.PreviousMouseState.ScrollWheelValue) {
                        component.OnScroll (+1, time);
                    }
                    break;
                }
            }
        }

        private ScreenPoint lastLeftClickPosition;
        private ScreenPoint lastRightClickPosition;

        private void UpdateMouseMove (GameTime time)
        {
            // aktuelle Position und die des letzten Frames
            ScreenPoint current = Screen.InputManager.CurrentMousePosition;
            ScreenPoint previous = Screen.InputManager.PreviousMousePosition;

            // zuweisen der Positionen beim Drücken der Maus
            if (Screen.InputManager.PreviousMouseState.LeftButton == ButtonState.Released && Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                lastLeftClickPosition = current;
            }
            else if (Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Released) {
                lastLeftClickPosition = null;
            }
            if (Screen.InputManager.PreviousMouseState.RightButton == ButtonState.Released && Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Pressed) {
                lastRightClickPosition = current;
            }
            else if (Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Released) {
                lastRightClickPosition = null;
            }
            //Log.Debug ("left=",(lastLeftClickPosition ?? ScreenPoint.Zero (Screen)),"right=",(lastRightClickPosition ?? ScreenPoint.Zero (Screen)));

            foreach (IMouseMoveEventListener component in Screen.Game.Components.OfType<IMouseMoveEventListener>()
                     .Where (c => c.IsMouseMoveEventEnabled).OrderByDescending (c => c.Index.Index)) {
                Bounds bounds = component.MouseMoveBounds;

                ScreenPoint relativePositionPrevious = previous - bounds.Position;
                ScreenPoint relativePositionCurrent = current - bounds.Position;
                ScreenPoint relativePositionMove = current - previous;

                bool notify = false;
                // wenn die Komponente die Position beim Drücken der linken Maustaste enthält
                if (lastLeftClickPosition != null && bounds.Contains (lastLeftClickPosition)) {
                    notify = true;
                    if (bounds.Contains (current) && Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                        lastLeftClickPosition = current;
                    }
                }
                // wenn die Komponente die Position beim Drücken der rechten Maustaste enthält
                else if (lastRightClickPosition != null && bounds.Contains (lastRightClickPosition)) {
                    notify = true;
                    if (bounds.Contains (current) && Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Pressed) {
                        lastRightClickPosition = current;
                    }
                }
                // wenn die Komponente die aktuelle Position enthält
                else if (lastLeftClickPosition == null && lastRightClickPosition == null && bounds.Contains (previous)) {
                    notify = true;
                }

                //Log.Debug ("notify=",notify,", component=",component,", cntains=",(lastLeftClickPosition != null ? bounds.Contains (lastLeftClickPosition),"" : ""),",bounds=",bounds,",precious=",previous);

                if (notify) {
                    if (relativePositionMove
                            || Screen.InputManager.PreviousMouseState.LeftButton != Screen.InputManager.CurrentMouseState.LeftButton
                            || Screen.InputManager.PreviousMouseState.RightButton != Screen.InputManager.CurrentMouseState.RightButton) {
                        if (Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed) {
                            component.OnLeftMove (
                                previousPosition: relativePositionPrevious,
                                currentPosition: relativePositionCurrent,
                                move: relativePositionMove,
                                time: time
                            );
                        }
                        else if (Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Pressed) {
                            component.OnRightMove (
                                previousPosition: relativePositionPrevious,
                                currentPosition: relativePositionCurrent,
                                move: relativePositionMove,
                                time: time
                            );
                        }
                        else {
                            component.OnMove (
                                previousPosition: relativePositionPrevious,
                                currentPosition: relativePositionCurrent,
                                move: relativePositionMove,
                                time: time
                            );
                        }
                        break;
                    }
                    else {
                        component.OnNoMove (
                            currentPosition: relativePositionCurrent,
                            time: time
                        );
                        break;
                    }
                }
            }
        }
    }
}
