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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Knot3.Data;
using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.MockObjects;

#endregion

namespace Knot3.UnitTests.Data
{
	[TestFixture]
	public class KnotFileIO_Tests
	{
		[Test]
		public void KnotStringIO_Test ()
		{
			KnotStringIO knotStringIO = new KnotStringIO (KnotGenerator.generateSquareKnot (10, KnotGenerator.FakeName));
			KnotStringIO other = new KnotStringIO (knotStringIO.Content);

			Assert.AreEqual (knotStringIO.Content, other.Content, "Content equal");
			KnotStringIO invalidContent = null;

			invalidContent = new KnotStringIO ("Name \n" + "Invalid Line \n");
			Assert.Catch<IOException> (() => {
				// damit der Compiler den Aufruf der Decode...-Methoden nicht wegoptimiert,
				// muss man zurück zum Konstruktur noch das eigentlich dort abgespeicherte
				// Attribut Edges abrufen (das ist ein Iterator mit lazy evaluation)
				// und das dann in eine Liste umwandeln
				Console.WriteLine (invalidContent.Edges.ToList ());
			}
			                          );
			Assert.AreEqual (knotStringIO.Content, other.Content, "Contetnt equal");
		}
	}
}
