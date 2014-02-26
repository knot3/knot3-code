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
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

namespace Knot3.Framework.Widgets
{
    [ExcludeFromCodeCoverageAttribute]
    public class Border : Widget
    {
        public int LineWidth { get; set; }

        public int Padding { get; set; }

        private Lines lines;
        private Vector2 lastPosition = Vector2.Zero;
        private Vector2 lastSize = Vector2.Zero;

        public override bool IsEnabled
        {
            get {
                return base.IsEnabled;
            }
            set {
                base.IsEnabled = value;
                lines.IsEnabled = value;
            }
        }

        private Action<GameTime> OnUpdate = (time) => {};

        public Border (IGameScreen screen, DisplayLayer drawOrder, Bounds bounds,
                       int lineWidth, int padding, Color lineColor, Color outlineColor)
        : base (screen, drawOrder)
        {
            LineWidth = lineWidth;
            Padding = padding;
            lines = new Lines (screen, drawOrder, lineWidth, lineColor, outlineColor);
            Bounds = bounds;
        }

        public Border (IGameScreen screen, DisplayLayer drawOrder, Widget widget, int lineWidth, int padding,
                       Color lineColor, Color outlineColor)
        : this (screen, drawOrder, widget.Bounds, lineWidth, padding, lineColor, outlineColor)
        {
            OnUpdate += (time) => IsVisible = lines.IsVisible = widget.IsVisible;
        }

        public Border (IGameScreen screen, DisplayLayer drawOrder, Bounds bounds, int lineWidth, int padding)
        : this (screen: screen, drawOrder: drawOrder, bounds: bounds, lineWidth: lineWidth, padding: lineWidth,
                lineColor: Design.DefaultLineColor, outlineColor: Design.DefaultOutlineColor)
        {
        }

        public Border (IGameScreen screen, DisplayLayer drawOrder, Widget widget, int lineWidth, int padding)
        : this (screen: screen, drawOrder: drawOrder, widget: widget, lineWidth: lineWidth, padding: lineWidth,
                lineColor: Design.DefaultLineColor, outlineColor: Design.DefaultOutlineColor)
        {
        }

        public Border (IGameScreen screen, DisplayLayer drawOrder, Widget widget)
        : this (screen: screen, drawOrder: drawOrder, widget: widget, lineWidth: 2, padding: 0)
        {
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            Vector2 position = Bounds.Position.Relative;
            Vector2 size = Bounds.Size.Relative;

            if (position != lastPosition || size != lastSize) {
                lastPosition = position;
                lastSize = size;
                Vector2 padding = Vector2.One * 0.001f * Padding;
                lines.Clear ();
                lines.AddPoints (
                    position.X - padding.X,
                    position.Y - padding.Y,
                    position.X + size.X + padding.X,
                    position.Y + size.Y + padding.Y,
                    position.X - padding.X,
                    position.Y - padding.Y
                );
            }

            OnUpdate (time);

            base.Update (time);
        }

        public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableGameScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return lines;
        }
    }
}
