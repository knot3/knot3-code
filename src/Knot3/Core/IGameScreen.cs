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

using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Widgets;
using Knot3.Audio;
using Knot3.Input;

#endregion

namespace Knot3.Core
{
	public interface IGameScreen
	{
		#region Properties

		/// <summary>
		/// Das Spiel, zu dem der Spielzustand gehört.
		/// </summary>
		Knot3Game Game { get; set; }

		/// <summary>
		/// Der Inputhandler des Spielzustands.
		/// </summary>
		InputManager Input { get; }

		AudioManager Audio { get; }

		/// <summary>
		/// Der aktuelle Postprocessing-Effekt des Spielzustands
		/// </summary>
		IRenderEffect PostProcessingEffect { get; set; }

		/// <summary>
		/// Ein Stack, der während dem Aufruf der Draw-Methoden der Spielkomponenten die jeweils aktuellen Rendereffekte enthält.
		/// </summary>
		IRenderEffectStack CurrentRenderEffects { get; set; }

		/// <summary>
		/// Der nächste Spielstand, der von Knot3Game gesetzt werden soll.
		/// </summary>
		IGameScreen NextScreen { get; set; }

		GraphicsDeviceManager Graphics { get; }

		GraphicsDevice Device { get; }

		Viewport Viewport { get; set; }

		ContentManager Content { get; }

		Color BackgroundColor { get; }

		Bounds Bounds { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Beginnt mit dem Füllen der Spielkomponentenliste des XNA-Frameworks und fügt sowohl für Tastatur- als auch für
		/// Mauseingaben einen Inputhandler für Widgets hinzu. Wird in Unterklassen von IGameScreen reimplementiert und fügt zusätzlich weitere
		/// Spielkomponenten hinzu.
		/// </summary>
		void Entered (IGameScreen previousScreen, GameTime time);

		/// <summary>
		/// Leert die Spielkomponentenliste des XNA-Frameworks.
		/// </summary>
		void BeforeExit (IGameScreen nextScreen, GameTime time);

		/// <summary>
		/// Zeichnet die Teile des IGameScreens, die keine Spielkomponenten sind.
		/// </summary>
		void Draw (GameTime time);

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		void Update (GameTime time);

		/// <summary>
		/// Fügt die angegebenen GameComponents in die Components-Liste des Games ein.
		/// </summary>
		void AddGameComponents (GameTime time, params IGameScreenComponent[] components);

		/// <summary>
		/// Entfernt die angegebenen GameComponents aus der Components-Liste des Games.
		/// </summary>
		void RemoveGameComponents (GameTime time, params IGameScreenComponent[] components);

		#endregion
	}
}
