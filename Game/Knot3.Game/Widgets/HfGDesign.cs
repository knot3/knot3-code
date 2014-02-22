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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Widgets
{
	[ExcludeFromCodeCoverageAttribute]
	public sealed class HfGDesign : IDesign
	{
		public HfGDesign ()
		{
		}

		public void Apply ()
		{
			Design.MenuFontName = "font-menu";
			Design.DefaultLineColor = new Color (0xb4, 0xff, 0x00);
			Design.DefaultOutlineColor = new Color (0x3b, 0x54, 0x00);
			Design.WidgetBackground = Color.Black;
			Design.WidgetForeground = Color.White;
			Design.InGameBackground = Color.Black;
			Design.ScreenBackground = Color.Black;
			Design.WidgetBackgroundColorFunc = WidgetBackgroundColor;
			Design.WidgetForegroundColorFunc = WidgetForegroundColor;
			Design.MenuItemBackgroundColorFunc = MenuItemBackgroundColor;
			Design.MenuItemForegroundColorFunc = MenuItemForegroundColor;
		}

		private static Color WidgetBackgroundColor (WidgetState state)
		{
			if (state == WidgetState.None || state == WidgetState.Hovered) {
				return Color.Transparent;
			}
			else if (state == WidgetState.Selected) {
				return Color.Black;
			}
			else {
				return Color.CornflowerBlue;
			}
		}

		private static Color WidgetForegroundColor (WidgetState state)
		{
			if (state == WidgetState.Hovered) {
				return Color.White;
			}
			else if (state == WidgetState.None) {
				return Color.White * 0.7f;
			}
			else if (state == WidgetState.Selected) {
				return Color.White;
			}
			else {
				return Color.CornflowerBlue;
			}
		}

		private Color MenuItemBackgroundColor (WidgetState state)
		{
			return Color.Transparent;
		}

		private Color MenuItemForegroundColor (WidgetState state)
		{
			if (state == WidgetState.Hovered) {
				return Color.White;
			}
			else {
				return Color.White * 0.7f;
			}
		}
	}
}