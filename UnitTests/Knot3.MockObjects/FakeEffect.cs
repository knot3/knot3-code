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

using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Models;

namespace Knot3.MockObjects
{
    /// <summary>
    /// Eine abstrakte Klasse, die eine Implementierung von IRenderEffect darstellt.
    /// </summary>
    public class FakeEffect : IRenderEffect
    {
        /// <summary>
        /// Das Rendertarget, in das zwischen dem Aufruf der Begin ()- und der End ()-Methode gezeichnet wird,
        /// weil es in Begin () als primäres Rendertarget des XNA-Frameworks gesetzt wird.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Der Spielzustand, in dem der Effekt verwendet wird.
        /// </summary>
        protected IGameScreen screen { get; set; }

        public bool SelectiveRendering { get { return true; } }

        public FakeEffect (IGameScreen screen)
        {
            this.screen = screen;
        }

        /// <summary>
        /// In der Methode Begin () wird das aktuell von XNA genutzte Rendertarget auf einem Stack gesichert
        /// und das Rendertarget des Effekts wird als aktuelles Rendertarget gesetzt.
        /// </summary>
        public void Begin (GameTime time)
        {
            screen.CurrentRenderEffects.Push (this);
        }

        /// <summary>
        /// Das auf dem Stack gesicherte, vorher genutzte Rendertarget wird wiederhergestellt und
        /// das Rendertarget dieses Rendereffekts wird, unter Umständen in Unterklassen verändert,
        /// auf dieses ubergeordnete Rendertarget gezeichnet.
        /// </summary>
        public virtual void End (GameTime time)
        {
            screen.CurrentRenderEffects.Pop ();
        }

        /// <summary>
        /// Zeichnet das Spielmodell model mit diesem Rendereffekt.
        /// </summary>
        public virtual void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = screen.Viewport;
            screen.Viewport = model.World.Viewport;

            // hier würde das Modell gezeichnet werden

            // Setze den Viewport wieder auf den ganzen Screen
            screen.Viewport = original;
        }

        protected void ModifyBasicEffect (BasicEffect effect, GameModel model)
        {
        }

        /// <summary>
        /// Beim Laden des Modells wird von der XNA-Content-Pipeline jedem ModelMeshPart ein Shader der Klasse
        /// BasicEffect zugewiesen. Für die Nutzung des Modells in diesem Rendereffekt kann jedem ModelMeshPart
        /// ein anderer Shader zugewiesen werden.
        /// </summary>
        public virtual void RemapModel (Model model)
        {
        }

        /// <summary>
        /// Zeichnet das Rendertarget.
        /// </summary>
        protected virtual void DrawRenderTarget (GameTime GameTime)
        {
        }

        public void DrawLastFrame (GameTime time)
        {
        }
    }
}
