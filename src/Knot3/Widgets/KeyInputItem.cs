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
using Knot3.Utilities;

#endregion

namespace Knot3.Widgets
{
	/// <summary>
	/// Ein Menüeintrag, der einen Tastendruck entgegennimmt und in der enthaltenen Option als Zeichenkette speichert.
	/// </summary>
	public sealed class KeyInputItem : InputItem
	{
		#region Properties

		/// <summary>
		/// Die Option in einer Einstellungsdatei.
		/// </summary>
		private KeyOptionInfo option { get; set; }

		/// <summary>
		/// Wie viel Prozent der Name des Eintrags (auf der linken Seite) von der Breite des Eintrags einnehmen darf.
		/// </summary>
		public override float NameWidth
		{
			get { return Math.Min (0.70f, 1.0f - ValueWidth); }
			set { throw new ArgumentException ("You can't change the NameWidth of a KeyInputItem!"); }
		}

		/// <summary>
		/// Wie viel Prozent der Wert des Eintrags (auf der rechten Seite) von der Breite des Eintrags einnehmen darf.
		/// </summary>
		public override float ValueWidth
		{
			get { return 3 * Bounds.Size.Relative.Y / Bounds.Size.Relative.X; }
			set { throw new ArgumentException ("You can't change the ValueWidth of a KeyInputItem!"); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues CheckBoxItem-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angaben zur Zeichenreihenfolge und der Eingabeoption Pflicht.
		/// </summary>
		public KeyInputItem (IGameScreen screen, DisplayLayer drawOrder, string text, KeyOptionInfo option)
		: base (screen, drawOrder, text, (option as DistinctOptionInfo).Value)
		{
			this.option = option;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Speichert die aktuell gedrückte Taste in der Option.
		/// </summary>
		public override void OnKeyEvent (List<Keys> keys, KeyEvent keyEvent, GameTime time)
		{
			if (keys.Count > 0) {
				Keys key = keys [0];
				if (KnotInputHandler.CurrentKeyAssignment.Count == 0) {
					KnotInputHandler.ReadKeyAssignments ();
				}
				Log.Debug ("Trying to assign key: ", key);
				if (KnotInputHandler.CurrentKeyAssignment.ContainsKey (key) && option.Value != key) {
					Log.Debug ("Key ", key, " is already assigned to: ", KnotInputHandler.CurrentKeyAssignment [key]);
				}
				else {
					Log.Debug ("Key ", key, " => ", option.Name);
					option.Value = key;
					InputText = (option as DistinctOptionInfo).Value;
					IsInputEnabled = false;
					OnValueChanged ();
					OnValueSubmitted ();
					KnotInputHandler.ReadKeyAssignments ();
				}
			}
		}

		/// <summary>
		/// Reaktionen auf einen Linksklick.
		/// </summary>
		public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
		{
			if (IsInputEnabled) {
				ValidKeys.Clear ();
				IsInputEnabled = false;
			}
			else {
				ValidKeys.AddRange (typeof (Keys).ToEnumValues<Keys> ());
				IsInputEnabled = true;
				InputText = String.Empty;
			}
		}

		#endregion
	}
}
