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
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Menüeintrag, der einen Auswahlkasten darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class CheckBoxItem : MenuItem
    {
        /// <summary>
        /// Die Option, die mit dem Auswahlkasten verknüpft ist.
        /// </summary>
        private BooleanOption option { get; set; }

        /// <summary>
        /// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public override float NameWidth
        {
            get { return System.Math.Min (0.90f, 1.0f - ValueWidth); }
            set { throw new ArgumentException ("You can't change the NameWidth of a CheckBoxItem!"); }
        }

        /// <summary>
        /// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public override float ValueWidth
        {
            get { return Bounds.Size.Relative.Y / Bounds.Size.Relative.X; }
            set { throw new ArgumentException ("You can't change the ValueWidth of a CheckBoxItem!"); }
        }

        private bool currentValue;

        public Action OnValueChanged = () => {};

        /// <summary>
        /// Erzeugt ein neues CheckBoxItem-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem sind Angaben zur Zeichenreihenfolge und der Auswahloption Pflicht.
        /// </summary>
        public CheckBoxItem (IScreen screen, DisplayLayer drawOrder, string text, BooleanOption option)
        : base (screen, drawOrder, text)
        {
            this.option = option;
            currentValue = option.Value;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            spriteBatch.Begin ();

            // berechne die Ausmaße des Wertefelds
            Rectangle bounds = ValueBounds.Rectangle;

            // zeichne den Hintergrund des Wertefelds
            spriteBatch.DrawColoredRectangle (ForegroundColor, bounds);
            spriteBatch.DrawColoredRectangle (Design.WidgetBackground, bounds.Shrink (2));

            // wenn der Wert wahr ist
            if (currentValue) {
                spriteBatch.DrawColoredRectangle (ForegroundColor, bounds.Shrink (4));
            }

            spriteBatch.End ();
        }

        private void onClick ()
        {
            currentValue = option.Value = !option.Value;
            Log.Debug ("option: ", option, " := ", currentValue);
            OnValueChanged ();
        }

        /// <summary>
        /// Reaktionen auf einen Linksklick.
        /// </summary>
        public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
            onClick ();
        }

        /// <summary>
        /// Reaktionen auf Tasteneingaben.
        /// </summary>
        public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
            if (keyEvent == KeyEvent.KeyDown) {
                onClick ();
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
