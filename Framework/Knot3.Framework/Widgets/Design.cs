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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Platform;

namespace Knot3.Framework.Widgets
{
    [ExcludeFromCodeCoverageAttribute]
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

        public static SpriteFont MenuFont (IScreen screen)
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
        public static Color DialogForeground;
        public static Color DialogBackground;
        public static Color TitleForegroundColor;
        public static Func<WidgetState, Color> WidgetBackgroundColorFunc;
        public static Func<WidgetState, Color> WidgetForegroundColorFunc;
        public static Func<WidgetState, Color> MenuItemBackgroundColorFunc;
        public static Func<WidgetState, Color> MenuItemForegroundColorFunc;
        public static Func<WidgetState, Color> ComboBoxItemBackgroundColorFunc;
        public static Func<WidgetState, Color> ComboBoxItemForegroundColorFunc;
        public static float NavigationItemHeight;
        public static float DataItemHeight;
    }
}
