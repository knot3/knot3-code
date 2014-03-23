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
    public sealed class ProjectorDesign : IDesign
    {
        public ProjectorDesign ()
        {
        }

        public void Apply ()
        {
            Design.MenuFontName = "font-menu";
            Design.DefaultLineColor = Color.DarkGreen;
            Design.DefaultOutlineColor = Color.DarkGreen.Mix (Color.White, 0.10f);
            Design.WidgetBackground = Color.GhostWhite;
            Design.WidgetForeground = Color.Black;
            Design.InGameBackground = Color.White;
            Design.ScreenBackground = Color.White;
            Design.DialogBackground = Color.Black.Mix (Color.White, 0.95f);
            Design.DialogForeground = Color.Black;
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
            if (state == WidgetState.None) {
                return Color.Transparent;
            }
            else if (state == WidgetState.Hovered) {
                return Color.WhiteSmoke;
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
                return Color.DarkGreen.Mix (Color.Black, 0.50f);
            }
            else if (state == WidgetState.None) {
                return Color.Black;
            }
            else if (state == WidgetState.Selected) {
                return Color.Black;
            }
            else {
                return Color.CornflowerBlue;
            }
        }

        private Color MenuItemBackgroundColor (WidgetState state)
        {
            if (state == WidgetState.Selected) {
                return Color.Black;
            }
            return Color.Transparent;
        }

        private Color MenuItemForegroundColor (WidgetState state)
        {
            if (state == WidgetState.Hovered) {
                return Color.DarkGreen.Mix (Color.Black, 0.50f);
            }
            else if (state == WidgetState.Selected) {
                return Color.White;
            }
            else {
                return Color.Black;
            }
        }

        private Color ComboBoxItemBackgroundColor (WidgetState state)
        {
            if (state == WidgetState.Selected) {
                return Color.DarkGreen;
            }
            if (state == WidgetState.Hovered) {
                return Color.White.Mix (Color.DarkGreen, 0.25f);
            }
            return Color.Transparent;
        }

        private Color ComboBoxItemForegroundColor (WidgetState state)
        {
            if (state == WidgetState.Hovered) {
                return Color.Black;
            }
            else if (state == WidgetState.Selected) {
                return Color.White;
            }
            else {
                return Color.Black;
            }
        }
    }
}
