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

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

#endregion

namespace Knot3.Framework.Core
{
	public class ScreenPoint : IEquatable<ScreenPoint>
	{
		#region Properties

		public IGameScreen Screen { get; private set; }

		public Vector2 Relative
		{
			get {
				return RelativeFunc ();
			}
			set {
				RelativeFunc = () => value;
			}
		}

		public Func<Vector2> RelativeFunc
		{
			set;
			private get;
		}

		public Point Absolute
		{
			get {
				Vector2 scaled = Relative.Scale (Screen.Viewport);
				return new Point ((int)scaled.X, (int)scaled.Y);
			}
		}

		public Vector2 AbsoluteVector
		{
			get {
				return Relative.Scale (Screen.Viewport);
			}
		}

		public ScreenPoint OnlyX
		{
			get {
				return new ScreenPoint (Screen, () => new Vector2 (RelativeFunc ().X, 0));
			}
		}

		public ScreenPoint OnlyY
		{
			get {
				return new ScreenPoint (Screen, () => new Vector2 (0, RelativeFunc ().Y));
			}
		}

		public ScreenPoint Const
		{
			get {
				return new ScreenPoint (Screen, Relative.X, Relative.Y);
			}
		}

		public bool IsEmpty { get { return Relative.Length () == 0; } }

		#endregion

		#region Constructors

		public ScreenPoint (IGameScreen screen, Func<Vector2> func)
		{
			Screen = screen;
			RelativeFunc = func;
		}

		public ScreenPoint (IGameScreen screen, Vector2 vector)
		{
			Screen = screen;
			Relative = vector;
		}

		public ScreenPoint (IGameScreen screen, float x, float y)
		{
			Screen = screen;
			Relative = new Vector2 (x, y);
		}

		public ScreenPoint (IGameScreen screen, Func<float> x, Func<float> y)
		{
			Screen = screen;
			RelativeFunc = () => new Vector2 (x (), y ());
		}

		#endregion

		#region Methods and Operators

		public void Assign (ScreenPoint other)
		{
			Screen = other.Screen;
			RelativeFunc = other.RelativeFunc;
		}

		public static ScreenPoint FromAbsolute (float x, float y, IGameScreen screen)
		{
			return new ScreenPoint (screen, x / screen.Viewport.Height, x / screen.Viewport.Width);
		}

		public static ScreenPoint FromAbsolute (Point point, IGameScreen screen)
		{
			return FromAbsolute ((float)point.X, (float)point.Y, screen);
		}

		public static ScreenPoint Zero (IGameScreen screen)
		{
			return new ScreenPoint (screen, Vector2.Zero);
		}

		public static ScreenPoint TopLeft (IGameScreen screen)
		{
			return new ScreenPoint (screen, Vector2.Zero);
		}

		public static ScreenPoint BottomRight (IGameScreen screen)
		{
			return new ScreenPoint (screen, Vector2.One);
		}

		public static ScreenPoint Centered (IGameScreen screen, Bounds sizeOf)
		{
			return new ScreenPoint (screen, () => (ScreenPoint.BottomRight (screen) - sizeOf.Size) / 2);
		}

		public static implicit operator Vector2 (ScreenPoint point)
		{
			return point.Relative;
		}

		public static implicit operator Func<Vector2> (ScreenPoint point)
		{
			return point.RelativeFunc;
		}

		public static implicit operator Point (ScreenPoint point)
		{
			return point.Absolute;
		}

		public static implicit operator bool (ScreenPoint point)
		{
			return !point.IsEmpty;
		}

		[ExcludeFromCodeCoverageAttribute]
		public override string ToString ()
		{
			return "(" + Relative.X + "x" + Relative.Y + ")";
		}

		public static ScreenPoint operator * (ScreenPoint a, float b)
		{
			return new ScreenPoint (a.Screen, () => a.Relative * b);
		}

		public static ScreenPoint operator * (ScreenPoint a, ScreenPoint b)
		{
			return new ScreenPoint (a.Screen, () => new Vector2 (a.Relative.X * b.Relative.X, a.Relative.X * b.Relative.Y));
		}

		public static ScreenPoint operator / (ScreenPoint a, float b)
		{
			return new ScreenPoint (a.Screen, () => a.Relative / b);
		}

		public static ScreenPoint operator + (ScreenPoint a, ScreenPoint b)
		{
			return new ScreenPoint (a.Screen, () => a.Relative + b.Relative);
		}

		public static ScreenPoint operator - (ScreenPoint a, ScreenPoint b)
		{
			return new ScreenPoint (a.Screen, () => a.Relative - b.Relative);
		}

		public ScreenPoint ScaleX (float percent)
		{
			return new ScreenPoint (Screen, () => new Vector2 (Relative.X * percent, Relative.Y));
		}

		public ScreenPoint ScaleY (float percent)
		{
			return new ScreenPoint (Screen, () => new Vector2 (Relative.X, Relative.Y * percent));
		}

		public static bool operator == (ScreenPoint a, ScreenPoint b)
		{
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}
			return a.Equals (b);
		}

		public static bool operator != (ScreenPoint d1, ScreenPoint d2)
		{
			return !(d1 == d2);
		}

		public bool Equals (ScreenPoint other)
		{
			float epsilon = 0.000001f;

			return other != null && Math.Abs (Relative.X - other.Relative.X) < epsilon && Math.Abs (Relative.Y - other.Relative.Y) < epsilon;
		}

		public override bool Equals (object other)
		{
			if (other == null) {
				return false;
			}
			else if (other is Vector2) {
				return Relative.Equals ((Vector2)other);
			}
			else if (other is Point) {
				return Absolute.Equals ((Point)other);
			}
			else if ((other = other as string) != null) {
				return ToString ().Equals (other);
			}
			else {
				return false;
			}
		}

		[ExcludeFromCodeCoverageAttribute]
		public override int GetHashCode ()
		{
			return Relative.GetHashCode ();
		}

		#endregion
	}
}
