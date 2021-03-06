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

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;

namespace Knot3.Game.Effects
{
    [ExcludeFromCodeCoverageAttribute]
    public class OpaqueEffect : RenderEffect
    {
        public OpaqueEffect (IScreen screen)
        : base (screen)
        {
            pascalEffect = screen.LoadEffect ("OpaqueShader");
            RegisterEffect (pascalEffect);
        }

        public override void RemapModel (Model model)
        {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = pascalEffect;
                }
            }
        }

        public Color Color
        {
            get {
                return Color.Red;
            }
            set {
            }
        }

        public override void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = model.World.Viewport;

            Camera camera = model.World.Camera;

            //lightDirection = new Vector4 (-Vector3.Cross (Vector3.Normalize (camera.TargetDirection), camera.UpVector), 1);
            pascalEffect.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
            pascalEffect.Parameters ["View"].SetValue (camera.ViewMatrix);
            pascalEffect.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);

            pascalEffect.Parameters ["color1"].SetValue (Color.Yellow.ToVector4 ());
            pascalEffect.Parameters ["color2"].SetValue (Color.Red.ToVector4 ());

            pascalEffect.CurrentTechnique = pascalEffect.Techniques ["Technique1"];

            foreach (ModelMesh mesh in model.Model.Meshes) {
                mesh.Draw ();
            }

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        public override void DrawPrimitive (GamePrimitive model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = model.World.Viewport;

            Camera camera = model.World.Camera;

            //lightDirection = new Vector4 (-Vector3.Cross (Vector3.Normalize (camera.TargetDirection), camera.UpVector), 1);
            pascalEffect.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
            pascalEffect.Parameters ["View"].SetValue (camera.ViewMatrix);
            pascalEffect.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);

            pascalEffect.Parameters ["color1"].SetValue (Color.Yellow.ToVector4 ());
            pascalEffect.Parameters ["color2"].SetValue (Color.Red.ToVector4 ());

            pascalEffect.CurrentTechnique = pascalEffect.Techniques ["Technique1"];

            model.Primitive.Draw (pascalEffect);

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        Effect pascalEffect;
        //Vector4 lightDirection; // Light source for toon shader
    }
}
