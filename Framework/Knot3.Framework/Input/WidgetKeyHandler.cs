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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;

namespace Knot3.Framework.Input
{
    /// <summary>
    /// Ein Inputhandler, der Tastatureingaben auf Widgets verarbeitet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class WidgetKeyHandler : ScreenComponent
    {
        public WidgetKeyHandler (IScreen screen)
        : base (screen, DisplayLayer.None)
        {
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            foreach (IKeyEventListener component in Screen.Game.Components.OfType<IKeyEventListener>()
                     .Where (c => c.IsKeyEventEnabled).OrderByDescending (c => c.Index.Index)) {
                // keyboard input
                KeyEvent keyEvent = KeyEvent.None;
                List<Keys> keysInvolved = new List<Keys> ();

                foreach (Keys key in component.ValidKeys) {
                    // Log.Debug ("receiver=",receiver,",validkeys=",key,", receiver.IsKeyEventEnabled=",((dynamic)receiver).IsVisible);

                    if (Screen.InputManager.KeyPressed (key)) {
                        keysInvolved.Add (key);
                        keyEvent = KeyEvent.KeyDown;
                    }
                    else if (Screen.InputManager.KeyHeldDown (key)) {
                        keysInvolved.Add (key);
                        keyEvent = KeyEvent.KeyHeldDown;
                    }
                }

                if (keysInvolved.Count > 0) {
                    component.OnKeyEvent (keysInvolved, keyEvent, time);
                    break;
                }
                if (component.IsModal) {
                    break;
                }
            }
        }
    }
}
