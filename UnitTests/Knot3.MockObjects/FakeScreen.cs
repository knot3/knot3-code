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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Math;

namespace Knot3.MockObjects
{
    public class FakeScreen : IScreen
    {
        /// <summary>
        /// Das Spiel, zu dem der Spielzustand gehört.
        /// </summary>
        public GameCore Game
        {
            get { return null;}
            set {}
        }

        /// <summary>
        /// Der Inputhandler des Spielzustands.
        /// </summary>
        public InputManager InputManager { get; private set; }

        public AudioManager AudioManager { get; private set; }

        /// <summary>
        /// Der aktuelle Postprocessing-Effekt des Spielzustands
        /// </summary>
        public IRenderEffect PostProcessingEffect { get; set; }

        /// <summary>
        /// Ein Stack, der während dem Aufruf der Draw-Methoden der Spielkomponenten die jeweils aktuellen Rendereffekte enthält.
        /// </summary>
        public IRenderEffectStack CurrentRenderEffects { get; set; }

        /// <summary>
        /// Der nächste Spielstand, der von Knot3Game gesetzt werden soll.
        /// </summary>
        public IScreen NextScreen { get; set; }

        public GraphicsDeviceManager GraphicsManager { get; private set; }

        public GraphicsDevice GraphicsDevice { get; private set; }

        public Viewport Viewport
        {
            get { return _viewport; }
            set { }
        }

        private Viewport _viewport;

        public ContentManager Content { get; private set; }

        public Color BackgroundColor { get; private set; }

        public Bounds Bounds
        {
            get { return new Bounds (screen: this, relX: 0f, relY: 0f, relWidth: 1f, relHeight: 1f); }
        }

        public FakeScreen ()
        :this (800, 600)
        {
        }

        public FakeScreen (int width, int height)
        {
            NextScreen = this;
            CurrentRenderEffects = new FakeEffectStack (
                screen: this,
                defaultEffect: new FakeEffect (this)
            );
            PostProcessingEffect = new FakeEffect (this);
            //Input = new InputManager (this);
            //Audio = new AudioManager (this);
            BackgroundColor = Color.Black;
            _viewport = new Viewport (new Rectangle (0, 0, width, height));
        }

        /// <summary>
        /// Beginnt mit dem Füllen der Spielkomponentenliste des XNA-Frameworks und fügt sowohl für Tastatur- als auch für
        /// Mauseingaben einen Inputhandler für Widgets hinzu. Wird in Unterklassen von IGameScreen reimplementiert und fügt zusätzlich weitere
        /// Spielkomponenten hinzu.
        /// </summary>
        public void Entered (IScreen previousScreen, GameTime time)
        {
        }

        /// <summary>
        /// Leert die Spielkomponentenliste des XNA-Frameworks.
        /// </summary>
        public void BeforeExit (IScreen nextScreen, GameTime time)
        {
        }

        /// <summary>
        /// Zeichnet die Teile des IGameScreens, die keine Spielkomponenten sind.
        /// </summary>
        public void Draw (GameTime time)
        {
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        public void Update (GameTime time)
        {
        }

        /// <summary>
        /// Fügt die angegebenen GameComponents in die Components-Liste des Games ein.
        /// </summary>
        public void AddGameComponents (GameTime time, params IScreenComponent[] components)
        {
        }

        /// <summary>
        /// Entfernt die angegebenen GameComponents aus der Components-Liste des Games.
        /// </summary>
        public void RemoveGameComponents (GameTime time, params IScreenComponent[] components)
        {
        }
    }
}
