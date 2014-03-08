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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using NUnit.Framework;

using Knot3.Game.Data;

namespace Knot3.UnitTests.Data
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class ChallengeMetaData_Tests
    {
        [Test]
        public void ChallengeMetaDataHighscore_Test ()
        {
            ChallengeFileIO file = new ChallengeFileIO ();
            ChallengeMetaData meta = file.LoadMetaData (TestHelper.TestResourcesDirectory + "TestChallenge.challenge");
            Assert.AreEqual (meta.AvgTime, 7.666666f, 0.0001f);
            Assert.AreEqual (ChallengeMetaData.formatTime (1337f), "00h:22m:17s");
            Assert.AreEqual (meta.FormatedAvgTime, "00h:00m:07s");
            {
                string[] names = { "Erster", "Dritter", "Zweiter" };
                int[] times = { 1, 15, 7 };
                int position = 0;
                foreach (KeyValuePair<string, int> entry in meta.Highscore) {
                    Assert.AreEqual (entry.Key, names[position]);
                    Assert.AreEqual (entry.Value, times[position]);
                    position++;
                }
            }
            meta.AddToHighscore ("Noob", 1337);
            {
                string[] names = { "Erster", "Dritter", "Zweiter", "Noob"};
                int[] times = { 1, 15, 7, 1337 };
                int position = 0;
                foreach (KeyValuePair<string, int> entry in meta.Highscore) {
                    Assert.AreEqual (entry.Key, names[position]);
                    Assert.AreEqual (entry.Value, times[position]);
                    position++;
                }
            }
        }
    }
}
