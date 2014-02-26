#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.Screens;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Effects
{
    /// <summary>
    /// Ein Postprocessing-Effekt, der eine Überblendung zwischen zwei Spielzuständen darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class FadeEffect : RenderEffect
    {
        #region Properties

        private float alpha;

        /// <summary>
        /// Gibt an, ob die Überblendung abgeschlossen ist und das RenderTarget nur noch den neuen Spielzustand darstellt.
        /// </summary>
        public Boolean IsFinished { get { return alpha <= 0; } }

        /// <summary>
        /// Der zuletzt gerenderte Frame im bisherigen Spielzustand.
        /// </summary>
        private RenderTarget2D PreviousRenderTarget { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Erstellt einen Überblende-Effekt zwischen den angegebenen Spielzuständen.
        /// </summary>
        public FadeEffect (IGameScreen newScreen, IGameScreen oldScreen)
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

        #endregion

        #region Methods

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

        #endregion
    }
}
