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

using Knot3.Audio;
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
		: base (game)
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
		[ExcludeFromCodeCoverageAttribute]
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
			//MediaPlayer.Play (song);
			//System.Media.SoundPlayer player = new System.Media.SoundPlayer ("Music-Challenge/Frame_-_13_-_Spiral_Beams.mp3");
			//player.PlayLooping ();
		}

		#endregion
	}
}
