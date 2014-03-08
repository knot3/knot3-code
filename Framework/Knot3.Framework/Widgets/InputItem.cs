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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Menüeintrag, der Texteingaben vom Spieler annimmt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class InputItem : MenuItem
    {
        /// <summary>
        /// Beinhaltet den vom Spieler eingegebenen Text.
        /// </summary>
        public string InputText { get; set; }

        public Action OnValueChanged = () => {};
        public Action OnValueSubmitted = () => {};

        /// <summary>
        /// Gibt an, ob gerade auf einen Tastendruck gewartet wird.
        /// </summary>
        public bool IsInputEnabled { get; set; }

        public override bool IsKeyEventEnabled
        {
            get { return isKeyEventEnabled.HasValue ? isKeyEventEnabled.Value : IsVisible && IsEnabled && IsInputEnabled; }
            set { isKeyEventEnabled = value; }
        }

        private bool? isKeyEventEnabled = null;

        public override bool IsMouseClickEventEnabled
        {
            get { return isMouseClickEventEnabled.HasValue ? isMouseClickEventEnabled.Value : base.IsMouseClickEventEnabled; }
            set { isMouseClickEventEnabled = value; }
        }

        private bool? isMouseClickEventEnabled = null;

        /// <summary>
        /// Erzeugt ein neues InputItem-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem sind Angaben zur Zeichenreihenfolge und für evtl. bereits vor-eingetragenen Text Pflicht.
        /// </summary>
        public InputItem (IScreen screen, DisplayLayer drawOrder, string text, string inputText)
        : base (screen, drawOrder, text)
        {
            InputText = inputText;
            ValidKeys.AddRange (TextHelper.ValidKeys);
            ValidKeys.Add (Keys.Enter);
            IsInputEnabled = false;
        }

        public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
            string temp = InputText;
            TextHelper.TryTextInput (ref temp, Screen, time);
            InputText = temp;
            OnValueChanged ();
            if (key.Contains (Keys.Enter)) {
                IsInputEnabled = false;
                OnValueSubmitted ();
            }
        }

        /// <summary>
        /// Reaktionen auf einen Linksklick.
        /// </summary>
        public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
            if (IsVisible) {
                IsInputEnabled = true;
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            if (IsVisible) {
                spriteBatch.Begin ();

                // berechne die Ausmaße des Eingabefelds
                Bounds bounds = ValueBounds;

                // zeichne den Hintergrund des Eingabefelds
                spriteBatch.DrawColoredRectangle (ForegroundColor, bounds);
                Color backgroundColor = IsInputEnabled ? Design.WidgetBackground.Mix (Design.WidgetForeground, 0.25f) : Design.WidgetBackground;
                spriteBatch.DrawColoredRectangle (backgroundColor, bounds.Shrink (xy: 2));

                // lade die Schrift
                SpriteFont font = Design.MenuFont (Screen);

                // zeichne die Schrift
                spriteBatch.DrawStringInRectangle (
                    font: font,
                    text: InputText,
                    color: ForegroundColor,
                    bounds: bounds.Shrink (x: 4, y: 2),
                    alignX: HorizontalAlignment.Left,
                    alignY: AlignY
                );

                spriteBatch.End ();
            }
        }
    }
}
