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
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;

#endregion

namespace Knot3.Game.Widgets
{
	[ExcludeFromCodeCoverageAttribute]
	public class ColorPickDialog : Dialog
	{
		#region Properties

		/// <summary>
		/// Die ausgewählte Farbe.
		/// </summary>
		public Color SelectedColor { get; private set; }

		private ColorPicker colorPicker;

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues ConfirmDialog-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angaben zur Zeichenreihenfolge, einer Zeichenkette für den Titel und für den eingeblendeten Text Pflicht.
		/// [base=screen, drawOrder, title, text]
		/// </summary>
		public ColorPickDialog (IGameScreen screen, DisplayLayer drawOrder, Color selectedColor)
		: base (screen, drawOrder, "Select a color")
		{
			// Die ausgewählte Farbe
			SelectedColor = selectedColor;

			// Der Titel-Text ist mittig ausgerichtet
			AlignX = HorizontalAlignment.Center;

			// Der Colorpicker
			colorPicker = new ColorPicker (Screen, Index + DisplayLayer.MenuItem, selectedColor);
			colorPicker.Bounds.Position = ContentBounds.Position;
			colorPicker.ColorSelected += OnColorSelected;
			//TODO
			//RelativeContentSize = colorPicker.RelativeSize ();

			// Diese Tasten werden akzeptiert
			ValidKeys.AddRange (new Keys[] { Keys.Escape });
		}

		#endregion

		#region Methods

		/// <summary>
		///
		/// </summary>
		public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			if (keyEvent == KeyEvent.KeyDown) {
				if (key.Contains (Keys.Escape)) {
					Close (time);
				}
			}
			base.OnKeyEvent (key, keyEvent, time);
		}

		private void OnColorSelected (Color obj, GameTime time)
		{
			SelectedColor = colorPicker.SelectedColor;
			Close (time);
		}

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents (time)) {
				yield return component;
			}
			yield return colorPicker;
		}

		#endregion
	}
}
