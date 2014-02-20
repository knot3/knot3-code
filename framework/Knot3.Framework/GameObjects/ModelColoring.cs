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

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.GameObjects
{
	[ExcludeFromCodeCoverageAttribute]
	public abstract class ModelColoring
	{
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
			HighlightColor = color;
			HighlightIntensity = intensity;
		}

		public void Unhighlight ()
		{
			HighlightColor = Color.Transparent;
			HighlightIntensity = 0f;
		}

		/// <summary>
		/// Die Transparenz des Modells.
		/// </summary>
		public float Alpha { get; set; }

		public abstract Color MixedColor { get; }

		public abstract bool IsTransparent { get; }
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
		public Color BaseColor { get; set; }

		public override Color MixedColor { get { return BaseColor.Mix (HighlightColor, HighlightIntensity); } }

		public override bool IsTransparent { get { return BaseColor == Color.Transparent; } }
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
		public Color Color1 { get; set; }

		/// <summary>
		/// Die zweite Farbe des Modells.
		/// </summary>
		public Color Color2 { get; set; }

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
	}
}
