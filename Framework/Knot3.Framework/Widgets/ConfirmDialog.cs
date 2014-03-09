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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Knot3.Framework.Core;
using Knot3.Framework.Input;
using System;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Dialog, der Schaltflächen zum Bestätigen einer Aktion anzeigt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class ConfirmDialog : Dialog
    {
        /// <summary>
        /// Wird aufgerufen, wenn der Dialog abgebrochen und geschlossen wird.
        /// </summary>
        public Action<GameTime> Cancel;
        /// <summary>
        /// Wird aufgerufen, wenn der Text im Dialog abgesendet werden soll und der Dialog geschlossen wird.
        /// </summary>
        public Action<GameTime> Submit;

        /// <summary>
        /// Das Menü, das Schaltflächen enthält.
        /// </summary>
        private Container buttons { get; set; }

        protected Menu menu;

        /// <summary>
        /// Erzeugt ein neues ConfirmDialog-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem sind Angaben zur Zeichenreihenfolge, einer Zeichenkette für den Titel und für den eingeblendeten Text Pflicht.
        /// [base=screen, drawOrder, title, text]
        /// </summary>
        public ConfirmDialog (IScreen screen, DisplayLayer drawOrder, string title)
        : base (screen, drawOrder, title)
        {
            // Der Titel-Text ist mittig ausgerichtet
            AlignX = HorizontalAlignment.Center;

            Cancel = (time) => {
                Close (time); };
            Submit = (time) => {
                Close (time); };

            // Menü, in dem die Textanzeige angezeigt wird
            menu = new Menu (Screen, Index + DisplayLayer.Menu);
            menu.Bounds = ContentBounds;
            menu.ItemForegroundColor = (s) => Color.White;
            menu.ItemBackgroundColor = (s) => Color.Transparent;
            menu.ItemAlignX = HorizontalAlignment.Left;
            menu.ItemAlignY = VerticalAlignment.Center;

            ValidKeys.AddRange (new Keys[] { Keys.Enter, Keys.Escape });
        }

        public ConfirmDialog (IScreen screen, DisplayLayer drawOrder, string title, string text)
        : this (screen, drawOrder, title)
        {
            // Die Textanzeige
            TextItem textInput = new TextItem (Screen, Index + DisplayLayer.MenuItem, text);
            menu.Add (textInput);
        }

        /// <summary>
        ///
        /// </summary>
        public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
            if (keyEvent == KeyEvent.KeyDown) {
                if (key.Contains (Keys.Enter)) {
                    Submit (time);
                }
                if (key.Contains (Keys.Escape)) {
                    Cancel (time);
                }
            }
            base.OnKeyEvent (key, keyEvent, time);
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return menu;
        }
    }
}
