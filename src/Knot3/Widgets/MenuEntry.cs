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
using Knot3.Data;
using Knot3.Development;
using Knot3.GameObjects;
using Knot3.Input;
using Knot3.RenderEffects;
using Knot3.Screens;

#endregion

namespace Knot3.Widgets
{
	/// <summary>
	/// Eine Schaltfläche, der eine Zeichenkette anzeigt und auf einen Linksklick reagiert.
	/// </summary>
	public class MenuEntry : MenuItem
	{
		#region Properties

		/// <summary>
		/// Die Aktion, die ausgeführt wird, wenn der Spieler auf die Schaltfläche klickt.
		/// </summary>
		public Action<GameTime> OnClick { get; set; }

		/// <summary>
		/// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
		/// </summary>
		public override float NameWidth
		{
			get { return 1.00f; }
			set { throw new ArgumentException ("You can't change the NameWidth of a MenuButton!"); }
		}

		/// <summary>
		/// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
		/// </summary>
		public override float ValueWidth
		{
			get { return 0.00f; }
			set { throw new ArgumentException ("You can't change the ValueWidth of a MenuButton!"); }
		}

		public bool Selectable
		{
			get;
			set;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues MenuButton-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angabe der Zeichenreihenfolge, einer Zeichenkette für den Namen der Schaltfläche
		/// und der Aktion, welche bei einem Klick ausgeführt wird Pflicht.
		/// </summary>
		public MenuEntry (IGameScreen screen, DisplayLayer drawOrder, string name, Action<GameTime> onClick)
		: base (screen, drawOrder, name)
		{
			Selectable = true;
			OnClick = onClick;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Reaktionen auf einen Linksklick.
		/// </summary>
		public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
		{
			base.OnLeftClick (position, state, time);
			if (Selectable) {
				State = State.Selected;

				if (Menu != null) {
					foreach (MenuItem item in Menu) {
						Log.Debug ("State: ", item.State);
						if (item is MenuEntry && item !=this) {
							item.State = State.None;
						}
					}
				}
			}

			OnClick (time);
		}

		/// <summary>
		/// Reaktionen auf Tasteneingaben.
		/// </summary>
		public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			// Log.Debug ("OnKeyEvent: ", key[0]);
			if (keyEvent == KeyEvent.KeyDown) {
				OnClick (time);
			}
		}

		public override void SetHovered (bool isHovered, GameTime time)
		{
			if (State != State.Selected && Enabled) {
				base.SetHovered (isHovered, time);
			}
		}

		public void AddKey (Keys key)
		{
			if (!ValidKeys.Contains (key)) {
				ValidKeys.Add (key);
			}
		}

		#endregion
	}
}
