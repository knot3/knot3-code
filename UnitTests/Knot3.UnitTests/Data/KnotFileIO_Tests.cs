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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Knot3.Game.Data;

using Knot3.MockObjects;

namespace Knot3.UnitTests.Data
{
    [TestFixture]
    public class KnotFileIO_Tests
    {
        [Test]
        public void KnotFileO_Test ()
        {
            Random random = new Random ();
            String randomname = random.Next (100000).ToString ();

            Knot testKnot = KnotGenerator.coloredKnot (randomname);

            KnotFileIO fileIO = new KnotFileIO ();
            fileIO.Save (testKnot, true);

            Assert.IsTrue (testKnot.Equals (fileIO.Load (testKnot.MetaData.Filename)));
            Assert.IsTrue (testKnot.MetaData.Equals (fileIO.LoadMetaData (testKnot.MetaData.Filename)));

            //Sollte nun im Cache sein
            Assert.IsTrue (testKnot.Equals (fileIO.Load (testKnot.MetaData.Filename)));
            Assert.Catch (() => {
                fileIO.Save (KnotGenerator.noMetadataKnot ("test"), true);
            });
        }
        [Test]
        public void KnotFileLoad_Test ()
        {
            KnotFileIO fileIO = new KnotFileIO ();
            Assert.DoesNotThrow (() => { fileIO.Load (TestHelper.TestResourcesDirectory + "AllDir.knot"); });
            Assert.Throws<FileNotFoundException>(() => { fileIO.Load (TestHelper.TestResourcesDirectory + "NotExistent.knot"); });
        }
        [Test]
        public void KnotFileIOContent_Test ()
        {
            KnotFileIO fileIO = new KnotFileIO ();
            Knot knot = fileIO.Load (TestHelper.TestResourcesDirectory + "AllDir.knot");
            Assert.AreEqual (knot.MetaData.Name, "AllDir");
            Assert.AreEqual (knot.Count (), 6);
        }
    }
}
