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
    /// Der Spielzustand, der den Ladebildschirm für Knoten darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class CreativeLoadScreen : MenuScreen
    {
        /// <summary>
        /// Das Menü, das die Spielstände enthält.
        /// </summary>
        private Menu savegameMenu;
        // Der Titel des Screens
        private TextItem title;
        // Spielstand-Loader
        private SavegameLoader<Knot, KnotMetaData> loader;
        // Zurück-Button
        private Button backButton;
        private Button startButton;
        // Preview
        private TextItem infoTitle;
        private World previewWorld;
        private KnotRenderer previewRenderer;
        private KnotMetaData previewKnotMetaData;
        private Border previewBorder;
        private KnotInputHandler previewInput;
        private ModelMouseHandler previewMouseHandler;
        private Menu knotInfo;

        /// <summary>
        /// Erzeugt ein neues CreativeLoadScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public CreativeLoadScreen (GameCore game)
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

            title = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Load Knot");
            title.Bounds.Position = ScreenTitleBounds.Position;
            title.Bounds.Size = ScreenTitleBounds.Size;
            title.ForegroundColorFunc = (s) => Color.White;

            infoTitle = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Knot Info:");
            infoTitle.Bounds.Position = new ScreenPoint (this, 0.45f, 0.62f);
            infoTitle.Bounds.Size = new ScreenPoint (this, 0.900f, 0.050f);
            infoTitle.ForegroundColorFunc = (s) => Color.White;

            knotInfo = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            knotInfo.Bounds.Position = new ScreenPoint (this, 0.47f, 0.70f);
            knotInfo.Bounds.Size = new ScreenPoint (this, 0.300f, 0.500f);
            knotInfo.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            knotInfo.ItemAlignX = HorizontalAlignment.Left;
            knotInfo.ItemAlignY = VerticalAlignment.Center;

            // Erstelle einen Parser für das Dateiformat
            KnotFileIO fileFormat = new KnotFileIO ();
            // Erstelle einen Spielstand-Loader
            loader = new SavegameLoader<Knot, KnotMetaData> (fileFormat, "index-knots");

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
                onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(s is CreativeLoadScreen)).ElementAt (0)
            );
            backButton.AddKey (Keys.Escape);
            backButton.SetCoordinates (left: 0.770f, top: 0.910f, right: 0.870f, bottom: 0.960f);
            backButton.AlignX = HorizontalAlignment.Center;

            startButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Load",
                onClick: (time) => NextScreen = new CreativeModeScreen (game: Game, knot: loader.FileFormat.Load (previewKnotMetaData.Filename))
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
        private void AddSavegameToList (string filename, KnotMetaData meta)
        {
            // Finde den Namen des Knotens
            string name = meta.Name.Length > 0 ? meta.Name : filename;

            // Erstelle eine Lamdafunktion, die beim Auswählen des Menüeintrags ausgeführt wird
            Action<GameTime> preview = (time) => {
                if (previewKnotMetaData != meta) {
                    RemoveGameComponents (time, knotInfo);
                    knotInfo.Clear ();

                    previewRenderer.RenderKnot (loader.FileFormat.Load (filename));
                    previewWorld.Camera.ResetCamera ();
                    previewKnotMetaData = meta;
                    startButton.IsVisible = true;

                    MenuEntry countEntry = new MenuEntry (
                        screen: this,
                        drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                        name: Localizer.Localize ("Edge Count: ") + previewKnotMetaData.CountEdges,
                    onClick: (t) => {}
                    );

                    countEntry.Enabled = false;
                    knotInfo.Add (countEntry);

                    if (filename.Contains (SystemInfo.SavegameDirectory)) {
                        MenuEntry deleteEntry = new MenuEntry (
                            screen: this,
                            drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                            name: "Delete",
                            onClick: (t) => deleteSavegame (filename, t)
                        );
                        deleteEntry.AddKey (Keys.Delete);

                        knotInfo.Add (deleteEntry);
                    }

                    AddGameComponents (time, knotInfo);
                }
            };

            // Erstelle den Menüeintrag
            MenuEntry button = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: name,
                onClick: preview
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
            AddGameComponents (time, title, previewBorder, previewWorld, previewInput, previewMouseHandler, backButton, startButton, infoTitle);
        }
    }
}
