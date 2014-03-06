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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Widgets;

using Knot3.Game.Screens;

namespace Knot3.Game.Widgets
{
    /// <summary>
    /// Pausiert ein Spieler im Creative- oder Challenge-Modus das Spiel,
    /// wird dieser Dialog über anderen Spielkomponenten angezeigt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ChallengePauseDialog : Dialog
    {
        /// <summary>
        /// Das Menü, das verschiedene Schaltflächen enthält.
        /// </summary>
        private Menu pauseMenu;

        /// <summary>
        ///
        /// </summary>
        public ChallengePauseDialog (IScreen screen, DisplayLayer drawOrder)
        : base (screen, drawOrder, "Pause")
        {
            // Der Titel-Text ist mittig ausgerichtet
            AlignX = HorizontalAlignment.Center;

            Bounds.Size = new ScreenPoint (screen, 0.3f, 0.31f);
            // Erstelle das Pause-Menü
            pauseMenu = new Menu (Screen, Index + DisplayLayer.Menu);
            pauseMenu.Bounds = ContentBounds;

            pauseMenu.ItemAlignX = HorizontalAlignment.Left;
            pauseMenu.ItemAlignY = VerticalAlignment.Center;

            MenuEntry settingsButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Settings",
            onClick: (time) => {
                Close (time);
                Screen.NextScreen = new SettingsScreen (Screen.Game);
            }
            );
            MenuEntry backButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Back to Game",
            onClick: (time) => {
                Close (time);
            }
            );

            backButton.AddKey (Keys.Escape);
            MenuEntry discardExitButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Abort Challenge",
            onClick: (time) => {
                Close (time);
                Screen.NextScreen = new StartScreen (Screen.Game);
            }
            );
            backButton.AddKey (Keys.Escape);

            pauseMenu.Add (settingsButton);
            pauseMenu.Add (backButton);
            pauseMenu.Add (discardExitButton);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            spriteBatch.Begin ();
            spriteBatch.DrawColoredRectangle (Design.WidgetBackground * 0.8f, Screen.Bounds);
            spriteBatch.End ();

            base.Draw (time);
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return pauseMenu;
        }
    }
}
