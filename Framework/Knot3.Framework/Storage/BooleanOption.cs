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

namespace Knot3.Framework.Storage
{
    /// <summary>
    /// Diese Klasse repräsentiert eine Option, welche die Werte \glqq Wahr\grqq~oder \glqq Falsch\grqq~annehmen kann.
    /// </summary>
    public sealed class BooleanOption : DistinctOption
    {
        /// <summary>
        /// Eine Eigenschaft, die den aktuell abgespeicherten Wert zurückgibt.
        /// </summary>
        public new bool Value
        {
            get {
                return base.Value == ConfigFile.True ? true : false;
            }
            set {
                base.Value = value ? ConfigFile.True : ConfigFile.False;
            }
        }

        public new static string[] ValidValues = new string[] {
            ConfigFile.True,
            ConfigFile.False
        };

        /// <summary>
        /// Erstellt eine neue Option, welche die Werte \glqq Wahr\grqq~oder \glqq Falsch\grqq~annehmen kann. Mit dem angegebenen Namen, in dem
        /// angegebenen Abschnitt der angegebenen Einstellungsdatei.
        /// [base=section, name, defaultValue?ConfigFile.True:ConfigFile.False, ValidValues, configFile]
        /// </summary>
        public BooleanOption (string section, string name, bool defaultValue, ConfigFile configFile)
        : base (section, name, defaultValue?ConfigFile.True:ConfigFile.False, ValidValues, configFile)
        {
        }
    }
}
