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

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.GameObjects;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.Core
{
	/// <summary>
	/// Ein Spielzustand, der zu einem angegebenen Spiel gehört und einen Inputhandler und Rendereffekte enthält.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public class GameScreen : IGameScreen
	{
		#region Properties

		/// <summary>
		/// Das Spiel, zu dem der Spielzustand gehört.
		/// </summary>
		public GameClass Game { get; set; }

		/// <summary>
		/// Der Inputhandler des Spielzustands.
		/// </summary>
		public InputManager Input { get; set; }

		public AudioManager Audio { get; private set; }

		/// <summary>
		/// Der aktuelle Postprocessing-Effekt des Spielzustands
		/// </summary>
		public IRenderEffect PostProcessingEffect { get; set; }

		/// <summary>
		/// Ein Stack, der während dem Aufruf der Draw-Methoden der Spielkomponenten die jeweils aktuellen Rendereffekte enthält.
		/// </summary>
		public IRenderEffectStack CurrentRenderEffects { get; set; }

		/// <summary>
		/// Der nächste Spielstand, der von Knot3Game gesetzt werden soll.
		/// </summary>
		public IGameScreen NextScreen { get; set; }

		public GraphicsDeviceManager Graphics { get { return Game.Graphics; } }

		public GraphicsDevice Device { get { return Game.GraphicsDevice; } }

		public Viewport Viewport
		{
			get { return Device.Viewport; }
			set { Device.Viewport = value; }
		}

		public ContentManager Content { get { return Game.Content; } }

		public Color BackgroundColor { get; protected set; }

		public Bounds Bounds
		{
			get { return new Bounds (screen: this, relX: 0f, relY: 0f, relWidth: 1f, relHeight: 1f); }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues IGameScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
		/// </summary>
		public GameScreen (GameClass game)
		{
			Game = game;
			NextScreen = this;
			CurrentRenderEffects = new RenderEffectStack (
			    screen: this,
			    defaultEffect: new StandardEffect (this)
			);
			PostProcessingEffect = new StandardEffect (this);
			Input = new InputManager (this);
			Audio = new AudioManager (this);
			BackgroundColor = Design.ScreenBackground;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Beginnt mit dem Füllen der Spielkomponentenliste des XNA-Frameworks und fügt sowohl für Tastatur- als auch für
		/// Mauseingaben einen Inputhandler für Widgets hinzu. Wird in Unterklassen von IGameScreen reimplementiert und fügt zusätzlich weitere
		/// Spielkomponenten hinzu.
		/// </summary>
		public virtual void Entered (IGameScreen previousScreen, GameTime time)
		{
			Log.Debug ("Entered: ", this);
			AddGameComponents (time, Input, Audio, new WidgetKeyHandler (this), new WidgetMouseHandler (this));
		}

		/// <summary>
		/// Leert die Spielkomponentenliste des XNA-Frameworks.
		/// </summary>
		public virtual void BeforeExit (IGameScreen nextScreen, GameTime time)
		{
			Log.Debug ("BeforeExit: ", this);
			Game.Components.Clear ();
		}

		/// <summary>
		/// Zeichnet die Teile des IGameScreens, die keine Spielkomponenten sind.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public virtual void Draw (GameTime time)
		{
		}

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public virtual void Update (GameTime time)
		{
		}

		/// <summary>
		/// Fügt die angegebenen GameComponents in die Components-Liste des Games ein.
		/// </summary>
		public virtual void AddGameComponents (GameTime time, params IGameScreenComponent[] components)
		{
				Log.Debug ("AddGameComponents...");
			foreach (IGameScreenComponent component in components) {
				Log.Debug ("AddGameComponents: ", component);
				Game.Components.Add (component);
				AddGameComponents (time, component.SubComponents (time).ToArray ());
			}
		}

		/// <summary>
		/// Entfernt die angegebenen GameComponents aus der Components-Liste des Games.
		/// </summary>
		public virtual void RemoveGameComponents (GameTime time, params IGameScreenComponent[] components)
		{
			foreach (IGameScreenComponent component in components) {
				Log.Debug ("RemoveGameComponents: ", component);
				RemoveGameComponents (time, component.SubComponents (time).ToArray ());
				Game.Components.Remove (component);
			}
		}

		#endregion
	}
}
