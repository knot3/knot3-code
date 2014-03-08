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
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Widgets;

namespace Knot3.Framework.Screens
{
    /// <summary>
    /// Der Spielzustand, der die Auflistung der Mitwirkenden darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class ErrorScreen : Screen
    {
        ErrorDialog dialog;

        /// <summary>
        /// Erzeugt ein neues CreditsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
        /// </summary>
        public ErrorScreen (GameCore game, Exception ex)
        : base (game)
        {
            string msg = CreateMessage (ex);
            Log.Error (ex);
            dialog = new ErrorDialog (screen: this, drawOrder: DisplayLayer.Dialog, message: msg);
            dialog.Bounds.Size = new ScreenPoint (screen: this, x: 0.750f, y: 0.750f);
        }

        private string CreateMessage (Exception ex)
        {
            return ex.ToString ();
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
        }

        /// <summary>
        /// Fügt das Menü mit den Mitwirkenden in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, dialog);
        }
    }
}
