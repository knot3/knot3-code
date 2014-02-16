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

using Microsoft.Xna.Framework;

using Knot3.Widgets;

#endregion

namespace Knot3.Widgets
{
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

		private static Color WidgetBackgroundColor (State state)
		{
			if (state == State.None || state == State.Hovered) {
				return Color.Transparent;
			}
			else if (state == State.Selected) {
				return Color.Black;
			}
			else {
				return Color.CornflowerBlue;
			}
		}

		private static Color WidgetForegroundColor (State state)
		{
			if (state == State.Hovered) {
				return Color.White;
			}
			else if (state == State.None) {
				return Color.White * 0.7f;
			}
			else if (state == State.Selected) {
				return Color.White;
			}
			else {
				return Color.CornflowerBlue;
			}
		}

		private Color MenuItemBackgroundColor (State state)
		{
			return Color.Transparent;
		}

		private Color MenuItemForegroundColor (State state)
		{
			if (state == State.Hovered) {
				return Color.White;
			}
			else {
				return Color.White * 0.7f;
			}
		}
	}
}
