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

using Knot3.Core;
using Knot3.Input;
using Knot3.GameObjects;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Widgets;
using Knot3.Screens;
using Knot3.Utilities;

#endregion

namespace Knot3.Development
{
	/// <summary>
	/// Der Spielzustand, der die Debugging-Einstellungen darstellt.
	/// </summary>
	public class DebugSettingsScreen : SettingsScreen
	{
		#region Properties

		/// <summary>
		/// Das Menü, das die Einstellungen enthält.
		/// </summary>
		private Menu settingsMenu;

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues DebugSettingsScreen-Objekt und initialisiert dieses mit einem Knot3Game-Objekt.
		/// </summary>
		public DebugSettingsScreen (Knot3Game game)
		: base (game)
		{
			MenuName = "Debug";

			settingsMenu = new Menu (this, DisplayLayer.ScreenUI + DisplayLayer.Menu);
			settingsMenu.Bounds.Position = new ScreenPoint (this, 0.400f, 0.180f);
			settingsMenu.Bounds.Size = new ScreenPoint (this, 0.500f, 0.770f);
			settingsMenu.Bounds.Padding = new ScreenPoint (this, 0.010f, 0.010f);
			settingsMenu.ItemAlignX = HorizontalAlignment.Left;
			settingsMenu.ItemAlignY = VerticalAlignment.Center;

			CheckBoxItem showOverlay = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Show Overlay",
			    option: new BooleanOptionInfo ("video", "camera-overlay", false, Options.Default)
			);
			settingsMenu.Add (showOverlay);

			CheckBoxItem showFps = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Show FPS",
			    option: new BooleanOptionInfo ("video", "fps-overlay", true, Options.Default)
			);
			settingsMenu.Add (showFps);

			CheckBoxItem showProfiler = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Show Profiler",
			    option: new BooleanOptionInfo ("video", "profiler-overlay", true, Options.Default)
			);
			settingsMenu.Add (showProfiler);

			CheckBoxItem showBoundings = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Show Bounding Boxes",
			    option: new BooleanOptionInfo ("debug", "show-boundings", false, Options.Default)
			);
			settingsMenu.Add (showBoundings);

			CheckBoxItem showStartEdgeArrow = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Show Start Edge Direction",
			    option: new BooleanOptionInfo ("debug", "show-startedge-direction", false, Options.Default)
			);
			settingsMenu.Add (showStartEdgeArrow);

			string[] unprojectMethods = { "SelectedObject", "NearFarAverage" };
			DistinctOptionInfo unprojectOption = new DistinctOptionInfo ("debug", "unproject", unprojectMethods[0], unprojectMethods, Options.Default);
			DropDownMenuItem unprojectItem = new DropDownMenuItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Unproject"
			);
			unprojectItem.AddEntries (unprojectOption);
			settingsMenu.Add (unprojectItem);

			/*
			CheckBoxItem shaderPascal = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Use Pascal's Shader",
			    option: new BooleanOptionInfo ("video", "pascal-shader", false, Options.Default)
			);te
			settingsMenu.Add (shaderPascal);

			CheckBoxItem shaderCel = new CheckBoxItem (
			    screen: this,
			    drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
			    text: "Use Cel Shader",
			    option: new BooleanOptionInfo ("video", "cel-shading", false, Options.Default)
			);
			settingsMenu.Add (shaderCel);
			*/
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
