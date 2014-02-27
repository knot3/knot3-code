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

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Widgets;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Eine abstrakte Klasse, von der alle Spielzustände erben, die Menüs darstellen.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class MenuScreen : GameScreen
    {
        private MousePointer pointer;

        // die Linien
        protected Lines lines;

        public MenuScreen (GameClass game)
        : base (game)
        {
            // die Linien
            lines = new Lines (screen: this, drawOrder: DisplayLayer.Dialog, lineWidth: 6);

            // der Mauszeiger
            pointer = new MousePointer (this);
        }

        /// <summary>
        /// Wird aufgerufen, wenn in diesen Spielzustand gewechselt wird.
        /// </summary>
        public override void Entered (IGameScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, pointer, lines);
            AudioManager.BackgroundMusic = Sound.MenuMusic;
        }
    }
}
