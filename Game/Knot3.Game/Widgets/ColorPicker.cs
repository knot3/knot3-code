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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

namespace Knot3.Game.Widgets
{
    /// <summary>
    /// Ein Steuerelement der grafischen Benutzeroberfläche, das eine Auswahl von Farben ermöglicht.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ColorPicker : Widget, IKeyEventListener, IMouseClickEventListener
    {
        /// <summary>
        /// Die ausgewählte Farbe.
        /// </summary>
        public Color SelectedColor { get; set; }

        /// <summary>
        /// Wird aufgerufen, wenn eine neue Farbe ausgewählt wurde.
        /// </summary>
        public Action<Color, GameTime> ColorSelected { get; set; }

        private List<Color> colors;
        private List<ScreenPoint> tiles;
        private ScreenPoint tileSize;
        private SpriteBatch spriteBatch;

        public Bounds MouseClickBounds { get { return Bounds; } }

        /// <summary>
        /// Erzeugt eine neue Instanz eines ColorPicker-Objekts und initialisiert diese
        /// mit der Farbe, auf welche der Farbwähler beim Aufruf aus Sicht des Spielers zeigt.
        /// </summary>
        public ColorPicker (IScreen screen, DisplayLayer drawOrder, Color def)
        : base (screen, drawOrder)
        {
            tileSize = new ScreenPoint (screen, 0.032f, 0.032f);

            // Widget-Attribute
            BackgroundColorFunc = (s) => Design.WidgetBackground;
            ForegroundColorFunc = (s) => Design.WidgetForeground;
            AlignX = HorizontalAlignment.Left;
            AlignY = VerticalAlignment.Top;

            // die Farb-Tiles
            colors = new List<Color> (CreateColors (64));
            colors.Sort (ColorHelper.SortColorsByLuminance);
            tiles = new List<ScreenPoint> (CreateTiles (colors));

            // einen Spritebatch
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);

            // Position und Größe
            Bounds.Position = ScreenPoint.Centered (screen, Bounds);
            Bounds.Size.RelativeFunc = () => {
                float sqrt = (float)Math.Ceiling (Math.Sqrt (colors.Count));
                return tileSize * sqrt;
            };

            // Die Callback-Funktion zur Selektion setzt das SelectedColor-Attribut
            ColorSelected += (color, time) => {
                SelectedColor = color;
            };
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (IsVisible) {
                spriteBatch.Begin ();

                // color tiles
                int i = 0;
                foreach (ScreenPoint tile in tiles) {
                    Bounds tileBounds = new Bounds (Bounds.Position + tile, tileSize);
                    Rectangle rect = tileBounds.Rectangle.Shrink (1);
                    Texture2D dummyTexture = ContentLoader.CreateTexture (Screen.GraphicsDevice, colors [i]);
                    spriteBatch.Draw (dummyTexture, rect, Color.White);

                    ++i;
                }

                spriteBatch.End ();
            }
        }

        /// <summary>
        /// Reagiert auf Tastatureingaben.
        /// </summary>
        public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
        }

        /// <summary>
        /// Bei einem Linksklick wird eine Farbe ausgewählt und im Attribut Color abgespeichert.
        /// </summary>
        public void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
            Log.Debug ("ColorPicker.OnLeftClick: position=", position);
            int i = 0;
            foreach (ScreenPoint tile in tiles) {
                //Log.Debug ("ColorPicker: tile=", tile, "  "
                //	+ (tile.X <= position.X) + " " + (tile.X + tileSize.X > position.X) + " " + (
                //                       tile.Y <= position.Y) + " " + (tile.Y + tileSize.Y > position.Y)
                //);
                if (tile.Relative.X <= position.X && tile.Relative.X + tileSize.Relative.X > position.X
                        && tile.Relative.Y <= position.Y && tile.Relative.Y + tileSize.Relative.Y > position.Y) {
                    Log.Debug ("ColorPicker: color=", colors [i]);

                    ColorSelected (colors [i], time);
                }
                ++i;
            }
        }

        /// <summary>
        /// Bei einem Rechtsklick geschieht nichts.
        /// </summary>
        public void OnRightClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        public void SetHovered (bool hovered, GameTime time)
        {
        }

        private static IEnumerable<Color> CreateColors (int num)
        {
            float steps = (float)Math.Pow (num, 1.0 / 3.0);
            int n = 0;
            for (int r = 0; r < steps; ++r) {
                for (int g = 0; g < steps; ++g) {
                    for (int b = 0; b < steps; ++b) {
                        yield return new Color (new Vector3 (r, g, b) / steps);
                        ++n;
                    }
                }
            }
        }

        private IEnumerable<ScreenPoint> CreateTiles (IEnumerable<Color> _colors)
        {
            Color[] colors = _colors.ToArray ();
            float sqrt = (float)Math.Sqrt (colors.Length);
            int row = 0;
            int column = 0;
            foreach (Color color in colors) {
                yield return new ScreenPoint (Screen, tileSize * new Vector2 (column, row));

                ++column;
                if (column >= sqrt) {
                    column = 0;
                    ++row;
                }
            }
        }
    }
}
