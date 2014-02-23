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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

#endregion

namespace Knot3.Framework.Development
{
	[ExcludeFromCodeCoverageAttribute]
	public class DebugBoundings : IGameObject
	{
		#region Properties

		private IGameScreen screen;

		public GameObjectInfo Info { get; private set; }

		public World World { get; set; }

		private VertexBuffer vertBuffer;
		private BasicEffect effect;
		private int sphereResolution;

		#endregion

		#region Constructors

		public DebugBoundings (IGameScreen screen, Vector3 position)
		{
			this.screen = screen;
			Info = new GameObjectInfo (position: position);

			sphereResolution = 40;
			effect = new BasicEffect (screen.Device);
			effect.LightingEnabled = false;
			effect.VertexColorEnabled = false;

			VertexPositionColor[] verts = new VertexPositionColor[(sphereResolution + 1) * 3];
			int index = 0;
			float step = MathHelper.TwoPi / (float)sphereResolution;
			for (float a = 0f; a <= MathHelper.TwoPi; a += step) {
				verts [index++] = new VertexPositionColor (
				    position: new Vector3 ((float)System.Math.Cos (a), (float)System.Math.Sin (a), 0f),
				    color: Color.White
				);
			}
			for (float a = 0f; a <= MathHelper.TwoPi; a += step) {
				verts [index++] = new VertexPositionColor (
				    position: new Vector3 ((float)System.Math.Cos (a), 0f, (float)System.Math.Sin (a)),
				    color: Color.White
				);
			}
			for (float a = 0f; a <= MathHelper.TwoPi; a += step) {
				verts [index++] = new VertexPositionColor (
				    position: new Vector3 (0f, (float)System.Math.Cos (a), (float)System.Math.Sin (a)),
				    color: Color.White
				);
			}
			vertBuffer = new VertexBuffer (screen.Device, typeof (VertexPositionColor), verts.Length, BufferUsage.None);
			vertBuffer.SetData (verts);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gibt den Ursprung des Knotens zurück.
		/// </summary>
		public Vector3 Center ()
		{
			return Info.Position;
		}

		[ExcludeFromCodeCoverageAttribute]
		public void Update (GameTime time)
		{
		}

		[ExcludeFromCodeCoverageAttribute]
		public void Draw (GameTime time)
		{
			if (!Info.IsVisible) {
				return;
			}

			// Setze den Viewport auf den der aktuellen Spielwelt
			Viewport original = screen.Viewport;
			screen.Viewport = World.Viewport;

			foreach (GameModel model in World.OfType<GameModel>()) {
				if (model.Info.IsVisible) {
					screen.Device.SetVertexBuffer (vertBuffer);

					foreach (BoundingSphere sphere in model.Bounds) {
						effect.World = Matrix.CreateScale (sphere.Radius) * Matrix.CreateTranslation (sphere.Center);
						effect.View = World.Camera.ViewMatrix;
						effect.Projection = World.Camera.ProjectionMatrix;
						effect.DiffuseColor = Color.White.ToVector3 ();

						foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
							pass.Apply ();

							screen.Device.DrawPrimitives (PrimitiveType.LineStrip, 0, sphereResolution);
							screen.Device.DrawPrimitives (PrimitiveType.LineStrip, sphereResolution + 1, sphereResolution);
							screen.Device.DrawPrimitives (PrimitiveType.LineStrip, (sphereResolution + 1) * 2, sphereResolution);
						}
					}
				}
			}

			// Setze den Viewport wieder auf den ganzen Screen
			screen.Viewport = original;
		}

		public GameObjectDistance Intersects (Ray ray)
		{
			return null;
		}

		#endregion
	}
}
