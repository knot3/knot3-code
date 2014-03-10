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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework;
using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Screens;
using Knot3.Framework.Utilities;

using Knot3.Game.Widgets;

namespace Knot3.VisualTests
{
    /// <summary>
    /// Die zentrale Spielklasse, die von der \glqq Game\grqq~-Klasse des XNA-Frameworks erbt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class VisualTestsGame : GameCore
    {
        /// <summary>
        /// Erstellt ein neues zentrales Spielobjekt und setzt die Auflösung des BackBuffers auf
        /// die in der Einstellungsdatei gespeicherte Auflösung oder falls nicht vorhanden auf die aktuelle
        /// Bildschirmauflösung und wechselt in den Vollbildmodus.
        /// </summary>
        public VisualTestsGame ()
        : base ()
        {
            Window.Title = "Knot3 Visual Tests " + Program.Version;

            IsMouseVisible = true;
            System.Windows.Forms.Cursor.Show ();
        }

        /// <summary>
        /// Initialisiert die Attribute dieser Klasse.
        /// </summary>
        protected override void Initialize ()
        {
            // base method
            base.Initialize ();

            // vsync
            VSync = true;

            // anti aliasing
            Graphics.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            Graphics.PreferMultiSampling = true;

            // design
            new HfGDesign ().Apply ();

            // audio
            AudioManager = new SilentAudioManager (game: this);
            AudioManager.Initialize ();
        }

        public override IScreen DefaultScreen { get { return new VisualTestsScreen (this); } }
    }
}
