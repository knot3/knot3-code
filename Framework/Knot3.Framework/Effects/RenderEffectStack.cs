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

using Knot3.Framework.Core;

namespace Knot3.Framework.Effects
{
    /// <summary>
    /// Ein Stapel, der während der Draw-Aufrufe die Hierarchie der aktuell verwendeten Rendereffekte verwaltet
    /// und automatisch das aktuell von MonoGame verwendete Rendertarget auf das Rendertarget des obersten Rendereffekts
    /// setzt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class RenderEffectStack : IRenderEffectStack
    {
        private IScreen screen;
        private static Stack<IRenderEffect> stack = new Stack<IRenderEffect> ();

        /// <summary>
        /// Der oberste Rendereffekt.
        /// </summary>
        public IRenderEffect CurrentEffect
        {
            get {
                if (stack.Count > 0) {
                    return stack.Peek ();
                }
                else {
                    return defaultEffect;
                }
            }
        }

        /// <summary>
        /// Der Standard-Rendereffekt, der verwendet wird, wenn der Stapel leer ist.
        /// </summary>
        private IRenderEffect defaultEffect { get; set; }

        /// <summary>
        /// Erstellt einen neuen Rendereffekt-Stapel.
        /// </summary>
        public RenderEffectStack (IScreen screen, IRenderEffect defaultEffect)
        {
            this.screen = screen;
            this.defaultEffect = defaultEffect;
            stack = new Stack<IRenderEffect> ();
        }

        /// <summary>
        /// Entfernt den obersten Rendereffekt vom Stapel.
        /// </summary>
        public IRenderEffect Pop ()
        {
            if (stack.Count > 0) {
                IRenderEffect removed = stack.Pop ();
                if (stack.Count > 0) {
                    screen.GraphicsDevice.SetRenderTarget (CurrentEffect.RenderTarget);
                }
                else {
                    screen.GraphicsDevice.SetRenderTarget (null);
                }
                return removed;
            }
            else {
                return defaultEffect;
            }
        }

        /// <summary>
        /// Legt einen Rendereffekt auf den Stapel.
        /// </summary>
        public void Push (IRenderEffect effect)
        {
            stack.Push (effect);
            screen.GraphicsDevice.SetRenderTarget (effect.RenderTarget);
        }
    }
}
