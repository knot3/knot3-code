using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Core;
using Microsoft.Xna.Framework;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot3.Framework.Development;

namespace Knot3.Framework.Effects
{
    [ExcludeFromCodeCoverageAttribute]
    public class HardwareInstancingEffect : RenderEffect
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
            VertexElement[] instanceStreamElements = new VertexElement[4];
            instanceStreamElements [0] = new VertexElement (0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1);
            instanceStreamElements [1] = new VertexElement (sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2);
            instanceStreamElements [2] = new VertexElement (sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3);
            instanceStreamElements [3] = new VertexElement (sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4);
            //instanceStreamElements [4] = new VertexElement (sizeof(float) * 16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 5);
            return new VertexDeclaration (instanceStreamElements);
        }

        public override void DrawModel (GameModel model, GameTime time)
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
        }

        private struct InstanceInfo
        {
            public Matrix WorldMatrix;
        }

        private class InstancedPrimitive
        {
            public Primitive Primitive;
            public World World;
            public Texture2D Texture;
            public InstanceInfo[] Instances;
            public int InstanceCount;

            public int InstanceCapacity { get { return Instances.Length; } }
        }

        private class InstancedBuffer
        {
            public VertexBuffer InstanceBuffer;
            public int InstanceCount;

            public int InstanceCapacity { get { return InstanceBuffer.VertexCount; } }
        }

        private Dictionary<string, InstancedPrimitive> cachePrimitivesInstances = new Dictionary<string, InstancedPrimitive> ();
        private Dictionary<string, InstancedBuffer> cachePrimitivesBuffers = new Dictionary<string, InstancedBuffer> ();

        public override void DrawPrimitive (GamePrimitive primitive, GameTime time)
        {
            int texture = GetTextureHashCode (primitive);
            string key = primitive.GetType ().Name + texture.GetHashCode ();
            InstancedPrimitive instances;
            if (!cachePrimitivesInstances.ContainsKey (key)) {
                cachePrimitivesInstances [key] = instances = new InstancedPrimitive () {
                    Primitive = primitive.Primitive,
                    World = primitive.World,
                    Texture = GetTexture (primitive),
                    Instances = new InstanceInfo[100],
                    InstanceCount = 0
                };
            }
            else {
                instances = cachePrimitivesInstances [key];
            }
            if (instances.InstanceCount + 1 >= instances.Instances.Length) {
                Array.Resize (ref instances.Instances, instances.Instances.Length + 200);
            }
            InstanceInfo instanceInfo = new InstanceInfo { WorldMatrix = primitive.WorldMatrix };
            instances.Instances [instances.InstanceCount++] = instanceInfo;
        }

        private void DrawAllPrimitives (GameTime time)
        {
            foreach (string key in cachePrimitivesInstances.Keys) {
                Console.WriteLine ("DrawAllPrimitives: " + key + " (" + cachePrimitivesInstances.Count + ")");
                InstancedPrimitive primitive = cachePrimitivesInstances [key];

                // Setze den Viewport auf den der aktuellen Spielwelt
                Viewport original = screen.Viewport;
                screen.Viewport = primitive.World.Viewport;

                Camera camera = primitive.World.Camera;
                //effect.Parameters ["World"].SetValue (primitive.WorldMatrix * camera.WorldMatrix);
                effect.Parameters ["View"].SetValue (camera.ViewMatrix);
                effect.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
                //effect.Parameters ["WorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (primitive.WorldMatrix * camera.WorldMatrix)));

                effect.Parameters ["ModelTexture"].SetValue (primitive.Texture);

                InstancedBuffer buffer;
                if (!cachePrimitivesBuffers.ContainsKey (key)) {
                    buffer = cachePrimitivesBuffers [key] = new InstancedBuffer ();
                    buffer.InstanceBuffer = new VertexBuffer (screen.GraphicsDevice, instanceVertexDeclaration, primitive.InstanceCapacity, BufferUsage.WriteOnly);
                    buffer.InstanceCount = 0;
                }
                else {
                    buffer = cachePrimitivesBuffers [key];
                }

                if (buffer.InstanceCapacity < primitive.InstanceCapacity) {
                    buffer.InstanceBuffer.Dispose ();
                    buffer.InstanceBuffer = new VertexBuffer (screen.GraphicsDevice, instanceVertexDeclaration, primitive.InstanceCapacity, BufferUsage.WriteOnly);
                }
                if (buffer.InstanceCount != primitive.InstanceCount) {
                    buffer.InstanceBuffer.SetData (primitive.Instances);
                    buffer.InstanceCount = primitive.InstanceCount;
                }

                primitive.Primitive.DrawInstances (effect: effect, instanceBuffer: ref buffer.InstanceBuffer, instanceCount: primitive.InstanceCount);

                // Setze den Viewport wieder auf den ganzen Screen
                screen.Viewport = original;

                primitive.InstanceCount = 0;
            }
        }

        protected override void DrawRenderTarget (GameTime time)
        {
            Profiler.ProfileDelegate ["Instancing"] = () => {
                DrawAllPrimitives (time);
            };

            base.DrawRenderTarget (time);
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
#monogame EffectParameter(name=View; class=Matrix; type=Single; rows=4; columns=4)
#monogame ConstantBuffer(name=View; sizeInBytes=64; parameters=[0]; offsets=[0])

#monogame EffectParameter(name=Projection; class=Matrix; type=Single; rows=4; columns=4)
#monogame ConstantBuffer(name=Projection; sizeInBytes=64; parameters=[1]; offsets=[0])

#monogame EffectParameter(name=ModelTexture; class=Object; type=Texture2D; semantic=; rows=0; columns=0; elements=[]; structMembers=[])


#monogame BeginShader(stage=pixel; constantBuffers=[])
#monogame Sampler(name=sampler0; type=Sampler2D; textureSlot=0; samplerSlot=0; parameter=2)
#version 130


uniform sampler2D sampler0;
in vec4 vTexCoord1;

void main()
{
    vec4 color = texture2D(sampler0, vTexCoord1.xy);
    color.w = 1.0;
    gl_FragColor = color;
}


#monogame EndShader()

#monogame BeginShader(stage=vertex; constantBuffers=[0, 1])
#monogame Attribute(name=inputPosition; usage=Position; index=0; format=0)
#monogame Attribute(name=inputNormal; usage=Normal; index=0; format=0)
#monogame Attribute(name=inputTexCoord; usage=TextureCoordinate; index=0; format=0)
#monogame Attribute(name=instanceWorld0; usage=TextureCoordinate; index=1; format=0)
#monogame Attribute(name=instanceWorld1; usage=TextureCoordinate; index=2; format=0)
#monogame Attribute(name=instanceWorld2; usage=TextureCoordinate; index=3; format=0)
#monogame Attribute(name=instanceWorld3; usage=TextureCoordinate; index=4; format=0)
#version 130


uniform vec4 View[4];
uniform vec4 Projection[4];
uniform vec4 posFixup;

in vec4 inputPosition;
in vec4 inputNormal;
in vec4 inputTexCoord;
out vec4 outputNormal;
out vec4 vTexCoord1;
in vec4 instanceWorld0;
in vec4 instanceWorld1;
in vec4 instanceWorld2;
in vec4 instanceWorld3;

void main()
{
    mat4 world = transpose(mat4(instanceWorld0, instanceWorld1, instanceWorld2, instanceWorld3));
    mat4 view = mat4(View[0], View[1], View[2], View[3]);
    mat4 proj = mat4(Projection[0], Projection[1], Projection[2], Projection[3]);
    mat4 worldInverseTranspose = transpose(inverse(world));
    
    gl_Position = inputPosition * world * view * proj;
    
    outputNormal.xyz = normalize(inputNormal * worldInverseTranspose).xyz;
    
    vTexCoord1.xy = inputTexCoord.xy;
    
    // https://github.com/flibitijibibo/MonoGame/blob/e9f61e3efbae6f11ebbf45012e7c692c8d0ee529/MonoGame.Framework/Graphics/GraphicsDevice.cs#L1209
    gl_Position.y = gl_Position.y * posFixup.y;
    gl_Position.xy += posFixup.zw * gl_Position.ww;
}


#monogame EndShader()

#monogame EffectPass(name=Pass1; vertexShader=1; pixelShader=0)
#monogame EffectTechnique(name=Textured)


";
    }
}

