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
using Knot3.Framework.GameObjects;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.GameObjects
{
	/// <summary>
	/// Die 3D-Modelle, die während einer Verschiebung von Kanten die Vorschaumodelle repräsentieren.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class ShadowGameModel : ShadowGameObject
	{
		#region Properties

		private GameModel decoratedModel
		{
			get {
				return decoratedObject as GameModel;
			}
		}

		/// <summary>
		/// Die Farbe der Vorschaumodelle.
		/// </summary>
		public Color ShadowColor { get; set; }

		/// <summary>
		/// Die Transparenz der Vorschaumodelle.
		/// </summary>
		public float ShadowAlpha { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt ein neues Vorschaumodell in dem angegebenen Spielzustand für das angegebene zu dekorierende Modell.
		/// </summary>
		public ShadowGameModel (IGameScreen screen, GameModel decoratedModel)
		: base (screen, decoratedModel)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Zeichnet das Vorschaumodell.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public override void Draw (GameTime time)
		{
			// swap position, colors, alpha
			Vector3 originalPositon = decoratedModel.Info.Position;
			ModelColoring originalColoring = decoratedModel.Coloring;
			decoratedModel.Info.Position = ShadowPosition;
			decoratedModel.Coloring = new SingleColor (originalColoring.MixedColor, alpha: ShadowAlpha);

			// draw
			screen.CurrentRenderEffects.CurrentEffect.DrawModel (decoratedModel, time);

			// swap everything back
			decoratedModel.Info.Position = originalPositon;
			decoratedModel.Coloring = originalColoring;
		}

		#endregion
	}
}
