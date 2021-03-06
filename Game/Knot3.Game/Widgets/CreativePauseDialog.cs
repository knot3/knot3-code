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
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Storage;
using Knot3.Framework.Widgets;

using Knot3.Game.Data;
using Knot3.Game.Screens;

namespace Knot3.Game.Widgets
{
    /// <summary>
    /// Pausiert ein Spieler im Creative- oder Challenge-Modus das Spiel,
    /// wird dieser Dialog über anderen Spielkomponenten angezeigt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class CreativePauseDialog : Dialog
    {
        /// <summary>
        /// Das Menü, das verschiedene Schaltflächen enthält.
        /// </summary>
        private Menu pauseMenu;
        private Knot knot;

        /// <summary>
        ///
        /// </summary>
        public CreativePauseDialog (IScreen screen, DisplayLayer drawOrder, Knot knot)
        : base (screen, drawOrder, "Pause")
        {
            this.knot = knot;

            // Der Titel-Text ist mittig ausgerichtet
            AlignX = HorizontalAlignment.Center;
            Bounds.Size = new ScreenPoint (screen, 0.45f, 0.31f);
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
                Screen.NextScreen = new GeneralSettingsScreen (Screen.Game);
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
            MenuEntry saveButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Save",
            onClick: (time) => {
                Close (time);
                KnotSave (time);
            }
            );
            MenuEntry saveAsButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Save As",
            onClick: (time) => {
                Close (time);
                KnotSaveAs (() => {}, time);
            }
            );
            MenuEntry saveExitButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Save and Exit",
            onClick: (time) => {
                Close (time);
                KnotSaveExit (time);
            }
            );
            MenuEntry discardExitButton = new MenuEntry (
                screen: Screen,
                drawOrder: Index + DisplayLayer.MenuItem,
                name: "Discard Changes and Exit",
            onClick: (time) => {
                Close (time);
                Screen.NextScreen = new StartScreen (Screen.Game);
            }
            );
            backButton.AddKey (Keys.Escape);

            pauseMenu.Add (settingsButton);
            pauseMenu.Add (backButton);
            pauseMenu.Add (saveButton);
            pauseMenu.Add (saveAsButton);
            pauseMenu.Add (saveExitButton);
            pauseMenu.Add (discardExitButton);
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return pauseMenu;
        }

        private void KnotSave (GameTime time)
        {
            try {
                knot.Save (true);
            }
            catch (NoFilenameException) {
                KnotSaveAs (() => {}, time);
            }
        }

        private void KnotSaveAs (Action onClose, GameTime time)
        {
            TextInputDialog saveDialog = new TextInputDialog (
                screen: Screen,
                drawOrder: DisplayLayer.Dialog,
                title: "Save Knot",
                text: "Name:",
                inputText: knot.Name != null ? knot.Name : String.Empty
            );
            saveDialog.NoCloseEmpty = true;
            saveDialog.NoWhiteSpace = true;
            saveDialog.Text = "Press Enter to save the Knot.";
            Screen.AddGameComponents (null, saveDialog);
            saveDialog.Submit += (t) => {
                try {
                    knot.Name = saveDialog.InputText;
                    knot.Save (false);
                    onClose ();
                }
                catch (FileAlreadyExistsException) {
                    ConfirmDialog confirm = new ConfirmDialog (
                        screen: Screen,
                        drawOrder: DisplayLayer.Dialog,
                        title: "Warning",
                        text: "Do you want to overwrite the existing knot?"
                    );
                    confirm.Cancel += (l) => {
                        KnotSaveAs (() => onClose (), time);
                    };
                    confirm.Submit += (r) => {
                        knot.Save (true);
                        onClose ();
                    };
                    Screen.AddGameComponents (null, confirm);
                }
            };
        }

        private void KnotSaveExit (GameTime time)
        {
            try {
                knot.Save (true);
                Screen.NextScreen = new StartScreen (Screen.Game);
            }
            catch (NoFilenameException) {
                KnotSaveAs (() => Screen.NextScreen = new StartScreen (Screen.Game), time);
            }
        }
    }
}
