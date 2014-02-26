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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Effects;
using Knot3.Game.Input;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

namespace Knot3.Game.Models
{
    [ExcludeFromCodeCoverageAttribute]
    public class SkyCube : IGameObject, IEnumerable<IGameObject>
    {
        /// <summary>
        /// Der dazugehörige Screen.
        /// </summary>
        private IGameScreen Screen;

        /// <summary>
        /// Enthält Informationen über die Position des Mitte der Welt.
        /// </summary>
        public GameObjectInfo Info { get; private set; }

        /// <summary>
        /// Die Spielwelt.
        /// </summary>
        public World World
        {
            get { return _world; }
            set {
                _world = value;
                assignWorld ();
            }
        }

        private World _world;

        /// <summary>
        /// Die Distanz zu den Wänden vom Ursprung aus.
        /// </summary>
        public float Distance
        {
            get { return _distance; }
            set {
                _distance = value;
                ConstructRectangles ();
            }
        }

        private float _distance;

        /// <summary>
        /// Die einzelnen texturierten Rechtecke.
        /// </summary>
        private List<TexturedRectangle> rectangles = new List<TexturedRectangle> ();

        /// <summary>
        /// Der Zufallsgenerator.
        /// </summary>
        private Random random;

        /// <summary>
        /// Der Texture-Cache.
        /// </summary>
        private Dictionary<Direction, Texture2D> textureCache = new Dictionary<Direction, Texture2D>();

        /// <summary>
        /// Erstellt ein neues KnotRenderer-Objekt für den angegebenen Spielzustand mit den angegebenen
        /// Spielobjekt-Informationen, die unter Anderem die Position des Knotenursprungs enthalten.
        /// </summary>
        public SkyCube (IGameScreen screen, Vector3 position)
        {
            Screen = screen;
            Info = new GameObjectInfo (position: position);
            random = new Random ();
        }

        public SkyCube (IGameScreen screen, Vector3 position, float distance)
        : this (screen, position)
        {
            Distance = distance;
            ConstructRectangles ();
        }

        private void ConstructRectangles ()
        {
            rectangles.Clear ();
            foreach (Direction direction in Direction.Values) {
                Vector3 position = direction * Distance;
                Vector3 up = direction == Direction.Up || direction == Direction.Down ? Vector3.Forward : Vector3.Up;
                Vector3 left = Vector3.Normalize (Vector3.Cross (position, up));
                TexturedRectangleInfo info = new TexturedRectangleInfo (
                    // texturename: "sky1",
                    texture: CachedSkyTexture (direction),
                    origin: position,
                    left: left,
                    width: 2 * Distance,
                    up: up,
                    height: 2 * Distance
                );

                rectangles.Add (new TexturedRectangle (Screen, info));
            }
            assignWorld ();
        }

        private Texture2D CachedSkyTexture (Direction direction)
        {
            if (!textureCache.ContainsKey (direction)) {
                textureCache [direction] = CreateSkyTexture ();
            }
            return textureCache [direction];
        }

        private Texture2D CreateSkyTexture ()
        {
            string effectName = Config.Default ["video", "knot-shader", "default"];
            if (effectName == "celshader") {
                return ContentLoader.CreateTexture (Screen.Device, Color.CornflowerBlue);
            }
            else {
                return CreateSpaceTexture ();
            }
        }

        private Texture2D CreateSpaceTexture ()
        {
            int width = 1000;
            int height = 1000;
            Texture2D texture = new Texture2D (Screen.Device, width, height);
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++) {
                colors [i] = Color.Black;
            }
            for (int h = 2; h+2 < height; ++h) {
                for (int w = 2; w+2 < width; ++w) {
                    int i = h * width + w;
                    if (random.Next () % (width * 3) == w) {
                        float alpha = (1 + random.Next () % 3) / 3f;
                        Color white = Color.White * alpha;
                        Color gray = Color.Gray * alpha;
                        colors [i] = white;
                        colors [i - 1] = white;
                        colors [i + 1] = white;
                        colors [i - width] = white;
                        colors [i - width - 1] = gray;
                        colors [i - width + 1] = gray;
                        colors [i + width] = white;
                        colors [i + width - 1] = gray;
                        colors [i + width + 1] = gray;
                    }
                }
            }
            texture.SetData (colors);
            return texture;
        }

        private void assignWorld ()
        {
            foreach (TexturedRectangle rect in rectangles) {
                rect.World = World;
            }
        }

        /// <summary>
        /// Gibt den Ursprung des Knotens zurück.
        /// </summary>
        public Vector3 Center ()
        {
            return Info.Position;
        }

        [ExcludeFromCodeCoverageAttribute]
        public void Update (GameTime time)
        {
            if (World.Camera.MaxPositionDistance + 500 != Distance) {
                Distance = World.Camera.MaxPositionDistance + 500;
            }
            foreach (TexturedRectangle rect in rectangles) {
                rect.Update (time);
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public void Draw (GameTime time)
        {
            foreach (TexturedRectangle rect in rectangles) {
                rect.Draw (time);
            }
        }

        public GameObjectDistance Intersects (Ray ray)
        {
            return null;
        }

        /// <summary>
        /// Gibt einen Enumerator der aktuell vorhandenen 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            foreach (TexturedRectangle rect in rectangles) {
                yield return rect;
            }
        }

        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }
    }
}
