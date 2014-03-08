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
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Widgets;

namespace Knot3.Game.Widgets
{
    /// <summary>
    /// Ein Dialog, der eine Texteingabe des Spielers entgegennimmt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class TextInputDialog : Dialog, IKeyEventListener
    {
        /// <summary>
        /// Der Text, der durch den Spieler eingegeben wurde.
        /// </summary>
        public string InputText
        {
            get {
                return textInput.InputText;
            }
            set {
                textInput.InputText = value;
            }
        }

        public bool NoCloseEmpty
        {
            get;
            set;
        }

        public bool NoWhiteSpace
        {
            get;
            set;
        }

        private static readonly Regex Whitespace = new Regex ("^\\s*$"); // Todo: global besser!?

        public string Text
        {
            get {
                return textItem.Text;
            }
            set {
                textItem.Text = value;
            }
        }
        /// <summary>
        ///
        /// </summary>
        public Action KeyEvent { get; set; }

        private Menu menu;
        private InputItem textInput;
        private TextItem textItem;

        /// <summary>
        ///
        /// </summary>
        public TextInputDialog (IScreen screen, DisplayLayer drawOrder, string title, string text, string inputText)
        : base (screen, drawOrder, title)
        {
            textItem = new TextItem (screen, drawOrder, String.Empty);

            Bounds.Size = new ScreenPoint (screen, 0.5f, 0.3f);
            // Der Titel-Text ist mittig ausgerichtet
            AlignX = HorizontalAlignment.Center;
            menu = new Menu (Screen, Index + DisplayLayer.Menu);
            menu.Bounds = ContentBounds;
            menu.Bounds.Padding = new ScreenPoint (screen, 0.010f, 0.019f);
            menu.ItemAlignX = HorizontalAlignment.Left;
            menu.ItemAlignY = VerticalAlignment.Center;

            //die Texteingabe
            textInput = new InputItem (Screen, Index + DisplayLayer.MenuItem, text, inputText);
            menu.Add (textInput);
            menu.Add (textItem);
            textInput.IsEnabled = true;
            textInput.IsInputEnabled = true;
            textInput.ValueWidth = 0.75f;

            ValidKeys.AddRange (new Keys[] { Keys.Enter, Keys.Escape });
        }

        /// <summary>
        ///
        /// </summary>
        public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
            if (key.Contains (Keys.Enter)) {
                bool canClose = true;

                if (NoCloseEmpty) {
                    if (textInput.InputText == null || textInput.InputText.Length == 0) {
                        canClose = false;
                        textInput.InputText = String.Empty;
                        textInput.IsInputEnabled = true; // Fokus
                        // FIX: bekommt bei schnellem ENTER dr�cken nicht wieder den Fokus.
                    }
                }

                if (NoWhiteSpace) {
                    if (Whitespace.IsMatch (textInput.InputText)) {
                        canClose = false;
                        textInput.InputText = String.Empty;
                        textInput.IsInputEnabled = true; // Fokus
                        // FIX: bekommt bei schnellem ENTER dr�cken nicht wieder den Fokus.
                    }
                }

                if (canClose) {
                    Close (time);
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
