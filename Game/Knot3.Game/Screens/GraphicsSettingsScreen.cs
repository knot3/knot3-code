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
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Primitives;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;
using Knot3.Framework.Platform;
using Knot3.Game.Core;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Der Spielzustand, der die Grafik-Einstellungen darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class GraphicsSettingsScreen : SettingsScreen
    {
        /// <summary>
        /// Erzeugt ein neues GraphicsSettingsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public GraphicsSettingsScreen (GameCore game)
        : base (game)
        {
            MenuName = "Graphics";

            CheckBoxItem showArrows = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Arrows",
                option: new BooleanOption ("video", "arrows", false, Config.Default)
            );
            settingsMenu.Add (showArrows);

            // blinking stars
            BooleanOption blinkingStarsOption = new BooleanOption ("video", "stars-blinking", true, Config.Default);
            CheckBoxItem blinkingStars = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Blinking Stars",
                option: blinkingStarsOption
                );
            blinkingStars.OnValueChanged += () => {
                (game as Knot3Game).NotAvailableOnXNA ();
            };
            settingsMenu.Add (blinkingStars);

            // star count
            float[] validStarCounts = {
                100, 250, 500, 750, 1000, 1250, 1500, 2000, 2500, 3000, 5000
            };
            FloatOption starCountOption = new FloatOption (
                section: "video",
                name: "stars-count",
                defaultValue: validStarCounts [4],
                validValues: validStarCounts,
                configFile: Config.Default
                );
            ComboBox starCountItem = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Number of stars"
                );
            starCountItem.AddEntries (starCountOption);
            settingsMenu.Add (starCountItem);

            // show sun
            BooleanOption sunOption = new BooleanOption ("video", "show-sun", false, Config.Default);
            CheckBoxItem showSun = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Sun",
                option: sunOption
            );
            settingsMenu.Add (showSun);

            // day cycle
            float[] validDayCycles = {
                10, 20, 30, 60, 120, 300, 600, 900, 1200, 3600, 7200
            };
            FloatOption dayCycleOption = new FloatOption (
                section: "video",
                name: "day-cycle-seconds",
                defaultValue: validDayCycles [0],
                validValues: validDayCycles,
                configFile: Config.Default
            );
            ComboBox dayCycleItem = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Day cycle in seconds"
            );
            dayCycleItem.AddEntries (dayCycleOption);
            settingsMenu.Add (dayCycleItem);

            // selective rendering
            CheckBoxItem selectiveRender = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Selective Rendering",
                option: new BooleanOption ("video", "selectiveRendering", false, Config.Default)
            );
            settingsMenu.Add (selectiveRender);

            // auto camera when moving
            CheckBoxItem autoCameraMove = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Auto Camera (edge move)",
                option: new BooleanOption ("video", "auto-camera-move", true, Config.Default)
            );
            settingsMenu.Add (autoCameraMove);
            
            // auto camera
            CheckBoxItem autoCamera = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Auto Camera",
                option: new BooleanOption ("video", "auto-camera-nomove", false, Config.Default)
            );
            settingsMenu.Add (autoCamera);

            // Resolutions
            string currentResolution = GraphicsManager.GraphicsDevice.DisplayMode.Width.ToString ()
                + "x"
                + GraphicsManager.GraphicsDevice.DisplayMode.Height.ToString ();

            DisplayModeCollection modes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;
            HashSet<string> reso = new HashSet<string> ();
            foreach (DisplayMode mode in modes) {
                reso.Add (mode.Width + "x" + mode.Height);
            }
            reso.Add ("1024x600");
            string[] validResolutions = reso.ToArray ();
            validResolutions = validResolutions.OrderBy (x => Decimal.Parse (x.Split ('x') [0], System.Globalization.NumberStyles.Any)).ToArray ();
            DistinctOption resolutionOption = new DistinctOption (
                section: "video",
                name: "resolution",
                defaultValue: currentResolution,
                validValues: validResolutions,
                configFile: Config.Default
            );
            ComboBox resolutionItem = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Resolution"
            );
            resolutionItem.AddEntries (resolutionOption);
            settingsMenu.Add (resolutionItem);

            // Supersampling
            float[] validSupersamples = {
                1f, 1.25f, 1.5f, 1.75f, 2f
            };
            FloatOption supersamplesOption = new FloatOption (
                section: "video",
                name: "Supersamples",
                defaultValue: 1f,
                validValues: validSupersamples,
                configFile: Config.Default
            );
            ComboBox supersamplesItem = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Supersamples"
            );
            supersamplesItem.AddEntries (supersamplesOption);
            settingsMenu.Add (supersamplesItem);

            // fullscreen
            BooleanOption fullscreenOption = new BooleanOption ("video", "fullscreen", false, Config.Default);
            CheckBoxItem fullscreen = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Fullscreen",
                option: fullscreenOption
            );
            fullscreen.OnValueChanged += () => Game.IsFullScreen = fullscreenOption.Value;
            settingsMenu.Add (fullscreen);

            // Model quality
            SliderItem sliderModelQuality = new SliderItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Model Quality",
                max: 1000,
                min: 0,
                value: 0
            );
            sliderModelQuality.Value = (int)(Primitive.ModelQualityOption.Value * 1000f);
            sliderModelQuality.OnValueChanged = (time) => {
                float quality = (float)sliderModelQuality.Value / 1000f;
                Primitive.ModelQualityOption.Value = quality;
                Primitive.OnModelQualityChanged (time);
            };
            settingsMenu.Add (sliderModelQuality);

            // Rendereffects
            RenderEffectOption renderEffectOption = new RenderEffectOption (
                section: "video",
                name: "knot-shader",
                configFile: Config.Default
            );
            ComboBox renderEffectItem = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Render Effect"
            );
            renderEffectItem.ValueChanged += (time) => {
                RenderEffectLibrary.RenderEffectChanged (Config.Default ["video", "knot-shader", "default"], time);
            };
            renderEffectItem.AddEntries (renderEffectOption);
            settingsMenu.Add (renderEffectItem);
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
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
