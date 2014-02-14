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

﻿using System;
using System.Text;
using System.Collections.Generic;

using NUnit.Framework;

using Knot3.Data;

namespace Knot3.UnitTests.Tests.KnotData
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für Test_CircleEntry
	/// </summary>
	[TestFixture]
	public class Test_CircleEntry
	{
		public Test_CircleEntry ()
		{
			//
			// TODO: Konstruktorlogik hier hinzufügen
			//
		}

		[Test]
		public void CircleEntry_Constructor_Tests ()
		{
			CircleEntry<int> start = new CircleEntry<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			for (int n = 0; n < 10; n++) {
				Assert.AreEqual (start.Value, n);
				start = start.Next;
			}
		}

		[Test]
		public void CircleEntry_RandomAccess_Test ()
		{
			int[] reff = new int[] { 2, 6, 4, 5, 8, 7, 3, 1, 0, 9 };
			CircleEntry<int> circle = new CircleEntry<int>(reff);
			foreach (int n in reff) {
				Assert.AreEqual (circle[n], reff[n]);
			}
		}

		[Test]
		public void CircleEntry_Insert_Test ()
		{
			CircleEntry<int> start = new CircleEntry<int>(new int[] {1});
			start.InsertBefore (0);
			start.InsertAfter (2);
			start = start.Previous;
			for (int n = 0; n < 3; n++) {
				Assert.AreEqual (start.Value, n);
				start = start.Next;
			}
			start.InsertBefore (3);
			for (int n = 0; n < 4; n++) {
				Assert.AreEqual (start.Value, n);
				start = start.Next;
			}
		}
	}
}
