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
using System.Linq;

namespace Knot3.Framework.Storage
{
    /// <summary>
    /// Diese Klasse repräsentiert eine Option, welche die Werte \glqq Wahr\grqq~oder \glqq Falsch\grqq~annehmen kann.
    /// </summary>
    public sealed class FloatOption : DistinctOption
    {
        /// <summary>
        /// Eine Eigenschaft, die den aktuell abgespeicherten Wert zurückgibt.
        /// </summary>
        public new float Value
        {
            get {
                return stringToFloat (base.Value);
            }
            set {
                base.Value = convertToString (value);
            }
        }

        public override string DisplayValue
        {
            get {
                return String.Empty + stringToFloat (base.Value);
            }
        }

        public override Dictionary<string,string> DisplayValidValues
        {
            get {
                return new Dictionary<string, string> (base.ValidValues.ToDictionary (s => String.Empty + stringToFloat (s), s => s));
            }
        }

        /// <summary>
        /// Erstellt eine neue Option, welche die Werte \glqq Wahr\grqq~oder \glqq Falsch\grqq~annehmen kann. Mit dem angegebenen Namen, in dem
        /// angegebenen Abschnitt der angegebenen Einstellungsdatei.
        /// [base=section, name, defaultValue?ConfigFile.True:ConfigFile.False, ValidValues, configFile]
        /// </summary>
        public FloatOption (string section, string name, float defaultValue, IEnumerable<float> validValues, ConfigFile configFile)
        : base (section, name, convertToString (defaultValue),validValues.Select (convertToString), configFile)
        {
        }

        private static string convertToString (float f)
        {
            return (String.Empty + (int)(f * 1000f));
        }

        private static float stringToFloat (string s)
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
    }
}
