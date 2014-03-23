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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Screens;
using Knot3.Framework.Storage;
using Knot3.Framework.Widgets;

using Knot3.Game.Audio;
using Knot3.Game.Effects;
using Knot3.Game.Input;
using Knot3.Game.Screens;
using Knot3.Game.Widgets;

namespace Knot3.Game.Core
{
    /// <summary>
    /// Die zentrale Spielklasse, die von der \glqq Game\grqq~-Klasse des XNA-Frameworks erbt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class Knot3Game : GameCore
    {
        /// <summary>
        /// Erstellt ein neues zentrales Spielobjekt und setzt die Auflösung des BackBuffers auf
        /// die in der Einstellungsdatei gespeicherte Auflösung oder falls nicht vorhanden auf die aktuelle
        /// Bildschirmauflösung und wechselt in den Vollbildmodus.
        /// </summary>
        public Knot3Game ()
        : base ()
        {
            Window.Title = "Knot3 " + Program.Version;
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

            // design
            if (Config.Default ["debug", "projector-mode", false]) {
                new ProjectorDesign ().Apply ();
            }
            else {
                new HfGDesign ().Apply ();
            }

            // audio
            AudioManager = new Knot3AudioManager (game: this);
            AudioManager.Initialize ();

            // effects
            RenderEffectLibrary.EffectLibrary.Add (new RenderEffectLibrary.EffectFactory (
                    name: "celshader",
                    displayName: "Cel Shading (XNA)",
                    createInstance: (screen) => new CelShadingEffect (screen)
                                                   )
                                                  );
            /*
            RenderEffectLibrary.EffectLibrary.Add (new RenderEffectLibrary.EffectFactory (
                    name: "opaque",
                    displayName: "Opaque (XNA)",
                    createInstance: (screen) => new OpaqueEffect (screen)
                                                   )
                                                  );
            RenderEffectLibrary.EffectLibrary.Add (new RenderEffectLibrary.EffectFactory (
                    name: "z-nebula",
                    displayName: "Z-Nebula (XNA)",
                    createInstance: (screen) => new Z_Nebula (screen)
                                                   )
                                                  );
            */

            ScreenTransitionEffect = (previous, next) => new FadeEffect (next, previous);
        }

        public override IScreen DefaultScreen { get { return new StartScreen (this); } }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        protected override void Update (GameTime time)
        {
            base.Update (time);

            if (CurrentScreen.InputManager.KeyPressed (Keys.F8)) {
                this.Exit ();
                return;
            }

            if (CurrentScreen.PostProcessingEffect is FadeEffect && (CurrentScreen.PostProcessingEffect as FadeEffect).IsFinished) {
                CurrentScreen.PostProcessingEffect = new StandardEffect (CurrentScreen);
            }
        }

        /// <summary>
        /// Macht nichts. Das Freigeben aller Objekte wird von der automatischen Speicherbereinigung übernommen.
        /// </summary>
        protected override void UnloadContent ()
        {
            base.UnloadContent ();
        }

        public void NotAvailableOnXNA ()
        {
            if (!SystemInfo.IsRunningOnMonogame ()||true) {
                ConfirmDialog confirm = new ConfirmDialog (
                    screen: Screens.Peek (),
                    drawOrder: DisplayLayer.Dialog,
                    title: "Old Runtime Environment",
                    text: "This feature is not available in the XNA version of this game.\nDo you want to download the MonoGame version?"
                );
                confirm.Bounds.Size = new ScreenPoint (Screens.Peek (), 0.800f, 0.400f);
                confirm.Cancel += (l) => {
                };
                confirm.Submit += (r) => {
                    Process.Start ("https://github.com/pse-knot/knot3-code/releases");
                    Exit ();
                };
                Screens.Peek ().AddGameComponents (null, confirm);
            }
        }
    }
}
