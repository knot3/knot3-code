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
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Development
{
	[ExcludeFromCodeCoverageAttribute]
	public sealed class Overlay : DrawableGameScreenComponent
	{
		// graphics-related classes
		private SpriteBatch spriteBatch;
		private BasicEffect effect;

		private World World { get; set; }
		// fonts
		private SpriteFont font;
		private float scale;
		private int lineHeight;
		private DebugModel debugModel;
		private bool debugModelAdded;

		public Overlay (IGameScreen screen, World world)
		: base (screen, DisplayLayer.Overlay)
		{
			// game world
			World = world;

			// create a new SpriteBatch, which can be used to draw textures
			effect = new BasicEffect (screen.Device);
			spriteBatch = new SpriteBatch (screen.Device);
			effect.VertexColorEnabled = true;
			effect.World = Matrix.CreateFromYawPitchRoll (0, 0, 0);
			if (Config.Default ["video", "camera-overlay", true]) {
				DebugModelInfo info = new DebugModelInfo ("sphere");
				debugModel = new DebugModel (screen, info);
				world.Add (debugModel);
			}

			// load fonts
			try {
				font = Screen.LoadFont ("font-overlay");
			}
			catch (ContentLoadException ex) {
				font = null;
				Log.Debug (ex.Message);
			}
		}

		[ExcludeFromCodeCoverageAttribute]
		public override void Draw (GameTime time)
		{
			if (Config.Default ["video", "debug-coordinates", false]) {
				DrawCoordinates (time);
			}
			if (Config.Default ["video", "camera-overlay", true]) {
				DrawOverlay (time);
			}
			if (Config.Default ["video", "fps-overlay", true]) {
				DrawFPS (time);
			}
			if (Config.Default ["video", "profiler-overlay", true]) {
				DrawProfiler (time);
			}
			base.Draw (time);
		}

		[ExcludeFromCodeCoverageAttribute]
		public override void Update (GameTime time)
		{
			scale = Math.Max (0.7f, (float)Screen.Device.PresentationParameters.BackBufferWidth / 1366f);
			lineHeight = (int)(20 * scale);

			if (Config.Default ["video", "camera-overlay", true]) {
				if (!debugModelAdded) {
					World.Add (debugModel);
					debugModelAdded = true;
				}
			}
			else {
				if (debugModelAdded) {
					World.Remove (debugModel);
					debugModelAdded = false;
				}
			}
			UpdateFPS (time);
			base.Update (time);
		}

		private void DrawCoordinates (GameTime time)
		{
			int length = 2000;
			var vertices = new VertexPositionColor[6];
			vertices [0].Position = new Vector3 (-length, 0, 0);
			vertices [0].Color = Color.Green;
			vertices [1].Position = new Vector3 (+length, 0, 0);
			vertices [1].Color = Color.Green;
			vertices [2].Position = new Vector3 (0, -length, 0);
			vertices [2].Color = Color.Red;
			vertices [3].Position = new Vector3 (0, +length, 0);
			vertices [3].Color = Color.Red;
			vertices [4].Position = new Vector3 (0, 0, -length);
			vertices [4].Color = Color.Yellow;
			vertices [5].Position = new Vector3 (0, 0, +length);
			vertices [5].Color = Color.Yellow;

			effect.View = World.Camera.ViewMatrix;
			effect.Projection = World.Camera.ProjectionMatrix;

			effect.CurrentTechnique.Passes [0].Apply ();

			Screen.Device.DrawUserPrimitives (PrimitiveType.LineList, vertices, 0, 3, VertexPositionColor.VertexDeclaration);
		}

		private void DrawOverlay (GameTime time)
		{
			spriteBatch.Begin ();

			int height = lineHeight;
			int width1 = 20, width2 = 150, width3 = 210, width4 = 270;
			DrawString ("Rotation: ", width1, height, Color.White);
			float x, y, z;
			World.Camera.Rotation.ToDegrees (out x, out y, out z);
			DrawString (x, width2, height, Color.Green);
			DrawString (y, width3, height, Color.Red);
			DrawString (z, width4, height, Color.Yellow);
			height += lineHeight;
			DrawString ("Camera Position: ", width1, height, Color.White);
			DrawVectorCoordinates (World.Camera.Position, width2, width3, width4, height);
			height += lineHeight;
			DrawString ("Camera Target: ", width1, height, Color.White);
			DrawVectorCoordinates (World.Camera.Target, width2, width3, width4, height);
			height += lineHeight;
			DrawString ("Distance: ", width1, height, Color.White);
			DrawString (World.Camera.PositionToTargetDistance, width2, height, Color.White);
			height += lineHeight;
			DrawString ("Selected Object: ", width1, height, Color.White);
			if (World.SelectedObject != null) {
				Vector3 selectedObjectCenter = World.SelectedObject.Center ();
				DrawVectorCoordinates (selectedObjectCenter, width2, width3, width4, height);

				if (World.SelectedObject is PipeModel) {
					DrawString ("Pipe: ", width1, height, Color.White);
					PipeModel pipe = World.SelectedObject as PipeModel;
					height += lineHeight;
					string str = pipe.Info.Edge.Direction;
					if (pipe.Info.Knot != null) {
						str += "   #" + pipe.Info.Knot.ToList ().FindIndex (g => g == pipe.Info.Edge).ToString ();
					}
					DrawString (str, width2, height, Color.Yellow);
				}
				else {
					height += lineHeight;
				}
			}
			height += lineHeight;
			DrawString ("Distance: ", width1, height, Color.White);
			DrawString (World.SelectedObjectDistance, width2, height, Color.White);
			height += lineHeight;
			DrawString ("FoV: ", width1, height, Color.White);
			DrawString (World.Camera.FoV, width2, height, Color.White);
			height += lineHeight;

			spriteBatch.End ();
		}

		private void DrawVectorCoordinates (Vector3 vector, int width2, int width3, int width4, int height)
		{
			DrawString ((int)vector.X, width2, height, Color.Green);
			DrawString ((int)vector.Y, width3, height, Color.Red);
			DrawString ((int)vector.Z, width4, height, Color.Yellow);
		}

		private void DrawString (string str, int width, int height, Color color)
		{
			if (font != null) {
				try {
					spriteBatch.DrawString (font, str, new Vector2 (width, height) * Config.Default ["video", "Supersamples", 1], color, 0f, Vector2.Zero, scale * Config.Default ["video", "Supersamples", 1], SpriteEffects.None, 0f);
				}
				catch (ArgumentException exp) {
					Log.Debug (exp);
				}
				catch (InvalidOperationException exp) {
					Log.Debug (exp);
				}
			}
		}

		private void DrawString (float n, int width, int height, Color color)
		{
			DrawString (String.Empty + n.ToString (), width, height, color);
		}

		int _total_frames = 0;
		float _elapsed_time = 0.0f;
		int _fps = 0;

		private void UpdateFPS (GameTime time)
		{
			_elapsed_time += (float)time.ElapsedGameTime.TotalMilliseconds;

			if (_elapsed_time >= 1000.0f) {
				_fps = (int)(_total_frames * 1000.0f / _elapsed_time);
				_total_frames = 0;
				_elapsed_time = 0;
			}
		}

		private void DrawFPS (GameTime time)
		{
			_total_frames++;
			spriteBatch.Begin ();
			DrawString ("FPS: " + _fps.ToString (), (int)(Screen.Viewport.Width / Config.Default ["video", "Supersamples", 1]) - (int)(170 * scale), (int)(50 * scale), Color.White);
			spriteBatch.End ();
		}

		private void DrawProfiler (GameTime time)
		{
			spriteBatch.Begin ();
			int height = (int)(90 * scale);
			foreach (string name in Profiler.ProfilerMap.Keys) {
				DrawString (name + ": " + Profiler.ProfilerMap [name], (int)(Screen.Viewport.Width / Config.Default ["video", "Supersamples", 1]) - (int)(170 * scale), height, Color.White);
				height += lineHeight;
			}
			spriteBatch.End ();
		}
	}
}
