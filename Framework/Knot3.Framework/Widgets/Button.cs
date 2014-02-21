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
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.Widgets
{
	/// <summary>
	/// Eine Schaltfläche, der eine Zeichenkette anzeigt und auf einen Linksklick reagiert.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class Button : Widget, IKeyEventListener, IMouseClickEventListener
	{
		#region Properties

		/// <summary>
		/// Die Aktion, die ausgeführt wird, wenn der Spieler auf die Schaltfläche klickt.
		/// </summary>
		public Action<GameTime> OnClick { get; set; }

		private string name;
		private SpriteBatch spriteBatch;

		public Bounds MouseClickBounds { get { return Bounds; } }

		public Action<bool, GameTime> Hovered = (isHovered, time) => {};

		public Texture2D BackgroundTexture { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues MenuButton-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angabe der Zeichenreihenfolge, einer Zeichenkette für den Namen der Schaltfläche
		/// und der Aktion, welche bei einem Klick ausgeführt wird Pflicht.
		/// </summary>
		public Button (IGameScreen screen, DisplayLayer drawOrder, string name, Action<GameTime> onClick)
		: base (screen, drawOrder)
		{
			this.name = name;
			OnClick = onClick;
			spriteBatch = new SpriteBatch (screen.Device);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reaktionen auf einen Linksklick.
		/// </summary>
		public void OnLeftClick (Vector2 position, ClickState state, GameTime time)
		{
			OnClick (time);
		}

		public void OnRightClick (Vector2 position, ClickState state, GameTime time)
		{
		}

		public void SetHovered (bool isHovered, GameTime time)
		{
			State = isHovered ? WidgetState.Hovered : WidgetState.None;
			Hovered (isHovered, time);
		}

		/// <summary>
		/// Reaktionen auf Tasteneingaben.
		/// </summary>
		public void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			Log.Debug ("OnKeyEvent: ", key [0]);
			if (keyEvent == KeyEvent.KeyDown) {
				OnClick (time);
			}
		}

		public void AddKey (Keys key)
		{
			if (!ValidKeys.Contains (key)) {
				ValidKeys.Add (key);
			}
		}

		[ExcludeFromCodeCoverageAttribute]
		public override void Draw (GameTime time)
		{
			base.Draw (time);

			if (IsVisible) {
				spriteBatch.Begin ();

				// zeichne den Hintergrund
				spriteBatch.DrawColoredRectangle (BackgroundColor, Bounds);

				if (BackgroundTexture != null) {
					spriteBatch.Draw (BackgroundTexture, Bounds, ForegroundColor * (IsEnabled ? 1f : 0.5f));
				}

				// lade die Schrift
				SpriteFont font = Design.MenuFont (Screen);

				// zeichne die Schrift
				Color foreground = ForegroundColor * (IsEnabled ? 1f : 0.5f);
				spriteBatch.DrawStringInRectangle (font, name, foreground, Bounds, AlignX, AlignY);

				spriteBatch.End ();
			}
		}

		#endregion
	}
}
