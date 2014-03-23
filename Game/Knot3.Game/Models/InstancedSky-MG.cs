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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Effects;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Primitives;
using Knot3.Framework.Storage;

using Knot3.Game.Data;
using Knot3.Game.Effects;

namespace Knot3.Game.Models
{
    [ExcludeFromCodeCoverageAttribute]
    public class InstancedSky : GameObject, IEnumerable<IGameObject>
    {
        private IScreen Screen;

        /// <summary>
        /// Die Distanz zu den Wänden vom Ursprung aus.
        /// </summary>
        public float Distance
        {
            get {
                return _distance;
            }
            set {
                if (_distance != value) {
                    _distance = value;
                    scaleMatrix = Matrix.CreateScale (Vector3.One * _distance);
                    UpdateScale ();
                }
            }
        }

        private float _distance;
        /// <summary>
        /// Der Zufallsgenerator.
        /// </summary>
        protected static Random random;
        /// <summary>
        /// Der Effekt, mit dem die Skybox gezeichnet wird.
        /// </summary>
        private Matrix scaleMatrix;
        private Star[] Stars;

        /// <summary>
        /// Erstellt ein neues KnotRenderer-Objekt für den angegebenen Spielzustand mit den angegebenen
        /// Spielobjekt-Informationen, die unter Anderem die Position des Knotenursprungs enthalten.
        /// </summary>
        public InstancedSky (IScreen screen, Vector3 position, float distance)
        {
            Screen = screen;
            Position = position;
            random = new Random ();
            Stars = CreateStars (count: (int)Config.Default ["video", "stars-count", 1000]);
            Distance = distance;
        }

        private Star[] CreateStars (int count)
        {
            Color[] colors = new Color[] { Color.White, Color.Red, Color.MediumBlue, Color.Orange };
            Star[] stars = new Star [count];

            Color color;
            for (int i = 0; i < count; ++i) {
                Vector3 position = new Vector3 ((float)random.NextDouble () - 0.5f, (float)random.NextDouble () - 0.5f, (float)random.NextDouble () - 0.5f);
                position.Normalize ();
                if ((random.Next () %100 )<65) {
                    color = Color.White;
                }
                else {
                    color = colors [random.Next () % colors.Length];
                }
                stars [i] = new Star (screen: Screen, color: color) {
                    RelativePosition = position,
                    Scale = Vector3.One * (500 + random.Next ()%1000)
                };
            }
            return stars;
        }

        private void UpdateScale ()
        {
            foreach (Star star in Stars) {
                star.Position = Vector3.Transform (star.RelativePosition, scaleMatrix);
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            if (Math.Abs (1f - (Distance / (World.Camera.FarPlane / 3.6f))) > 0.05f) {
                Distance = World.Camera.FarPlane / 3.6f;
            }

            UpdateStars (time);
        }

        private void UpdateStars (GameTime time)
        {
            Profiler.ProfileDelegate ["Sky Upd"] = () => {
                foreach (Star star in Stars) {
                    star.Update (time);
                }
            };
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (!Config.Default ["debug", "projector-mode", false]) {
                Profiler.ProfileDelegate ["Sky Draw"] = () => {
                    foreach (Star star in Stars) {
                        star.World = World;
                        star.IsLightingEnabled = false;
                        star.Draw (time);
                    }
                };
            }
        }

        public override GameObjectDistance Intersects (Ray ray)
        {
            return null;
        }

        /// <summary>
        /// Gibt einen Enumerator der aktuell vorhandenen 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            yield break;
        }
        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }

        private class Star : GamePrimitive
        {
            public Vector3 RelativePosition { get; set; }

            private float AlphaCounter;

            public Star (IScreen screen, Color color) : base (screen: screen)
            {
                IsSkyObject = true;
                Coloring = new SingleColor (color);
                UpdateCategory ();
                AlphaCounter = (float)random.NextDouble ();
                Scale = new Vector3 (3f,3f,3f);
            }

            protected override Primitive CreativePrimitive ()
            {
                return new Knot3.Framework.Primitives.Star (Screen.GraphicsDevice);
            }

            public override void Update (GameTime gameTime)
            {
                AlphaCounter += 0.001f;
                float newAlpha = distinctValues (0.1f+(Math.Abs ((float)Math.Sin (MathHelper.TwoPi * AlphaCounter))*0.9f) );
                if (newAlpha != Coloring.Alpha) {
                    Coloring.Alpha = newAlpha;
                    UpdateCategory ();
                }
                base.Update (gameTime);
            }

            private float distinctValues (float value)
            {
                for (float i = 1f; i > 0f; i -= 0.05f) {
                    if (value >= i) {
                        return i;
                    }
                }
                return 0f;
            }
        }
    }
}
