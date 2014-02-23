#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using Knot3.Framework.Storage;

#endregion

#region Using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.GameObjects;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.RenderEffects
{
	/// <summary>
	/// Eine abstrakte Klasse, die eine Implementierung von IRenderEffect darstellt.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public abstract class RenderEffect : IRenderEffect
	{
		#region Properties

		/// <summary>
		/// Das Rendertarget, in das zwischen dem Aufruf der Begin ()- und der End ()-Methode gezeichnet wird,
		/// weil es in Begin () als primäres Rendertarget des XNA-Frameworks gesetzt wird.
		/// </summary>
		public RenderTarget2D RenderTarget { get; private set; }

		/// <summary>
		/// Der Spielzustand, in dem der Effekt verwendet wird.
		/// </summary>
		protected IGameScreen screen { get; set; }

		/// <summary>
		/// Ein Spritestapel (s. Glossar oder http://msdn.microsoft.com/en-us/library/bb203919.aspx), der verwendet wird, um das Rendertarget dieses Rendereffekts auf das übergeordnete Rendertarget zu zeichnen.
		/// </summary>
		protected SpriteBatch spriteBatch { get; set; }

		protected float Supersampling { get { return Config.Default ["video", "Supersamples", 1]; } }

		public bool SelectiveRendering { get; set; }

		#endregion

		#region Constructors

		public RenderEffect (IGameScreen screen)
		{
			this.screen = screen;
			spriteBatch = new SpriteBatch (screen.Device);
			screen.Game.FullScreenChanged += () => renderTargets.Clear ();
			SelectiveRendering = true;
		}

		#endregion

		#region Methods

		/// <summary>
		/// In der Methode Begin () wird das aktuell von XNA genutzte Rendertarget auf einem Stack gesichert
		/// und das Rendertarget des Effekts wird als aktuelles Rendertarget gesetzt.
		/// </summary>
		public void Begin (GameTime time)
		{
			if (screen.CurrentRenderEffects.CurrentEffect == this) {
				throw new InvalidOperationException ("Begin () can be called only once on " + this + "!");
			}

			RenderTarget = CurrentRenderTarget;
			screen.CurrentRenderEffects.Push (this);
			screen.Device.Clear (Color.Transparent);

			//spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			//spriteBatch.Draw (TextureHelper.Create (screen.Device, screen.Viewport.Width, screen.Viewport.Height, background),
			//                  Vector2.Zero, Color.White);
			//spriteBatch.End ();

			// set the stencil screen
			screen.Device.DepthStencilState = DepthStencilState.Default;
			// Setting the other screens isn't really necessary but good form
			screen.Device.BlendState = BlendState.Opaque;
			screen.Device.RasterizerState = RasterizerState.CullCounterClockwise;
			screen.Device.SamplerStates [0] = SamplerState.LinearWrap;
		}

		/// <summary>
		/// Das auf dem Stack gesicherte, vorher genutzte Rendertarget wird wiederhergestellt und
		/// das Rendertarget dieses Rendereffekts wird, unter Umständen in Unterklassen verändert,
		/// auf dieses ubergeordnete Rendertarget gezeichnet.
		/// </summary>
		public virtual void End (GameTime time)
		{
			screen.CurrentRenderEffects.Pop ();

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
			Viewport original = screen.Viewport;
			screen.Viewport = model.World.Viewport;

			foreach (ModelMesh mesh in model.Model.Meshes) {
				foreach (ModelMeshPart part in mesh.MeshParts) {
					if (part.Effect is BasicEffect) {
						ModifyBasicEffect (part.Effect as BasicEffect, model);
					}
				}
			}

			foreach (ModelMesh mesh in model.Model.Meshes) {
				mesh.Draw ();
			}

			// Setze den Viewport wieder auf den ganzen Screen
			screen.Viewport = original;
		}

		protected void ModifyBasicEffect (BasicEffect effect, GameModel model)
		{
			// lighting
			if (screen.InputManager.KeyHeldDown (Keys.L)) {
				effect.LightingEnabled = false;
			}
			else {
				effect.EnableDefaultLighting ();  // Beleuchtung aktivieren
			}

			// matrices
			effect.World = model.WorldMatrix * model.World.Camera.WorldMatrix;
			effect.View = model.World.Camera.ViewMatrix;
			effect.Projection = model.World.Camera.ProjectionMatrix;

			// colors
			if (!model.Coloring.IsTransparent) {
				effect.DiffuseColor = model.Coloring.MixedColor.ToVector3 ();
			}

			//effect.TextureEnabled = true;
			//effect.Texture = TextureHelper.CreateGradient (screen.Device, model.BaseColor, Color.White.Mix (_Color.Black, 0.2f));

			effect.Alpha = model.Coloring.Alpha;
			effect.FogEnabled = false;
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
		protected virtual void DrawRenderTarget (GameTime GameTime)
		{
			spriteBatch.Draw (
			    RenderTarget,
			    new Vector2 (screen.Viewport.X, screen.Viewport.Y),
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

		#endregion

		#region RenderTarget Cache

		private Dictionary<Point,Dictionary<Rectangle,Dictionary<float, RenderTarget2D>>> renderTargets
		    = new Dictionary<Point,Dictionary<Rectangle,Dictionary<float, RenderTarget2D>>> ();

		public RenderTarget2D CurrentRenderTarget
		{
			get {
				PresentationParameters pp = screen.Device.PresentationParameters;
				Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
				Rectangle viewport = new Rectangle (screen.Viewport.X, screen.Viewport.Y,
				                                    screen.Viewport.Width, screen.Viewport.Height);
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
						    screen.Device, (int)(viewport.Width * Supersampling), (int)(viewport.Height * Supersampling),
						    false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.PreserveContents
						);
						break;
					}
					catch (NotSupportedException ex) {
						Log.Debug (ex);
						if (Config.Default ["video", "Supersamples", 1] > 1) {
							Config.Default ["video", "Supersamples", 1] *= 0.8f;
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

		#endregion
	}
}
