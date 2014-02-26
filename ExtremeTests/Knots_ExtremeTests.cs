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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Knot3.Game.Data;

using Knot3.MockObjects;

#endregion

namespace Knot3.ExtremeTests
{
    public class ExtremeKnots
    {
        private static KnotFileIO knotFileIO;
        // public static List<string> testKnotNames;
        public static int[] SquareKnot_TestLengths = new int[] {100};

        static ExtremeKnots ()
        {
            knotFileIO = new KnotFileIO ();
            //testKnotNames = new List<string>();
        }

        public static void generateTestKnots ()
        {
            Knot knot = null;
            string knotName = null;

            foreach (int knotLength in SquareKnot_TestLengths) {
                knotName = "Square-Knot_" + knotLength;
                knot = KnotGenerator.generateSquareKnot (knotLength / 4, knotName);
                Console.WriteLine ("Generating SK: " + knot.MetaData.Filename);
            }

            // todo: einstellen, dass die Testknoten nicht bei den Savegames landen.

            //knotFileIO.Save (knot);
        }

        public static void SaveSquareKnot (int numberOfEdges, string knotName)
        {
            Knot knot = null;
            KnotFileIO knotFileIO = new KnotFileIO ();

            // todo

            knotFileIO.Save (knot);
        }

        public static void LoadSquareKnot (string knotName)
        {
            KnotFileIO knotFileIO = new KnotFileIO ();
            knotFileIO.Load (knotName);
        }
    }
}
