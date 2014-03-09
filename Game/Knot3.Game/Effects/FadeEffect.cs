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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;

namespace Knot3.Game.Effects
{
    /// <summary>
    /// Ein Postprocessing-Effekt, der eine Überblendung zwischen zwei Spielzuständen darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class FadeEffect : RenderEffect
    {
        private float alpha;

        /// <summary>
        /// Gibt an, ob die Überblendung abgeschlossen ist und das RenderTarget nur noch den neuen Spielzustand darstellt.
        /// </summary>
        public Boolean IsFinished { get { return alpha <= 0; } }

        /// <summary>
        /// Der zuletzt gerenderte Frame im bisherigen Spielzustand.
        /// </summary>
        private RenderTarget2D PreviousRenderTarget { get; set; }

        /// <summary>
        /// Erstellt einen Überblende-Effekt zwischen den angegebenen Spielzuständen.
        /// </summary>
        public FadeEffect (IScreen newScreen, IScreen oldScreen)
        : base (newScreen)
        {
            if (oldScreen != null) {
                PreviousRenderTarget = oldScreen.PostProcessingEffect.RenderTarget;
                alpha = 1.0f;
            }
            else {
                alpha = 0.0f;
            }
            SelectiveRendering = false;
        }

        /// <summary>
        /// Zeichnet das Rendertarget.
        /// </summary>
        protected override void DrawRenderTarget (GameTime GameTime)
        {
            if (PreviousRenderTarget != null) {
                alpha -= 0.05f;

                spriteBatch.Draw (
                    PreviousRenderTarget,
                    new Vector2 (screen.Viewport.X, screen.Viewport.Y),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One / Supersampling,
                    SpriteEffects.None,
                    1f
                );
            }
            if (alpha <= 0) {
                PreviousRenderTarget = null;
                alpha = 0.0f;
            }

            spriteBatch.Draw (
                RenderTarget,
                new Vector2 (screen.Viewport.X, screen.Viewport.Y),
                null,
                Color.White * (1 - alpha),
                0f,
                Vector2.Zero,
                Vector2.One / Supersampling,
                SpriteEffects.None,
                1f
            );
        }
    }
}
