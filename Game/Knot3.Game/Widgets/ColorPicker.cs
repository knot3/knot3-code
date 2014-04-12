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

        private Color[] colors;
        private ScreenPoint[] tiles;
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
            BackgroundColorFunc = state => Design.WidgetBackground;
            ForegroundColorFunc = state => Design.WidgetForeground;
            AlignX = HorizontalAlignment.Left;
            AlignY = VerticalAlignment.Top;

            // die Farb-Tiles
            colors = AllColors;
            Array.Sort (colors, ColorHelper.SortColorsByLuminance);
            int maxRow = 0, maxColumn = 0;
            tiles = CreateTiles (colors: colors, maxRow: ref maxRow, maxColumn: ref maxColumn);

            // einen Spritebatch
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);

            // Position und Größe
            Bounds.Position = ScreenPoint.Centered (screen, Bounds);
            Bounds.Size = new ScreenPoint (Screen, tileSize * new Vector2 (maxColumn + 1, maxRow + 1));

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

        private ScreenPoint[] CreateTiles (Color[] colors, ref int maxRow, ref int maxColumn)
        {
            List<ScreenPoint> tiles = new List<ScreenPoint> ();
            float sqrt = (float)Math.Sqrt (colors.Length);
            int row = 0;
            int column = 0;
            foreach (Color color in colors) {
                tiles.Add (new ScreenPoint (Screen, tileSize * new Vector2 (column, row)));

                maxRow = Math.Max (row, maxRow);
                maxColumn = Math.Max (column, maxColumn);

                ++column;
                if (column >= sqrt) {
                    column = 0;
                    ++row;
                }
            }
            return tiles.ToArray ();
        }

        public static Color[] AllColors = {
            Color.Aqua,
            Color.Aquamarine,
            Color.Azure,
            Color.Beige,
            Color.Bisque,
            Color.Black,
            Color.BlanchedAlmond,
            Color.Blue,
            Color.BlueViolet,
            Color.Brown,
            Color.BurlyWood,
            Color.CadetBlue,
            Color.Chartreuse,
            Color.Chocolate,
            Color.Coral,
            Color.CornflowerBlue,
            Color.Cornsilk,
            Color.Crimson,
            Color.Cyan,
            Color.DarkBlue,
            Color.DarkCyan,
            Color.DarkGoldenrod,
            Color.DarkGray,
            Color.DarkGreen,
            Color.DarkKhaki,
            Color.DarkMagenta,
            Color.DarkOliveGreen,
            Color.DarkOrange,
            Color.DarkOrchid,
            Color.DarkRed,
            Color.DarkSalmon,
            Color.DarkSeaGreen,
            Color.DarkSlateBlue,
            Color.DarkSlateGray,
            Color.DarkTurquoise,
            Color.DarkViolet,
            Color.DeepPink,
            Color.DeepSkyBlue,
            Color.DimGray,
            Color.DodgerBlue,
            Color.Firebrick,
            Color.ForestGreen,
            Color.Fuchsia,
            Color.Gainsboro,
            Color.Gold,
            Color.Goldenrod,
            Color.Gray,
            Color.Green,
            Color.GreenYellow,
            Color.Honeydew,
            Color.HotPink,
            Color.IndianRed,
            Color.Indigo,
            Color.Ivory,
            Color.Khaki,
            Color.Lavender,
            Color.LavenderBlush,
            Color.LawnGreen,
            Color.LemonChiffon,
            Color.LightBlue,
            Color.LightCoral,
            Color.LightCyan,
            Color.LightGoldenrodYellow,
            Color.LightGray,
            Color.LightGreen,
            Color.LightPink,
            Color.LightSalmon,
            Color.LightSeaGreen,
            Color.LightSkyBlue,
            Color.LightSlateGray,
            Color.LightSteelBlue,
            Color.LightYellow,
            Color.Lime,
            Color.LimeGreen,
            Color.Linen,
            Color.Magenta,
            Color.Maroon,
            Color.MediumAquamarine,
            Color.MediumBlue,
            Color.MediumOrchid,
            Color.MediumPurple,
            Color.MediumSeaGreen,
            Color.MediumSlateBlue,
            Color.MediumSpringGreen,
            Color.MediumTurquoise,
            Color.MediumVioletRed,
            Color.MidnightBlue,
            Color.MintCream,
            Color.MistyRose,
            Color.Moccasin,
            Color.NavajoWhite,
            Color.Navy,
            Color.OldLace,
            Color.Olive,
            Color.OliveDrab,
            Color.Orange,
            Color.OrangeRed,
            Color.Orchid,
            Color.PaleGoldenrod,
            Color.PaleGreen,
            Color.PaleTurquoise,
            Color.PaleVioletRed,
            Color.PapayaWhip,
            Color.PeachPuff,
            Color.Peru,
            Color.Pink,
            Color.Plum,
            Color.PowderBlue,
            Color.Purple,
            Color.Red,
            Color.RosyBrown,
            Color.RoyalBlue,
            Color.SaddleBrown,
            Color.SandyBrown,
            Color.SeaGreen,
            Color.SeaShell,
            Color.Sienna,
            Color.Silver,
            Color.SkyBlue,
            Color.Yellow,
            Color.YellowGreen
        };
    }
}
