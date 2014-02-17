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
using Knot3.Widgets;

#endregion

namespace Knot3.Screens
{
	/// <summary>
	/// Der Spielzustand, der die Grafik-Einstellungen darstellt.
	/// </summary>
	public class GraphicsSettingsScreen : SettingsScreen
	{
		#region Properties

		/// <summary>
		/// Das Menü, das die Einstellungen enthält.
		/// </summary>
		private Menu settingsMenu;

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues GraphicsSettingsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
		/// </summary>
		public GraphicsSettingsScreen (Knot3Game game)
		: base (game)
		{
			MenuName = "Graphics";

			settingsMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			settingsMenu.Bounds.Position = new ScreenPoint (this, 0.400f, 0.180f);
			settingsMenu.Bounds.Size = new ScreenPoint (this, 0.500f, 0.720f);
			settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			settingsMenu.ItemAlignX = HorizontalAlignment.Left;
			settingsMenu.ItemAlignY = VerticalAlignment.Center;

			CheckBoxItem showArrows = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Show Arrows",
			    option: new BooleanOptionInfo ("video", "arrows", false, Options.Default)
			);
			settingsMenu.Add (showArrows);

			CheckBoxItem selectiveRender = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Selective Rendering",
			    option: new BooleanOptionInfo ("video", "selectiveRendering", false, Options.Default)
			);
			settingsMenu.Add (selectiveRender);

			CheckBoxItem autoCameraMove = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Auto Camera (edge move)",
			    option: new BooleanOptionInfo ("video", "auto-camera-move", true, Options.Default)
			);
			settingsMenu.Add (autoCameraMove);

			CheckBoxItem autoCamera = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Auto Camera",
			    option: new BooleanOptionInfo ("video", "auto-camera-nomove", false, Options.Default)
			);
			settingsMenu.Add (autoCamera);

			string currentResolution = Graphics.GraphicsDevice.DisplayMode.Width.ToString ()
			                           + "x"
			                           + Graphics.GraphicsDevice.DisplayMode.Height.ToString ();

			DisplayModeCollection modes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;
			HashSet<string> reso = new HashSet<string> ();
			foreach (DisplayMode mode in modes) {
				reso.Add (mode.Width + "x" + mode.Height);
			}
			reso.Add ("1024x600");

			string[] validResolutions = reso.ToArray ();
			validResolutions = validResolutions.OrderBy (x => Decimal.Parse (x.Split ('x') [0], System.Globalization.NumberStyles.Any)).ToArray ();
			DistinctOptionInfo resolutionOption = new DistinctOptionInfo (
			    section: "video",
			    name: "resolution",
			    defaultValue: currentResolution,
			    validValues: validResolutions,
			    configFile: Options.Default
			);
			DropDownMenuItem resolutionItem = new DropDownMenuItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Resolution"
			);
			resolutionItem.AddEntries (resolutionOption);
			settingsMenu.Add (resolutionItem);

			float[] validSupersamples = {
				1f, 1.25f, 1.5f, 1.75f, 2f
			};
			FloatOptionInfo supersamplesOption = new FloatOptionInfo (
			    section: "video",
			    name: "Supersamples",
			    defaultValue: 1f,
			    validValues: validSupersamples,
			    configFile: Options.Default
			);
			DropDownMenuItem supersamplesItem = new DropDownMenuItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Supersamples"
			);
			supersamplesItem.AddEntries (supersamplesOption);
			settingsMenu.Add (supersamplesItem);

			string[] validRenderEffects = RenderEffectLibrary.Names.ToArray ();
			DistinctOptionInfo renderEffectOption = new DistinctOptionInfo (
			    section: "video",
			    name: "knot-shader",
			    defaultValue: "default",
			    validValues: validRenderEffects,
			    configFile: Options.Default
			);
			DropDownMenuItem renderEffectItem = new DropDownMenuItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Render Effect"
			);
			renderEffectItem.ValueChanged += (time) => {
				RenderEffectLibrary.RenderEffectChanged (Options.Default ["video", "knot-shader", "default"], time);
			};
			renderEffectItem.AddEntries (renderEffectOption);
			settingsMenu.Add (renderEffectItem);
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
		/// Fügt das Menü mit den Einstellungen in die Spielkomponentenliste ein.
		/// </summary>
		public override void Entered (IGameScreen previousScreen, GameTime time)
		{
			base.Entered (previousScreen, time);
			AddGameComponents (time, settingsMenu);
		}

		#endregion
	}
}
