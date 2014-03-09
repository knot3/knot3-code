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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Widgets;

using Knot3.Game.Data;
using Knot3.Game.Screens;

namespace Knot3.Game.Widgets
{
    /// <summary>
    ///Dieser Dialog Zeigt den Highscore f�r die Gegebene Challenge an und bietet die Option
    ///zum Neustarten oder R�ckkehr zum Hauptmen�
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class HighscoreDialog : Dialog
    {
        private Menu highscoreList;
        private Container buttons;

        /// <summary>
        ///
        /// </summary>
        public HighscoreDialog (IScreen screen, DisplayLayer drawOrder, Challenge challenge)
        : base (screen, drawOrder, "Highscores")
        {
            // Der Titel-Text ist mittig ausgerichtet
            AlignX = HorizontalAlignment.Center;
            highscoreList = new Menu (Screen, Index + DisplayLayer.Menu);
            highscoreList.Bounds = ContentBounds;
            highscoreList.ItemAlignX = HorizontalAlignment.Left;
            highscoreList.ItemAlignY = VerticalAlignment.Center;

            if (challenge.Highscore != null) {
                //sotiert die Highscoreliste wird nach der Zeit sotiert
                int highscoreCounter = 0;
                foreach (KeyValuePair<string, int> entry in challenge.Highscore.OrderBy (key => key.Value)) {
                    TextItem firstScore = new TextItem (screen, drawOrder, entry.Value + " " + entry.Key);
                    highscoreList.Add (firstScore);
                    highscoreCounter++;
                    if (highscoreCounter > 8) {
                        break;
                    }
                }
            }

            buttons = new Container (screen, Index + DisplayLayer.Menu);
            buttons.ItemAlignX = HorizontalAlignment.Center;

            // Button zum Neustarten der Challenge
            Action<GameTime> restartAction = (time) => {
                Close (time);
                Screen.NextScreen = new ChallengeModeScreen (Screen.Game, challenge);
            };
            MenuEntry restartButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Restart challenge",
                onClick: restartAction
            );
            restartButton.Bounds.Size = new ScreenPoint (screen, ContentBounds.Size.Relative.X / 2, 0.05f);
            restartButton.Bounds.Position = ContentBounds.Position + ContentBounds.Size.OnlyY
                                            - restartButton.Bounds.Size.OnlyY;
            buttons.Add (restartButton);

            // Button für die Rückkehr zum StartScreen
            Action<GameTime> returnAction = (time) => {
                Close (time);
                Screen.NextScreen = new StartScreen (Screen.Game);
            };
            MenuEntry returnButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Return to menu",
                onClick: returnAction
            );
            returnButton.Bounds.Size = new ScreenPoint (screen, ContentBounds.Size.Relative.X / 2, 0.05f);
            returnButton.Bounds.Position = ContentBounds.Position + ContentBounds.Size.OnlyY
                                           - returnButton.Bounds.Size.OnlyY + ContentBounds.Size.OnlyX / 2;
            buttons.Add (returnButton);
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return highscoreList;
            yield return buttons;
        }
    }
}
