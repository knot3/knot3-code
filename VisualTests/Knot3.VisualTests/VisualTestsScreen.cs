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

using Knot3.MockObjects;

namespace Knot3.VisualTests
{
    [ExcludeFromCodeCoverageAttribute]
    public class VisualTestsScreen : Screen
    {
        /// <summary>
        /// Die Spielwelt in der die 3D-Objekte des dargestellten Knotens enthalten sind.
        /// </summary>
        private World world;
        /// <summary>
        /// Der Controller, der aus dem Knoten die 3D-Modelle erstellt.
        /// </summary>
        private KnotRenderer knotRenderer;
        private FloatOption optionEdgeCount;
        private DropDownMenuItem itemEdgeCount;
        private Menu settingsMenu;
        private InputItem itemDisplayTime;

        private int EdgeCount { get { return (int)optionEdgeCount.Value; } }

        /// <summary>
        /// Erzeugt eine neue Instanz eines CreativeModeScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt game, sowie einem Knoten knot.
        /// </summary>
        public VisualTestsScreen (GameCore game)
        : base (game)
        {
            // die Spielwelt
            world = new World (screen: this, drawOrder: DisplayLayer.GameWorld, bounds: Bounds.FromLeft (0.60f));

            // der Knoten-Renderer
            knotRenderer = new KnotRenderer (screen: this, position: Vector3.Zero);
            world.Add (knotRenderer);

            // Hintergrund
            //SkyCube skyCube = new SkyCube (screen: this, position: Vector3.Zero, distance: world.Camera.MaxPositionDistance + 500);
            //world.Add (skyCube);

            // Menü
            settingsMenu = new Menu (this, DisplayLayer.Overlay + DisplayLayer.Menu);
            settingsMenu.Bounds = Bounds.FromRight (0.40f).FromBottom (0.9f).FromLeft (0.8f);
            settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            settingsMenu.RelativeItemHeight = 0.030f;

            float[] validEdgeCounts = new float[] { 500, 1000, 500, 10000, 15000 };
            optionEdgeCount = new FloatOption (
                section: "visualtests",
                name: "edgecount",
                defaultValue: validEdgeCounts.At (1),
                validValues: validEdgeCounts,
                configFile: Config.Default
            ) { Verbose = false };
            optionEdgeCount.Value = validEdgeCounts.At (1);
            itemEdgeCount = new DropDownMenuItem (
                screen: this,
                drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                text: "Edges:"
            );
            itemEdgeCount.AddEntries (optionEdgeCount);
            itemEdgeCount.ValueChanged += OnEdgeCountChanged;

            itemDisplayTime = new InputItem (
                screen: this,
                drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
                text: "Time:",
                inputText: ""
            );

            OnEdgeCountChanged (null);
        }

        private void OnEdgeCountChanged (GameTime time)
        {
            if (time != null) {
                RemoveGameComponents (time, settingsMenu);
            }
            settingsMenu.Clear ();
            settingsMenu.Add (itemEdgeCount);
            if (time != null) {
                AddGameComponents (time, settingsMenu);
            }

            Render (time);
        }

        private void Render (GameTime time)
        {
            Knot knot = KnotGenerator.generateSquareKnot (EdgeCount, "VisualTest-Knot");
            knotRenderer.Knot = knot;
        }

        private Angles3 rotation = Angles3.Zero;

        public override void Draw (GameTime time)
        {
            base.Draw (time);
            
            // World neu zeichnen
            world.Redraw = true;

            // Hier könnte man die Zeit von world.Draw () messen
            world.Draw (time);
            world.SubComponents (time).OfType<DrawableScreenComponent> ().ForEach (comp => comp.Draw (time));

            // und itemDisplayTime.InputText zuweisen zum darstellen
            itemDisplayTime.InputText = "blubb";
        }

        public override void Update (GameTime time)
        {
            base.Update (time);

            // Kamera
            rotation.X += 0.005f;
            rotation.Y += 0.001f;
            rotation.Z += 0.0005f;
            world.Camera.Target = Vector3.Zero;
            world.Camera.Position = (Vector3.Backward * Node.Scale * EdgeCount / 5)
                                    .RotateX (rotation.X).RotateY (rotation.Y).RotateZ (rotation.Z);

            // Update auf der World
            world.Update (time);
            world.SubComponents (time).OfType<ScreenComponent> ().ForEach (comp => comp.Update (time));
        }

        /// <summary>
        /// Fügt die 3D-Welt und den Inputhandler in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, settingsMenu);
            AudioManager.BackgroundMusic = Knot3Sound.CreativeMusic;
        }
    }
}
