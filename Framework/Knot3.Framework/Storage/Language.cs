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

using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Knot3.Framework.Storage
{
    public class Language
    {
        /// <summary>
        /// Der Sprachcode der Sprache.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Der Anzeigename der Sprache.
        /// </summary>
        public string DisplayName { get { return Localization ["language", "displayname", Code]; } set { Localization ["language", "displayname", Code] = value; } }

        /// <summary>
        /// Die Datei, welche Informationen für die Lokalisierung enthält.
        /// </summary>
        public ConfigFile Localization { get; private set; }

        public Language (string file)
        {
            Code = Path.GetFileNameWithoutExtension (file).ToLower ();
            file = Localizer.LanguageDirectory + Code + ".ini";
            Localization = new ConfigFile (file);
        }

        public static bool operator != (Language a, Language b)
        {
            return !(a == b);
        }

        public static bool operator == (Language a, Language b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Code == b.Code;
        }

        public bool Equals (Language other)
        {
            return other != null && Code == other.Code;
        }

        public override bool Equals (object other)
        {
            if (other == null) {
                return false;
            }
            else if (other is Language) {
                return Equals (other as Language);
            }
            else {
                return false;
            }
        }

        public static implicit operator string (Language language)
        {
            return language.Code;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return Code.GetHashCode ();
        }
    }
}
