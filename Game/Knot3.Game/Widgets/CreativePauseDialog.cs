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
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
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
	/// <summary>
	/// Pausiert ein Spieler im Creative- oder Challenge-Modus das Spiel,
	/// wird dieser Dialog über anderen Spielkomponenten angezeigt.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class CreativePauseDialog : Dialog
	{
		#region Properties

		/// <summary>
		/// Das Menü, das verschiedene Schaltflächen enthält.
		/// </summary>
		private Menu pauseMenu;
		private Knot knot;

		#endregion

		#region Constructors

		/// <summary>
		///
		/// </summary>
		public CreativePauseDialog (IGameScreen screen, DisplayLayer drawOrder, Knot knot)
		: base (screen, drawOrder, "Pause")
		{
			this.knot = knot;

			// Der Titel-Text ist mittig ausgerichtet
			AlignX = HorizontalAlignment.Center;
			Bounds.Size = new ScreenPoint (screen, 0.3f, 0.31f);
			// Erstelle das Pause-Menü
			pauseMenu = new Menu (Screen, Index + DisplayLayer.Menu);
			pauseMenu.Bounds = ContentBounds;
			pauseMenu.ItemAlignX = HorizontalAlignment.Left;
			pauseMenu.ItemAlignY = VerticalAlignment.Center;

			MenuEntry settingsButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Settings",
			onClick: (time) => {
				Close (time);
				Screen.NextScreen = new SettingsScreen (Screen.Game);
			}
			);
			MenuEntry backButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Back to Game",
			onClick: (time) => {
				Close (time);
			}
			);
			MenuEntry saveButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Save",
			onClick: (time) => {
				Close (time);
				KnotSave (time);
			}
			);
			MenuEntry saveAsButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Save As",
			onClick: (time) => {
				Close (time);
				KnotSaveAs (time);
			}
			);
			MenuEntry saveExitButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Save and Exit",
			onClick: (time) => {
				Close (time);
				KnotSave (time);
				Screen.NextScreen = new StartScreen (Screen.Game);
			}
			);
			MenuEntry discardExitButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Discard Changes and Exit",
			onClick: (time) => {
				Close (time);
				Screen.NextScreen = new StartScreen (Screen.Game);
			}
			);
			backButton.AddKey (Keys.Escape);

			pauseMenu.Add (settingsButton);
			pauseMenu.Add (backButton);
			pauseMenu.Add (saveButton);
			pauseMenu.Add (saveAsButton);
			pauseMenu.Add (saveExitButton);
			pauseMenu.Add (discardExitButton);
		}

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents (time)) {
				yield return component;
			}
			yield return pauseMenu;
		}

		private void KnotSave (GameTime time)
		{
			try {
				knot.Save ();
			}
			catch (IOException) {
				KnotSaveAs (time);
			}
		}

		private void KnotSaveAs (GameTime time)
		{
			TextInputDialog saveDialog = new TextInputDialog (
			    screen: Screen,
			    drawOrder: DisplayLayer.Dialog,
			    title: "Save Knot",
			    text: "Name:",
			    inputText: knot.Name != null ? knot.Name : String.Empty
			);
			saveDialog.NoCloseEmpty = true;
			saveDialog.NoWhiteSpace = true;
			saveDialog.Text = "Press Enter to save the Knot.";
			Screen.AddGameComponents (null, saveDialog);
			saveDialog.Close += (t) => {
				try {
					knot.Name = saveDialog.InputText;
					knot.Save ();
				}
				catch (IOException ex) {
					ErrorDialog errorDialog = new ErrorDialog (
					    screen: Screen,
					    drawOrder: DisplayLayer.Dialog * 2,
					    message: "Error in Knot.Save (): " + ex.ToString ()
					);
					Screen.AddGameComponents (null, errorDialog);
				}
			};
		}

		#endregion
	}
}
