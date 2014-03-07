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
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Input;
using Knot3.Game.Widgets;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Der Spielzustand, der die Steuerungs-Einstellungen darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class ControlSettingsScreen : SettingsScreen
    {
        /// <summary>
        /// Das Menü, das die Einstellungen enthält.
        /// </summary>
        private Menu settingsMenu;
        public static Action ControlSettingsChanged = () => {};

        /// <summary>
        /// Erzeugt ein neues ControlSettingsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public ControlSettingsScreen (GameCore game)
        : base (game)
        {
            MenuName = "Controls";

            settingsMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            settingsMenu.Bounds.Position = new ScreenPoint (this, 0.400f, 0.180f);
            settingsMenu.Bounds.Size = new ScreenPoint (this, 0.500f, 0.620f);
            settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            settingsMenu.ItemAlignX = HorizontalAlignment.Left;
            settingsMenu.ItemAlignY = VerticalAlignment.Center;

            CheckBoxItem moveToCenter = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Move Rotation Targets to Center",
                option: new BooleanOption ("video", "arcball-around-center", true, Config.Default)
            );
            settingsMenu.Add (moveToCenter);

            // Make sure that the static initializers for the key binding listeners are called!!
            KeyBindingListener.InitializeListeners (typeof (InputManager), typeof (KnotInputHandler));

            // Lade die Standardbelegung
            Dictionary<PlayerAction, Keys> defaultReversed = KeyBindingListener.DefaultKeyAssignment.ReverseDictionary ();

            // Iteriere dazu über alle gültigen PlayerActions...
            foreach (PlayerAction action in PlayerAction.Values) {
                string actionName = action.Name;

                if (defaultReversed.ContainsKey (action)) {
                    // Erstelle das dazugehörige Options-Objekt...
                    KeyOption option = new KeyOption (
                        section: "controls",
                        name: actionName,
                        defaultValue: defaultReversed [action],
                        configFile: Config.Default
                    );

                    // Erstelle ein KeyInputItem zum Festlegen der Tastenbelegung
                    KeyInputItem item = new KeyInputItem (
                        screen: this,
                        drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                        text: actionName,
                        option: option
                    );
                    item.OnValueChanged += () => ControlSettingsChanged ();

                    // Füge es in das Menü ein
                    settingsMenu.Add (item);
                }
                else {
                    Log.Debug ("Key binding ", actionName, " not found!");
                }
            }
        }

        /// <summary>
        /// Fügt das Menü mit den Einstellungen in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, settingsMenu);
        }
    }
}
