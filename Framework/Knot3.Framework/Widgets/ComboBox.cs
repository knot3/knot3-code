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
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Menüeintrag, der den ausgewählten Wert anzeigt und bei einem Linksklick ein Dropdown-Menü zur Auswahl eines neuen Wertes ein- oder ausblendet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ComboBox : MenuItem
    {
        /// <summary>
        /// Das Dropdown-Menü, das ein- und ausgeblendet werden kann.
        /// </summary>
        private Menu dropdown;
        private Border dropdownBorder;
        private InputItem currentValue;
        private Action<GameTime> updateCurrentValue = (t) => {};

        public override bool IsVisible
        {
            get { return base.IsVisible; }
            set {
                base.IsVisible = value;
                if (currentValue != null) {
                    currentValue.IsVisible = value;
                }
            }
        }

        public Action<GameTime> ValueChanged = (time) => {};
        public override float NameWidth { get { return currentValue.NameWidth; } set { currentValue.NameWidth = value; } }
        public override float ValueWidth { get { return currentValue.ValueWidth; } set { currentValue.ValueWidth = value; } }

        /// <summary>
        /// Erzeugt ein neues ConfirmDialog-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem ist die Angabe der Zeichenreihenfolge Pflicht.
        /// </summary>
        public ComboBox (IScreen screen, DisplayLayer drawOrder, string text)
        : base (screen, drawOrder, String.Empty)
        {
            dropdown = new Menu (screen: screen, drawOrder: Index + DisplayLayer.Menu);
            dropdown.Bounds.Position = ValueBounds.Position;
            dropdown.Bounds.Size = new ScreenPoint (Screen, () => ValueBounds.Size.OnlyX + ValueBounds.Size.OnlyY * 10);
            dropdown.ItemForegroundColor = Design.ComboBoxItemForegroundColorFunc; // (s) => Container.ItemForegroundColor (s);
            dropdown.ItemBackgroundColor = Design.ComboBoxItemBackgroundColorFunc; // (s) => Design.WidgetBackground;
            dropdown.BackgroundColorFunc = (s) => Design.WidgetBackground;
            dropdown.ItemAlignX = HorizontalAlignment.Left;
            dropdown.ItemAlignY = VerticalAlignment.Center;
            dropdown.IsVisible = false;
            dropdownBorder = new Border (
                screen: screen,
                drawOrder: Index + DisplayLayer.Menu,
                widget: dropdown,
                lineWidth: 2,
                padding: 2
            );

            currentValue = new InputItem (screen: screen, drawOrder: Index, text: text, inputText: String.Empty);
            currentValue.Bounds = Bounds;
            currentValue.ForegroundColorFunc = (s) => ForegroundColor;
            currentValue.BackgroundColorFunc = (s) => Color.Transparent;
            currentValue.IsVisible = IsVisible;
            currentValue.IsMouseClickEventEnabled = false;

            ValidKeys.Add (Keys.Escape);
        }

        /// <summary>
        /// Fügt Einträge in das Dropdown-Menü ein, die auf Einstellungsoptionen basieren.
        /// </summary>
        public void AddEntries (DistinctOption option)
        {
            dropdown.Clear ();
            foreach (string _value in new HashSet<string>(option.DisplayValidValues.Keys)) {
                string value = _value; // create a copy for the action
                Action<GameTime> onSelected = (time) => {
                    Log.Debug ("OnClick: ", value);
                    option.Value = option.DisplayValidValues [value];
                    currentValue.InputText = option.DisplayValue;
                    dropdown.IsVisible = false;
                    ValueChanged (time);
                };
                MenuEntry button = new MenuEntry (
                    screen: Screen,
                    drawOrder: Index + DisplayLayer.MenuItem,
                    name: value,
                    onClick: onSelected
                );
                button.IsSelectable = false;
                dropdown.Add (button);
            }
            currentValue.InputText = option.DisplayValue;
            updateCurrentValue = (t) => currentValue.InputText = option.DisplayValue;
        }

        public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
            if (key.Contains (Keys.Escape)) {
                Container.Collapse ();
                dropdown.IsVisible = false;
            }
        }

        /// <summary>
        /// Reaktionen auf einen Linksklick.
        /// </summary>
        public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
            onClick ();
        }

        private void onClick ()
        {
            if (Container is Menu) {
                float itemHeight = (Container as Menu).RelativeItemHeight;
                dropdown.RelativeItemHeight = itemHeight;
                dropdown.Bounds.Padding = new ScreenPoint (Screen, itemHeight / 5);
            }
            else {
                dropdown.Bounds.Padding = new ScreenPoint (Screen, 0.010f);
            }

            bool newValue = !dropdown.IsVisible;
            Container.Collapse ();
            dropdown.IsVisible = newValue;
        }

        public override void Collapse ()
        {
            dropdown.IsVisible = false;
        }

        public override void Update (GameTime time)
        {
            updateCurrentValue (time);
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return dropdown;
            yield return dropdownBorder;
            yield return currentValue;
        }
    }
}
