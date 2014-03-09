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
using Knot3.Framework.Math;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Eine abstrakte Klasse, von der alle Element der grafischen Benutzeroberfläche erben.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class Widget : DrawableScreenComponent
    {
        /// <summary>
        /// Die Ausmaße des Widgets.
        /// </summary>
        public Bounds Bounds { get; set; }

        /// <summary>
        /// Gibt an, ob das grafische Element sichtbar ist.
        /// </summary>
        public virtual bool IsVisible
        {
            get { return _isVisible && !Bounds.Size.IsEmpty; }
            set { _isVisible = value; }
        }

        private bool _isVisible;

        /// <summary>
        /// Eine Funktion, die die Hintergrundfarbe für einen bestimmten Status zurückgibt.
        /// </summary>
        public Func<WidgetState, Color> BackgroundColorFunc { private get; set; }

        /// <summary>
        /// Eine Funktion, die die Vordergrundfarbe für einen bestimmten Status zurückgibt.
        /// </summary>
        public Func<WidgetState, Color> ForegroundColorFunc { private get; set; }

        /// <summary>
        /// Ruft BackgroundColorFunc mit dem aktuellen Status auf und gibt die aktuelle Hintergrundfarbe zurück.
        /// </summary>
        public Color BackgroundColor { get { return BackgroundColorFunc (State); } }

        /// <summary>
        /// Ruft ForegroundColorFunc mit dem aktuellen Status auf und gibt die aktuelle Vordergrundfarbe zurück.
        /// </summary>
        public Color ForegroundColor { get { return ForegroundColorFunc (State); } }

        /// <summary>
        /// Die horizontale Ausrichtung.
        /// </summary>
        public HorizontalAlignment AlignX { get; set; }

        /// <summary>
        /// Die vertikale Ausrichtung.
        /// </summary>
        public VerticalAlignment AlignY { get; set; }

        /// <summary>
        /// Die Tasten, auf die dieses Widget reagiert.
        /// </summary>
        public List<Keys> ValidKeys { get; protected set; }

        public virtual bool IsKeyEventEnabled
        {
            get { return IsVisible && ValidKeys.Count > 0; }
            set { }
        }

        public virtual bool IsMouseClickEventEnabled
        {
            get { return IsVisible; }
            set { }
        }

        public virtual bool IsMouseMoveEventEnabled
        {
            get { return IsVisible; }
            set { }
        }

        public virtual bool IsMouseScrollEventEnabled
        {
            get { return IsVisible; }
            set { }
        }

        public virtual bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        private bool _isEnabled;

        public virtual WidgetState State { get; set; }

        public bool IsModal { get; set; }

        public bool IsLocalized { get; set; }

        /// <summary>
        /// Erstellt ein neues grafisches Benutzerschnittstellenelement in dem angegebenen Spielzustand
        /// mit der angegebenen Zeichenreihenfolge.
        /// </summary>
        public Widget (IScreen screen, DisplayLayer drawOrder)
        : base (screen, drawOrder)
        {
            Bounds = Bounds.Zero (screen);
            AlignX = HorizontalAlignment.Left;
            AlignY = VerticalAlignment.Center;
            ForegroundColorFunc = Design.WidgetForegroundColorFunc;
            BackgroundColorFunc = Design.WidgetBackgroundColorFunc;
            ValidKeys = new List<Keys> ();
            IsVisible = true;
            _isEnabled = true;
            IsModal = false;
            IsLocalized = true;
            State = WidgetState.None;
        }
    }
}
