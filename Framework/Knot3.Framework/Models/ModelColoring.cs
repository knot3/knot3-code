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

using Knot3.Framework.Utilities;

namespace Knot3.Framework.Models
{
    [ExcludeFromCodeCoverageAttribute]
    public abstract class ModelColoring
    {
        public Action OnColorChanged = () => {};

        public ModelColoring ()
        {
            HighlightColor = Color.Transparent;
            HighlightIntensity = 0f;
            Alpha = 1f;
        }

        /// <summary>
        /// Die Auswahlfarbe des Modells.
        /// </summary>
        public Color HighlightColor { get; private set; }

        /// <summary>
        /// Die Intensität der Auswahlfarbe.
        /// </summary>
        public float HighlightIntensity { get; private set; }

        public void Highlight (float intensity, Color color)
        {
            if (HighlightColor != color || HighlightIntensity != intensity) {
                HighlightColor = color;
                HighlightIntensity = intensity;
                OnColorChanged ();
            }
        }

        public void Unhighlight ()
        {
            if (HighlightIntensity > 0f) {
                HighlightColor = Color.Transparent;
                HighlightIntensity = 0f;
                OnColorChanged ();
            }
        }

        /// <summary>
        /// Die Transparenz des Modells.
        /// </summary>
        public float Alpha { get; set; }

        public abstract Color MixedColor { get; }

        public abstract bool IsTransparent { get; }

        protected int ColorHashCode (Color color)
        {
            return (int)color.PackedValue;
        }
    }

    [ExcludeFromCodeCoverageAttribute]
    public sealed class SingleColor : ModelColoring
    {
        public SingleColor (Color color)
        : base ()
        {
            BaseColor = color;
        }

        public SingleColor (Color color, float alpha)
        : this (color)
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Die Farbe des Modells.
        /// </summary>
        public Color BaseColor { get { return _baseColor; } set { if (_baseColor != value) { _baseColor = value; OnColorChanged (); } } }

        private Color _baseColor;

        public override Color MixedColor { get { return BaseColor.Mix (HighlightColor, HighlightIntensity); } }

        public override bool IsTransparent { get { return BaseColor == Color.Transparent; } }

        public override int GetHashCode ()
        {
            return ColorHashCode (HighlightColor) * 83*83 + ColorHashCode (BaseColor) * 83 + (int)(Alpha*1000);
        }
    }

    [ExcludeFromCodeCoverageAttribute]
    public sealed class GradientColor : ModelColoring
    {
        public GradientColor (Color color1, Color color2)
        : base ()
        {
            Color1 = color1;
            Color2 = color2;
        }
        public GradientColor (Color color1, Color color2, float alpha)
        : this (color1, color2)
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Die erste Farbe des Modells.
        /// </summary>
        public Color Color1 { get { return _color1; } set { if (_color1 != value) { _color1 = value; OnColorChanged (); } } }

        private Color _color1;

        /// <summary>
        /// Die zweite Farbe des Modells.
        /// </summary>
        public Color Color2 { get { return _color2; } set { if (_color2 != value) { _color2 = value; OnColorChanged (); } } }

        private Color _color2;

        public override Color MixedColor
        {
            get {
                return Color1.Mix (Color2, 0.5f).Mix (HighlightColor, HighlightIntensity);
            }
        }

        public override bool IsTransparent
        {
            get {
                return Color1 == Color.Transparent && Color2 == Color.Transparent;
            }
        }

        public override int GetHashCode ()
        {
            return ColorHashCode (HighlightColor) * 83 * 83 * 83 + ColorHashCode (Color1) * 83 * 83 + ColorHashCode (Color2) * 83 + (int)(Alpha*1000);
        }
    }
}
