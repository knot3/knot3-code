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

            // screens
            Screens = new Stack<IScreen> ();
            Screens.Push (new VisualTestsScreen (this));
            Screens.Peek ().Entered (null, null);

            // base method
            base.Initialize ();
        }

        /// <summary>
        /// Ruft die Draw ()-Methode des aktuellen Spielzustands auf.
        /// </summary>
        protected override void Draw (GameTime time)
        {
            try {
                // Lade den aktuellen Screen
                IScreen current = Screens.Peek ();

                // Starte den Post-Processing-Effekt des Screens
                current.PostProcessingEffect.Begin (time);
                Graphics.GraphicsDevice.Clear (current.BackgroundColor);

                try {
                    // Rufe Draw () auf dem aktuellen Screen auf
                    current.Draw (time);

                    // Rufe Draw () auf den Spielkomponenten auf
                    base.Draw (time);
                }
                catch (Exception ex) {
                    // Error Screen
                    ShowError (ex);
                }

                // Beende den Post-Processing-Effekt des Screens
                current.PostProcessingEffect.End (time);
            }
            catch (Exception ex) {
                // Error Screen
                ShowError (ex);
            }
        }

        /// <summary>
        /// Macht nichts. Das Freigeben aller Objekte wird von der automatischen Speicherbereinigung übernommen.
        /// </summary>
        protected override void UnloadContent ()
        {
        }

        public void ShowError (Exception ex)
        {
            Screens = new Stack<IScreen> ();
            Screens.Push (new ErrorScreen (this, ex));
            Screens.Peek ().Entered (null, null);
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        protected override void Update (GameTime time)
        {
            try {
                updateResolution ();
                // falls der Screen gewechselt werden soll...
                IScreen current = Screens.Peek ();
                IScreen next = current.NextScreen;
                if (current != next) {
                    current.BeforeExit (next, time);
                    current.NextScreen = current;
                    next.NextScreen = next;
                    Screens.Push (next);
                    next.Entered (current, time);
                }

                if (current.InputManager.KeyPressed (Keys.F8)) {
                    this.Exit ();
                    return;
                }

                // Rufe Update () auf dem aktuellen Screen auf
                Screens.Peek ().Update (time);

                // base method
                base.Update (time);
            }
            catch (Exception ex) {
                // Error Screen
                ShowError (ex);
            }
        }
    }
}