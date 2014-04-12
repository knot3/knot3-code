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
using Knot3.Framework.Platform;
using Knot3.Framework.Primitives;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Core;
using Knot3.Game.Widgets;

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

            // arrows
            CheckBoxItem itemShowArrows = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Arrows",
                option: new BooleanOption ("video", "arrows", false, Config.Default)
            );
            settingsMenu.Add (itemShowArrows);

            // surfaces
            CheckBoxItem itemShowSurfaces = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Surfaces",
                option: new BooleanOption ("video", "surfaces", false, Config.Default)
            );
            settingsMenu.Add (itemShowSurfaces);

            // blinking stars
            BooleanOption optionBlinkingStars = new BooleanOption ("video", "stars-blinking", true, Config.Default);
            CheckBoxItem itemBlinkingStars = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Blinking Stars",
                option: optionBlinkingStars
            );
            itemBlinkingStars.OnValueChanged += () => {
                (game as Knot3Game).NotAvailableOnXNA ();
            };
            settingsMenu.Add (itemBlinkingStars);

            // star count
            float[] validStarCounts = {
                100, 250, 500, 750, 1000, 1250, 1500, 2000, 2500, 3000, 5000
            };
            FloatOption optionStarCount = new FloatOption (
                section: "video",
                name: "stars-count",
                defaultValue: validStarCounts [4],
                validValues: validStarCounts,
                configFile: Config.Default
            );
            ComboBox itemStarCount = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Number of stars"
            ) {
                NameWidth = 0.75f,
                ValueWidth = 0.25f
            };
            itemStarCount.AddEntries (optionStarCount);
            settingsMenu.Add (itemStarCount);

            // show sun
            BooleanOption optionShowSun = new BooleanOption ("video", "show-sun", false, Config.Default);
            CheckBoxItem itemShowSun = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Sun",
                option: optionShowSun
            );
            settingsMenu.Add (itemShowSun);

            // day cycle
            float[] validDayCycles = {
                10, 20, 30, 60, 120, 300, 600, 900, 1200, 3600, 7200
            };
            FloatOption optionDayCycle = new FloatOption (
                section: "video",
                name: "day-cycle-seconds",
                defaultValue: validDayCycles [3],
                validValues: validDayCycles,
                configFile: Config.Default
            );
            ComboBox itemDayCycle = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Day cycle in seconds"
            ) {
                NameWidth = 0.75f,
                ValueWidth = 0.25f
            };
            itemDayCycle.AddEntries (optionDayCycle);
            settingsMenu.Add (itemDayCycle);

            // selective rendering
            CheckBoxItem itemSelectiveRender = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Selective Rendering",
                option: new BooleanOption ("video", "selectiveRendering", false, Config.Default)
            );
            settingsMenu.Add (itemSelectiveRender);

            // auto camera when moving
            CheckBoxItem itemAutoCameraMove = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Auto Camera (edge move)",
                option: new BooleanOption ("video", "auto-camera-move", true, Config.Default)
            );
            settingsMenu.Add (itemAutoCameraMove);

            // auto camera
            CheckBoxItem itemAutoCamera = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Auto Camera",
                option: new BooleanOption ("video", "auto-camera-nomove", false, Config.Default)
            );
            settingsMenu.Add (itemAutoCamera);

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
            DistinctOption optionResolution = new DistinctOption (
                section: "video",
                name: "resolution",
                defaultValue: currentResolution,
                validValues: validResolutions,
                configFile: Config.Default
            );
            ComboBox itemResolution = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Resolution"
            ) {
                NameWidth = 0.75f,
                ValueWidth = 0.25f
            };
            itemResolution.AddEntries (optionResolution);
            settingsMenu.Add (itemResolution);

            // Supersampling
            float[] validSupersamples = {
                1f, 1.25f, 1.5f, 1.75f, 2f
            };
            FloatOption optionSupersamples = new FloatOption (
                section: "video",
                name: "Supersamples",
                defaultValue: 2f,
                validValues: validSupersamples,
                configFile: Config.Default
            );
            ComboBox itemSupersamples = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Supersamples"
            ) {
                NameWidth = 0.75f,
                ValueWidth = 0.25f
            };
            itemSupersamples.AddEntries (optionSupersamples);
            settingsMenu.Add (itemSupersamples);

            // fullscreen
            BooleanOption optionFullscreen = new BooleanOption ("video", "fullscreen", false, Config.Default);
            CheckBoxItem itemFullscreen = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Fullscreen",
                option: optionFullscreen
            );
            itemFullscreen.OnValueChanged += () => Game.IsFullScreen = optionFullscreen.Value;
            settingsMenu.Add (itemFullscreen);

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
            RenderEffectOption optionRenderEffect = new RenderEffectOption (
                section: "video",
                name: "current-world-effect",
                configFile: Config.Default
            );
            ComboBox itemRenderEffect = new ComboBox (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Render Effect"
            );
            itemRenderEffect.ValueChanged += (time) => {
                RenderEffectLibrary.RenderEffectChanged (Config.Default ["video", "current-world-effect", "default"], time);
            };
            itemRenderEffect.AddEntries (optionRenderEffect);
            settingsMenu.Add (itemRenderEffect);

            // Projector Mode
            BooleanOption optionProjectorMode = new BooleanOption ("debug", "projector-mode", false, Config.Default);
            CheckBoxItem itemProjectorMode = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Video projector mode",
                option: optionProjectorMode
            );
            itemProjectorMode.OnValueChanged += () => {
                if (optionProjectorMode.Value) {
                    new ProjectorDesign ().Apply ();
                    Config.Default ["video", "camera-overlay", false] = false;
                    Config.Default ["video", "profiler-overlay", false] = false;
                    Config.Default ["video", "current-world-effect", "default"] = "default";
                    Config.Default ["video", "Supersamples", 2f] = 2f;
                    Config.Default ["video", "arrows", false] = false;
                    Config.Default ["language", "current", "en"] = "de";
                    Config.Default ["video", "day-cycle-seconds", 60] = 10;
                }
                else {
                    new HfGDesign ().Apply ();
                }
                NextScreen = new StartScreen (game);
            };
            settingsMenu.Add (itemProjectorMode);
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
