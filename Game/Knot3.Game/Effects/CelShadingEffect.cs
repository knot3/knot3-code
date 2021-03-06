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
    /// <summary>
    /// Ein Cel-Shading-Effekt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class CelShadingEffect : RenderEffect
    {
        Effect celShader;
        // Toon shader effect
        Texture2D celMap;
        // Texture map for cell shading
        Vector4 lightDirection;
        // Light source for toon shader
        Effect outlineShader;
        // Outline shader effect
        float outlineThickness = 1.0f;
        // current outline thickness
        float outlineThreshold = 0.2f;
        // current edge detection threshold
        /// <summary>
        /// Erstellt einen neuen Cel-Shading-Effekt für den angegebenen IGameScreen.
        /// </summary>
        public CelShadingEffect (IScreen screen)
        : base (screen)
        {
            /* Set our light direction for the cel-shader
             */
            lightDirection = new Vector4 (0.0f, 0.0f, 1.0f, 1.0f);

            /* Load and initialize the cel-shader effect
             */
            celShader = screen.LoadEffect ("CelShader");
            RegisterEffect (celShader);
            celShader.Parameters ["LightDirection"].SetValue (lightDirection);
            celMap = screen.LoadTexture ("CelMap");
            if (celMap == null) {
                celMap = ContentLoader.CreateTexture (screen.GraphicsDevice, Color.White);
            }
            celShader.Parameters ["Color"].SetValue (Color.Green.ToVector4 ());
            celShader.Parameters ["CelMap"].SetValue (celMap);

            /* Load and initialize the outline shader effect
             */
            outlineShader = screen.LoadEffect ("OutlineShader");
            RegisterEffect (outlineShader);
            outlineShader.Parameters ["Thickness"].SetValue (outlineThickness);
            outlineShader.Parameters ["Threshold"].SetValue (outlineThreshold);
            outlineShader.Parameters ["ScreenSize"].SetValue (new Vector2 (screen.Viewport.Bounds.Width, screen.Viewport.Bounds.Height));
        }

        /// <summary>
        /// Zeichnet das Rendertarget.
        /// </summary>
        protected override void DrawRenderTarget (GameTime GameTime)
        {
            spriteBatch.End ();
            spriteBatch.Begin (SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, outlineShader);

            spriteBatch.Draw (
                RenderTarget,
                new Vector2 (Screen.Viewport.X, Screen.Viewport.Y),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                Vector2.One / Supersampling,
                SpriteEffects.None,
                1f
            );
        }

        /// <summary>
        /// Zeichnet das Spielmodell model mit dem Cel-Shading-Effekt.
        /// Eine Anwendung des NVIDIA-Toon-Shaders.
        /// </summary>
        public override void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = model.World.Viewport;

            Camera camera = model.World.Camera;
            lightDirection = new Vector4 (-Vector3.Cross (Vector3.Normalize (camera.PositionToTargetDirection), camera.UpVector), 1);
            celShader.Parameters ["LightDirection"].SetValue (lightDirection);
            celShader.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
            celShader.Parameters ["InverseWorld"].SetValue (Matrix.Invert (model.WorldMatrix * camera.WorldMatrix));
            celShader.Parameters ["View"].SetValue (camera.ViewMatrix);
            celShader.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
            celShader.CurrentTechnique = celShader.Techniques ["ToonShader"];

            if (!model.Coloring.IsTransparent) {
                Color = model.Coloring.MixedColor;
            }

            foreach (ModelMesh mesh in model.Model.Meshes) {
                mesh.Draw ();
            }

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        /// <summary>
        /// Zeichnet das Spielmodell model mit dem Cel-Shading-Effekt.
        /// Eine Anwendung des NVIDIA-Toon-Shaders.
        /// </summary>
        public override void DrawPrimitive (GamePrimitive model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = model.World.Viewport;

            Camera camera = model.World.Camera;
            lightDirection = new Vector4 (-Vector3.Cross (Vector3.Normalize (camera.PositionToTargetDirection), camera.UpVector), 1);
            celShader.Parameters ["LightDirection"].SetValue (lightDirection);
            celShader.Parameters ["World"].SetValue (model.WorldMatrix * camera.WorldMatrix);
            celShader.Parameters ["InverseWorld"].SetValue (Matrix.Invert (model.WorldMatrix * camera.WorldMatrix));
            celShader.Parameters ["View"].SetValue (camera.ViewMatrix);
            celShader.Parameters ["Projection"].SetValue (camera.ProjectionMatrix);
            celShader.CurrentTechnique = celShader.Techniques ["ToonShader"];

            if (!model.Coloring.IsTransparent) {
                Color = model.Coloring.MixedColor;
            }

            model.Primitive.Draw (effect: celShader);

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        /// <summary>
        /// Weist dem 3D-Modell den Cel-Shader zu.
        /// </summary>
        public override void RemapModel (Model model)
        {
            foreach (ModelMesh mesh in model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    part.Effect = celShader;
                }
            }
        }

        public Color Color
        {
            get {
                return new Color (celShader.Parameters ["Color"].GetValueVector4 ());
            }
            set {
                celShader.Parameters ["Color"].SetValue (value.ToVector4 ());
            }
        }
    }
}
