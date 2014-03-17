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

namespace Knot3.Framework.Effects
{
    [ExcludeFromCodeCoverageAttribute]
    public class NoLightingHardwareInstancingEffect : RenderEffect
    {
        Effect effect;
        VertexDeclaration instanceVertexDeclaration;

        public NoLightingHardwareInstancingEffect (IScreen screen)
        : base (screen)
        {
            //Effect hitest = screen.LoadEffect ("hitest");
            //System.IO.File.WriteAllText (SystemInfo.RelativeBaseDirectory + "hitest.glfx_gen", hitest.EffectCode);

            effect = new Effect (screen.GraphicsDevice, SHADER_CODE, GetType ().Name);
            instanceVertexDeclaration = GenerateInstanceVertexDeclaration ();
        }

        private VertexDeclaration GenerateInstanceVertexDeclaration ()
        {
            VertexElement[] instanceStreamElements = new VertexElement [8];
            instanceStreamElements [0] = new VertexElement (0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1);
            instanceStreamElements [1] = new VertexElement (sizeof (float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2);
            instanceStreamElements [2] = new VertexElement (sizeof (float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3);
            instanceStreamElements [3] = new VertexElement (sizeof (float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4);
            instanceStreamElements [4] = new VertexElement (sizeof (float) * 16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5);
            instanceStreamElements [5] = new VertexElement (sizeof (float) * 20, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6);
            instanceStreamElements [6] = new VertexElement (sizeof (float) * 24, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 7);
            instanceStreamElements [7] = new VertexElement (sizeof (float) * 28, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 8);
            //instanceStreamElements [4] = new VertexElement (sizeof (float) * 16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 5);
            return new VertexDeclaration (instanceStreamElements);
        }

        /*public override void DrawModel (GameModel model, GameTime time)
        {
            return;
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = screen.Viewport;
            screen.Viewport = model.World.Viewport;

            Camera camera = model.World.Camera;
            effect.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
            effect.Parameters ["View"].SetValue (camera.ViewMatrix);
            effect.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
            effect.Parameters ["WorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (model.WorldMatrix * camera.WorldMatrix)));

            effect.Parameters ["ModelTexture"].SetValue (GetTexture (model));

            foreach (ModelMesh mesh in model.Model.Meshes) {
                mesh.Draw ();
            }

            // Setze den Viewport wieder auf den ganzen Screen
            screen.Viewport = original;
        }*/

        /*public override void RemapModel (Model model)
        {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = effect;
                }
            }
        }*/

        private struct InstanceInfo {
            public Matrix WorldMatrix;
            public Matrix TransposeInverseWorldMatrix;
        }

        private class InstancedPrimitive
        {
            public Primitive Primitive;
            public World World;
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
            int texture = GetTextureHashCode (primitive);
            string key = primitive.GetType ().Name + primitive.Primitive.GetType ().Name + texture.GetHashCode ();
            InstancedPrimitive instancedPrimitive;
            if (!cacheInstancedPrimitives.ContainsKey (key)) {
                cacheInstancedPrimitives [key] = instancedPrimitive = new InstancedPrimitive () {
                    Primitive = primitive.Primitive,
                    World = primitive.World,
                    Texture = GetTexture (primitive),
                    Instances = new InstanceInfo [100],
                    InstanceCount = 0,
                    InstanceUniqueHash = 0
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
                TransposeInverseWorldMatrix = primitive.WorldMatrixInverseTranspose * primitive.World.Camera.WorldMatrix
            };
            instancedPrimitive.Instances [instancedPrimitive.InstanceCount++] = instanceInfo;
            instancedPrimitive.InstanceUniqueHash += primitive.Position.LengthSquared ();
        }

        private void DrawAllPrimitives (GameTime time)
        {
            foreach (string key in cacheInstancedPrimitives.Keys) {
                InstancedPrimitive instancedPrimitive = cacheInstancedPrimitives [key];

                // Setze den Viewport auf den der aktuellen Spielwelt
                Viewport original = screen.Viewport;
                screen.Viewport = instancedPrimitive.World.Viewport;

                Camera camera = instancedPrimitive.World.Camera;
                //effect.Parameters ["World"].SetValue (primitive.WorldMatrix * camera.WorldMatrix);
                effect.Parameters ["xView"].SetValue (camera.ViewMatrix);
                effect.Parameters ["xProjection"].SetValue (camera.ProjectionMatrix);
                //effect.Parameters ["WorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (primitive.WorldMatrix * camera.WorldMatrix)));

                effect.Parameters ["xModelTexture"].SetValue (instancedPrimitive.Texture);

                InstancedBuffer buffer;
                if (!cachePrimitivesBuffers.ContainsKey (key)) {
                    buffer = cachePrimitivesBuffers [key] = new InstancedBuffer ();
                    buffer.InstanceBuffer = new VertexBuffer (screen.GraphicsDevice, instanceVertexDeclaration, instancedPrimitive.InstanceCapacity, BufferUsage.WriteOnly);
                    buffer.InstanceCount = 0;
                }
                else {
                    buffer = cachePrimitivesBuffers [key];
                }

                if (buffer.InstanceCapacity < instancedPrimitive.InstanceCapacity) {
                    buffer.InstanceBuffer.Dispose ();
                    buffer.InstanceBuffer = new VertexBuffer (screen.GraphicsDevice, instanceVertexDeclaration, instancedPrimitive.InstanceCapacity, BufferUsage.WriteOnly);
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

        //#monogame EffectParameter (name=xView; class=Matrix; type=Single; rows=4; columns=4)
        //#monogame ConstantBuffer (name=xView; sizeInBytes=64; parameters=[0]; offsets=[0])

        //#monogame EffectParameter (name=xProjection; class=Matrix; type=Single; rows=4; columns=4)
        //#monogame ConstantBuffer (name=xProjection; sizeInBytes=64; parameters=[1]; offsets=[0])

        private readonly string SHADER_CODE = @"
#monogame BeginShader (stage=pixel; constantBuffers=[0])
#monogame Attribute (name=fragNormal; usage=Normal; index=0)
#monogame Attribute (name=fragTexCoord; usage=TextureCoordinate; index=0)
#version 130

uniform sampler2D xModelTexture;
in vec4 fragNormal;
in vec4 fragTexCoord;
out vec4 fragColor;

void main ()
{
    vec4 color = texture2D (xModelTexture, fragTexCoord.xy);
    color.w = 1.0;
    fragColor = color;
}

#monogame EndShader ()

#monogame BeginShader (stage=vertex; constantBuffers=[])
#monogame Attribute (name=vertexPosition; usage=Position; index=0)
#monogame Attribute (name=vertexNormal; usage=Normal; index=0)
#monogame Attribute (name=vertexTexCoord; usage=TextureCoordinate; index=0)
#monogame Attribute (name=instanceWorld; usage=TextureCoordinate; index=[1, 2, 3, 4])
#monogame Attribute (name=instanceWorldInverseTranspose; usage=TextureCoordinate; index=[5, 6, 7, 8])
#version 130

uniform mat4 xView;
uniform mat4 xProjection;

in vec4 vertexPosition;
in vec4 vertexNormal;
in vec4 vertexTexCoord;
in mat4 instanceWorld;
in mat4 instanceWorldInverseTranspose;

out vec4 fragNormal;
out vec4 fragTexCoord;

void main ()
{
    mat4 world = transpose (instanceWorld);
    mat4 worldInverseTranspose = transpose (instanceWorldInverseTranspose);
    
    gl_Position = vertexPosition * world * xView * xProjection;
    fragNormal.xyz = normalize (vertexNormal * worldInverseTranspose).xyz;
    fragTexCoord.xy = vertexTexCoord.xy;
}

#monogame EndShader ()

#monogame EffectPass (name=Pass1; vertexShader=1; pixelShader=0)
#monogame EffectTechnique (name=Textured)

";
    }
}
