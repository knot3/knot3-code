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
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Data;
using Knot3.Game.Utilities;

namespace Knot3.Game.Screens
{
    /// <summary>
    ///
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class ChallengeCreateScreen : MenuScreen
    {
        /// <summary>
        /// Das Menü, das die Spielstände enthält, die als Startknoten ausgewählt werden.
        /// </summary>
        private Menu startKnotMenu;
        /// <summary>
        /// Das Menü, das die Spielstände enthält, die als Zielknoten ausgewählt werden.
        /// </summary>
        private Menu targetKnotMenu;
        private TextItem title;
        private InputItem challengeName;
        private Button createButton;
        private Border createButtonBorder;
        // Zurück-Button.
        private Button backButton;
        // Spielstand-Loader
        private SavegameLoader<Knot, KnotMetaData> loader;
        private Knot selectedStartKnot;
        private Knot selectedTargetKnot;
        private Challenge selectedChallenge;

        /// <summary>
        /// Erzeugt eine neue Instanz eines ChallengeCreateScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt game.
        /// </summary>
        public ChallengeCreateScreen (GameCore game)
        : base (game)
        {
            startKnotMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            startKnotMenu.Bounds = ScreenContentBounds.FromLeft (0.47f).FromTop (0.98f);
            startKnotMenu.ItemBackgroundColor = Design.ComboBoxItemBackgroundColorFunc;
            startKnotMenu.ItemForegroundColor = Design.ComboBoxItemForegroundColorFunc;
            startKnotMenu.RelativeItemHeight = Design.DataItemHeight;

            targetKnotMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            targetKnotMenu.Bounds = ScreenContentBounds.FromRight (0.47f).FromTop (0.98f);
            targetKnotMenu.ItemBackgroundColor = Design.ComboBoxItemBackgroundColorFunc;
            targetKnotMenu.ItemForegroundColor = Design.ComboBoxItemForegroundColorFunc;
            targetKnotMenu.RelativeItemHeight = Design.DataItemHeight;

            challengeName = new InputItem (this, DisplayLayer.ScreenUI + DisplayLayer.MenuItem, "Name:", String.Empty);
            challengeName.Bounds.Position = ScreenContentBounds.Position + ScreenContentBounds.Size.OnlyY + new ScreenPoint (this, 0f, 0.050f);
            challengeName.Bounds.Size = new ScreenPoint (this, 0.375f, 0.040f);
            challengeName.OnValueChanged += () => TryConstructChallenge ();
            challengeName.NameWidth = 0.2f;
            challengeName.ValueWidth = 0.8f;

            createButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Create!",
                onClick: OnCreateChallenge
            );
            createButton.Bounds.Position = ScreenContentBounds.Position + ScreenContentBounds.FromLeft (0.50f).Size + new ScreenPoint (this, 0f, 0.050f);
            createButton.Bounds.Size = new ScreenPoint (this, 0.125f, 0.050f);

            createButtonBorder = new Border (this, DisplayLayer.ScreenUI + DisplayLayer.MenuItem, createButton, 4, 4);
            createButton.AlignX = HorizontalAlignment.Center;

            startKnotMenu.Bounds.Padding = targetKnotMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            startKnotMenu.ItemAlignX = targetKnotMenu.ItemAlignX = HorizontalAlignment.Left;
            startKnotMenu.ItemAlignY = targetKnotMenu.ItemAlignY = VerticalAlignment.Center;

            lines.AddPoints (.000f, .050f, .030f, .970f, .760f, .895f, .880f, .970f, .970f, .050f, 1.000f);

            title = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Create Challenge");
            title.Bounds.Position = ScreenTitleBounds.Position;
            title.Bounds.Size = ScreenTitleBounds.Size;
            title.ForegroundColorFunc = (s) => Color.White;

            // Erstelle einen Parser für das Dateiformat
            KnotFileIO fileFormat = new KnotFileIO ();
            // Erstelle einen Spielstand-Loader
            loader = new SavegameLoader<Knot, KnotMetaData> (fileFormat, "index-knots");

            backButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Back",
                onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(s is ChallengeCreateScreen)).ElementAt (0)
            );
            backButton.AddKey (Keys.Escape);
            backButton.SetCoordinates (left: 0.770f, top: 0.910f, right: 0.870f, bottom: 0.960f);

            backButton.AlignX = HorizontalAlignment.Center;
        }

        private void UpdateFiles ()
        {
            // Leere das Spielstand-Menü
            startKnotMenu.Clear ();
            targetKnotMenu.Clear ();

            // Suche nach Spielständen
            loader.FindSavegames (AddSavegameToList);
        }

        /// <summary>
        /// Diese Methode wird für jede gefundene Spielstanddatei aufgerufen
        /// </summary>
        private void AddSavegameToList (string filename, KnotMetaData meta)
        {
            // Finde den Namen des Knotens
            string name = meta.Name.Length > 0 ? meta.Name : filename;

            // Erstelle die Lamdafunktionen, die beim Auswählen des Menüeintrags ausgeführt werden
            Action<GameTime> SelectStartKnot = (time) => {
                selectedStartKnot = loader.FileFormat.Load (filename);

                TryConstructChallenge ();
            };

            // Erstelle die Lamdafunktionen, die beim Auswählen des Menüeintrags ausgeführt werden
            Action<GameTime> SelectTargetKnot = (time) => {
                selectedTargetKnot = loader.FileFormat.Load (filename);

                TryConstructChallenge ();
            };

            // Erstelle die Menüeinträge
            MenuEntry buttonStart = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: name,
                onClick: SelectStartKnot
            );
            MenuEntry buttonTarget = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: name,
                onClick: SelectTargetKnot
            );
            buttonStart.IsSelectable = true;
            buttonTarget.IsSelectable = true;
            buttonStart.IsLocalized = false;
            buttonTarget.IsLocalized = false;

            startKnotMenu.Add (buttonStart);
            targetKnotMenu.Add (buttonTarget);
        }

        /// <summary>
        /// Prüft, ob alle Werte vorhanden sind, um eine Challenge daraus zu erstellen.
        /// Das ist dann der Fall, wenn zwei Knoten selektiert sind und ein Name eingegeben wurde.
        /// </summary>
        public bool CanCreateChallenge
        {
            get {
                return selectedStartKnot != null && selectedTargetKnot != null
                       && selectedStartKnot.MetaData.Filename != selectedTargetKnot.MetaData.Filename
                       && challengeName.InputText.Length > 0
                       && !selectedStartKnot.Equals (selectedTargetKnot);
            }
        }

        /// <summary>
        /// Versucht ein Challenge-Objekt zu erstellen.
        /// </summary>
        private bool TryConstructChallenge ()
        {
            bool can = createButton.IsEnabled = createButtonBorder.IsEnabled = CanCreateChallenge;

            if (can) {
                ChallengeMetaData challengeMeta = new ChallengeMetaData (
                    name: challengeName.InputText,
                    start: selectedStartKnot.MetaData,
                    target: selectedTargetKnot.MetaData,
                    filename: null,
                    format: new ChallengeFileIO (),
                    highscore: new List<KeyValuePair<string,int>> ()
                );
                selectedChallenge = new Challenge (
                    meta: challengeMeta,
                    start: selectedStartKnot,
                    target: selectedTargetKnot
                );
            }
            else {
                selectedChallenge = null;
            }

            return can;
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf den Button zum Erstellen der Challenge gedrückt wird
        /// </summary>
        private void OnCreateChallenge (GameTime time)
        {
            if (TryConstructChallenge ()) {
                Log.Debug ("Save Challenge: ", selectedChallenge);
                try {
                    selectedChallenge.Save (false);
                    NextScreen = new ChallengeStartScreen (Game);
                }
                catch (FileAlreadyExistsException) {
                    ConfirmDialog confirm = new ConfirmDialog (
                        screen: this,
                        drawOrder: DisplayLayer.Dialog,
                        title: "Warning",
                        text: "Do you want to overwrite the existing challenge?"
                    );
                    confirm.Submit += (r) => {
                        selectedChallenge.Save (true);
                        NextScreen = new ChallengeStartScreen (Game);
                    };
                    AddGameComponents (null, confirm);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            UpdateFiles ();
            base.Entered (previousScreen, time);
            AddGameComponents (time, startKnotMenu, targetKnotMenu, challengeName, createButton,
                               createButtonBorder, title, backButton);
            TryConstructChallenge ();
        }
    }
}
