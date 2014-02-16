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

using Knot3.Core;
using Knot3.Data;
using Knot3.GameObjects;
using Knot3.Input;
using Knot3.RenderEffects;
using Knot3.Utilities;
using Knot3.Widgets;

#endregion

namespace Knot3.Widgets
{
	public static class Design
	{
		private static string menuFontName;

		public static string MenuFontName
		{
			get {
				return menuFontName;
			}
			set {
				menuFontName = value;
				menuFont = null;
			}
		}

		private static SpriteFont menuFont;

		public static SpriteFont MenuFont (IGameScreen screen)
		{
			if (menuFont != null) {
				return menuFont;
			}
			else {
				// lade die Schriftart der Menüs in das private Attribut
				menuFont = screen.LoadFont ("font-menu");
				return menuFont;
			}
		}

		// die Standardfarben der Linien
		public static Color DefaultLineColor;
		public static Color DefaultOutlineColor;
		public static Color InGameBackground;
		public static Color WidgetBackground;
		public static Color WidgetForeground;
		public static Color ScreenBackground;
		public static Func<State, Color> WidgetBackgroundColorFunc;
		public static Func<State, Color> WidgetForegroundColorFunc;
		public static Func<State, Color> MenuItemBackgroundColorFunc;
		public static Func<State, Color> MenuItemForegroundColorFunc;
	}
}
