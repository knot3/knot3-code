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

using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Input
{
    /// <summary>
    /// Repräsentiert einen Mauszeiger.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class MousePointer : DrawableScreenComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D cursorTex;

        /// <summary>
        /// Erstellt einen neuen Mauszeiger für den angegebenen Spielzustand.
        /// </summary>
        public MousePointer (IScreen screen)
        : base (screen, DisplayLayer.Cursor)
        {
            cursorTex = Screen.LoadTexture ("cursor");
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);
        }

        /// <summary>
        /// Zeichnet den Mauszeiger.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            DrawCursor (time);
        }

        private void DrawCursor (GameTime time)
        {
            if (!SystemInfo.IsRunningOnLinux ()) {
                spriteBatch.Begin ();

                if (Screen.InputManager.GrabMouseMovement || Screen.InputManager.CurrentInputAction == InputAction.CameraTargetMove
                        || (Screen.InputManager.CurrentInputAction == InputAction.ArcballMove
                            && (Screen.InputManager.CurrentMouseState.LeftButton == ButtonState.Pressed
                                || Screen.InputManager.CurrentMouseState.RightButton == ButtonState.Pressed))) {
                    //spriteBatch.Draw (cursorTex, Screen.Device.Viewport.Center (), Color.White);
                }
                else {
                    spriteBatch.Draw (
                        cursorTex,
                        Screen.InputManager.CurrentMousePosition.AbsoluteVector * Config.Default ["video", "Supersamples", 2],
                        null,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        Vector2.One * Config.Default ["video", "Supersamples", 2],
                        SpriteEffects.None,
                        1f
                    );
                }

                spriteBatch.End ();
            }
        }
    }
}
