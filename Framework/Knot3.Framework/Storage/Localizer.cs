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
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

#endregion

namespace Knot3.Framework.Storage
{
	/// <summary>
	/// Eine statische Klasse, die Bezeichner in lokalisierten Text umsetzen kann.
	/// </summary>
	public static class Localizer
	{
		#region Properties

		public static readonly string DefaultLanguageCode = "en";

		/// <summary>
		/// Gibt die zur Zeit in der zentralen Konfigurationsdatei eingestellte Sprache zurück.
		/// </summary>
		private static Option CurrentLanguageCode
		{
			get {
				if (_currentLanguageCode == null) {
					_currentLanguageCode = new Option ("language", "current", DefaultLanguageCode, Config.Default)
					{ Verbose = false };
				}
				return _currentLanguageCode;
			}
		}

		private static Option _currentLanguageCode;

		/// <summary>
		/// Die aktuell geladene Sprache.
		/// </summary>
		public static Language CurrentLanguage { get; private set; }

		public static string LanguageDirectory
		{
			get {
				string directory = SystemInfo.RelativeContentDirectory + "Languages" + SystemInfo.PathSeparator;
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		private static Language[] _validLanguages;

		public static Language[] ValidLanguages
		{
			get {
				if (_validLanguages != null) {
					return _validLanguages;
				}
				else {
					string[] files = Directory.GetFiles (LanguageDirectory);
					List<Language> languages = new List<Language> ();
					foreach (string file in files) {
						try {
							languages.Add (new Language (file: file));
						}
						catch (Exception ex) {
							Log.Error (ex);
						}
					}
					if (languages.Count == 0) {
						languages.Add (new Language (file: LanguageDirectory + "en.ini"));
					}
					return _validLanguages = languages.ToArray ();
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Liefert zu dem übergebenen Bezeichner den zugehörigen Text aus der Lokalisierungsdatei der
		/// aktuellen Sprache zurück, die dabei aus der Einstellungsdatei des Spiels gelesen wird.
		/// </summary>
		public static string Localize (this string text)
		{
			if (text == null) {
				return "";
			}
			else if (text == string.Empty || text.Contains ("Exception") || text.Any(char.IsDigit)) {
				return text;
			}
			else {
				if (CurrentLanguage == null || CurrentLanguage.Code != CurrentLanguageCode.Value) {
					_validLanguages = null;
					foreach (Language lang in ValidLanguages) {
						if (lang.Code == CurrentLanguageCode.Value) {
							CurrentLanguage = lang;
						}
					}
					CurrentLanguageCode.Value = CurrentLanguage.Code;
				}
				return CurrentLanguage.Localization ["text", text, text];
			}
		}

		#endregion
	}
}
