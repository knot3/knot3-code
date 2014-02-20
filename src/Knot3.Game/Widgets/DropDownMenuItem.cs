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
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Development;
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;

#endregion

namespace Knot3.Game.Widgets
{
	/// <summary>
	/// Ein Menüeintrag, der den ausgewählten Wert anzeigt und bei einem Linksklick ein Dropdown-Menü zur Auswahl eines neuen Wertes ein- oder ausblendet.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class DropDownMenuItem : MenuItem
	{
		#region Properties

		/// <summary>
		/// Das Dropdown-Menü, das ein- und ausgeblendet werden kann.
		/// </summary>
		private Menu dropdown;
		private Border dropdownBorder;
		private InputItem currentValue;

		public override bool IsVisible
		{
			get { return base.IsVisible; }
			set {
				base.IsVisible = value;
				if (currentValue != null) {
					currentValue.IsVisible = value;
				}
			}
		}

		public Action<GameTime> ValueChanged = (time) => {};

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues ConfirmDialog-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem ist die Angabe der Zeichenreihenfolge Pflicht.
		/// </summary>
		public DropDownMenuItem (IGameScreen screen, DisplayLayer drawOrder, string text)
		: base (screen, drawOrder, String.Empty)
		{
			dropdown = new Menu (screen: screen, drawOrder: Index + DisplayLayer.Menu);
			dropdown.Bounds.Position = ValueBounds.Position;
			dropdown.Bounds.Size = new ScreenPoint (Screen, () => ValueBounds.Size.OnlyX + ValueBounds.Size.OnlyY * 10);
			dropdown.Bounds.Padding = new ScreenPoint (screen, 0.010f, 0.010f);
			dropdown.ItemForegroundColor = (i) => Menu.ItemForegroundColor (i);
			dropdown.ItemBackgroundColor = (i) => Design.WidgetBackground;
			dropdown.ItemAlignX = HorizontalAlignment.Left;
			dropdown.ItemAlignY = VerticalAlignment.Center;
			dropdown.IsVisible = false;
			dropdownBorder = new Border (
			    screen: screen,
			    drawOrder: Index + DisplayLayer.Menu,
			    widget: dropdown,
			    lineWidth: 2,
			    padding: 2
			);

			currentValue = new InputItem (screen: screen, drawOrder: Index, text: text, inputText: String.Empty);
			currentValue.Bounds = Bounds;
			currentValue.ForegroundColorFunc = (s) => ForegroundColor;
			currentValue.BackgroundColorFunc = (s) => Color.Transparent;
			currentValue.IsVisible = IsVisible;
			currentValue.IsMouseClickEventEnabled = false;

			ValidKeys.Add (Keys.Escape);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Fügt Einträge in das Dropdown-Menü ein, die auf Einstellungsoptionen basieren.
		/// </summary>
		public void AddEntries (DistinctOptionInfo option)
		{
			dropdown.Clear ();
			foreach (string _value in option.DisplayValidValues.Keys) {
				string value = _value; // create a copy for the action
				Action<GameTime> onSelected = (time) => {
					Log.Debug ("OnClick: ", value);
					option.Value = option.DisplayValidValues [value];
					currentValue.InputText = value;
					dropdown.IsVisible = false;
					ValueChanged (time);
				};
				MenuEntry button = new MenuEntry (
				    screen: Screen,
				    drawOrder: Index + DisplayLayer.MenuItem,
				    name: value,
				    onClick: onSelected
				);
				button.Selectable = false;
				dropdown.Add (button);
			}
			currentValue.InputText = option.DisplayValue;
		}

		/// <summary>
		/// Fügt Einträge in das Dropdown-Menü ein, die nicht auf Einstellungsoptionen basieren.
		/// </summary>
		public void AddEntries (DropDownEntry enties)
		{
			throw new System.NotImplementedException ();
		}

		public override void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
		{
			if (key.Contains (Keys.Escape)) {
				Menu.Collapse ();
				dropdown.IsVisible = false;
			}
		}

		/// <summary>
		/// Reaktionen auf einen Linksklick.
		/// </summary>
		public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
		{
			onClick ();
		}

		private void onClick ()
		{
			bool newValue = !dropdown.IsVisible;
			Menu.Collapse ();
			dropdown.IsVisible = newValue;
		}

		public override void Collapse ()
		{
			dropdown.IsVisible = false;
		}

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents (time)) {
				yield return component;
			}
			yield return dropdown;
			yield return dropdownBorder;
			yield return currentValue;
		}

		[ExcludeFromCodeCoverageAttribute]
		public override void Draw (GameTime time)
		{
			base.Draw (time);
		}

		#endregion
	}
}
