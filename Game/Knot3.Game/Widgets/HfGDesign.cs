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

using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

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
            Design.DialogBackground = Color.Black.Mix (Color.White, 0.05f);
            Design.DialogForeground = Color.White;
            Design.WidgetBackgroundColorFunc = WidgetBackgroundColor;
            Design.WidgetForegroundColorFunc = WidgetForegroundColor;
            Design.MenuItemBackgroundColorFunc = MenuItemBackgroundColor;
            Design.MenuItemForegroundColorFunc = MenuItemForegroundColor;
            Design.ComboBoxItemBackgroundColorFunc = ComboBoxItemBackgroundColor;
            Design.ComboBoxItemForegroundColorFunc = ComboBoxItemForegroundColor;
            Design.NavigationItemHeight = 0.040f;
            Design.DataItemHeight = 0.033f;
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
            if (state == WidgetState.Selected) {
                return Color.White;
            }
            return Color.Transparent;
        }

        private Color MenuItemForegroundColor (WidgetState state)
        {
            if (state == WidgetState.Hovered) {
                return Color.White;
            }
            else if (state == WidgetState.Selected) {
                return Color.Black;
            }
            else {
                return Color.White * 0.7f;
            }
        }

        private Color ComboBoxItemBackgroundColor (WidgetState state)
        {
            if (state == WidgetState.Selected) {
                return Color.White;
            }
            if (state == WidgetState.Hovered) {
                return Color.Black.Mix (Color.White, 0.15f);
            }
            return Color.Transparent;
        }

        private Color ComboBoxItemForegroundColor (WidgetState state)
        {
            if (state == WidgetState.Hovered) {
                return Color.White;
            }
            else if (state == WidgetState.Selected) {
                return Color.Black;
            }
            else {
                return Color.White * 0.7f;
            }
        }
    }
}
