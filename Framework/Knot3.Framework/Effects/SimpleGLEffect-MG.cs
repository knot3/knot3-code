using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Core;
using Microsoft.Xna.Framework;
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
        
        public override void DrawPrimitive (GamePrimitive primitive, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = screen.Viewport;
            screen.Viewport = primitive.World.Viewport;

            Camera camera = primitive.World.Camera;
            effect.Parameters ["World"].SetValue (primitive.WorldMatrix * camera.WorldMatrix);
            effect.Parameters ["View"].SetValue (camera.ViewMatrix);
            effect.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
            effect.Parameters ["WorldInverseTranspose"].SetValue (Matrix.Transpose (Matrix.Invert (primitive.WorldMatrix * camera.WorldMatrix)));

            effect.Parameters ["ModelTexture"].SetValue (GetTexture (primitive));
            
            primitive.Primitive.Draw (effect: effect);

            // Setze den Viewport wieder auf den ganzen Screen
            screen.Viewport = original;
        }

        private Texture2D GetTexture (IGameObject obj)
        {
            if (obj is ITexturedObject && (obj as ITexturedObject).Texture != null) {
                return (obj as ITexturedObject).Texture;
            }
            else if (obj is IColoredObject) {
                ModelColoring coloring = (obj as IColoredObject).Coloring;
                if (coloring is GradientColor) {
                    return ContentLoader.CreateGradient (screen.GraphicsDevice, coloring as GradientColor);
                }
                else {
                    return ContentLoader.CreateTexture (screen.GraphicsDevice, coloring.MixedColor);
                }
            }
            else {
                return ContentLoader.CreateTexture (screen.GraphicsDevice, Color.CornflowerBlue);
            }
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
#monogame EffectParameter(name=World; class=Matrix; type=Single; rows=4; columns=4)
#monogame ConstantBuffer(name=World; sizeInBytes=64; parameters=[0]; offsets=[0])

#monogame EffectParameter(name=View; class=Matrix; type=Single; rows=4; columns=4)
#monogame ConstantBuffer(name=View; sizeInBytes=64; parameters=[1]; offsets=[0])

#monogame EffectParameter(name=Projection; class=Matrix; type=Single; rows=4; columns=4)
#monogame ConstantBuffer(name=Projection; sizeInBytes=64; parameters=[2]; offsets=[0])

#monogame EffectParameter(name=WorldInverseTranspose; class=Matrix; type=Single; rows=4; columns=4)
#monogame ConstantBuffer(name=WorldInverseTranspose; sizeInBytes=64; parameters=[3]; offsets=[0])

#monogame EffectParameter(name=ModelTexture; class=Object; type=Texture2D; semantic=; rows=0; columns=0; elements=[]; structMembers=[])


#monogame BeginShader(stage=pixel; constantBuffers=[])
#monogame Sampler(name=sampler0; type=Sampler2D; textureSlot=0; samplerSlot=0; parameter=4)
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

#monogame BeginShader(stage=vertex; constantBuffers=[0,1,2,3])
#monogame Attribute(name=inputPosition; usage=Position; index=0; format=0)
#monogame Attribute(name=inputNormal; usage=Normal; index=0; format=0)
#monogame Attribute(name=inputTexCoord; usage=TextureCoordinate; index=0; format=0)
#version 130


uniform vec4 World[4];
uniform vec4 View[4];
uniform vec4 Projection[4];
uniform vec4 WorldInverseTranspose[4];
uniform vec4 posFixup;

in vec4 inputPosition;
in vec4 inputNormal;
in vec4 inputTexCoord;
out vec4 outputNormal;
out vec4 vTexCoord1;

void main()
{
    mat4 world = mat4(World[0], World[1], World[2], World[3]);
    mat4 view = mat4(View[0], View[1], View[2], View[3]);
    mat4 proj = mat4(Projection[0], Projection[1], Projection[2], Projection[3]);
    mat4 worldInverseTranspose = mat4(WorldInverseTranspose[0], WorldInverseTranspose[1], WorldInverseTranspose[2], WorldInverseTranspose[3]);
    
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

