using System;
using System.Collections;
using System.Collections.Generic;
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

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.KnotData;
using Knot3.Utilities;

namespace Knot3.Widgets
{
	public sealed class Lines : DrawableGameScreenComponent
	{
		private Texture2D texture;
		private SpriteBatch spriteBatch;

		public bool IsEnabled { get; set; }

		public bool IsVisible { get; set; }

		// die Punkte, zwischen denen die Linien gezeichnet werden sollen
		private List<Vector2> points;

		// die Dicke der Linien
		private int lineWidth;

		// die Farben der Linien
		public Color LineColor { get; private set; }

		public Color OutlineColor { get; private set; }

		public Lines (IGameScreen screen, DisplayLayer drawOrder, int lineWidth, Color lineColor, Color outlineColor)
		: base(screen, drawOrder)
		{
			this.lineWidth = lineWidth;
			points = new List<Vector2> ();
			spriteBatch = new SpriteBatch (screen.Device);
			texture = TextureHelper.Create (Screen.Device, Color.White);
			LineColor = lineColor;
			OutlineColor = outlineColor;
			IsEnabled = true;
			IsVisible = true;
		}

		public Lines (IGameScreen screen, DisplayLayer drawOrder, int lineWidth)
		: this(screen, drawOrder, lineWidth, Design.DefaultLineColor, Design.DefaultOutlineColor)
		{
		}

		public override void Draw (GameTime time)
		{
			if (IsVisible) {
				int scaledLineWidth = (int)new Vector2 (lineWidth, lineWidth).Scale (Screen.Viewport).X;

				if (points.Count >= 2) {
					Rectangle[] rects = new Rectangle[points.Count - 1];
					for (int i = 1; i < points.Count; ++i) {
						Vector2 nodeA = points [i - 1];
						Vector2 nodeB = points [i];
						if (nodeA.X == nodeB.X || nodeA.Y == nodeB.Y) {
							Vector2 direction = (nodeB - nodeA).PrimaryDirection ();
							Vector2 position = nodeA.Scale (Screen.Viewport);
							int length = (int)(nodeB - nodeA).Scale (Screen.Viewport).Length ();
							if (direction.X == 0 && direction.Y > 0) {
								rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X, position.Y, 0, length);
							}
							else if (direction.X == 0 && direction.Y < 0) {
								rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X, position.Y - length, 0, length);
							}
							else if (direction.Y == 0 && direction.X > 0) {
								rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X, position.Y, length, 0);
							}
							else if (direction.Y == 0 && direction.X < 0) {
								rects [i - 1] = VectorHelper.CreateRectangle (scaledLineWidth, position.X - length, position.Y, length, 0);
							}
						}
					}

					spriteBatch.Begin ();
					foreach (Rectangle inner in rects) {
						Rectangle outer = new Rectangle (inner.X - 1, inner.Y - 1, inner.Width + 2, inner.Height + 2);
						spriteBatch.Draw (texture, outer, Design.DefaultOutlineColor * (IsEnabled ? 1f : 0.5f));
					}
					foreach (Rectangle rect in rects) {
						spriteBatch.Draw (texture, rect, Design.DefaultLineColor * (IsEnabled ? 1f : 0.5f));
					}
					spriteBatch.End ();
				}
			}
		}

		public void AddPoints (float startX, float startY, params float[] xyxy)
		{
			Vector2 start = new Vector2 (startX, startY);
			points.Add (start);

			Vector2 current = start;
			for (int i = 0; i < xyxy.Length; ++i) {
				// this is a new X value
				if (i % 2 == 0) {
					current.X = xyxy [i];
				}
				// this is a new Y value
				else {
					current.Y = xyxy [i];
				}

				points.Add (current);
			}
		}

		public void Clear ()
		{
			points.Clear ();
		}
	}
}