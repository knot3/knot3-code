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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Audio;
using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Effects;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

namespace Knot3.ModelEditor
{
    [ExcludeFromCodeCoverageAttribute]
    public class JunctionEditorScreen : Screen
    {
        /// <summary>
        /// Die Spielwelt in der die 3D-Objekte des dargestellten Knotens enthalten sind.
        /// </summary>
        private World world;

        /// <summary>
        /// Der Controller, der aus dem Knoten die 3D-Modelle erstellt.
        /// </summary>
        private JunctionEditorRenderer knotRenderer;
        private KnotInputHandler knotInput;
        private ModelMouseHandler modelMouseHandler;
        private MousePointer pointer;
        private DebugBoundings debugBoundings;
        private MenuEntry backButton;
        private Menu settingsMenu;
        private FloatOption optionJunctionCount;
        private DistinctOption[] optionJuctionDirection;
        private DropDownMenuItem[] itemJunctionDirection;
        private DropDownMenuItem[] itemBumpRotation;
        private DropDownMenuItem[] itemModels;
        private DropDownMenuItem itemJunctionCount;

        /// <summary>
        /// Erzeugt eine neue Instanz eines CreativeModeScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt game, sowie einem Knoten knot.
        /// </summary>
        public JunctionEditorScreen (GameCore game)
        : base (game)
        {
            // die Spielwelt
            world = new World (screen: this, drawOrder: DisplayLayer.GameWorld, bounds: Bounds.FromLeft (0.60f));
            // der Input-Handler
            knotInput = new KnotInputHandler (screen: this, world: world);
            // der Mauszeiger
            pointer = new MousePointer (screen: this);
            // der Maus-Handler für die 3D-Modelle
            modelMouseHandler = new ModelMouseHandler (screen: this, world: world);

            // der Knoten-Renderer
            knotRenderer = new JunctionEditorRenderer (screen: this, position: Vector3.Zero);
            world.Add (knotRenderer);

            // visualisiert die BoundingSpheres
            debugBoundings = new DebugBoundings (screen: this, position: Vector3.Zero);
            world.Add (debugBoundings);

            // Hintergrund
            SkyCube skyCube = new SkyCube (screen: this, position: Vector3.Zero, distance: world.Camera.MaxPositionDistance + 500);
            world.Add (skyCube);

            // Backbutton
            backButton = new MenuEntry (
                screen: this,
                drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                name: "Back",
                onClick: (time) => NextScreen = new StartScreen (Game)
            );
            backButton.AddKey (Keys.Escape);
            backButton.IsVisible = true;

            // Menü
            settingsMenu = new Menu (this, DisplayLayer.Overlay + DisplayLayer.Menu);
            settingsMenu.Bounds = Bounds.FromRight (0.40f).FromBottom (0.9f).FromLeft (0.8f);
            settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            settingsMenu.RelativeItemHeight = 0.030f;

            float[] validJunctionCounts = new float[] { 1, 2, 3 };
            optionJunctionCount = new FloatOption (
                section: "debug",
                name: "debug_junction_count",
                defaultValue: validJunctionCounts.At (-1),
                validValues: validJunctionCounts,
                configFile: Config.Default
            );
            itemJunctionCount = new DropDownMenuItem (
                screen: this,
                drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                text: "Junctions #"
            );
            itemJunctionCount.AddEntries (optionJunctionCount);
            itemJunctionCount.ValueChanged += OnJunctionCountChanged;

            Direction[] validDirections = Direction.Values;
            optionJuctionDirection = new DistinctOption[3];
            itemJunctionDirection = new DropDownMenuItem[3];
            for (int i = 0; i < 3; ++i) {
                DistinctOption option = new DistinctOption (
                    section: "debug",
                    name: "debug_junction_direction" + i.ToString (),
                    defaultValue: validDirections [i * 2],
                    validValues: validDirections.Select (d => d.Description),
                    configFile: Config.Default
                );
                DropDownMenuItem item = new DropDownMenuItem (
                    screen: this,
                    drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                    text: Localizer.Localize ("Direction ") + i.ToString ()
                );
                item.AddEntries (option);
                optionJuctionDirection [i] = option;
                item.ValueChanged += OnDirectionsChanged;
                itemJunctionDirection [i] = item;
            }

            itemBumpRotation = new DropDownMenuItem[3];
            for (int i = 0; i < 3; ++i) {
                DropDownMenuItem item = new DropDownMenuItem (
                    screen: this,
                    drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                    text: Localizer.Localize ("Bump Angle ") + i.ToString ()
                );
                item.ValueChanged += OnAnglesChanged;
                itemBumpRotation [i] = item;
            }

            itemModels = new DropDownMenuItem[3];
            for (int i = 0; i < 3; ++i) {
                DropDownMenuItem item = new DropDownMenuItem (
                    screen: this,
                    drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                    text: Localizer.Localize ("Model ") + i.ToString ()
                );
                item.ValueChanged += OnModelsChanged;
                itemModels [i] = item;
            }

            OnDirectionsChanged (null);
            OnJunctionCountChanged (null);

            world.Camera.PositionToTargetDistance = 180;
        }

        private void OnDirectionsChanged (GameTime time)
        {
            float[] validAngles = new float[] {
                0, 45, 90, 135, 180, 225, 270, 315
            };
            string[] validModelnames = AvailableModels;
            if (validModelnames.Length == 0) {
                throw new Exception ("No models are available!");
            }

            for (int i = 0; i < JunctionCount; ++i) {
                FloatOption optionBumpRotation = new FloatOption (
                    section: NodeConfigKey (Directions),
                    name: "bump" + i.ToString (),
                    defaultValue: 0,
                    validValues: validAngles,
                    configFile: Config.Models
                );
                itemBumpRotation [i].AddEntries (optionBumpRotation);
                if (time != null) {
                    RemoveGameComponents (time, itemBumpRotation [i]);
                    AddGameComponents (time, itemBumpRotation [i]);
                }

                DistinctOption optionModel = new DistinctOption (
                    section: NodeConfigKey (Directions),
                    name: "modelname" + i.ToString (),
                    defaultValue: validModelnames [0],
                    validValues: validModelnames,
                    configFile: Config.Models
                );
                itemModels [i].AddEntries (optionModel);
                if (time != null) {
                    RemoveGameComponents (time, itemModels [i]);
                    AddGameComponents (time, itemModels [i]);
                }
            }

            knotRenderer.Render (directions: Directions);
        }

        private void OnAnglesChanged (GameTime time)
        {
            knotRenderer.Render (directions: Directions);
        }

        private void OnModelsChanged (GameTime time)
        {
            knotRenderer.Render (directions: Directions);
        }

        private void OnJunctionCountChanged (GameTime time)
        {
            if (time != null) {
                RemoveGameComponents (time, settingsMenu);
            }
            settingsMenu.Clear ();
            settingsMenu.Add (itemJunctionCount);

            for (int i = 0; i < 3 && i < JunctionCount; ++i) {
                settingsMenu.Add (itemJunctionDirection [i]);
            }
            for (int i = 0; i < 3 && i < JunctionCount; ++i) {
                settingsMenu.Add (itemBumpRotation [i]);
            }
            for (int i = 0; i < 3 && i < JunctionCount; ++i) {
                settingsMenu.Add (itemModels [i]);
            }
            if (time != null) {
                AddGameComponents (time, settingsMenu);
            }

            OnDirectionsChanged (time);
        }

        private int JunctionCount { get { return (int)optionJunctionCount.Value; } }

        private Direction[] Directions
        {
            get {
                List<Direction> directions = new List<Direction> ();
                for (int i = 0; i < JunctionCount; ++i) {
                    DistinctOption option = optionJuctionDirection [i];
                    Direction d = Direction.FromString (option.Value);
                    if (d != Direction.Zero) {
                        directions.Add (d);
                    }
                }
                if (directions.Count == 0) {
                    directions.Add (Direction.Values [0]);
                }

                return directions.ToArray ();
            }
        }

        public static string NodeConfigKey (IEnumerable<Direction> directions)
        {
            IEnumerable<string> _directions = directions.Select (direction => direction + String.Empty + direction);
            return "Node" + directions.Count ().ToString () + ":" + string.Join (",", _directions);
        }

        private string[] _availableModels;

        public string[] AvailableModels
        {
            get {
                if (_availableModels != null) {
                    return _availableModels;
                }
                else {
                    List<string> modelnames = new List<string> ();
                    Action<string> fileFound = (file) => {
                        string name = Path.GetFileNameWithoutExtension (file);
                        Log.Debug ("Model file: ", file, ", canonical name: ", name);
                        modelnames.Add (name);
                    };
                    string directory = SystemInfo.RelativeContentDirectory + "Models";
                    FileUtility.SearchFiles (directory: directory, extensions: new string[] {
                        "xnb",
                        "fbx"
                    }, add: fileFound);
                    return _availableModels = modelnames.ToArray ();
                }
            }
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            Profiler.ProfilerMap.Clear ();
        }

        /// <summary>
        /// Fügt die 3D-Welt und den Inputhandler in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, knotInput, pointer, world, modelMouseHandler, settingsMenu);
            AudioManager.BackgroundMusic = Knot3Sound.CreativeMusic;

            // Einstellungen anwenden
            debugBoundings.Info.IsVisible = Config.Default ["debug", "show-boundings", false];
        }
    }
}
