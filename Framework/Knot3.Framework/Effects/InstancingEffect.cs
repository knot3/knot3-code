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
using Knot3.Framework.Development;
using Knot3.Framework.Models;

namespace Knot3.Framework.Effects
{
    public abstract class InstancingEffect : RenderEffect
    {
        public InstancingEffect (IScreen screen)
        : base (screen: screen)
        {
            screen.Game.Graphics.ApplyChanges ();
            Profiler.ProfilerMap.Clear ();
        }

        private GameModel[] modelQueue = new GameModel [100];
        private int queuedModels = 0;

        public override void DrawModel (GameModel model, GameTime time)
        {
            if (queuedModels + 1 >= modelQueue.Length) {
                Array.Resize (ref modelQueue, modelQueue.Length + 100);
            }
            modelQueue [queuedModels++] = model;
        }

        protected virtual void DrawAllModels (GameTime time)
        {
            for (int i = 0; i < queuedModels; ++i) {
                base.DrawModel (modelQueue [i], time);
            }
            queuedModels = 0;
        }

        protected abstract void DrawAllPrimitives (GameTime time);

        protected override void BeforeEnd (GameTime time)
        {
            Screen.GraphicsDevice.Clear (Color.Transparent);

            Profiler.ProfileDelegate ["Instancing.Prm"] = () => {
                DrawAllPrimitives (time);
            };
            Profiler.ProfileDelegate ["Instancing.Mds"] = () => {
                DrawAllModels (time);
            };
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing) {
            }
            base.Dispose (disposing);
        }
    }
}
