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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Effects
{
    /// <summary>
    /// Eine abstrakte Klasse, die eine Implementierung von IRenderEffect darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class RenderEffect : IRenderEffect
    {
        /// <summary>
        /// Das Rendertarget, in das zwischen dem Aufruf der Begin ()- und der End ()-Methode gezeichnet wird,
        /// weil es in Begin () als primäres Rendertarget des MonoGame-Frameworks gesetzt wird.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Der Spielzustand, in dem der Effekt verwendet wird.
        /// </summary>
        protected IScreen Screen { get; set; }

        /// <summary>
        /// Ein Spritestapel (s. Glossar oder http://msdn.microsoft.com/en-us/library/bb203919.aspx), der verwendet wird, um das Rendertarget dieses Rendereffekts auf das übergeordnete Rendertarget zu zeichnen.
        /// </summary>
        protected SpriteBatch spriteBatch { get; set; }

        protected float Supersampling { get { return Config.Default ["video", "Supersamples", 2]; } }

        public bool SelectiveRendering { get; set; }

        public RenderEffect (IScreen screen)
        {
            Screen = screen;
            spriteBatch = new SpriteBatch (Screen.GraphicsDevice);
            Screen.Game.FullScreenChanged += () => renderTargets.Clear ();
            SelectiveRendering = true;
            Screen.Game.Graphics.ApplyChanges ();
        }

        /// <summary>
        /// In der Methode Begin () wird das aktuell von MonoGame genutzte Rendertarget auf einem Stack gesichert
        /// und das Rendertarget des Effekts wird als aktuelles Rendertarget gesetzt.
        /// </summary>
        public void Begin (GameTime time)
        {
            if (Screen.CurrentRenderEffects.CurrentEffect == this) {
                throw new InvalidOperationException ("Begin () can be called only once on " + this + "!");
            }

            RenderTarget = CurrentRenderTarget;
            Screen.CurrentRenderEffects.Push (this);
            Screen.GraphicsDevice.Clear (Color.Transparent);

            //spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            //spriteBatch.Draw (TextureHelper.Create (Screen.Device, Screen.Viewport.Width, Screen.Viewport.Height, background),
            //                  Vector2.Zero, Color.White);
            //spriteBatch.End ();

            // set the stencil Screen
            Screen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Setting the other Screens isn't really necessary but good form
            Screen.GraphicsDevice.BlendState = BlendState.Opaque;
            Screen.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Screen.GraphicsDevice.SamplerStates [0] = SamplerState.LinearWrap;
        }

        protected virtual void BeforeEnd (GameTime time)
        {
        }

        /// <summary>
        /// Das auf dem Stack gesicherte, vorher genutzte Rendertarget wird wiederhergestellt und
        /// das Rendertarget dieses Rendereffekts wird, unter Umständen in Unterklassen verändert,
        /// auf dieses ubergeordnete Rendertarget gezeichnet.
        /// </summary>
        public virtual void End (GameTime time)
        {
            BeforeEnd (time);
            Screen.CurrentRenderEffects.Pop ();

            spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawRenderTarget (time);
            spriteBatch.End ();
        }

        /// <summary>
        /// Zeichnet das Spielmodell model mit diesem Rendereffekt.
        /// </summary>
        public virtual void DrawModel (GameModel model, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = model.World.Viewport;

            foreach (ModelMesh mesh in model.Model.Meshes) {
                foreach (ModelMeshPart part in mesh.MeshParts) {
                    if (part.Effect is BasicEffect) {
                        ModifyBasicEffect (effect: part.Effect as BasicEffect, model: model);
                    }
                }
            }

            foreach (ModelMesh mesh in model.Model.Meshes) {
                mesh.Draw ();
            }

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        private BasicEffect basicEffectForPrimitives;

        /// <summary>
        /// Zeichnet das Spielmodell model mit diesem Rendereffekt.
        /// </summary>
        public virtual void DrawPrimitive (GamePrimitive primitive, GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = primitive.World.Viewport;

            if (basicEffectForPrimitives == null) {
                basicEffectForPrimitives = new BasicEffect (Screen.GraphicsDevice);
                RegisterEffect (basicEffectForPrimitives);
            }

            ModifyBasicEffect (effect: basicEffectForPrimitives, primitive: primitive);
            primitive.Primitive.Draw (effect: basicEffectForPrimitives);

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        protected void ModifyBasicEffect (BasicEffect effect, IGameObject obj)
        {
            // lighting
            if (Screen.InputManager.KeyHeldDown (Keys.L)) {
                effect.LightingEnabled = false;
            }
            else if (!obj.IsLightingEnabled) {
                effect.LightingEnabled = false;
            }
            else {
                effect.EnableDefaultLighting ();
                effect.DirectionalLight0.Direction = obj.World.Camera.LightDirection;
                effect.DirectionalLight0.DiffuseColor = Color.White.ToVector3 ();
                effect.DirectionalLight0.Enabled = true;
                effect.DirectionalLight1.Direction = -obj.World.Camera.LightDirection;
                effect.DirectionalLight1.DiffuseColor = Color.White.ToVector3 ()/4;
                effect.DirectionalLight1.Enabled = true;
                effect.DirectionalLight2.Enabled = false;
                effect.PreferPerPixelLighting = true;
            }
            effect.FogEnabled = false;
            effect.Alpha = obj.Coloring.Alpha;

            // matrices
            effect.World = obj.WorldMatrix * obj.World.Camera.WorldMatrix;
            effect.View = obj.World.Camera.ViewMatrix;
            effect.Projection = obj.World.Camera.ProjectionMatrix;
        }

        protected void ModifyBasicEffect (BasicEffect effect, GameModel model)
        {
            ModifyBasicEffect (effect: effect, obj: model as IGameObject);

            // colors
            if (!model.Coloring.IsTransparent) {
                effect.DiffuseColor = model.Coloring.MixedColor.ToVector3 ();
            }
            effect.Alpha = model.Coloring.Alpha;
        }

        protected void ModifyBasicEffect (BasicEffect effect, GamePrimitive primitive)
        {
            ModifyBasicEffect (effect: effect, obj: primitive as IGameObject);

            effect.TextureEnabled = true;
            effect.Texture = GetTexture (primitive);
        }

        protected Texture2D GetTexture (IGameObject obj)
        {
            if (obj.Texture != null) {
                return obj.Texture;
            }
            else {
                ModelColoring coloring = obj.Coloring;
                if (coloring is GradientColor) {
                    return ContentLoader.CreateGradient (Screen.GraphicsDevice, coloring as GradientColor);
                }
                else {
                    return ContentLoader.CreateTexture (Screen.GraphicsDevice, coloring.MixedColor);
                }
            }
        }

        protected Matrix SkyViewMatrix (Matrix viewMatrix)
        {
            Matrix skyboxView = viewMatrix;
            skyboxView.M41 = 0;
            skyboxView.M42 = 0;
            skyboxView.M43 = 0;
            return skyboxView;
        }

        /// <summary>
        /// Beim Laden des Modells wird von der XNA-Content-Pipeline jedem ModelMeshPart ein Shader der Klasse
        /// BasicEffect zugewiesen. Für die Nutzung des Modells in diesem Rendereffekt kann jedem ModelMeshPart
        /// ein anderer Shader zugewiesen werden.
        /// </summary>
        public virtual void RemapModel (Model model)
        {
        }

        /// <summary>
        /// Zeichnet das Rendertarget.
        /// </summary>
        protected virtual void DrawRenderTarget (GameTime time)
        {
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

        public void DrawLastFrame (GameTime time)
        {
            spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            DrawRenderTarget (time);
            spriteBatch.End ();
        }

        private Dictionary<Point,Dictionary<Rectangle,Dictionary<float, RenderTarget2D>>> renderTargets
            = new Dictionary<Point,Dictionary<Rectangle,Dictionary<float, RenderTarget2D>>> ();

        public RenderTarget2D CurrentRenderTarget
        {
            get {
                PresentationParameters pp = Screen.GraphicsDevice.PresentationParameters;
                Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
                Rectangle viewport = new Rectangle (Screen.Viewport.X, Screen.Viewport.Y,
                                                    Screen.Viewport.Width, Screen.Viewport.Height);
                if (!renderTargets.ContainsKey (resolution)) {
                    renderTargets [resolution] = new Dictionary<Rectangle, Dictionary<float, RenderTarget2D>> ();
                }
                if (!renderTargets [resolution].ContainsKey (viewport)) {
                    renderTargets [resolution] [viewport] = new Dictionary<float, RenderTarget2D> ();
                }
                while (!renderTargets [resolution][viewport].ContainsKey (Supersampling)) {
                    try {
                        Log.Debug ("Supersampling=", Supersampling);
                        renderTargets [resolution] [viewport] [Supersampling] = new RenderTarget2D (
                            Screen.GraphicsDevice, (int)(viewport.Width * Supersampling), (int)(viewport.Height * Supersampling),
                            false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents
                        );
                        break;
                    }
                    catch (NotSupportedException ex) {
                        Log.Debug (ex);
                        if (Config.Default ["video", "Supersamples", 2] > 1) {
                            Config.Default ["video", "Supersamples", 2] *= 0.8f;
                        }
                        else {
                            throw;
                        }
                        continue;
                    }
                }
                return renderTargets [resolution] [viewport] [Supersampling];
            }
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        private List<Effect> effectsToDispose = new List<Effect>();

        protected void RegisterEffect (Effect effect)
        {
            effectsToDispose.Add (effect);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                foreach (Effect effect in effectsToDispose) {
                    effect.Dispose ();
                }
            }
        }

        ~RenderEffect ()
        {
            Dispose (false);
        }
    }
}
