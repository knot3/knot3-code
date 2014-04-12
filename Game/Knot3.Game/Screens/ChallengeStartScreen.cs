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
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Data;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.Utilities;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Der Spielzustand, der den Ladebildschirm für Challenges darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ChallengeStartScreen : MenuScreen
    {
        /// <summary>
        /// Das Menü, das die Spielstände enthält.
        /// </summary>
        private Menu savegameMenu;
        private Button backButton;
        // Der Titel des Screens
        private TextItem title;
        // Spielstand-Loader
        private SavegameLoader<Challenge, ChallengeMetaData> loader;
        // Preview
        private TextItem infoTitle;
        private Menu challengeInfo;
        private World previewWorld;
        private Challenge previewChallenge;
        private KnotRenderer previewRenderer;
        private Border previewBorder;
        private KnotInputHandler previewInput;
        private ModelMouseHandler previewMouseHandler;
        private Button startButton;

        /// <summary>
        /// Erstellt eine neue Instanz eines ChallengeStartScreen-Objekts und
        /// initialisiert diese mit einem Knot3Game-Objekt.
        /// </summary>
        public ChallengeStartScreen (GameCore game)
        : base (game)
        {
            savegameMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            savegameMenu.Bounds.Position = ScreenContentBounds.Position;
            savegameMenu.Bounds.Size = new ScreenPoint (this, 0.300f, ScreenContentBounds.Size.Relative.Y);
            savegameMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            savegameMenu.ItemAlignX = HorizontalAlignment.Left;
            savegameMenu.ItemAlignY = VerticalAlignment.Center;
            savegameMenu.ItemBackgroundColor = Design.ComboBoxItemBackgroundColorFunc;
            savegameMenu.ItemForegroundColor = Design.ComboBoxItemForegroundColorFunc;
            savegameMenu.RelativeItemHeight = Design.DataItemHeight;

            lines.AddPoints (.000f, .050f, .030f, .970f, .620f, .895f, .740f, .970f, .760f, .895f, .880f, .970f, .970f, .050f, 1.000f);

            title = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Start Challenge");
            title.Bounds.Position = ScreenTitleBounds.Position;
            title.Bounds.Size = ScreenTitleBounds.Size;
            title.ForegroundColorFunc = (s) => Color.White;

            infoTitle = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Challenge Info:");
            infoTitle.Bounds.Position = new ScreenPoint (this, 0.45f, 0.62f);
            infoTitle.Bounds.Size = new ScreenPoint (this, 0.900f, 0.050f);
            infoTitle.ForegroundColorFunc = (s) => Color.White;

            challengeInfo = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            challengeInfo.Bounds.Position = new ScreenPoint (this, 0.47f, 0.70f);
            challengeInfo.Bounds.Size = new ScreenPoint (this, 0.300f, 0.500f);
            challengeInfo.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            challengeInfo.ItemAlignX = HorizontalAlignment.Left;
            challengeInfo.ItemAlignY = VerticalAlignment.Center;

            // Erstelle einen Parser für das Dateiformat
            ChallengeFileIO fileFormat = new ChallengeFileIO ();
            // Erstelle einen Spielstand-Loader
            loader = new SavegameLoader<Challenge, ChallengeMetaData> (fileFormat, "index-challenges");

            // Preview
            Bounds previewBounds = new Bounds (this, 0.45f, 0.1f, 0.48f, 0.5f);
            previewWorld = new World (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.GameWorld,
                bounds: previewBounds
            );
            previewRenderer = new KnotRenderer (screen: this, position: Vector3.Zero);
            previewWorld.Add (previewRenderer);
            previewBorder = new Border (
                screen: this,
                drawOrder: DisplayLayer.GameWorld,
                bounds: previewBounds,
                lineWidth: 2,
                padding: 0
            );
            previewInput = new KnotInputHandler (screen: this, world: previewWorld);
            previewMouseHandler = new ModelMouseHandler (screen: this, world: previewWorld);

            backButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Back",
                onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(s is ChallengeStartScreen)).ElementAt (0)
            );
            backButton.AddKey (Keys.Escape);

            backButton.SetCoordinates (left: 0.770f, top: 0.910f, right: 0.870f, bottom: 0.960f);
            backButton.AlignX = HorizontalAlignment.Center;
            Action<GameTime> loadSelectedChallenge = (time) => {
                NextScreen = new ChallengeModeScreen (
                    game: Game,
                    challenge: loader.FileFormat.Load (previewChallenge.MetaData.Filename)
                );
            };
            startButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Start",
                onClick: loadSelectedChallenge
            );
            startButton.IsVisible = false;
            startButton.AddKey (Keys.Enter);
            startButton.SetCoordinates (left: 0.630f, top: 0.910f, right: 0.730f, bottom: 0.960f);

            startButton.AlignX = HorizontalAlignment.Center;
        }

        private void UpdateFiles (GameTime time)
        {
            RemoveGameComponents (time, savegameMenu);
            // Leere das Spielstand-Menü
            savegameMenu.Clear ();
            // Suche nach Spielständen
            loader.FindSavegames (AddSavegameToList);
            AddGameComponents (time, savegameMenu);
        }

        /// <summary>
        /// Diese Methode wird für jede gefundene Spielstanddatei aufgerufen
        /// </summary>
        private void AddSavegameToList (string filename, ChallengeMetaData meta)
        {
            // Erstelle eine Lamdafunktion, die beim Auswählen des Menüeintrags ausgeführt wird
            Action<GameTime> LoadFile = (time) => {
                if (previewChallenge == null || previewChallenge.MetaData != meta) {
                    RemoveGameComponents (time, challengeInfo);
                    challengeInfo.Clear ();

                    previewChallenge = loader.FileFormat.Load (filename);
                    previewRenderer.RenderKnot (newKnot: previewChallenge.Target, isFinalDestination: true);
                    previewWorld.Camera.ResetCamera ();
                    startButton.IsVisible = true;

                    MenuEntry countEntry = new MenuEntry (
                        screen: this,
                        drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                        name: Localizer.Localize ("Edge Count: ") + meta.Target.CountEdges,
                    onClick: (t) => {}
                    );
                    countEntry.Enabled = false;
                    challengeInfo.Add (countEntry);

                    MenuEntry avgtimeEntry = new MenuEntry (
                        screen: this,
                        drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                        name: ("Avg Time: ").Localize () + (meta.FormatedAvgTime).Localize (),
                    onClick: (t) => {}
                    );
                    avgtimeEntry.IsLocalized = false;
                    avgtimeEntry.Enabled = false;
                    challengeInfo.Add (avgtimeEntry);

                    if (filename.Contains (SystemInfo.SavegameDirectory)) {
                        MenuEntry deleteEntry = new MenuEntry (
                            screen: this,
                            drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                            name: "Delete",
                            onClick: (t) => deleteSavegame (filename, t)
                        );
                        deleteEntry.AddKey (Keys.Delete);
                        challengeInfo.Add (deleteEntry);
                    }

                    AddGameComponents (time, challengeInfo);
                }
            };

            // Finde den Namen der Challenge
            string name = meta.Name.Length > 0 ? meta.Name : filename;

            // Erstelle den Menüeintrag
            MenuEntry button = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: name,
                onClick: LoadFile
            );
            button.IsSelectable = true;
            button.IsLocalized = false;
            savegameMenu.Add (button);
        }

        private void deleteSavegame (string filename, GameTime time)
        {
            File.Delete (filename);
            UpdateFiles (time);
        }

        /// <summary>
        /// Fügt das Menü mit den Spielständen in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            UpdateFiles (time);
            base.Entered (previousScreen, time);
            AddGameComponents (time, title, previewWorld, previewBorder, previewInput, previewMouseHandler, backButton, startButton, infoTitle);
        }
    }
}
