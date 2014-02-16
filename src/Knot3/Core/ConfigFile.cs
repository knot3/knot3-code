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

using Knot3.Data;
using Knot3.GameObjects;
using Knot3.RenderEffects;
using Knot3.Screens;
using Knot3.Utilities;
using Knot3.Widgets;

#endregion

namespace Knot3.Core
{
	/// <summary>
	/// Repräsentiert eine Einstellungsdatei.
	/// </summary>
	public sealed class ConfigFile
	{
		#region Properties

		/// <summary>
		/// Die Repräsentation des Wahrheitswerts "wahr" als String in einer Einstellungsdatei.
		/// </summary>
		public static string True { get { return "true"; } }

		/// <summary>
		/// Die Repräsentation des Wahrheitswerts "falsch" als String in einer Einstellungsdatei.
		/// </summary>
		public static string False { get { return "false"; } }

		private string Filename;
		private IniFile ini;

		#endregion

		#region Constructors

		public ConfigFile (string filename)
		{
			// load ini file
			Filename = filename;

			// create a new ini parser
			using (StreamWriter w = File.AppendText (Filename)) {
			}
			ini = new IniFile (Filename);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Setzt den Wert der Option mit dem angegebenen Namen in den angegebenen Abschnitt auf den angegebenen Wert.
		/// </summary>
		public void SetOption (string section, string option, string _value)
		{
			ini [section, option] = _value;
		}

		/// <summary>
		/// Gibt den aktuell in der Datei vorhandenen Wert für die angegebene Option in dem angegebenen Abschnitt zurück.
		/// </summary>
		public string GetOption (string section, string option, string defaultValue)
		{
			return ini [section, option, defaultValue];
		}

		/// <summary>
		/// Setzt den Wert der Option mit dem angegebenen Namen in den angegebenen Abschnitt auf den angegebenen Wert.
		/// </summary>
		public void SetOption (string section, string option, bool _value)
		{
			SetOption (section, option, _value ? True : False);
		}

		/// <summary>
		/// Gibt den aktuell in der Datei vorhandenen Wert für die angegebene Option in dem angegebenen Abschnitt zurück.
		/// </summary>
		public bool GetOption (string section, string option, bool defaultValue)
		{
			return GetOption (section, option, defaultValue ? True : False) == True ? true : false;
		}

		public void SetOption (string section, string option, float _value)
		{
			SetOption (section, option, floatToString (_value));
		}

		public float GetOption (string section, string option, float defaultValue)
		{
			return stringToFloat (GetOption (section, option, floatToString (defaultValue)));
		}

		private string floatToString (float f)
		{
			return String.Empty + ((int) (f * 1000)).ToString ();
		}

		private float stringToFloat (string s)
		{
			int i;
			bool result = Int32.TryParse (s, out i);
			if (true == result) {
				return ((float)i) / 1000f;
			}
			else {
				return 0;
			}
		}

		public bool this [string section, string option, bool defaultValue = false]
		{
			get {
				return GetOption (section, option, defaultValue);
			}
			set {
				SetOption (section, option, value);
			}
		}

		public float this [string section, string option, float defaultValue = 0f]
		{
			get {
				return GetOption (section, option, defaultValue);
			}
			set {
				SetOption (section, option, value);
			}
		}

		public string this [string section, string option, string defaultValue = null]
		{
			get {
				return GetOption (section, option, defaultValue);
			}
			set {
				SetOption (section, option, value);
			}
		}

		#endregion
	}
}
