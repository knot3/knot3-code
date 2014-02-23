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
using Knot3.Framework.Math;
using Knot3.Framework.Storage;

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
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Screens
{
	/// <summary>
	/// Der Spielzustand, der die Profil-Einstellungen darstellt.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public class ProfileSettingsScreen : SettingsScreen
	{
		#region Properties

		/// <summary>
		/// Das vertikale Menü wo die Einstellungen anzeigt. Hier nimmt der Spieler Einstellungen vor.
		/// </summary>
		private Menu settingsMenu { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines ProfileSettingsScreen-Objekts und initialisiert dieses mit einem Knot3Game-Objekt.
		/// </summary>
		public ProfileSettingsScreen (GameClass game)
		: base (game)
		{
			MenuName = "Profile";

			settingsMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			settingsMenu.Bounds.Position = new ScreenPoint (this, 0.400f, 0.180f);
			settingsMenu.Bounds.Size = new ScreenPoint (this, 0.500f, 0.720f);
			settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			settingsMenu.ItemAlignX = HorizontalAlignment.Left;
			settingsMenu.ItemAlignY = VerticalAlignment.Center;

			InputItem playerNameInput = new InputItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Player Name:",
			    inputText: Config.Default["profile", "name", "Player"]
			);
			playerNameInput.OnValueSubmitted += () => {
				Config.Default["profile", "name", String.Empty] = playerNameInput.InputText;
			};

			settingsMenu.Add (playerNameInput);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public override void Update (GameTime time)
		{
		}

		/// <summary>
		/// Fügt das Menü mit den Einstellungen in die Spielkomponentenliste ein.
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			base.Entered (previousScreen, time);
			AddGameComponents (time, settingsMenu);
		}

		#endregion
	}
}
