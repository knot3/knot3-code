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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Audio;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Math;

namespace Knot3.Framework.Core
{
    public interface IScreen
    {
        /// <summary>
        /// Das Spiel, zu dem der Spielzustand gehört.
        /// </summary>
        GameCore Game { get; set; }

        /// <summary>
        /// Der InputManager des Spielzustands.
        /// </summary>
        InputManager InputManager { get; }

        /// <summary>
        /// Der AudioManager des Spielzustands.
        /// </summary>
        AudioManager AudioManager { get; }

        /// <summary>
        /// Der aktuelle Postprocessing-Effekt des Spielzustands
        /// </summary>
        IRenderEffect PostProcessingEffect { get; set; }

        /// <summary>
        /// Ein Stack, der während dem Aufruf der Draw-Methoden der Spielkomponenten die jeweils aktuellen Rendereffekte enthält.
        /// </summary>
        IRenderEffectStack CurrentRenderEffects { get; set; }

        /// <summary>
        /// Der nächste Spielstand, der von Knot3Game gesetzt werden soll.
        /// </summary>
        IScreen NextScreen { get; set; }

        /// <summary>
        /// Der GraphicsDeviceManager von XNA.
        /// </summary>
        GraphicsDeviceManager GraphicsManager { get; }

        /// <summary>
        /// Das GraphicsDevice von XNA.
        /// </summary>
        GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// Der aktuelle Viewport.
        /// </summary>
        Viewport Viewport { get; set; }

        /// <summary>
        /// Die Hintergrundfarbe des Screens.
        /// </summary>
        Color BackgroundColor { get; }

        /// <summary>
        /// Gibt die Ausmaße des Screens zurück.
        /// </summary>
        Bounds Bounds { get; }

        /// <summary>
        /// Beginnt mit dem Füllen der Spielkomponentenliste des XNA-Frameworks und fügt sowohl für Tastatur- als auch für
        /// Mauseingaben einen Inputhandler für Widgets hinzu. Wird in Unterklassen von IGameScreen reimplementiert und fügt zusätzlich weitere
        /// Spielkomponenten hinzu.
        /// </summary>
        void Entered (IScreen previousScreen, GameTime time);

        /// <summary>
        /// Leert die Spielkomponentenliste des XNA-Frameworks.
        /// </summary>
        void BeforeExit (IScreen nextScreen, GameTime time);

        /// <summary>
        /// Zeichnet die Teile des IGameScreens, die keine Spielkomponenten sind.
        /// </summary>
        void Draw (GameTime time);

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        void Update (GameTime time);

        /// <summary>
        /// Fügt die angegebenen GameComponents in die Components-Liste des Games ein.
        /// </summary>
        void AddGameComponents (GameTime time, params IScreenComponent[] components);

        /// <summary>
        /// Entfernt die angegebenen GameComponents aus der Components-Liste des Games.
        /// </summary>
        void RemoveGameComponents (GameTime time, params IScreenComponent[] components);
    }
}
