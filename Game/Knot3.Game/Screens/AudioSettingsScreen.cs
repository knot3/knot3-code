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

using Microsoft.Xna.Framework;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Widgets;
using Knot3.Game.Audio;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Die Klasse AudioSettingsScreen steht für den Spielzustand, der die Audio-Einstellungen repräsentiert.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class AudioSettingsScreen : SettingsScreen
    {
        /// <summary>
        /// Das Menü, das die Einstellungen enthält.
        /// </summary>
        private Menu settingsMenu { get; set; }

        private Dictionary<string, HashSet<Sound>> soundCategories = new Dictionary<string, HashSet<Sound>>()
        {
            {
                "Music",
                new HashSet<Sound>() {
                    Knot3Sound.CreativeMusic,
                    Knot3Sound.ChallengeMusic,
                    Knot3Sound.MenuMusic,
                }
            }, {
                "Sound",
                new HashSet<Sound>()
                {
                    Knot3Sound.PipeMoveSound,
                }
            },
        };

        private Action UpdateSliders = () => {};

        /// <summary>
        /// Erzeugt ein neues AudioSettingsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public AudioSettingsScreen (GameCore game)
        : base (game)
        {
            MenuName = "Audio";

            settingsMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
            settingsMenu.Bounds.Position = new ScreenPoint (this, 0.400f, 0.180f);
            settingsMenu.Bounds.Size = new ScreenPoint (this, 0.500f, 0.720f);
            settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
            settingsMenu.ItemAlignX = HorizontalAlignment.Left;
            settingsMenu.ItemAlignY = VerticalAlignment.Center;

            foreach (KeyValuePair<string, HashSet<Sound>> soundCategory in soundCategories) {
                string volumeName = soundCategory.Key;
                HashSet<Sound> sounds = soundCategory.Value;

                SliderItem slider = new SliderItem (
                    screen: this,
                    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                    text: volumeName,
                    max: 100,
                    min: 0,
                    step: 1,
                    value: 50
                );
                slider.OnValueChanged = () => {
                    float volume = (float)slider.Value / 100f;
                    foreach (Sound sound in sounds) {
                        AudioManager.SetVolume (soundType: sound, volume: volume);
                    }
                };
                settingsMenu.Add (slider);
                UpdateSliders += () => {
                    float volume = 0f;
                    foreach (Sound sound in sounds) {
                        volume += AudioManager.Volume (soundType: sound) * 100f;
                    }
                    volume /= sounds.Count;
                    slider.Value = (int)volume;
                };
            }
            UpdateSliders ();
        }

        /// <summary>
        /// Fügt das Menü mit den Einstellungen in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime GameTime)
        {
            base.Entered (previousScreen, GameTime);
            AddGameComponents (GameTime, settingsMenu);
            UpdateSliders ();
        }
    }
}
