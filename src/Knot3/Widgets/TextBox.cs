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
	/// <summary>
	/// Ein Widget, der eine Zeichenkette anzeigt.
	/// </summary>
	public class TextBox : MenuItem
	{
		#region Properties

		/// <summary>
		/// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
		/// </summary>
		public override float NameWidth
		{
			get { return 1.00f; }
			set { throw new ArgumentException ("You can't change the NameWidth of a TextItem!"); }
		}

		/// <summary>
		/// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
		/// </summary>
		public override float ValueWidth
		{
			get { return 0.00f; }
			set { throw new ArgumentException ("You can't change the ValueWidth of a TextItem!"); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues TextItem-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angabe der Zeichenreihenfolge und der Zeichenkette, die angezeigt wird, für Pflicht.
		/// </summary>
		public TextBox (IGameScreen screen, DisplayLayer drawOrder, string text)
		: base(screen, drawOrder, text)
		{
		}

		#endregion

		#region Methods

		public override void Draw (GameTime time)
		{
			if (IsVisible) {
				spriteBatch.Begin ();

				// zeichne den Hintergrund
				spriteBatch.DrawColoredRectangle (BackgroundColor, Bounds);

				// lade die Schrift
				SpriteFont font = Design.MenuFont (Screen);

				// zeichne die Schrift
				Color foreground = ForegroundColor * (IsEnabled ? 1f : 0.5f);
				spriteBatch.DrawStringInRectangle (font, parseText (Text), foreground, Bounds, AlignX, AlignY);

				spriteBatch.End ();
			}
		}

		private String parseText (String text)
		{
			// lade die Schrift
			SpriteFont font = Design.MenuFont (Screen);

			String line = String.Empty;
			String returnString = String.Empty;
			String[] wordArray = text.Split (' ');

			foreach (String word in wordArray) {
				if (font.MeasureString (line + word).Length () > Bounds.Rectangle.Width) {
					returnString = returnString + line + '\n';
					line = String.Empty;
				}

				line = line + word + ' ';
			}

			return returnString + line;
		}

		//Da TextItems werden nicht unterlegt um sie von Buttons abzugrenzen
		public override void SetHovered (bool hovered, GameTime time)
		{
			State = State.None;
		}

		#endregion
	}
}
