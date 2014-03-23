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
    public class SkyCube : GameObject, IEnumerable<IGameObject>
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
        /// Die einzelnen Rechtecke.
        /// </summary>
        private Parallelogram[] rectangles;
        /// <summary>
        /// Der Zufallsgenerator.
        /// </summary>
        private Random random;
        /// <summary>
        /// Der Texture-Cache.
        /// </summary>
        private Dictionary<Direction, SkyTexture> textureCache = new Dictionary<Direction, SkyTexture> ();
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
        public SkyCube (IScreen screen, Vector3 position, float distance)
        {
            Screen = screen;
            Position = position;
            random = new Random ();
            effect = new BasicEffect (screen.GraphicsDevice);
            Distance = distance;
            ConstructRectangles ();
        }

        private void ConstructRectangles ()
        {
            if (rectangles != null) {
                foreach (Parallelogram parallelogram in rectangles) {
                    parallelogram.Dispose ();
                }
            }
            rectangles = new Parallelogram [Direction.Values.Length];
            int i = 0;
            foreach (Direction direction in Direction.Values) {
                Vector3 position = direction;
                Vector3 up = direction == Direction.Up || direction == Direction.Down ? Vector3.Forward : Vector3.Up;
                Vector3 left = Vector3.Normalize (Vector3.Cross (position, up));
                Parallelogram parallelogram = new Parallelogram (
                    device: Screen.GraphicsDevice,
                    left: left,
                    width: 2,
                    up: up,
                    height: 2,
                    origin: position,
                    normalToCenter: true
                );
                rectangles [i] = parallelogram;
                ++i;
            }
            if (SkyTextures == null) {
                SkyTextures = new SkyTexture [Direction.Values.Length];
                i = 0;
                foreach (Direction direction in Direction.Values) {
                    SkyTextures [i] = CachedSkyTexture (direction);
                    ++i;
                }
            }
        }

        private SkyTexture CachedSkyTexture (Direction direction)
        {
            if (textureCache.ContainsKey (direction)) {
                return textureCache [direction];
            }
            else if (textureCache.ContainsKey (direction.Reverse)) {
                return textureCache [direction.Reverse];
            }
            else {
                return textureCache [direction] = CreateSkyTexture ();
            }
        }

        private SkyTexture CreateSkyTexture ()
        {
            string effectName = Config.Default ["video", "knot-shader", "default"];
            if (Config.Default ["debug", "projector-mode", false]) {
                return CreateSolidColorTexture (ContentLoader.CreateTexture (Screen.GraphicsDevice, Color.White));
            }
            else if (effectName == "celshader") {
                return CreateSolidColorTexture (ContentLoader.CreateTexture (Screen.GraphicsDevice, Color.CornflowerBlue));
            }
            else {
                return CreateSpaceTexture ();
            }
        }

        private SkyTexture CreateSolidColorTexture (Texture2D texture)
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            SkyTexture skyTexture = new SkyTexture {
                Texture = texture,
                SmallStars = new Star[0],
                BigStars = new Star[0],
                Width = texture.Width,
                Height = texture.Height,
                Colors = colors
            };
            return skyTexture;
        }

        private SkyTexture CreateSpaceTexture ()
        {
            List<Star> bigStars = new List<Star> ();
            List<Star> smallStars = new List<Star> ();
            int width = 800;
            int height = 800;
            Texture2D texture = new Texture2D (Screen.GraphicsDevice, width, height);
            Color[] colors = new Color [width * height];
            for (int i = 0; i < colors.Length; i++) {
                colors [i] = Color.Black;
            }
            for (int h = 2; h+2 < height; ++h) {
                for (int w = 2; w+2 < width; ++w) {
                    int i = h * width + w;
                    if (random.Next () % (width * 3) == w) {
                        float alpha = (1 + random.Next () % 3) / 3f;
                        Color white = Color.White * alpha;
                        colors [i] = white;
                        colors [i + 1] = white;
                        colors [i - 1] = white;
                        colors [i + width] = white;
                        colors [i - width] = white;
                        bigStars.Add (new Star (random) { W = w, H = h, Color = Color.White });
                    }
                    else if (random.Next () % (width * 3) == w) {
                        float alpha = (1 + random.Next () % 3) / 3f;
                        Color white = Color.White * alpha;
                        colors [i] = white;
                        smallStars.Add (new Star (random) { W = w, H = h, Color = Color.White });
                    }
                    else if (random.Next () % (width * 3) == w) {
                        float alpha = (1 + random.Next () % 3) / 3f;
                        Color green = Color.LimeGreen * alpha;
                        colors [i] = green;
                        smallStars.Add (new Star (random) { W = w, H = h, Color = Color.LimeGreen });
                    }
                    else if (random.Next () % (width * 3) == w) {
                        float alpha = (1 + random.Next () % 3) / 3f;
                        Color green = Color.DarkSeaGreen * alpha;
                        colors [i] = green;
                        smallStars.Add (new Star (random) { W = w, H = h, Color = Color.DarkSeaGreen });
                    }
                }
            }
            texture.SetData (colors);
            SkyTexture skyTexture = new SkyTexture {
                Texture = texture,
                SmallStars = smallStars.ToArray (),
                BigStars = bigStars.ToArray (),
                Width = width,
                Height = height,
                Colors = colors
            };
            return skyTexture;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            if (Math.Abs (1f - (Distance / (World.Camera.FarPlane / 3.6f))) > 0.05f) {
                Distance = World.Camera.FarPlane / 3.6f;
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

            for (int i = 0; i < rectangles.Length; ++i) {
                effect.Texture = SkyTextures [i].Texture;
                rectangles [i].Draw (effect);
            }

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
