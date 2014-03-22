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

using Knot3.Framework.Platform;

namespace Knot3.Framework.Storage
{
    /// <summary>
    /// Enthält Informationen über einen Eintrag in einer Einstellungsdatei.
    /// </summary>
    public class Option
    {
        /// <summary>
        /// Die Einstellungsdatei.
        /// </summary>
        private ConfigFile ConfigFile;

        /// <summary>
        /// Der Abschnitt der Einstellungsdatei.
        /// </summary>
        public string Section { get; private set; }

        /// <summary>
        /// Der Name der Option.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Der Standardwert der Option.
        /// </summary>
        public string DefaultValue { get; private set; }

        public bool Verbose { get; set; }

        /// <summary>
        /// Der Wert der Option.
        /// </summary>
        public virtual string Value
        {
            get {
                if (Verbose) {
                    Log.Debug ("Option: ", Section, ".", Name, " => ", ConfigFile [Section, Name, DefaultValue]);
                }
                return ConfigFile [Section, Name, DefaultValue];
            }
            set {
                Log.Debug ("Option: ", Section, ".", Name, " <= ", value);
                ConfigFile [Section, Name, DefaultValue] = value;
            }
        }

        /// <summary>
        /// Erstellt ein neues OptionsInfo-Objekt aus den übergegebenen Werten.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Option (string section, string name, string defaultValue, ConfigFile configFile)
        {
            Section = section;
            Name = name;
            DefaultValue = defaultValue;
            ConfigFile = configFile != null ? configFile : Config.Default;
            Verbose = false; //SystemInfo.IsRunningOnLinux ();
        }
    }
}
