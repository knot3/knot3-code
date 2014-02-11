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
using Knot3.RenderEffects;
using Knot3.KnotData;
using Knot3.Widgets;
using Knot3.Utilities;
using Knot3.Development;

namespace Knot3.Screens
{
	/// <summary>
	///
	/// </summary>
	public class ChallengeCreateScreen : MenuScreen
	{
		#region Properties

		/// <summary>
		/// Das Menü, das die Spielstände enthält, die als Startknoten ausgewählt werden.
		/// </summary>
		private Menu startKnotMenu;
		/// <summary>
		/// Das Menü, das die Spielstände enthält, die als Zielknoten ausgewählt werden.
		/// </summary>
		private Menu targetKnotMenu;
		private TextItem title;
		private InputItem challengeName;
		private Button createButton;
		private Border createButtonBorder;
		// Zurück-Button.
		private Button backButton;
		// Spielstand-Loader
		private SavegameLoader<Knot, KnotMetaData> loader;
		private Knot selectedStartKnot;
		private Knot selectedTargetKnot;
		private Challenge selectedChallenge;

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines ChallengeCreateScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt game.
		/// </summary>
		public ChallengeCreateScreen (Knot3Game game)
		: base (game)
		{
			startKnotMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			startKnotMenu.Bounds.Position = new ScreenPoint (this, 0.100f, 0.180f);
			startKnotMenu.Bounds.Size = new ScreenPoint (this, 0.375f, 0.620f);

			targetKnotMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			targetKnotMenu.Bounds.Position = new ScreenPoint (this, 0.525f, 0.180f);
			targetKnotMenu.Bounds.Size = new ScreenPoint (this, 0.375f, 0.620f);

			challengeName = new InputItem (this, DisplayLayer.ScreenUI + DisplayLayer.MenuItem, "Name:", String.Empty);
			challengeName.Bounds.Position = new ScreenPoint (this, 0.100f, 0.860f);
			challengeName.Bounds.Size = new ScreenPoint (this, 0.375f, 0.040f);
			challengeName.OnValueChanged += () => TryConstructChallenge ();
			challengeName.NameWidth = 0.2f;
			challengeName.ValueWidth = 0.8f;

			createButton = new Button (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: "Create!",
			    onClick: OnCreateChallenge
			);
			createButton.Bounds.Position = new ScreenPoint (this, 0.525f, 0.8525f);
			createButton.Bounds.Size = new ScreenPoint (this, 0.125f, 0.050f);

			createButtonBorder = new Border (this, DisplayLayer.ScreenUI + DisplayLayer.MenuItem, createButton, 4, 4);
			createButton.AlignX = HorizontalAlignment.Center;

			startKnotMenu.Bounds.Padding = targetKnotMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			startKnotMenu.ItemAlignX = targetKnotMenu.ItemAlignX = HorizontalAlignment.Left;
			startKnotMenu.ItemAlignY = targetKnotMenu.ItemAlignY = VerticalAlignment.Center;

			lines.AddPoints (.000f, .050f,
			                 .030f, .970f,
			                 .770f, .895f,
			                 .870f, .970f,
			                 .970f, .050f,
			                 1.000f
			                );

			title = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, name: "Create Challenge");
			title.Bounds.Position = new ScreenPoint (this, 0.100f, 0.050f);
			title.Bounds.Size = new ScreenPoint (this, 0.900f, 0.050f);
			title.ForegroundColorFunc = (s) => Color.White;

			// Erstelle einen Parser für das Dateiformat
			KnotFileIO fileFormat = new KnotFileIO ();
			// Erstelle einen Spielstand-Loader
			loader = new SavegameLoader<Knot, KnotMetaData> (fileFormat, "index-knots");

			backButton = new Button (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: "Back",
			    onClick: (time) => NextScreen = Game.Screens.Where ((s) => !(s is ChallengeCreateScreen)).ElementAt (0)
			);
			backButton.AddKey (Keys.Escape);
			backButton.SetCoordinates (left: 0.770f, top: 0.910f, right: 0.870f, bottom: 0.960f);

			backButton.AlignX = HorizontalAlignment.Center;
		}

		#endregion

		#region Methods

		private void UpdateFiles ()
		{
			// Leere das Spielstand-Menü
			startKnotMenu.Clear ();
			targetKnotMenu.Clear ();

			// Suche nach Spielständen
			loader.FindSavegames (AddSavegameToList);
		}

		/// <summary>
		/// Diese Methode wird für jede gefundene Spielstanddatei aufgerufen
		/// </summary>
		private void AddSavegameToList (string filename, KnotMetaData meta)
		{
			// Finde den Namen des Knotens
			string name = meta.Name.Length > 0 ? meta.Name : filename;

			// Erstelle die Lamdafunktionen, die beim Auswählen des Menüeintrags ausgeführt werden
			Action<GameTime> SelectStartKnot = (time) => {
				selectedStartKnot = loader.FileFormat.Load (filename);

				TryConstructChallenge ();
			};

			// Erstelle die Lamdafunktionen, die beim Auswählen des Menüeintrags ausgeführt werden
			Action<GameTime> SelectTargetKnot = (time) => {
				selectedTargetKnot = loader.FileFormat.Load (filename);

				TryConstructChallenge ();
			};

			// Erstelle die Menüeinträge
			MenuEntry buttonStart = new MenuEntry (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: name,
			    onClick: SelectStartKnot
			);
			MenuEntry buttonTarget = new MenuEntry (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: name,
			    onClick: SelectTargetKnot
			);
			buttonStart.SelectedColorBackground = Design.WidgetForeground;
			buttonStart.SelectedColorForeground = Design.WidgetBackground;
			buttonTarget.SelectedColorBackground = Design.WidgetForeground;
			buttonTarget.SelectedColorForeground = Design.WidgetBackground;

			startKnotMenu.Add (buttonStart);
			targetKnotMenu.Add (buttonTarget);
		}

		/// <summary>
		/// Prüft, ob alle Werte vorhanden sind, um eine Challenge daraus zu erstellen.
		/// Das ist dann der Fall, wenn zwei Knoten selektiert sind und ein Name eingegeben wurde.
		/// </summary>
		public bool CanCreateChallenge
		{
			get {
				return selectedStartKnot != null && selectedTargetKnot != null
				       && selectedStartKnot.MetaData.Filename != selectedTargetKnot.MetaData.Filename
				       && challengeName.InputText.Length > 0
				       && (selectedStartKnot.MetaData.CountEdges > 4 || selectedTargetKnot.MetaData.CountEdges > 4);
			}
		}

		/// <summary>
		/// Versucht ein Challenge-Objekt zu erstellen.
		/// </summary>
		private bool TryConstructChallenge ()
		{
			bool can = createButton.IsEnabled = createButtonBorder.IsEnabled = CanCreateChallenge;

			if (can) {
				ChallengeMetaData challengeMeta = new ChallengeMetaData (
				    name: challengeName.InputText,
				    start: selectedStartKnot.MetaData,
				    target: selectedTargetKnot.MetaData,
				    filename: null,
				    format: new ChallengeFileIO (),
				    highscore: new List<KeyValuePair<string,int>> ()
				);
				selectedChallenge = new Challenge (
				    meta: challengeMeta,
				    start: selectedStartKnot,
				    target: selectedTargetKnot
				);
			}
			else {
				selectedChallenge = null;
			}

			return can;
		}

		/// <summary>
		/// Wird aufgerufen, wenn auf den Button zum Erstellen der Challenge gedrückt wird
		/// </summary>
		private void OnCreateChallenge (GameTime time)
		{
			if (TryConstructChallenge ()) {
				Log.Debug ("Save Challenge: ", selectedChallenge);
				try {
					selectedChallenge.Save ();
					NextScreen = new ChallengeStartScreen (Game);
				}
				catch (Exception ex) {
					Dialog errorDialog = new ErrorDialog (this, DisplayLayer.Dialog * 3, ex.ToString ());
					AddGameComponents (time, errorDialog);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			UpdateFiles ();
			base.Entered (previousScreen, time);
			AddGameComponents (time, startKnotMenu, targetKnotMenu, challengeName, createButton,
			                   createButtonBorder, title, backButton);
			TryConstructChallenge ();
		}

		#endregion
	}
}