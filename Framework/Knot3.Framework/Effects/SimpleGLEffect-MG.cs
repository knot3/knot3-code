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
using Knot3.Framework.Platform;

namespace Knot3.Framework.Effects
{
    [ExcludeFromCodeCoverageAttribute]
    public class SimpleGLEffect : RenderEffect
    {
        Effect effect;

        public SimpleGLEffect (IScreen screen)
        : base (screen)
        {
            effect = new Effect (screen.GraphicsDevice, SHADER_CODE, GetType ().Name);
        }

        public override void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = screen.Viewport;
            screen.Viewport = model.World.Viewport;

            Camera camera = model.World.Camera;
            effect.Parameters ["xWorld"].SetValue (model.WorldMatrix * camera.WorldMatrix);
            effect.Parameters ["xView"].SetValue (camera.ViewMatrix);
            effect.Parameters ["xProjection"].SetValue (camera.ProjectionMatrix);
            effect.Parameters ["xWorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (model.WorldMatrix * camera.WorldMatrix)));

            effect.Parameters ["xModelTexture"].SetValue (GetTexture (model));
            effect.Parameters ["xLightDirection"].SetValue (new Vector4 (-1.0f, -2.0f, -1.0f, 0));

            foreach (ModelMesh mesh in model.Model.Meshes) {
                mesh.Draw ();
            }

            // Setze den Viewport wieder auf den ganzen Screen
            screen.Viewport = original;
        }

        public override void DrawPrimitive (GamePrimitive primitive, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = screen.Viewport;
            screen.Viewport = primitive.World.Viewport;

            Camera camera = primitive.World.Camera;
            effect.Parameters ["xWorld"].SetValue (primitive.WorldMatrix * camera.WorldMatrix);
            effect.Parameters ["xView"].SetValue (camera.ViewMatrix);
            effect.Parameters ["xProjection"].SetValue (camera.ProjectionMatrix);
            effect.Parameters ["xWorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (primitive.WorldMatrix * camera.WorldMatrix)));

            effect.Parameters ["xModelTexture"].SetValue (GetTexture (primitive));
            effect.Parameters ["xLightDirection"].SetValue (new Vector4 (-1.0f, -2.0f, -1.0f, 0));

            primitive.Primitive.Draw (effect: effect);

            // Setze den Viewport wieder auf den ganzen Screen
            screen.Viewport = original;
        }

        /// <summary>
        /// Weist dem 3D-Modell den Cel-Shader zu.
        /// </summary>
        public override void RemapModel (Model model)
        {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = effect;
                }
            }
        }

        private readonly string SHADER_CODE = @"
#monogame BeginShader (stage=pixel)
#monogame Attribute (name=fragNormal; usage=Normal; index=0)
#monogame Attribute (name=fragTexCoord; usage=TextureCoordinate; index=0)
#monogame Attribute (name=fragLightingFactor; usage=TextureCoordinate; index=0)
#version 130

uniform sampler2D xModelTexture;
uniform vec4 xLightDirection;

in vec4 fragNormal;
in vec4 fragTexCoord;
in vec4 fragLightingFactor;
out vec4 fragColor;

void main ()
{
    vec4 colorTexture = texture2D (xModelTexture, fragTexCoord.xy);
    colorTexture.w = 1.0;
    vec4 intensityDiffuse = colorTexture * clamp (dot (-normalize (fragNormal.xyz), normalize (xLightDirection.xyz)), -1.0, 2.0);

    vec4 color = colorTexture * 0.4 + normalize (colorTexture+vec4 (1.0)) * intensityDiffuse * 0.6;
    color.w = 1.0;
    fragColor = color;
}

#monogame EndShader ()

#monogame BeginShader (stage=vertex)
#monogame Attribute (name=inputPosition; usage=Position; index=0)
#monogame Attribute (name=inputNormal; usage=Normal; index=0)
#monogame Attribute (name=inputTexCoord; usage=TextureCoordinate; index=0)
#version 130

uniform mat4 xWorld;
uniform mat4 xView;
uniform mat4 xProjection;
uniform mat4 xWorldInverseTranspose;

in vec4 inputPosition;
in vec4 inputNormal;
in vec4 inputTexCoord;
out vec4 fragNormal;
out vec4 fragTexCoord;

void main ()
{
    gl_Position = inputPosition * xWorld * xView * xProjection;
    fragNormal = normalize (vec4 ((inputNormal * xWorldInverseTranspose).xyz, 0));
    fragTexCoord.xy = inputTexCoord.xy;
}

#monogame EndShader ()

#monogame EffectPass (name=Pass1; vertexShader=1; pixelShader=0)
#monogame EffectTechnique (name=Textured)

";
    }
}
