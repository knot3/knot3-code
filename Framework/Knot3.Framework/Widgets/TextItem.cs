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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Widget, der eine Zeichenkette anzeigt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class TextItem : MenuItem
    {
        /// <summary>
        /// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public override float NameWidth
        {
            get { return 1.00f; }
            set { throw new ArgumentException ("You can't change the NameWidth of a TextItem!"); }
        }

        /// <summary>
        /// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
        /// </summary>
        public override float ValueWidth
        {
            get { return 0.00f; }
            set { throw new ArgumentException ("You can't change the ValueWidth of a TextItem!"); }
        }

        /// <summary>
        /// Erzeugt ein neues TextItem-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem sind Angabe der Zeichenreihenfolge und der Zeichenkette, die angezeigt wird, für Pflicht.
        /// </summary>
        public TextItem (IScreen screen, DisplayLayer drawOrder, string text)
        : base (screen, drawOrder, text)
        {
        }

        //Da TextItems werden nicht unterlegt um sie von Buttons abzugrenzen
        public override void SetHovered (bool hovered, GameTime time)
        {
            State = WidgetState.None;
        }
    }
}
