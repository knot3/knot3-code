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
using Knot3.Framework.Models;
using Knot3.Framework.Primitives;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Models
{
    public class Sun : GamePrimitive
    {
        private BasicEffect effect;

        public Sun (IScreen screen) : base (screen: screen)
        {
            Coloring = new SingleColor (Color.Yellow);
            effect = new BasicEffect (screen.GraphicsDevice);
        }

        protected override Primitive CreativePrimitive ()
        {
            int tessellation = Primitive.CurrentCircleTessellation;
            return new Sphere (
                       device: Screen.GraphicsDevice,
                       diameter: 1f,
                       tessellation: tessellation
                   );
        }

        int j = 0;

        public override void Update (GameTime gameTime)
        {
            Position = World.Camera.SunPosition;
            Scale = Vector3.One * MathHelper.Clamp ((int)(World.Camera.SunPosition.Length () / 25f), 1000f, 10000f);
            if (j % 60 == 0) {
                IsVisible = Config.Default ["video", "show-sun", true] && !Config.Default ["debug", "projector-mode", false];
                World.Redraw = true;
            }
            ++j;
            base.Update (gameTime);
        }

        public override void Draw (GameTime gameTime)
        {
            if (IsVisible) {
                effect.World = WorldMatrix * World.Camera.WorldMatrix;
                effect.View = World.Camera.ViewMatrix;
                effect.Projection = World.Camera.ProjectionMatrix;
                effect.FogEnabled = false;
                effect.LightingEnabled = true;
                effect.PreferPerPixelLighting = true;
                effect.DirectionalLight0.DiffuseColor = Color.Yellow.ToVector3 ();
                effect.DirectionalLight0.SpecularColor = Color.Yellow.ToVector3 ();
                effect.DirectionalLight0.Direction = -World.Camera.LightDirection;
                effect.DirectionalLight0.Enabled = true;
                effect.Alpha = 0.85f;
                Primitive.Draw (effect: effect);
            }
        }
    }
}
