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
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.Widgets
{
	/// <summary>
	/// Ein Widget, der eine Zeichenkette anzeigt.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public class TextBox : Widget
	{
		#region Properties

		// ein Spritebatch
		protected SpriteBatch spriteBatch;

		public string Text { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues TextItem-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angabe der Zeichenreihenfolge und der Zeichenkette, die angezeigt wird, für Pflicht.
		/// </summary>
		public TextBox (IGameScreen screen, DisplayLayer drawOrder, string text)
		: base (screen, drawOrder)
		{
			Text = text;
			State = WidgetState.None;
			spriteBatch = new SpriteBatch (screen.Device);
		}

		#endregion

		#region Methods

		[ExcludeFromCodeCoverageAttribute]
		public override void Draw (GameTime time)
		{
			base.Draw (time);

			if (IsVisible) {
				spriteBatch.Begin ();

				// zeichne den Hintergrund
				spriteBatch.DrawColoredRectangle (BackgroundColor, Bounds);

				// lade die Schrift
				SpriteFont font = Design.MenuFont (Screen);

				// zeichne die Schrift
				Color foreground = ForegroundColor * (IsEnabled ? 1f : 0.5f);
				Vector2 scale = new Vector2 (0.1f, 0.1f) // Standard-Skalierung ist 10%
				/ new Vector2 (800, 600) // bei 800x600
				* Screen.Bounds.Size.AbsoluteVector; // Auf aktuelle Auflösung hochrechnen
				string wrappedText = parseText (Text, scale);
				spriteBatch.DrawScaledString (font, wrappedText, foreground, Bounds.Position, scale);

				spriteBatch.End ();
			}
		}

		private String parseText (String text, Vector2 scale)
		{
			// lade die Schrift
			SpriteFont font = Design.MenuFont (Screen);
			// berechne die Skalierung der schrift
			//spriteBatch.DrawStringInRectangle (font, parseText (Text), foreground, Bounds, AlignX, AlignY);

			String line = String.Empty;
			String returnString = String.Empty;
			String[] wordArray = Regex.Split (text, @"(?<=[.,; !])");

			foreach (String word in wordArray) {
				if ((font.MeasureString (line + word) * scale).X > Bounds.Rectangle.Width) {
					returnString = returnString + line + '\n';
					line = String.Empty;
				}

				line = line + word;
			}

			return returnString + line;
		}

		#endregion
	}
}
