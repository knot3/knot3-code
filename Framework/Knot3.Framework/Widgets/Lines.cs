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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Widgets
{
    [ExcludeFromCodeCoverageAttribute]
    public sealed class Lines : DrawableScreenComponent
    {
        private Texture2D texture;
        private SpriteBatch spriteBatch;

        public bool IsEnabled { get; set; }

        public bool IsVisible { get; set; }

        // die Punkte, zwischen denen die Linien gezeichnet werden sollen
        private List<Vector2> points;

        // die Dicke der Linien
        private int lineWidth;

        // die Farben der Linien
        public Color LineColor { get; private set; }

        public Color OutlineColor { get; private set; }

        public Lines (IScreen screen, DisplayLayer drawOrder, int lineWidth, Color lineColor, Color outlineColor)
        : base (screen, drawOrder)
        {
            this.lineWidth = lineWidth;
            points = new List<Vector2> ();
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);
            texture = ContentLoader.CreateTexture (Screen.GraphicsDevice, Color.White);
            LineColor = lineColor;
            OutlineColor = outlineColor;
            IsEnabled = true;
            IsVisible = true;
        }

        public Lines (IScreen screen, DisplayLayer drawOrder, int lineWidth)
        : this (screen, drawOrder, lineWidth, Design.DefaultLineColor, Design.DefaultOutlineColor)
        {
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (IsVisible) {
                int scaledLineWidth = (int)(new Vector2 (lineWidth, lineWidth).Scale (Screen.Viewport).X);

                if (points.Count >= 2) {
                    Rectangle[] rects = new Rectangle [points.Count - 1];
                    for (int i = 1; i < points.Count; ++i) {
                        Vector2 nodeA = points [i - 1];
                        Vector2 nodeB = points [i];
                        if (nodeA.X == nodeB.X || nodeA.Y == nodeB.Y) {
                            Vector2 direction = (nodeB - nodeA).PrimaryDirection ();
                            Vector2 position = nodeA.Scale (Screen.Viewport);
                            int length = (int)(nodeB - nodeA).Scale (Screen.Viewport).Length ();
                            if (direction.X == 0 && direction.Y > 0) {
                                rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X, position.Y, 0, length);
                            }
                            else if (direction.X == 0 && direction.Y < 0) {
                                rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X, position.Y - length, 0, length);
                            }
                            else if (direction.Y == 0 && direction.X > 0) {
                                rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X, position.Y, length, 0);
                            }
                            else if (direction.Y == 0 && direction.X < 0) {
                                rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X - length, position.Y, length, 0);
                            }
                        }
                    }

                    spriteBatch.Begin ();
                    foreach (Rectangle inner in rects) {
                        Rectangle outer = new Rectangle (inner.X - 1, inner.Y - 1, inner.Width + 2, inner.Height + 2);
                        spriteBatch.Draw (texture, outer, Design.DefaultOutlineColor * (IsEnabled ? 1f : 0.5f));
                    }
                    foreach (Rectangle rect in rects) {
                        spriteBatch.Draw (texture, rect, Design.DefaultLineColor * (IsEnabled ? 1f : 0.5f));
                    }
                    spriteBatch.End ();
                }
            }
        }

        public void AddPoints (float startX, float startY, params float[] xyxy)
        {
            Vector2 start = new Vector2 (startX, startY);
            points.Add (start);

            Vector2 current = start;
            for (int i = 0; i < xyxy.Length; ++i) {
                // this is a new X value
                if (i % 2 == 0) {
                    current.X = xyxy [i];
                }
                // this is a new Y value
                else {
                    current.Y = xyxy [i];
                }

                points.Add (current);
            }
        }

        public void Clear ()
        {
            points.Clear ();
        }
    }
}
