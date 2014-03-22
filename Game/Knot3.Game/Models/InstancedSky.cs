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
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Primitives;
using Knot3.Framework.Storage;
using Knot3.Game.Data;
using Knot3.Framework.Development;

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
                }
            }
        }

        private float _distance;
        /// <summary>
        /// Der Zufallsgenerator.
        /// </summary>
        private Random random;
        /// <summary>
        /// Der Effekt, mit dem die Skybox gezeichnet wird.
        /// </summary>
        private BasicEffect effect;
        private Matrix scaleMatrix;
        /// <summary>
        /// Die einzelnen Texturen.
        /// </summary>
        private SkyTexture[] SkyTextures;

        /// <summary>
        /// Erstellt ein neues KnotRenderer-Objekt für den angegebenen Spielzustand mit den angegebenen
        /// Spielobjekt-Informationen, die unter Anderem die Position des Knotenursprungs enthalten.
        /// </summary>
        public InstancedSky (IScreen screen, Vector3 position, float distance)
        {
            Screen = screen;
            Position = position;
            random = new Random ();
            effect = new BasicEffect (screen.GraphicsDevice);
            Distance = distance;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            if (Math.Abs (1f - (Distance / (World.Camera.FarPlane / 3.6f))) > 0.05f) {
                Distance = World.Camera.FarPlane / 3.6f;
            }
            if (Config.Default ["video", "blinking-stars", true]) {
                UpdateStars (time);
            }
        }

        private int k = 0;

        private void UpdateStars (GameTime time)
        {
            if (k++ % 10 == 0) {
                Profiler.ProfileDelegate ["Stars Blink"] = () => {
                    int SkyTextureCacheSize = (int)Config.Default ["video", "blinking-stars-cachesize", 20f];
                    float alphaCounter = k * 0.002f;
                    int key = (int)(Math.Abs(alphaCounter / SkyTextureCacheSize));
                    Console.WriteLine("key="+key);
                    foreach (SkyTexture SkyTexture in SkyTextures) {
                        Profiler.Values ["Stars Textures #"] = SkyTexture.TextureCache.Count;
                        if (SkyTexture.TextureCache.ContainsKey (key)) {
                            SkyTexture.Texture = SkyTexture.TextureCache [key];
                        }
                        else {
                            for (int s = 0; s < SkyTexture.SmallStars.Length; ++s) {
                                Star star = SkyTexture.SmallStars [s];
                                int i = star.H * SkyTexture.Width + star.W;
                                SkyTexture.Colors [i] = star.Color * (float)Math.Abs(Math.Sin (MathHelper.TwoPi * alphaCounter + star.AlphaDiff));
                            }
                            for (int s = 0; s < SkyTexture.BigStars.Length; ++s) {
                                Star star = SkyTexture.BigStars [s];
                                int i = star.H * SkyTexture.Width + star.W;
                                Color color = star.Color * (float)Math.Abs(Math.Sin (MathHelper.TwoPi * alphaCounter + star.AlphaDiff));
                                SkyTexture.Colors [i] = color;
                                SkyTexture.Colors [i + 1] = color;
                                SkyTexture.Colors [i - 1] = color;
                                SkyTexture.Colors [i + SkyTexture.Width] = color;
                                SkyTexture.Colors [i - SkyTexture.Width] = color;
                            }
                            Texture2D tex = new Texture2D (Screen.GraphicsDevice, SkyTexture.Width, SkyTexture.Height);
                            tex.SetData (SkyTexture.Colors);
                            SkyTexture.Texture = SkyTexture.TextureCache [key] = tex;
                        }
                    }
                };
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = World.Viewport;

            effect.World = scaleMatrix * World.Camera.WorldMatrix;
            effect.Projection = World.Camera.ProjectionMatrix;

            Matrix skyboxView = World.Camera.ViewMatrix;
            skyboxView.M41 = 0;
            skyboxView.M42 = 0;
            skyboxView.M43 = 0;
            effect.View = skyboxView;

            effect.AmbientLightColor = new Vector3 (0.8f, 0.8f, 0.8f);
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = false;

            effect.LightingEnabled = false;
            string effectName = Config.Default ["video", "knot-shader", "default"];
            if (Screen.InputManager.KeyHeldDown (Keys.F7) || effectName == "celshader") {
                effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
            }

            /*
            for (int i = 0; i < rectangles.Length; ++i) {
                effect.Texture = SkyTextures [i].Texture;
                rectangles [i].Draw (effect);
            }*/

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
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

        private class SkyTexture
        {
            public Texture2D Texture;
            public Dictionary<int, Texture2D> TextureCache = new Dictionary<int, Texture2D> ();
            public Color[] Colors;
            public Star[] BigStars;
            public Star[] SmallStars;
            public int Width;
            public int Height;
        }

        private class Star
        {
            public int W;
            public int H;
            public Color Color;
            public float AlphaDiff;

            public Star (Random random)
            {
                AlphaDiff = (float)random.Next (1000);
            }
        }
    }
}
