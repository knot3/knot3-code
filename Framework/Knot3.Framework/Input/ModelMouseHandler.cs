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

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Models;

namespace Knot3.Framework.Input
{
    /// <summary>
    /// Ein Inputhandler, der Mauseingaben auf 3D-Modellen verarbeitet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ModelMouseHandler : ScreenComponent
    {
        private World World;
        private double lastRayCheck = 0;
        private ScreenPoint lastMousePosition;

        /// <summary>
        /// Erzeugt eine neue Instanz eines ModelMouseHandler-Objekts und ordnet dieser ein IGameScreen-Objekt screen zu,
        /// sowie eine Spielwelt world.
        /// </summary>
        public ModelMouseHandler (IScreen screen, World world)
        : base (screen, DisplayLayer.None)
        {
            World = world;
            lastMousePosition = ScreenPoint.Zero (screen);
        }

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
                if (obj.IsVisible) {
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
    }
}
