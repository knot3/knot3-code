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
using System.Linq;

using Knot3.Framework.Storage;

#endregion

namespace Knot3.Framework.Storage
{
    public class LanguageOption: DistinctOption
    {
        public new Language Value
        {
            get {
                string code = base.Value;
                foreach (Language lang in Localizer.ValidLanguages) {
                    if (lang.Code == code) {
                        return lang;
                    }
                }
                return Localizer.CurrentLanguage;
            }
            set {
                base.Value = value.Code;
            }
        }

        public override string DisplayValue
        {
            get {
                return toDisplayName (Value);
            }
        }

        public override Dictionary<string,string> DisplayValidValues
        {
            get {
                Dictionary<string, string> dict = new Dictionary<string, string> ();
                foreach (string value in base.ValidValues) {
                    dict [toDisplayName (value)] = value;
                }
                return dict;
            }
        }

        public LanguageOption (string section, string name, ConfigFile configFile)
        : base (section, name, Localizer.DefaultLanguageCode, from lang in Localizer.ValidLanguages select lang.Code, configFile)
        {
        }

        private string toDisplayName (string code)
        {
            foreach (Language lang in Localizer.ValidLanguages) {
                if (lang.Code == code) {
                    return lang.DisplayName;
                }
            }
            return Localizer.CurrentLanguage.DisplayName;
        }

        private string fromDisplayName (string displayName)
        {
            foreach (Language lang in Localizer.ValidLanguages) {
                if (lang.DisplayName == displayName) {
                    return lang.Code;
                }
            }
            return Localizer.CurrentLanguage.Code;
        }
    }
}
