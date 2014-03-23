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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Menü enthält Bedienelemente zur Benutzerinteraktion. Diese Klasse bietet Standardwerte für
    /// Positionen, Größen, Farben und Ausrichtungen der Menüeinträge. Sie werden gesetzt, wenn die Werte
    /// der Menüeinträge \glqq null\grqq~sind.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class Container : Widget, IEnumerable<Widget>
    {
        /// <summary>
        /// Die vom Zustand des Menüeintrags abhängige Vordergrundfarbe des Menüeintrags.
        /// </summary>
        public Func<WidgetState, Color> ItemForegroundColor { get; set; }

        /// <summary>
        /// Die vom Zustand des Menüeintrags abhängige Hintergrundfarbe des Menüeintrags.
        /// </summary>
        public Func<WidgetState, Color> ItemBackgroundColor { get; set; }

        /// <summary>
        /// Die horizontale Ausrichtung der Menüeinträge.
        /// </summary>
        public HorizontalAlignment ItemAlignX { get; set; }

        /// <summary>
        /// Die vertikale Ausrichtung der Menüeinträge.
        /// </summary>
        public VerticalAlignment ItemAlignY { get; set; }

        protected List<Widget> items;
        private bool isVisible;

        public override bool IsVisible
        {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
                if (items != null) {
                    foreach (Widget item in items) {
                        item.IsVisible = value;
                    }
                }
            }
        }

        /// <summary>
        /// Erzeugt ein neues Menu-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem ist die Angabe der Zeichenreihenfolge Pflicht.
        /// </summary>
        public Container (IScreen screen, DisplayLayer drawOrder)
        : base (screen, drawOrder)
        {
            items = new List<Widget> ();
            ItemAlignX = HorizontalAlignment.Left;
            ItemAlignY = VerticalAlignment.Center;
            ItemForegroundColor = Design.MenuItemForegroundColorFunc;
            ItemBackgroundColor = Design.MenuItemBackgroundColorFunc;
        }

        /// <summary>
        /// Fügt einen Eintrag in das Menü ein. Falls der Menüeintrag \glqq null\grqq~oder leere Werte für
        /// Position, Größe, Farbe oder Ausrichtung hat, werden die Werte mit denen des Menüs überschrieben.
        /// </summary>
        public virtual void Add (MenuItem item)
        {
            item.ItemOrder = items.Count;
            assignMenuItemInformation (item);
            items.Add (item);
            item.Container = this;
        }

        public void Add (Widget item)
        {
            assignMenuItemInformation (item);
            items.Add (item);
        }

        /// <summary>
        /// Entfernt einen Eintrag aus dem Menü.
        /// </summary>
        public virtual void Delete (Widget item)
        {
            if (items.Contains (item)) {
                items.Remove (item);
            }
            for (int i = 0, j = 0; i < items.Count; ++i) {
                if (items [i] is MenuItem) {
                    (items [i] as MenuItem).ItemOrder = j;
                    ++j;
                }
            }
        }

        /// <summary>
        /// Gibt einen Eintrag des Menüs zurück.
        /// </summary>
        public Widget GetItem (int i)
        {
            while (i < 0) {
                i += items.Count;
            }
            return items [i % items.Count];
        }

        public Widget this [int i] { get { return GetItem (i); } }

        /// <summary>
        /// Gibt die Anzahl der Einträge des Menüs zurück.
        /// </summary>
        public int Size ()
        {
            return Count;
        }

        public int Count { get { return items.Count; } }

        public void Clear ()
        {
            items.Clear ();
        }

        /// <summary>
        /// Gibt einen Enumerator über die Einträge des Menüs zurück.
        /// [returntype=IEnumerator<MenuItem>]
        /// </summary>
        public virtual IEnumerator<Widget> GetEnumerator ()
        {
            return items.GetEnumerator ();
        }

        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            foreach (DrawableScreenComponent item in items) {
                yield return item;
            }
        }

        /// <summary>
        /// Die Reaktion auf eine Bewegung des Mausrads. Ein Container zeigt keine Reaktion.
        /// </summary>
        public virtual void OnScroll (int scrollValue, GameTime time)
        {
        }

        protected virtual void assignMenuItemInformation (MenuItem item)
        {
            if (ItemForegroundColor != null) {
                item.ForegroundColorFunc = (s) => ItemForegroundColor (s);
            }
            if (ItemBackgroundColor != null) {
                item.BackgroundColorFunc = (s) => ItemBackgroundColor (s);
            }
            assignMenuItemInformation (item as Widget);
        }

        protected virtual void assignMenuItemInformation (Widget item)
        {
            item.AlignX = ItemAlignX;
            item.AlignY = ItemAlignY;
            item.IsVisible = isVisible;
        }

        public void Collapse ()
        {
            foreach (MenuItem item in items) {
                item.Collapse ();
            }
        }
    }
}
