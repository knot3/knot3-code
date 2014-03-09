/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 *
 * See the LICENSE file for full license details of the Knot3 project.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using Knot3.Framework.Platform;

namespace Knot3.Framework.Storage
{
    /// <summary>
    /// Eine statische Klasse, die Bezeichner in lokalisierten Text umsetzen kann.
    /// </summary>
    public static class Localizer
    {
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
                            Log.Debug ("Language file: ", file);
                            languages.Add (new Language (file: file));
                        }
                        catch (Exception ex) {
                            Log.Error (ex);
                        }
                    }
                    if (languages.Count == 0) {
                        languages.Add (new Language (file: LanguageDirectory + "en.ini"));
                    }
                    Log.Message ("Valid Languages: " + string.Join (", ", from lang in languages select "(" + lang.Code + "," + lang.DisplayName + ")"));
                    return _validLanguages = languages.ToArray ();
                }
            }
        }

        /// <summary>
        /// Liefert zu dem übergebenen Bezeichner den zugehörigen Text aus der Lokalisierungsdatei der
        /// aktuellen Sprache zurück, die dabei aus der Einstellungsdatei des Spiels gelesen wird.
        /// </summary>
        public static string Localize (this string text)
        {
            if (text == null) {
                return "";
            }
            else if (text == string.Empty || text.Contains ("Exception") || text.Any (char.IsDigit)) {
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

                string trimmed = text.Trim ('\r', '\n', ' ', '\t', ':', '!', '.', '?', ';');
                string localized = CurrentLanguage.Localization ["text", trimmed, trimmed];
                return ToUnicode (text.Replace (trimmed, localized));
            }
        }

        public static string ToUnicode (string text)
        {
            return text.Replace ("&auml;", "\u00E4").Replace ("&ouml;", "\u00F6").Replace ("&uuml;", "\u00FC")
                   .Replace ("&Auml;", "\u00C4").Replace ("&Ouml;", "\u00D6").Replace ("&Uuml;", "\u00DC")
                   .Replace ("&szlig;", "\u00DF");
        }
    }
}
