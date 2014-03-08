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

using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Storage;
using Knot3.Framework.Widgets;

using Knot3.Game.Screens;

namespace Knot3.Game.Development
{
    /// <summary>
    /// Der Spielzustand, der die Debugging-Einstellungen darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class DebugSettingsScreen : SettingsScreen
    {
        /// <summary>
        /// Das Menü, das die Einstellungen enthält.
        /// </summary>
        private Menu settingsMenu;

        /// <summary>
        /// Erzeugt ein neues DebugSettingsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public DebugSettingsScreen (GameCore game)
        : base (game)
        {
            MenuName = "Debug";

            settingsMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            settingsMenu.Bounds.Position = new ScreenPoint (this, 0.400f, 0.180f);
            settingsMenu.Bounds.Size = new ScreenPoint (this, 0.500f, 0.770f);
            settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            settingsMenu.ItemAlignX = HorizontalAlignment.Left;
            settingsMenu.ItemAlignY = VerticalAlignment.Center;

            CheckBoxItem showOverlay = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Overlay",
                option: new BooleanOption ("video", "camera-overlay", false, Config.Default)
            );
            settingsMenu.Add (showOverlay);

            CheckBoxItem showFps = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show FPS",
                option: new BooleanOption ("video", "fps-overlay", true, Config.Default)
            );
            settingsMenu.Add (showFps);

            CheckBoxItem showProfiler = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Profiler",
                option: new BooleanOption ("video", "profiler-overlay", true, Config.Default)
            );
            settingsMenu.Add (showProfiler);

            CheckBoxItem showBoundings = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Bounding Boxes",
                option: new BooleanOption ("debug", "show-boundings", false, Config.Default)
            );
            settingsMenu.Add (showBoundings);

            CheckBoxItem showStartEdgeArrow = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Show Start Edge Direction",
                option: new BooleanOption ("debug", "show-startedge-direction", false, Config.Default)
            );
            settingsMenu.Add (showStartEdgeArrow);

            string[] unprojectMethods = { "SelectedObject", "NearFarAverage" };
            DistinctOption unprojectOption = new DistinctOption ("debug", "unproject", unprojectMethods [0], unprojectMethods, Config.Default);
            DropDownMenuItem unprojectItem = new DropDownMenuItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Unproject"
            );
            unprojectItem.AddEntries (unprojectOption);
            settingsMenu.Add (unprojectItem);

            /*
            CheckBoxItem shaderPascal = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Use Pascal's Shader",
                option: new BooleanOptionInfo ("video", "pascal-shader", false, Options.Default)
            );te
            settingsMenu.Add (shaderPascal);

            CheckBoxItem shaderCel = new CheckBoxItem (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                text: "Use Cel Shader",
                option: new BooleanOptionInfo ("video", "cel-shading", false, Options.Default)
            );
            settingsMenu.Add (shaderCel);
            */
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
