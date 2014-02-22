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
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;

#endregion

namespace Knot3.Game.Widgets
{
	/// <summary>
	///Dieser Dialog Zeigt den Highscore f�r die Gegebene Challenge an und bietet die Option
	///zum Neustarten oder R�ckkehr zum Hauptmen�
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class HighscoreDialog : Dialog
	{
		#region Properties

		private Menu highscoreList;
		private Container buttons;

		#endregion

		#region Constructors

		/// <summary>
		///
		/// </summary>
		public HighscoreDialog (IGameScreen screen, DisplayLayer drawOrder, Challenge challenge)
		: base (screen, drawOrder, "Highscores")
		{
			// Der Titel-Text ist mittig ausgerichtet
			AlignX = HorizontalAlignment.Center;
			highscoreList = new Menu (Screen, Index + DisplayLayer.Menu);
			highscoreList.Bounds = ContentBounds;
			highscoreList.ItemAlignX = HorizontalAlignment.Left;
			highscoreList.ItemAlignY = VerticalAlignment.Center;

			if (challenge.Highscore != null) {
				//sotiert die Highscoreliste wird nach der Zeit sotiert
				int highscoreCounter = 0;
				foreach (KeyValuePair<string, int> entry in challenge.Highscore.OrderBy (key => key.Value)) {
					TextItem firstScore = new TextItem (screen, drawOrder, entry.Value + " " + entry.Key);
					highscoreList.Add (firstScore);
					highscoreCounter++;
					if (highscoreCounter > 8) {
						break;
					}
				}
			}

			buttons = new Container (screen, Index + DisplayLayer.Menu);

			// Button zum Neustarten der Challenge
			Action<GameTime> restartAction = (time) => {
				Close (time);
				Screen.NextScreen = new ChallengeModeScreen (Screen.Game, challenge);
			};
			MenuEntry restartButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Restart challenge",
			    onClick: restartAction
			);
			restartButton.Bounds.Size = new ScreenPoint (screen, ContentBounds.Size.Relative.X/2, 0.05f);
			restartButton.Bounds.Position = ContentBounds.Position + ContentBounds.Size.OnlyY
			                                - restartButton.Bounds.Size.OnlyY;
			restartButton.AlignX = HorizontalAlignment.Center;
			buttons.Add (restartButton);

			// Button für die Rückkehr zum StartScreen
			Action<GameTime> returnAction = (time) => {
				Close (time);
				Screen.NextScreen = new StartScreen (Screen.Game);
			};
			MenuEntry returnButton = new MenuEntry (
			    screen: Screen,
			    drawOrder: Index + DisplayLayer.MenuItem,
			    name: "Return to menu",
			    onClick: returnAction
			);
			returnButton.Bounds.Size = new ScreenPoint (screen, ContentBounds.Size.Relative.X/2, 0.05f);
			returnButton.Bounds.Position = ContentBounds.Position + ContentBounds.Size.OnlyY
			                               - returnButton.Bounds.Size.OnlyY + ContentBounds.Size.OnlyX / 2;
			returnButton.AlignX = HorizontalAlignment.Center;
			buttons.Add (returnButton);
		}

		#endregion

		#region Methods

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents (time)) {
				yield return component;
			}
			yield return highscoreList;
			yield return buttons;
		}

		#endregion
	}
}