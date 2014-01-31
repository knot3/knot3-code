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
using Knot3.Audio;

namespace Knot3.Screens
{
	/// <summary>
	/// Eine abstrakte Klasse, von der alle Spielzustände erben, die Menüs darstellen.
	/// </summary>
	public abstract class MenuScreen : GameScreen
	{
		#region Properties

		private MousePointer pointer;

		// die Linien
		protected Lines lines;

		#endregion

		#region Constructors

		public MenuScreen (Knot3Game game)
		: base(game)
		{
			// die Linien
			lines = new Lines (screen: this, drawOrder: DisplayLayer.Dialog, lineWidth: 6);

			// der Mauszeiger
			pointer = new MousePointer (this);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		public override void Update (GameTime time)
		{
		}

		/// <summary>
		/// Wird aufgerufen, wenn in diesen Spielzustand gewechselt wird.
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			base.Entered (previousScreen, time);
			AddGameComponents (time, pointer, lines);
			Audio.BackgroundMusic = Sound.MenuMusic;

			//test:
			//MediaPlayer.IsRepeating = true;
			//var song = Content.Load<Song>("Music-Challenge/Frame_-_13_-_Spiral_Beams");
			//MediaPlayer.Play(song);
			//System.Media.SoundPlayer player = new System.Media.SoundPlayer("Music-Challenge/Frame_-_13_-_Spiral_Beams.mp3");
			//player.PlayLooping();
		}



		#endregion
	}
}

