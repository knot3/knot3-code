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
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Eine Schaltfläche, der eine Zeichenkette anzeigt und auf einen Linksklick reagiert.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class MenuEntry : MenuItem
    {
        /// <summary>
        /// Die Aktion, die ausgeführt wird, wenn der Spieler auf die Schaltfläche klickt.
        /// </summary>
        public Action<GameTime> OnClick { get; set; }

        /// <summary>
        /// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public override float NameWidth
        {
            get { return 1.00f; }
            set { throw new ArgumentException ("You can't change the NameWidth of a MenuButton!"); }
        }

        /// <summary>
        /// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public override float ValueWidth
        {
            get { return 0.00f; }
            set { throw new ArgumentException ("You can't change the ValueWidth of a MenuButton!"); }
        }

        public bool IsSelectable { get; set; }

        /// <summary>
        /// Erzeugt ein neues MenuButton-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem sind Angabe der Zeichenreihenfolge, einer Zeichenkette für den Namen der Schaltfläche
        /// und der Aktion, welche bei einem Klick ausgeführt wird Pflicht.
        /// </summary>
        public MenuEntry (IScreen screen, DisplayLayer drawOrder, string name, Action<GameTime> onClick)
        : base (screen, drawOrder, name)
        {
            IsSelectable = false;
            OnClick = onClick;
        }

        /// <summary>
        /// Reaktionen auf einen Linksklick.
        /// </summary>
        public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
            base.OnLeftClick (position, state, time);
            if (IsSelectable) {
                State = WidgetState.Selected;

                if (Container != null) {
                    foreach (MenuItem item in Container) {
                        Log.Debug ("State: ", item.State);
                        if (item is MenuEntry && item !=this) {
                            item.State = WidgetState.None;
                        }
                    }
                }
            }

            OnClick (time);
        }

        /// <summary>
        /// Reaktionen auf Tasteneingaben.
        /// </summary>
        public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
            // Log.Debug ("OnKeyEvent: ", key [0]);
            if (keyEvent == KeyEvent.KeyDown) {
                OnClick (time);
            }
        }

        public override void SetHovered (bool isHovered, GameTime time)
        {
            if (State != WidgetState.Selected && Enabled) {
                base.SetHovered (isHovered, time);
            }
        }

        public void AddKey (Keys key)
        {
            if (!ValidKeys.Contains (key)) {
                ValidKeys.Add (key);
            }
        }
    }
}
