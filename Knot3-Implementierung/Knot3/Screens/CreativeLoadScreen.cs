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

namespace Knot3.Screens
{
	/// <summary>
	/// Der Spielzustand, der den Ladebildschirm für Knoten darstellt.
	/// </summary>
	public sealed class CreativeLoadScreen : MenuScreen
	{
		#region Properties

		/// <summary>
		/// Das Menü, das die Spielstände enthält.
		/// </summary>
		private VerticalMenu savegameMenu;
		private TextItem title;

		// Spielstand-Loader
		private SavegameLoader<Knot, KnotMetaData> loader;

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues CreativeLoadScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
		/// </summary>
		public CreativeLoadScreen (Knot3Game game)
		: base(game)
		{
			savegameMenu = new VerticalMenu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			savegameMenu.RelativePosition = () => new Vector2 (0.100f, 0.180f);
			savegameMenu.RelativeSize = () => new Vector2 (0.800f, 0.720f);
			savegameMenu.RelativePadding = () => new Vector2 (0.010f, 0.010f);
			savegameMenu.ItemForegroundColor = base.MenuItemForegroundColor;
			savegameMenu.ItemBackgroundColor = base.MenuItemBackgroundColor;
			savegameMenu.ItemAlignX = HorizontalAlignment.Left;
			savegameMenu.ItemAlignY = VerticalAlignment.Center;

			lines.AddPoints (0, 50, 30, 970, 970, 50, 1000);

			title = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, name: "Load Knot");
			title.RelativePosition = () => new Vector2 (0.100f, 0.050f);
			title.RelativeSize = () => new Vector2 (0.900f, 0.050f);
			title.ForegroundColor = () => Color.White;

			// Erstelle einen Parser für das Dateiformat
			KnotFileIO fileFormat = new KnotFileIO ();
			// Erstelle einen Spielstand-Loader
			loader = new SavegameLoader<Knot, KnotMetaData> (fileFormat, "index-knots");
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
		private void AddSavegameToList (string filename, KnotMetaData meta)
		{
			// Erstelle eine Lamdafunktion, die beim Auswählen des Menüeintrags ausgeführt wird
			Action<GameTime> LoadFile = (time) => {
				NextScreen = new CreativeModeScreen (game: Game, knot: loader.FileFormat.Load (filename));
			};

			// Finde den Namen des Knotens
			string name = meta.Name.Length > 0 ? meta.Name : filename;

			// Erstelle den Menüeintrag
			MenuButton button = new MenuButton (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    name: name,
			    onClick: LoadFile
			);
			savegameMenu.Add (button);
		}

		/// <summary>
		/// Fügt das Menü mit den Spielständen in die Spielkomponentenliste ein.
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			UpdateFiles ();
			base.Entered (previousScreen, time);
			AddGameComponents (time, savegameMenu, title);
		}

		#endregion
	}
}

