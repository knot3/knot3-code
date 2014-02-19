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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;
using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Development
{
	[ExcludeFromCodeCoverageAttribute]
	public class JunctionEditorScreen : GameScreen
	{
		#region Properties

		/// <summary>
		/// Die Spielwelt in der die 3D-Objekte des dargestellten Knotens enthalten sind.
		/// </summary>
		private World world;

		/// <summary>
		/// Der Controller, der aus dem Knoten die 3D-Modelle erstellt.
		/// </summary>
		private JunctionEditorRenderer knotRenderer;
		private KnotInputHandler knotInput;
		private ModelMouseHandler modelMouseHandler;
		private MousePointer pointer;
		private Overlay overlay;
		private DebugBoundings debugBoundings;
		private MenuEntry backButton;
		private Menu settingsMenu;
		private DropDownMenuItem[] itemBumpRotation;

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines CreativeModeScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt game, sowie einem Knoten knot.
		/// </summary>
		public JunctionEditorScreen (GameClass game)
		: base (game)
		{
			// die Spielwelt
			world = new World (screen: this, drawIndex: DisplayLayer.GameWorld, bounds: Bounds.FromLeft (0.60f));
			// der Input-Handler
			knotInput = new KnotInputHandler (screen: this, world: world);
			// das Overlay zum Debuggen
			overlay = new Overlay (screen: this, world: world);
			// der Mauszeiger
			pointer = new MousePointer (screen: this);
			// der Maus-Handler für die 3D-Modelle
			modelMouseHandler = new ModelMouseHandler (screen: this, world: world);

			// der Knoten-Renderer
			knotRenderer = new JunctionEditorRenderer (screen: this, position: Vector3.Zero);
			world.Add (knotRenderer);

			// visualisiert die BoundingSpheres
			debugBoundings = new DebugBoundings (screen: this, position: Vector3.Zero);
			world.Add (debugBoundings);

			// Hintergrund
			SkyCube skyCube = new SkyCube (screen: this, position: Vector3.Zero, distance: world.Camera.MaxPositionDistance + 500);
			world.Add (skyCube);

			// Backbutton
			backButton = new MenuEntry (
			    screen: this,
			    drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
			    name: "Back",
			    onClick: (time) => NextScreen = new StartScreen (Game)
			);
			backButton.AddKey (Keys.Escape);
			backButton.IsVisible = true;

			// Menü
			settingsMenu = new Menu (this, DisplayLayer.Overlay + DisplayLayer.Menu);
			settingsMenu.Bounds = Bounds.FromRight (0.40f).FromBottom (0.9f).FromLeft (0.8f);
			settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			settingsMenu.ItemForegroundColor = (state) => Design.WidgetForeground;
			settingsMenu.ItemBackgroundColor = (state) => Design.WidgetBackground;
			settingsMenu.ItemAlignX = HorizontalAlignment.Left;
			settingsMenu.ItemAlignY = VerticalAlignment.Center;

			Direction[] validDirections = Direction.Values;
			for (int i = 0; i < 3; ++i) {
				DistinctOptionInfo option = new DistinctOptionInfo (
				    section: "debug",
				    name: "debug_junction_direction" + i.ToString (),
				    defaultValue: validDirections [i * 2],
				    validValues: validDirections.Select (d => d.Description),
				    configFile: Options.Default
				);
				DropDownMenuItem item = new DropDownMenuItem (
				    screen: this,
				    drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
				    text: "Direction " + i.ToString ()
				);
				item.AddEntries (option);
				item.ValueChanged += OnDirectionsChanged;
				settingsMenu.Add (item);
			}

			itemBumpRotation = new DropDownMenuItem[3];
			for (int i = 0; i < 3; ++i) {
				DropDownMenuItem item = new DropDownMenuItem (
				    screen: this,
				    drawOrder: DisplayLayer.Overlay + DisplayLayer.MenuItem,
				    text: "Bump Angle " + i.ToString ()
				);
				item.ValueChanged += OnAnglesChanged;
				settingsMenu.Add (item);
				itemBumpRotation [i] = item;
			}

			OnDirectionsChanged (null);

			settingsMenu.Add (backButton);

			world.Camera.PositionToTargetDistance = 180;
		}

		#endregion

		#region Methods

		private void OnDirectionsChanged (GameTime time)
		{
			var directions = Directions;
			float[] validAngles = new float[] {
				0, 45, 90, 135, 180, 225, 270, 315
			};
			for (int i = 0; i < 3; ++i) {
				FloatOptionInfo option = new FloatOptionInfo (
				    section: NodeConfigKey (directions.ToEnumerable ()),
				    name: "bump" + i.ToString (),
				    defaultValue: 0,
				    validValues: validAngles,
				    configFile: Options.Models
				);
				itemBumpRotation [i].AddEntries (option);
				RemoveGameComponents (time, itemBumpRotation [i]);
				AddGameComponents (time, itemBumpRotation [i]);
			}

			/*
			for (int i = 0; i < 3; ++i) {
				Options.Default ["debug", "debug_junction_angle_bump" + i, 0f] = Options.Models [, "i, 0f];
			}
			*/
			knotRenderer.Render (directions: directions);
		}

		private void OnAnglesChanged (GameTime time)
		{
			/*
			var directions = Directions;
			for (int i = 0; i < 3; ++i) {
				Options.Models [NodeConfigKey (directions.ToEnumerable ()), "bump" + i, 0f] = Options.Default ["debug", "debug_junction_angle_bump" + i, 0f];
			}
			*/

			knotRenderer.Render (directions: Directions);
		}

		private Tuple<Direction, Direction, Direction> Directions
		{
			get {
				Direction[] validDirections = Direction.Values;
				Direction d1 = Direction.FromString (Options.Default ["debug", "debug_junction_direction0" /* + 0 */, validDirections [0]]);
				Direction d2 = Direction.FromString (Options.Default ["debug", "debug_junction_direction1" /* + 1 */, validDirections [2]]);
				Direction d3 = Direction.FromString (Options.Default ["debug", "debug_junction_direction2" /* + 2 */, validDirections [4]]);
				return Tuple.Create (d1, d2, d3);
			}
		}

		public static string NodeConfigKey (IEnumerable<Direction> directions)
		{
			IEnumerable<string> _directions = directions.Select (direction => direction + String.Empty + direction);
			return "Node" + directions.Count ().ToString () + ":" + string.Join (",", _directions);
		}

		/// <summary>
		/// Wird für jeden Frame aufgerufen.
		/// </summary>
		[ExcludeFromCodeCoverageAttribute]
		public override void Update (GameTime time)
		{
			Profiler.ProfilerMap.Clear ();
		}

		/// <summary>
		/// Fügt die 3D-Welt und den Inputhandler in die Spielkomponentenliste ein.
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			base.Entered (previousScreen, time);
			AddGameComponents (time, knotInput, overlay, pointer, world, modelMouseHandler, settingsMenu);
			Audio.BackgroundMusic = Sound.CreativeMusic;

			// Einstellungen anwenden
			debugBoundings.Info.IsVisible = Options.Default ["debug", "show-boundings", false];
		}

		#endregion
	}
}
