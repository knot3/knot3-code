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

using Knot3.Core;
using Knot3.Data;
using Knot3.GameObjects;
using Knot3.Input;
using Knot3.RenderEffects;
using Knot3.Utilities;
using Knot3.Widgets;

#endregion

namespace Knot3.Screens
{
	/// <summary>
	/// Der Spielzustand, der den Ladebildschirm für Challenges darstellt.
	/// </summary>
	public sealed class ChallengeStartScreen : MenuScreen
	{
		#region Properties

		/// <summary>
		/// Das Menü, das die Spielstände enthält.
		/// </summary>
		private Menu savegameMenu;
		private Button backButton;
		// Der Titel des Screens
		private TextItem title;
		// Spielstand-Loader
		private SavegameLoader<Challenge, ChallengeMetaData> loader;
		// Preview
		private TextItem infoTitle;
		private Menu challengeInfo;
		private World previewWorld;
		private Challenge previewChallenge;
		private KnotRenderer previewRenderer;
		private Border previewBorder;
		private KnotInputHandler previewInput;
		private ModelMouseHandler previewMouseHandler;
		private Button startButton;

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt eine neue Instanz eines ChallengeStartScreen-Objekts und
		/// initialisiert diese mit einem Knot3Game-Objekt.
		/// </summary>
		public ChallengeStartScreen (Knot3Game game)
		: base (game)
		{
			savegameMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			savegameMenu.Bounds.Position = new ScreenPoint (this, 0.100f, 0.180f);
			savegameMenu.Bounds.Size = new ScreenPoint (this, 0.300f, 0.720f);
			savegameMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			savegameMenu.ItemAlignX = HorizontalAlignment.Left;
			savegameMenu.ItemAlignY = VerticalAlignment.Center;

			/*			lines.AddPoints (
						   0, 50,

						    30, 970,
						    170, 895,
						    270, 970,
						    970, 50,
						    1000
						);*/

			lines.AddPoints (.000f, .050f, .030f, .970f, .630f, .895f, .730f, .970f, .770f, .895f, .870f, .970f, .970f, .050f, 1.000f);

			title = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Start Challenge");
			title.Bounds.Position = new ScreenPoint (this, 0.100f, 0.050f);
			title.Bounds.Size = new ScreenPoint (this, 0.900f, 0.050f);
			title.ForegroundColorFunc = (s) => Color.White;

			infoTitle = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: "Challenge Info:");
			infoTitle.Bounds.Position = new ScreenPoint (this, 0.45f, 0.62f);
			infoTitle.Bounds.Size = new ScreenPoint (this, 0.900f, 0.050f);
			infoTitle.ForegroundColorFunc = (s) => Color.White;

			challengeInfo = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			challengeInfo.Bounds.Position = new ScreenPoint (this, 0.47f, 0.70f);
			challengeInfo.Bounds.Size = new ScreenPoint (this, 0.300f, 0.500f);
			challengeInfo.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			challengeInfo.ItemAlignX = HorizontalAlignment.Left;
			challengeInfo.ItemAlignY = VerticalAlignment.Center;

			// Erstelle einen Parser für das Dateiformat
			ChallengeFileIO fileFormat = new ChallengeFileIO ();
			// Erstelle einen Spielstand-Loader
			loader = new SavegameLoader<Challenge, ChallengeMetaData> (fileFormat, "index-challenges");

			// Preview
			Bounds previewBounds = new Bounds (this, 0.45f, 0.1f, 0.48f, 0.5f);
			previewWorld = new World (
			    screen: this,
			    drawIndex: DisplayLayer.ScreenUI + DisplayLayer.GameWorld,
			    bounds: previewBounds
			);
			previewRenderer = new KnotRenderer (screen: this, position: Vector3.Zero);
			previewWorld.Add (previewRenderer);
			previewBorder = new Border (
			    screen: this,
			    drawOrder: DisplayLayer.GameWorld,
			    bounds: previewBounds,
			    lineWidth: 2,
			    padding: 0
			);
			previewInput = new KnotInputHandler (screen: this, world: previewWorld);
			previewMouseHandler = new ModelMouseHandler (screen: this, world: previewWorld);

			backButton = new Button (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: "Back",
			    onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(s is ChallengeStartScreen)).ElementAt (0)
			);
			backButton.AddKey (Keys.Escape);

			backButton.SetCoordinates (left: 0.770f, top: 0.910f, right: 0.870f, bottom: 0.960f);
			backButton.AlignX = HorizontalAlignment.Center;
			Action<GameTime> loadSelectedChallenge = (time) => {
				NextScreen = new ChallengeModeScreen (
				    game: Game,
				    challenge: loader.FileFormat.Load (previewChallenge.MetaData.Filename)
				);
			};
			startButton = new Button (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: "Start",
			    onClick: loadSelectedChallenge
			);
			startButton.IsVisible = false;
			startButton.AddKey (Keys.Enter);
			startButton.SetCoordinates (left: 0.630f, top: 0.910f, right: 0.730f, bottom: 0.960f);

			startButton.AlignX = HorizontalAlignment.Center;
		}

		#endregion

		#region Methods

		private void UpdateFiles ()
		{
			// Leere das Spielstand-Menü
			savegameMenu.Clear ();

			// Suche nach Spielständen
			loader.FindSavegames (AddSavegameToList);
		}

		/// <summary>
		/// Diese Methode wird für jede gefundene Spielstanddatei aufgerufen
		/// </summary>
		private void AddSavegameToList (string filename, ChallengeMetaData meta)
		{
			// Erstelle eine Lamdafunktion, die beim Auswählen des Menüeintrags ausgeführt wird
			Action<GameTime> nullAction = (time) => {
			};
			Action<GameTime> LoadFile = (time) => {
				if (previewChallenge == null || previewChallenge.MetaData != meta) {
					RemoveGameComponents (time, challengeInfo);
					challengeInfo.Clear ();

					previewChallenge = loader.FileFormat.Load (filename);
					previewRenderer.Knot = previewChallenge.Target;
					previewWorld.Camera.ResetCamera ();
					startButton.IsVisible = true;

					MenuEntry count = new MenuEntry (
					    screen: this,
					    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
					    name: "Knot Count: " + meta.Target.CountEdges,
					    onClick: nullAction
					);
					count.Selectable = false;
					count.Enabled = false;
					challengeInfo.Add (count);
					MenuEntry avgtime = new MenuEntry (
					    screen: this,
					    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
					    name: "Avg Time: " + meta.FormatedAvgTime,
					    onClick: nullAction
					);
					avgtime.Selectable = false;
					avgtime.Enabled = false;
					challengeInfo.Add (avgtime);
					AddGameComponents (time, challengeInfo);
				}
			};

			// Finde den Namen der Challenge
			string name = meta.Name.Length > 0 ? meta.Name : filename;

			// Erstelle den Menüeintrag
			MenuEntry button = new MenuEntry (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: name,
			    onClick: LoadFile
			);
			button.SelectedColorBackground = Design.WidgetForeground;
			button.SelectedColorForeground = Design.WidgetBackground;
			savegameMenu.Add (button);
		}

		/// <summary>
		/// Fügt das Menü mit den Spielständen in die Spielkomponentenliste ein.
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			UpdateFiles ();
			base.Entered (previousScreen, time);
			AddGameComponents (time, savegameMenu, title, previewWorld, previewBorder, previewInput, previewMouseHandler, backButton, startButton, infoTitle);
		}

		#endregion
	}
}
