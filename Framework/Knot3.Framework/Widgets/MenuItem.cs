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
using Knot3.Framework.Storage;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein abstrakte Klasse für Menüeinträge.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class MenuItem : Widget, IKeyEventListener, IMouseClickEventListener, IMouseScrollEventListener
    {
        /// <summary>
        /// Die Zeichenreihenfolge.
        /// </summary>
        public int ItemOrder { get; set; }

        /// <summary>
        /// Der Name des Menüeintrags, der auf der linken Seite angezeigt wird.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Eine Referenz auf das Menu, in dem sich der Eintrag befindet.
        /// </summary>
        public Container Container { get; set; }

        /// <summary>
        /// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public virtual float NameWidth { get { return _nameWidth; } set { _nameWidth = value; } }

        private float _nameWidth = 0.5f;

        /// <summary>
        /// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public virtual float ValueWidth { get { return _valueWidth; } set { _valueWidth = value; } }

        private float _valueWidth = 0.5f;
        // ein Spritebatch
        protected SpriteBatch spriteBatch;

        public virtual Bounds MouseClickBounds { get { return Bounds; } }

        public Bounds MouseScrollBounds { get { return Bounds; } }

        public Action<bool, GameTime> Hovered = (isHovered, time) => {};

        public MenuItem (IScreen screen, DisplayLayer drawOrder, string text)
        : base (screen, drawOrder)
        {
            Text = text;
            ItemOrder = -1;
            State = WidgetState.None;
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);
        }

        /// <summary>
        /// Reaktionen auf einen Linksklick.
        /// </summary>
        public virtual void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        /// <summary>
        /// Reaktionen auf einen Rechtsklick.
        /// </summary>
        public virtual void OnRightClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        /// <summary>
        /// Reaktionen auf Tasteneingaben.
        /// </summary>
        public virtual void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
        }

        public virtual void SetHovered (bool isHovered, GameTime time)
        {
            State = isHovered ? WidgetState.Hovered : WidgetState.None;
            Hovered (isHovered, time);
        }

        /// <summary>
        /// Die Reaktion auf eine Bewegung des Mausrads.
        /// </summary>
        public void OnScroll (int scrollValue, GameTime time)
        {
            if (Container != null) {
                Container.OnScroll (scrollValue, time);
            }
            else {
                Log.Debug ("Warning: MenuItem is not assigned to a menu: ", this);
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            if (IsVisible) {
                spriteBatch.Begin ();

                // zeichne den Hintergrund
                spriteBatch.DrawColoredRectangle (BackgroundColor, Bounds);

                // lade die Schrift
                SpriteFont font = Design.MenuFont (Screen);

                // zeichne die Schrift
                Color foreground = ForegroundColor * (IsEnabled ? 1f : 0.5f);
                if (IsLocalized) {
                    spriteBatch.DrawStringInRectangle (font, Text.Localize (), foreground, Bounds, AlignX, AlignY);
                }
                else {
                    spriteBatch.DrawStringInRectangle (font, Text, foreground, Bounds, AlignX, AlignY);
                }

                spriteBatch.End ();
            }
        }

        /// <summary>
        /// Berechnet die Ausmaße des Namens des Menüeintrags.
        /// </summary>
        protected virtual Bounds NameBounds
        {
            get {
                return Bounds.FromLeft (() => NameWidth);
            }
        }

        /// <summary>
        /// Berechnet die Ausmaße des Wertes des Menüeintrags.
        /// </summary>
        protected virtual Bounds ValueBounds
        {
            get {
                return Bounds.FromRight (() => ValueWidth);
            }
        }

        public virtual void Collapse ()
        {
        }
    }
}
