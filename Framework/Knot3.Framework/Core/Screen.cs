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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Audio;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Widgets;

namespace Knot3.Framework.Core
{
    /// <summary>
    /// Ein Spielzustand, der zu einem angegebenen Spiel gehört und einen Inputhandler und Rendereffekte enthält.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class Screen : IScreen
    {
        /// <summary>
        /// Das Spiel, zu dem der Spielzustand gehört.
        /// </summary>
        public GameCore Game { get; set; }

        /// <summary>
        /// Der InputManager des Spielzustands.
        /// </summary>
        public InputManager InputManager { get; set; }

        /// <summary>
        /// Der AudioManager des Spielzustands.
        /// </summary>
        public AudioManager AudioManager { get { return Game.AudioManager; } }

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

        /// <summary>
        /// Der GraphicsDeviceManager von XNA.
        /// </summary>
        public GraphicsDeviceManager GraphicsManager { get { return Game.Graphics; } }

        /// <summary>
        /// Das GraphicsDevice von XNA.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get { return Game.GraphicsDevice; } }

        /// <summary>
        /// Der aktuelle Viewport.
        /// </summary>
        public Viewport Viewport
        {
            get { return GraphicsDevice.Viewport; }
            set { GraphicsDevice.Viewport = value; }
        }

        /// <summary>
        /// Die Hintergrundfarbe des Screens.
        /// </summary>
        public Color BackgroundColor { get; protected set; }

        /// <summary>
        /// Gibt die Ausmaße des Screens zurück. Dabei handelt es sich
        /// immer um die relative Position (0,0) und die relative Größe (1,1).
        /// </summary>
        public Bounds Bounds
        {
            get { return new Bounds (screen: this, relX: 0f, relY: 0f, relWidth: 1f, relHeight: 1f); }
        }

        /// <summary>
        /// Gibt an, ob die Screen History beim öffnen dieses Screens geleert werden soll.
        /// </summary>
        public bool ClearScreenHistory { get; protected set; }

        /// <summary>
        /// Erzeugt ein neues IGameScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public Screen (GameCore game)
        {
            Game = game;
            NextScreen = this;
            CurrentRenderEffects = new RenderEffectStack (screen: this, defaultEffect: new StandardEffect (screen: this));
            PostProcessingEffect = new StandardEffect (screen: this);
            InputManager = new InputManager (screen: this);
            BackgroundColor = Design.ScreenBackground;
        }

        /// <summary>
        /// Beginnt mit dem Füllen der Spielkomponentenliste des XNA-Frameworks und fügt sowohl für Tastatur- als auch für
        /// Mauseingaben einen Inputhandler für Widgets hinzu. Wird in Unterklassen von IGameScreen reimplementiert und fügt zusätzlich weitere
        /// Spielkomponenten hinzu.
        /// </summary>
        public virtual void Entered (IScreen previousScreen, GameTime time)
        {
            Log.Debug ("Entered: ", this);
            IScreenComponent audioManager = AudioManager.ToScreenComponent (screen: this);
            AddGameComponents (time, InputManager, audioManager, new WidgetKeyHandler (this), new WidgetMouseHandler (this));
        }

        /// <summary>
        /// Leert die Spielkomponentenliste des XNA-Frameworks.
        /// </summary>
        public virtual void BeforeExit (IScreen nextScreen, GameTime time)
        {
            Log.Debug ("BeforeExit: ", this);
            Game.Components.Clear ();
        }

        /// <summary>
        /// Zeichnet die Teile des IGameScreens, die keine Spielkomponenten sind.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public virtual void Draw (GameTime time)
        {
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public virtual void Update (GameTime time)
        {
        }

        /// <summary>
        /// Fügt die angegebenen GameComponents in die Components-Liste des Games ein.
        /// </summary>
        public void AddGameComponents (GameTime time, params IScreenComponent[] components)
        {
            Log.BlockList (id: 27, before: "  - ", after: "", begin: "Add components: ", end: "");
            foreach (IScreenComponent component in components) {
                Log.ListElement (id: 27, element: component);
                Game.Components.Add (component);
                AddGameComponents (time, component.SubComponents (time).ToArray ());
            }
        }

        /// <summary>
        /// Entfernt die angegebenen GameComponents aus der Components-Liste des Games.
        /// </summary>
        public void RemoveGameComponents (GameTime time, params IScreenComponent[] components)
        {
            Log.BlockList (id: 28, before: "  - ", after: "", begin: "Remove components: ", end: "");
            foreach (IScreenComponent component in components) {
                RemoveGameComponents (time, component.SubComponents (time).ToArray ());
                Log.ListElement (id: 28, element: component);
                Game.Components.Remove (component);
            }
        }
    }
}
