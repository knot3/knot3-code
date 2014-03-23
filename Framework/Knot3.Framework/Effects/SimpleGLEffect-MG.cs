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

        private void SetShaderParameters (GameObject obj, GameTime time)
        {
            Camera camera = obj.World.Camera;
            effect.Parameters ["xWorld"].SetValue (obj.WorldMatrix * camera.WorldMatrix);
            effect.Parameters ["xView"].SetValue (obj.IsSkyObject ? SkyViewMatrix (camera.ViewMatrix) : camera.ViewMatrix);
            effect.Parameters ["xProjection"].SetValue (camera.ProjectionMatrix);
            effect.Parameters ["xWorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (obj.WorldMatrix * camera.WorldMatrix)));
            effect.Parameters ["xModelTexture"].SetValue (GetTexture (obj));
            effect.Parameters ["xLightDirection"].SetValue (camera.LightDirection);
            effect.Parameters ["xCameraPosition"].SetValue (camera.Position);
            Vector4 flags = Vector4.Zero;
            flags.X = obj.Coloring.Alpha;
            flags.Y = obj.IsLightingEnabled ? 1 : 0;
            effect.Parameters ["xFlags"].SetValue (flags);
        }

        public override void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = screen.Viewport;
            screen.Viewport = model.World.Viewport;
            
            SetShaderParameters (obj: model, time: time);

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

            SetShaderParameters (obj: primitive, time: time);
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
uniform vec4 xFlags;
#define xAlpha xFlags[0]
#define xIsLightingEnabled xFlags[1]

in vec4 fragNormal;
in vec4 fragTexCoord;
in vec4 fragEyeDirection;

out vec4 fragColor;

void main ()
{
    // texture color
    vec4 colorTexture = vec4 (texture2D (xModelTexture, fragTexCoord.xy).xyz, 1.0);
    // normal
    vec3 normal = normalize (fragNormal.xyz);
    // light direction
    vec3 lightDirection = normalize(xLightDirection.xyz);

    // diffuse light intensity
    float diffuse = clamp (dot (normal, -lightDirection), 0.0, 1.0);

    // eye direction
    vec3 eyeDirection = normalize (fragEyeDirection.xyz);
    // reflected vector
    vec3 reflect = normalize (reflect(lightDirection, normal));

    // specular light intensity
    float shininess = 25.0;
    float specular = pow (clamp (dot (reflect, eyeDirection), 0.0, 1.0), shininess);
    vec4 white = vec4(1.0);

    // final color
    if (xIsLightingEnabled > 0) {
        vec4 color = clamp (colorTexture * clamp (white * 0.2 + white * diffuse, -1.0, 1.0) + specular, 0.0, 1.0);
        color.w = xAlpha;
        fragColor = color;
    }
    else {
        vec4 color = colorTexture;
        color.w = xAlpha;
        fragColor = color;
    }
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
uniform vec4 xCameraPosition;

in vec4 inputPosition;
in vec4 inputNormal;
in vec4 inputTexCoord;
out vec4 fragNormal;
out vec4 fragTexCoord;
out vec4 fragEyeDirection;

void main ()
{
    gl_Position = inputPosition * xWorld * xView * xProjection;
    fragNormal = normalize (vec4 ((inputNormal * xWorldInverseTranspose).xyz, 0));
    fragTexCoord.xy = inputTexCoord.xy;
    fragEyeDirection = normalize(vec4(xCameraPosition.xyz, 1.0) - inputPosition * xWorld);
}

#monogame EndShader ()

#monogame EffectPass (name=Pass1; vertexShader=1; pixelShader=0)
#monogame EffectTechnique (name=Textured)

";
    }
}
