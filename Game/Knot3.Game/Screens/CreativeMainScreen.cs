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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Data;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// In diesem Menü trifft der Spieler die Wahl, ob er im Creative-Modus einen neuen Knoten erstellen, einen Knoten laden oder
    /// eine neue Challenge erstellen möchte.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class CreativeMainScreen : MenuScreen
    {
        /// <summary>
        /// Ein Menü aus Schaltflächen, welche den Spielwunsch des Spielers weiterleiten.
        /// </summary>
        private Container buttons;

        /// <summary>
        /// Erzeugt ein neues CreativeMainScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public CreativeMainScreen (GameCore game)
        : base (game)
        {
            buttons = new Container (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            buttons.ItemAlignX = HorizontalAlignment.Center;
            buttons.ItemAlignY = VerticalAlignment.Center;

            Button newKnotButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "NEW\nKnot",
                onClick: (time) => NextScreen = new CreativeModeScreen (Game, new Knot ())
            );
            newKnotButton.SetCoordinates (left: 0.100f, top: 0.150f, right: 0.300f, bottom: 0.350f);

            Button loadKnotButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "LOAD\nKnot",
                onClick: (time) => NextScreen = new CreativeLoadScreen (Game)
            );
            loadKnotButton.SetCoordinates (left: 0.675f, top: 0.300f, right: 0.875f, bottom: 0.475f);

            Button newChallengeButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "NEW\nChallenge",
                onClick: (time) => NextScreen = new ChallengeCreateScreen (Game)
            );
            newChallengeButton.SetCoordinates (left: 0.250f, top: 0.525f, right: 0.600f, bottom: 0.750f);

            Button backButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Back",
                onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(

                                                    s is CreativeMainScreen
                                                    || s is CreativeLoadScreen
                                                    || s is ChallengeCreateScreen)).ElementAt (0)
            );
            backButton.AddKey (Keys.Escape);
            backButton.SetCoordinates (left: 0.825f, top: 0.850f, right: 0.975f, bottom: 0.950f);

            buttons.Add (newKnotButton);
            buttons.Add (loadKnotButton);
            buttons.Add (newChallengeButton);
            buttons.Add (backButton);
            buttons.IsVisible = true;

            // die Linien
            lines.AddPoints (0.000f, 0.150f,
                             0.300f, 0.350f, 0.100f, 0.070f, 0.600f, 0.750f, 0.250f,
                             0.525f, 0.875f, 0.300f, 0.675f, 0.475f, 0.950f, 0.000f
                            );

            lines.AddPoints (0.975f, 0.850f, 0.825f, 0.950f, 0.975f, 0.850f);
        }

        /// <summary>
        ///
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, buttons);
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
        }
    }
}
