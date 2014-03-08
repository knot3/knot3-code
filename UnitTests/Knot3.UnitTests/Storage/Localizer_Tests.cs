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

using NUnit.Framework;

using Knot3.Framework.Storage;

namespace Knot3.UnitTests.Storage
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class Localizer_Tests
    {
        private Language lang1;
        private Language lang2;
        private LanguageOption languageOption;

        [SetUp]
        public void Init ()
        {
            lang1 = new Language (file: Localizer.LanguageDirectory + "xx.ini");
            lang1.DisplayName = "XX";
            lang1.Localization ["text", "new game", ""] = "Neues Spiel";

            lang2 = new Language (file: Localizer.LanguageDirectory + "xy.ini");
            lang2.DisplayName = "XY";
            lang2.Localization ["text", "new game", ""] = "New Game";

            languageOption = new LanguageOption (
                section: "language",
                name: "current",
                configFile: Config.Default
            );
        }

        [Test]
        public void Localizer_String_Tests ()
        {
            SetLanguage (lang1);
            Assert.AreEqual (Localizer.Localize ("new game"), "Neues Spiel");
            SetLanguage (lang2);
            Assert.AreNotEqual (Localizer.Localize ("new game"), "Neues Spiel");
            Assert.AreEqual (Localizer.Localize ("new game"), "New Game");
        }

        [Test]
        public void Language_Equals_Tests ()
        {
            Assert.AreNotEqual (lang1, lang2);
            Assert.AreEqual (lang1, lang1);
        }

        [Test]
        public void Language_DisplayName_Tests ()
        {
            SetLanguage (lang1);

            Assert.AreNotEqual (languageOption.DisplayValue, lang2.DisplayName);
            Assert.AreEqual (languageOption.DisplayValue, lang1.DisplayName);

            Language lang3 = new Language (file: Localizer.LanguageDirectory + "xz.ini");
            Assert.AreEqual (lang3.Localization ["language", "displayname", ""], lang3.DisplayName);
            lang3.DisplayName = "XZ";
            Assert.AreEqual (lang3.Localization ["language", "displayname", ""], lang3.DisplayName);
            Assert.AreEqual (lang3.Localization ["language", "displayname", ""], "XZ");
        }

        [Test]
        public void Language_ValidDisplayNames_Tests ()
        {
            SetLanguage (lang1);
            Assert.True (languageOption.DisplayValidValues.ContainsKey (lang1.DisplayName));
            Assert.True (languageOption.DisplayValidValues.ContainsKey (languageOption.DisplayValue));
            Assert.True (languageOption.DisplayValidValues.ContainsValue (lang1.Code));
        }

        public void SetLanguage (Language lang)
        {
            languageOption.Value = lang;
            //Config.Default ["language", "current", "xx"] = code;
        }
    }
}
