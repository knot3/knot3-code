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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Primitives;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Effects
{
    [ExcludeFromCodeCoverageAttribute]
    public class HardwareInstancingEffect : RenderEffect, IHardwareInstancingEffect
    {
        Effect effect;
        VertexDeclaration instanceVertexDeclaration;

        public HardwareInstancingEffect (IScreen screen)
        : base (screen)
        {
            //Effect hitest = screen.LoadEffect ("hitest");
            //System.IO.File.WriteAllText (SystemInfo.RelativeBaseDirectory + "hitest.glfx_gen", hitest.EffectCode);

            effect = new Effect (screen.GraphicsDevice, SHADER_CODE, GetType ().Name);
            instanceVertexDeclaration = GenerateInstanceVertexDeclaration ();
        }

        private VertexDeclaration GenerateInstanceVertexDeclaration ()
        {
            VertexElement[] instanceStreamElements = new VertexElement [10];
            // WorldMatrix
            instanceStreamElements [0] = new VertexElement (0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1);
            instanceStreamElements [1] = new VertexElement (sizeof (float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2);
            instanceStreamElements [2] = new VertexElement (sizeof (float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3);
            instanceStreamElements [3] = new VertexElement (sizeof (float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4);
            // TransposeInverseWorldMatrix
            instanceStreamElements [4] = new VertexElement (sizeof (float) * 16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5);
            instanceStreamElements [5] = new VertexElement (sizeof (float) * 20, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6);
            instanceStreamElements [6] = new VertexElement (sizeof (float) * 24, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 7);
            instanceStreamElements [7] = new VertexElement (sizeof (float) * 28, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 8);
            // Alpha
            instanceStreamElements [8] = new VertexElement (sizeof (float) * 32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 9);
            // IsLightingEnabled
            instanceStreamElements [9] = new VertexElement (sizeof (float) * 33, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 10);
            return new VertexDeclaration (instanceStreamElements);
        }

        private struct InstanceInfo {
            public Matrix WorldMatrix;
            public Matrix TransposeInverseWorldMatrix;
            public float Alpha;
            public float IsLightingEnabled;
        }

        private class InstancedPrimitive
        {
            public Primitive Primitive;
            public World World;
            public bool IsSkyObject;
            public Texture2D Texture;
            public InstanceInfo[] Instances;
            public int InstanceCount;

            public int InstanceCapacity { get { return Instances.Length; } }

            public double InstanceUniqueHash;
        }

        private class InstancedBuffer
        {
            public VertexBuffer InstanceBuffer;
            public int InstanceCount;

            public int InstanceCapacity { get { return InstanceBuffer.VertexCount; } }

            public double InstanceUniqueHash;
        }

        private Dictionary<string, InstancedPrimitive> cacheInstancedPrimitives = new Dictionary<string, InstancedPrimitive> ();
        private Dictionary<string, InstancedBuffer> cachePrimitivesBuffers = new Dictionary<string, InstancedBuffer> ();

        public override void DrawPrimitive (GamePrimitive primitive, GameTime time)
        {
            string key = primitive.GameObjectCategory;
            InstancedPrimitive instancedPrimitive;
            if (!cacheInstancedPrimitives.ContainsKey (key)) {
                cacheInstancedPrimitives [key] = instancedPrimitive = new InstancedPrimitive () {
                    Primitive = primitive.Primitive,
                    World = primitive.World,
                    Texture = GetTexture (primitive),
                    Instances = new InstanceInfo [100],
                    InstanceCount = 0,
                    InstanceUniqueHash = 0,
                    IsSkyObject = primitive.IsSkyObject
                };
            }
            else {
                instancedPrimitive = cacheInstancedPrimitives [key];
            }
            if (instancedPrimitive.InstanceCount + 1 >= instancedPrimitive.Instances.Length) {
                Array.Resize (ref instancedPrimitive.Instances, instancedPrimitive.Instances.Length + 200);
            }
            InstanceInfo instanceInfo = new InstanceInfo {
                WorldMatrix = primitive.WorldMatrix * primitive.World.Camera.WorldMatrix,
                TransposeInverseWorldMatrix = primitive.WorldMatrixInverseTranspose * primitive.World.Camera.WorldMatrix,
                Alpha = primitive.Coloring.Alpha,
                IsLightingEnabled = primitive.IsLightingEnabled ? 1 : 0
            };
            instancedPrimitive.Instances [instancedPrimitive.InstanceCount++] = instanceInfo;
            instancedPrimitive.InstanceUniqueHash += primitive.Position.LengthSquared ();
        }

        private void DrawAllPrimitives (GameTime time)
        {
            Profiler.Values ["Diff. Inst."] = cacheInstancedPrimitives.Count;
            foreach (string key in cacheInstancedPrimitives.Keys) {
                InstancedPrimitive instancedPrimitive = cacheInstancedPrimitives [key];

                // Setze den Viewport auf den der aktuellen Spielwelt
                Viewport original = screen.Viewport;
                screen.Viewport = instancedPrimitive.World.Viewport;

                Camera camera = instancedPrimitive.World.Camera;
                effect.Parameters ["xView"].SetValue (instancedPrimitive.IsSkyObject ? SkyViewMatrix (camera.ViewMatrix) : camera.ViewMatrix);
                effect.Parameters ["xProjection"].SetValue (camera.ProjectionMatrix);
                effect.Parameters ["xModelTexture"].SetValue (instancedPrimitive.Texture);
                effect.Parameters ["xLightDirection"].SetValue (camera.LightDirection);
                effect.Parameters ["xCameraPosition"].SetValue (camera.Position);

                InstancedBuffer buffer;
                if (!cachePrimitivesBuffers.ContainsKey (key)) {
                    buffer = cachePrimitivesBuffers [key] = new InstancedBuffer ();
                    buffer.InstanceBuffer = new VertexBuffer (screen.GraphicsDevice, instanceVertexDeclaration, instancedPrimitive.InstanceCapacity, BufferUsage.WriteOnly);
                    buffer.InstanceCount = 0;
                    Log.Debug ("new VertexBuffer");
                }
                else {
                    buffer = cachePrimitivesBuffers [key];
                }

                if (buffer.InstanceCapacity < instancedPrimitive.InstanceCapacity) {
                    Profiler.ProfileDelegate ["NewVertexBuffer"] = () => {
                        buffer.InstanceBuffer.Dispose ();
                        buffer.InstanceBuffer = new VertexBuffer (screen.GraphicsDevice, instanceVertexDeclaration, instancedPrimitive.InstanceCapacity, BufferUsage.WriteOnly);
                        Log.Debug ("Dispose -> new VertexBuffer");
                    };
                }
                if (buffer.InstanceCount != instancedPrimitive.InstanceCount || buffer.InstanceUniqueHash != instancedPrimitive.InstanceUniqueHash) {
                    buffer.InstanceBuffer.SetData (instancedPrimitive.Instances);
                    buffer.InstanceCount = instancedPrimitive.InstanceCount;
                    buffer.InstanceUniqueHash = instancedPrimitive.InstanceUniqueHash;
                }

                instancedPrimitive.Primitive.DrawInstances (effect: effect, instanceBuffer: ref buffer.InstanceBuffer, instanceCount: instancedPrimitive.InstanceCount);

                // Setze den Viewport wieder auf den ganzen Screen
                screen.Viewport = original;

                instancedPrimitive.InstanceCount = 0;
            }
        }

        protected override void BeforeEnd (GameTime time)
        {
            Profiler.ProfileDelegate ["Instancing"] = () => {
                DrawAllPrimitives (time);
            };
        }

        protected override void Dispose (bool disposing)
        {
            if (disposing) {
                foreach (InstancedBuffer buffer in cachePrimitivesBuffers.Values) {
                    Log.Debug ("Dispose VertexBuffer");
                    buffer.InstanceBuffer.Dispose ();
                }
            }
        }
        //#monogame EffectParameter (name=xView; class=Matrix; type=Single; rows=4; columns=4)
        //#monogame ConstantBuffer (name=xView; sizeInBytes=64; parameters=[0]; offsets=[0])
        //#monogame EffectParameter (name=xProjection; class=Matrix; type=Single; rows=4; columns=4)
        //#monogame ConstantBuffer (name=xProjection; sizeInBytes=64; parameters=[1]; offsets=[0])
        private readonly string SHADER_CODE = @"
#monogame BeginShader (stage=pixel; constantBuffers=[])
#monogame Attribute (name=fragNormal; usage=Normal; index=0)
#monogame Attribute (name=fragTexCoord; usage=TextureCoordinate; index=0)
#monogame Attribute (name=fragEyeDirection; usage=TextureCoordinate; index=1)
#monogame Attribute (name=fragAlpha; usage=TextureCoordinate; index=2)
#monogame Attribute (name=fragIsLightingEnabled; usage=TextureCoordinate; index=3)
#version 130

uniform sampler2D xModelTexture;
uniform vec4 xLightDirection;

in vec4 fragNormal;
in vec4 fragTexCoord;
in vec4 fragEyeDirection;
in float fragAlpha;
in float fragIsLightingEnabled;

out vec4 fragColor;

void main ()
{
    // texture color
    vec4 colorTexture = vec4 (texture2D (xModelTexture, fragTexCoord.xy).xyz, 1.0);
    // normal
    vec3 normal = normalize (fragNormal.xyz);
    // light direction
    vec3 lightDirection = normalize (xLightDirection.xyz);

    // diffuse light intensity
    float diffuse = clamp (dot (normal, -lightDirection), 0.0, 1.0);
    float diffuseReverse = clamp (dot (normal, lightDirection), 0.0, 1.0);

    // eye direction
    vec3 eyeDirection = normalize (fragEyeDirection.xyz);
    // reflected vector
    vec3 reflect = normalize (reflect (lightDirection, normal));

    // specular light intensity
    float shininess = 20.0;
    float specular = pow (clamp (dot (reflect, eyeDirection), 0.0, 1.0), shininess);
    vec4 white = vec4 (1.0);

    // final color
    if (fragIsLightingEnabled > 0) {
        vec4 color = clamp (colorTexture * clamp (white * 0.2 + white * diffuse + white * diffuseReverse * 0.2, -1.0, 1.0) + specular, 0.0, 1.0);
        color.w = fragAlpha;
        fragColor = color;
    }
    else {
        vec4 color = colorTexture;
        color.w = fragAlpha;
        fragColor = color;
    }
}

#monogame EndShader ()

#monogame BeginShader (stage=vertex; constantBuffers=[])
#monogame Attribute (name=vertexPosition; usage=Position; index=0)
#monogame Attribute (name=vertexNormal; usage=Normal; index=0)
#monogame Attribute (name=vertexTexCoord; usage=TextureCoordinate; index=0)
#monogame Attribute (name=instanceWorld; usage=TextureCoordinate; index=[1, 2, 3, 4])
#monogame Attribute (name=instanceWorldInverseTranspose; usage=TextureCoordinate; index=[5, 6, 7, 8])
#monogame Attribute (name=instanceAlpha; usage=TextureCoordinate; index=9)
#monogame Attribute (name=instanceIsLightingEnabled; usage=TextureCoordinate; index=10)
#version 130

uniform mat4 xView;
uniform mat4 xProjection;
uniform vec4 xCameraPosition;

in vec4 vertexPosition;
in vec4 vertexNormal;
in vec4 vertexTexCoord;
in mat4 instanceWorld;
in mat4 instanceWorldInverseTranspose;
in float instanceAlpha;
in float instanceIsLightingEnabled;

out vec4 fragNormal;
out vec4 fragTexCoord;
out vec4 fragEyeDirection;
out float fragAlpha;
out float fragIsLightingEnabled;

void main ()
{
    mat4 world = transpose (instanceWorld);
    mat4 worldInverseTranspose = transpose (instanceWorldInverseTranspose);
    
    gl_Position = vertexPosition * world * xView * xProjection;
    fragNormal = normalize (vec4 ((vertexNormal * worldInverseTranspose).xyz, 0));
    fragTexCoord.xy = vertexTexCoord.xy;
    fragEyeDirection = normalize (vec4 (xCameraPosition.xyz, 1.0) - vertexPosition * world);
    fragAlpha = instanceAlpha;
    fragIsLightingEnabled = instanceIsLightingEnabled;
}

#monogame EndShader ()

#monogame EffectPass (name=Pass1; vertexShader=1; pixelShader=0)
#monogame EffectTechnique (name=Textured)

";
    }
}
