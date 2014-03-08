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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;

namespace Knot3.Framework.Math
{
    public class Bounds
    {
        /// <summary>
        /// Die von der Auflösung unabhängige Position in Prozent.
        /// </summary>
        public ScreenPoint Position
        {
            get { return _position; }
            set { _position.Assign (value); }
        }

        private ScreenPoint _position;

        /// <summary>
        /// Die von der Auflösung unabhängige Größe in Prozent.
        /// </summary>
        public ScreenPoint Size
        {
            get { return _size; }
            set { _size.Assign (value); }
        }

        private ScreenPoint _size;

        /// <summary>
        /// Der von der Auflösung unabhängige Abstand in Prozent.
        /// </summary>
        public ScreenPoint Padding
        {
            get { return _padding; }
            set { _padding.Assign (value); }
        }

        private ScreenPoint _padding;

        /// <summary>
        /// Gibt ein auf die Auflösujng skaliertes Rechteck zurück, das in den XNA-Klassen verwendet werden kann.
        /// </summary>
        public Rectangle Rectangle
        {
            get {
                Point pos = Position.Absolute;
                Point size = Size.Absolute;
                return new Rectangle (pos.X, pos.Y, size.X, size.Y);
            }
        }

        public Vector4 Vector4
        {
            get {
                Point pos = Position.Absolute;
                Point size = Size.Absolute;
                return new Vector4 (pos.X, pos.Y, size.X, size.Y);
            }
        }

        public Bounds (ScreenPoint position, ScreenPoint size, ScreenPoint padding)
        {
            _position = position;
            _size = size;
            _padding = padding;
        }

        public Bounds (ScreenPoint position, ScreenPoint size)
        {
            _position = position;
            _size = size;
            _padding = new ScreenPoint (position.Screen, Vector2.Zero);
        }

        public Bounds (IScreen screen, float relX, float relY, float relWidth, float relHeight)
        {
            _position = new ScreenPoint (screen, relX, relY);
            _size = new ScreenPoint (screen, relWidth, relHeight);
            _padding = new ScreenPoint (screen, Vector2.Zero);
        }

        public bool Contains (Point point)
        {
            return Rectangle.Contains (point);
        }

        public bool Contains (ScreenPoint point)
        {
            return Rectangle.Contains ((Point)point);
        }

        public static Bounds Zero (IScreen screen)
        {
            return new Bounds (
                       position: ScreenPoint.Zero (screen),
                       size: ScreenPoint.Zero (screen),
                       padding: ScreenPoint.Zero (screen)
                   );
        }

        public Bounds FromLeft (Func<float> percent)
        {
            return new Bounds (
                       position: Position,
                       size: new ScreenPoint (Size.Screen, () => Size.Relative.X * percent (), () => Size.Relative.Y),
                       padding: Padding
                   );
        }

        public Bounds FromRight (Func<float> percent)
        {
            return new Bounds (
                       position: Position + new ScreenPoint (Size.Screen, () => Size.Relative.X * (1f - percent ()), () => 0),
                       size: new ScreenPoint (Size.Screen, () => Size.Relative.X * percent (), () => Size.Relative.Y),
                       padding: Padding
                   );
        }

        public Bounds FromTop (Func<float> percent)
        {
            return new Bounds (
                       position: Position,
                       size: new ScreenPoint (Size.Screen, () => Size.Relative.X, () => Size.Relative.Y * percent ()),
                       padding: Padding
                   );
        }

        public Bounds FromBottom (Func<float> percent)
        {
            return new Bounds (
                       position: Position + new ScreenPoint (Size.Screen, () => 0, () => Size.Relative.Y * (1f - percent ())),
                       size: new ScreenPoint (Size.Screen, () => Size.Relative.X, () => Size.Relative.Y * percent ()),
                       padding: Padding
                   );
        }

        public Bounds FromLeft (float percent)
        {
            return FromLeft (() => percent);
        }

        public Bounds FromRight (float percent)
        {
            return FromRight (() => percent);
        }

        public Bounds FromTop (float percent)
        {
            return FromTop (() => percent);
        }

        public Bounds FromBottom (float percent)
        {
            return FromBottom (() => percent);
        }

        public Bounds In (Bounds container)
        {
            return new Bounds (Position + container.Position, Size, Padding);
        }

        public Bounds Grow (int x, int y)
        {
            ScreenPoint diff = ScreenPoint.FromAbsolute (x, y, Position.Screen);
            return new Bounds (Position - diff, Size + diff * 2);
        }

        public Bounds Shrink (int x, int y)
        {
            return Grow (-x, -y);
        }

        public Bounds Grow (int xy)
        {
            return Grow (xy, xy);
        }

        public Bounds Shrink (int xy)
        {
            return Grow (-xy, -xy);
        }

        public static implicit operator Rectangle (Bounds bounds)
        {
            return bounds.Rectangle;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            return "(" + Position.Relative.X + "x" + Position.Relative.Y + "," + Size.Relative.X + "x" + Size.Relative.Y + ")";
        }
    }
}
