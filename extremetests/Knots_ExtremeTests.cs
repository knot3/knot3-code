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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Knot3.Data;
using Knot3.MockObjects;

#endregion

namespace Knot3.ExtremeTests
{
	public class ExtremeKnots
	{
		private static KnotFileIO knotFileIO;
		private static List<string> testKnotNames;

		static ExtremeKnots ()
		{
			knotFileIO = new KnotFileIO ();
			testKnotNames = new List<string>();
		}

		public static void generateTestKnots ()
		{
			Knot knot = null;
			string knotName = null;

			knotName = "Square-Knot_100";
			testKnotNames.Add (knotName);
			knot = KnotGenerator.generateSquareKnot (100 / 4, knotName);

			Console.WriteLine ("------->>>> " + knot.MetaData.Filename);

			//knotFileIO.Save (knot);
		}

		public static void SaveSquareKnot (string knotPath)
		{
			Knot knot = null;
			KnotFileIO knotFileIO = new KnotFileIO ();

			// todo

			knotFileIO.Save (knot);
		}

		public static void LoadSquareKnot (string knotPath)
		{
			KnotFileIO knotFileIO = new KnotFileIO ();
			knotFileIO.Load (knotPath);
		}
	}
}
