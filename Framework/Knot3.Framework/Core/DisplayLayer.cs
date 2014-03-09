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

using Knot3.Framework.Widgets;

namespace Knot3.Framework.Core
{
    /// <summary>
    /// Die Zeichenreihenfolge der Elemente der grafischen Benutzeroberfläche.
    /// </summary>
    public class DisplayLayer : IEquatable<DisplayLayer>
    {
        /// <summary>
        /// Steht für die hinterste Ebene bei der Zeichenreihenfolge.
        /// </summary>
        public static readonly DisplayLayer None = new DisplayLayer (0, "None");
        /// <summary>
        /// Steht für eine Ebene hinter der Spielwelt, z.B. um
        /// Hintergrundbilder darzustellen.
        /// </summary>
        public static readonly DisplayLayer Background = new DisplayLayer (10, "Background");
        /// <summary>
        /// Steht für die Ebene in der die Spielwelt dargestellt wird.
        /// </summary>
        public static readonly DisplayLayer GameWorld = new DisplayLayer (20, "GameWorld");
        public static readonly DisplayLayer ScreenUI = new DisplayLayer (30, "ScreenUI");
        /// <summary>
        /// Steht für die Ebene in der die Dialoge dargestellt werden.
        /// Dialoge werden vor der Spielwelt gezeichnet, damit der Spieler damit interagieren kann.
        /// </summary>
        public static readonly DisplayLayer Dialog = new DisplayLayer (55, "Dialog");
        /// <summary>
        /// Steht für die Ebene in der Menüs gezeichnet werden. Menüs werden innerhalb von Dialogen angezeigt, müssen also davor gezeichnet werden, damit sie nicht vom Hintergrund des Dialogs verdeckt werden.
        /// </summary>
        public static readonly DisplayLayer Menu = new DisplayLayer (10, "Menu");
        /// <summary>
        /// Steht für die Ebene in der Menüeinträge gezeichnet werden. Menüeinträge werden vor Menüs gezeichnet.
        /// </summary>
        public static readonly DisplayLayer MenuItem = new DisplayLayer (20, "MenuItem");
        /// <summary>
        /// Zum Anzeigen zusätzlicher Informationen bei der (Weiter-)Entwicklung oder beim Testen (z.B. ein FPS-Counter).
        /// </summary>
        public static readonly DisplayLayer Overlay = new DisplayLayer (300, "Overlay");
        /// <summary>
        /// Die Maus ist das Hauptinteraktionswerkzeug, welches der Spieler
        /// ständig verwendet. Daher muss die Maus bei der Interaktion immer
        /// im Vordergrund sein. Cursor steht für die vorderste Ebene.
        /// </summary>
        public static readonly DisplayLayer Cursor = new DisplayLayer (500, "Cursor");

        public static readonly DisplayLayer[] Values = {
            None, Background, GameWorld, ScreenUI, Dialog, Menu, MenuItem, Overlay, Cursor
        };

        public int Index { get; private set; }

        public string Description { get; private set; }

        private DisplayLayer (int index, string desciption)
        {
            Index = index;
            Description = desciption;
        }

        private DisplayLayer (DisplayLayer layer1, DisplayLayer layer2)
        {
            Index = layer1.Index + layer2.Index;
            Description = layer1.Description + "+" + layer2.Description;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            return Description;
        }

        public static DisplayLayer operator + (DisplayLayer layer1, DisplayLayer layer2)
        {
            return new DisplayLayer (layer1, layer2);
        }

        public static DisplayLayer operator + (DisplayLayer layer, Widget widget)
        {
            return new DisplayLayer (widget.Index, layer);
        }

        public static DisplayLayer operator * (DisplayLayer layer, int i)
        {
            return new DisplayLayer (layer.Index * i, "(" + layer + "*" + i + ")");
        }

        public static bool operator == (DisplayLayer a, DisplayLayer b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Index == b.Index;
        }

        public static bool operator != (DisplayLayer d1, DisplayLayer d2)
        {
            return !(d1 == d2);
        }

        public bool Equals (DisplayLayer other)
        {
            return other != null && Index == other.Index;
        }

        public override bool Equals (object other)
        {
            return other != null && Equals (other as DisplayLayer);
        }

        public static implicit operator string (DisplayLayer layer)
        {
            return layer.Description;
        }

        public static implicit operator int (DisplayLayer layer)
        {
            return layer.Index;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return Description.GetHashCode ();
        }
    }
}
