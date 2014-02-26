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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Development;
using Knot3.Game.Effects;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Ein Spielzustand, der das Haupt-Einstellungsmenü zeichnet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class SettingsScreen : MenuScreen
    {
        /// <summary>
        /// Der Name des Menüs.
        /// </summary>
        protected string MenuName;

        /// <summary>
        /// Der Spritebatch.
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Das Menu, in dem man die Einstellungs-Kategorie auswählen kann.
        /// </summary>
        private Menu navigationMenu;

        /// <summary>
        /// Zurück-Button.
        /// </summary>
        private MenuEntry backButton;

        public SettingsScreen (GameClass game)
        : base (game)
        {
            MenuName = "Settings";

            spriteBatch = new SpriteBatch (Device);

            navigationMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            navigationMenu.Bounds.Position = new ScreenPoint (this, 0.100f, 0.180f);
            navigationMenu.Bounds.Size = new ScreenPoint (this, 0.300f, 0.770f);
            navigationMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            navigationMenu.ItemAlignX = HorizontalAlignment.Left;
            navigationMenu.ItemAlignY = VerticalAlignment.Center;

            MenuEntry debugButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Debug",
                onClick: (time) => NextScreen = new DebugSettingsScreen (Game)
            );
            MenuEntry graphicsButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Video",
                onClick: (time) => NextScreen = new GraphicsSettingsScreen (Game)
            );
            MenuEntry audioButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Audio",
                onClick: (time) => NextScreen = new AudioSettingsScreen (Game)
            );
            MenuEntry controlsButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Controls",
                onClick: (time) => NextScreen = new ControlSettingsScreen (Game)
            );
            MenuEntry profileButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Profile",
                onClick: (time) => NextScreen = new ProfileSettingsScreen (Game)
            );

            navigationMenu.Add (debugButton);
            navigationMenu.Add (graphicsButton);
            navigationMenu.Add (audioButton);
            navigationMenu.Add (controlsButton);
            navigationMenu.Add (profileButton);

            lines.AddPoints (0.000f, 0.050f,
                             0.030f, 0.970f,
                             0.770f, 0.895f,
                             0.870f, 0.970f,
                             0.970f, 0.050f,
                             1.000f
                            );

            backButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Back",
                onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(s is SettingsScreen)).ElementAt (0)
            );
            backButton.AddKey (Keys.Escape);
            backButton.SetCoordinates (left: 0.770f, top: 0.910f, right: 0.870f, bottom: 0.960f);
            backButton.AlignX = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Fügt das Haupt-Einstellungsmenü in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IGameScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, navigationMenu, backButton);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            spriteBatch.Begin ();
            // text
            spriteBatch.DrawStringInRectangle (
                font: Design.MenuFont (this),
                text: MenuName.Localize (),
                color: Color.White,
                bounds: new Bounds (this, 0.050f, 0.050f, 0.900f, 0.050f),
                alignX: HorizontalAlignment.Left,
                alignY: VerticalAlignment.Center
            );
            spriteBatch.End ();
        }
    }
}
