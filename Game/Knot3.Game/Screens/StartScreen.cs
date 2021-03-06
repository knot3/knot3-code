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
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Der Startbildschirm.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class StartScreen : MenuScreen
    {
        /// <summary>
        /// Die Schaltflächen des Startbildschirms.
        /// </summary>
        private Container buttons;
        // das Logo
        private Texture2D logo;
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Erzeugt eine neue Instanz eines StartScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt.
        /// </summary>
        public StartScreen (GameCore game)
        : base (game)
        {
            // leere den Screen-Stack beim Öffnen dieses Screens
            ClearScreenHistory = true;

            // der Container für die Buttons
            buttons = new Container (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.Menu);

            // logo
            logo = this.LoadTexture (name: "logo");
            if (Config.Default ["debug", "projector-mode", false]) {
                logo = ContentLoader.InvertTexture (screen: this, texture: logo);
            }

            // create a new SpriteBatch, which can be used to draw textures
            spriteBatch = new SpriteBatch (GraphicsDevice);

            // menu
            buttons.ItemAlignX = HorizontalAlignment.Center;
            buttons.ItemAlignY = VerticalAlignment.Center;

            Button creativeButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Creative",
                onClick: (time) => NextScreen = new CreativeMainScreen (Game)
            );
            creativeButton.SetCoordinates (left: 0.700f, top: 0.250f, right: 0.960f, bottom: 0.380f);

            Button challengeButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Challenge",
                onClick: (time) => NextScreen = new ChallengeStartScreen (Game)
            );
            challengeButton.SetCoordinates (left: 0.000f, top: 0.050f, right: 0.380f, bottom: 0.190f);

            Button settingsButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Settings",
                onClick: (time) => NextScreen = new GeneralSettingsScreen (Game)
            );
            settingsButton.SetCoordinates (left: 0.260f, top: 0.840f, right: 0.480f, bottom: 0.950f);

            Button exitButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: String.Empty, // "Exit",
                onClick: (time) => Game.Exit ()
            );

            exitButton.AddKey (Keys.Escape);
            exitButton.SetCoordinates (left: 0.815f, top: 0.585f, right: 0.895f, bottom: 0.705f);
            exitButton.BackgroundTexture = this.LoadTexture ("exit300");
            if (Config.Default ["debug", "projector-mode", false]) {
                exitButton.BackgroundTexture = ContentLoader.InvertTexture (screen: this, texture: exitButton.BackgroundTexture);
            }

            buttons.Add (creativeButton);
            buttons.Add (challengeButton);
            buttons.Add (settingsButton);
            buttons.Add (exitButton);

            // Linien:

            lines.AddPoints (
                0.000f,
                0.050f,
                0.380f,
                0.250f,
                0.960f,
                0.380f,
                0.700f,
                0.160f,
                1.000f
            );

            lines.AddPoints (0.000f,
                             0.190f,
                             0.620f,
                             0.785f,
                             0.800f,
                             0.565f, // Exit oben.
                             0.910f, // Exit rechts.
                             0.730f, // Exit unten.
                             0.480f,
                             0.950f,
                             0.260f,
                             0.840f,
                             0.520f,
                             1.000f
                            );
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            // Versteckte Funktionen
            /*
            if (Keys.F1.IsDown ()) {
            	Button debugButton = new Button (
            	    screen: this,
            	    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
            	    name: "Junction Editor",
            	    onClick: (t) => NextScreen = new JunctionEditorScreen (Game)
            	);

            	debugButton.AlignX = HorizontalAlignment.Center;
            	debugButton.AlignY = VerticalAlignment.Center;

            	debugButton.AddKey (Keys.D);
            	debugButton.SetCoordinates (left: 0.800f, top: 0.030f, right: 0.950f, bottom: 0.100f);
            	AddGameComponents (time, debugButton);
            	Border border = new Border (this, DisplayLayer.ScreenUI, debugButton);
            	AddGameComponents (time, border);
            }
            */
        }

        /// <summary>
        /// Fügt die das Menü in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, buttons);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            // Zeichne das Logo
            spriteBatch.Begin ();
            spriteBatch.Draw (logo, new Bounds (this, 0.050f, 0.360f, 0.500f, 0.300f), Color.White);
            spriteBatch.End ();
        }
    }
}
