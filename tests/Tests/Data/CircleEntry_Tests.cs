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
using System.Text;

using NUnit.Framework;

using Knot3.Data;
using Knot3.Utilities;

#endregion

namespace Knot3.UnitTests.Data
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für Test_CircleEntry
	/// </summary>
	[TestFixture]
	public class CircleEntry_Tests
	{
		[Test]
		public void CircleEntry_Constructor_Tests ()
		{
			int[] arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7,	8, 9 };
			CircleEntry<int> start = new CircleEntry<int> (arr);
			for (int n = 0; n < 10; n++) {
				Assert.AreEqual (n, start.Value);
				start = start.Next;
			}
			for (int n = 0; n < 10; n++) {
				Assert.AreEqual (n, start.Value);
				start ++;
			}
			start --;
			for (int n = 9; n >= 0; n--) {
				Assert.AreEqual (n, start.Value);
				start --;
			}
		}

		[Test]
		public void CircleEntry_Clear_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);
			Assert.AreEqual (reff.Length, circle.Count);
			circle.Clear ();
			Assert.AreEqual (1, circle.Count);
		}

		[Test]
		public void CircleEntry_CopyTo_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);
			int[] copy = count.Repeat (i => -1).ToArray ();
			circle.CopyTo (copy, 0);
			count.Repeat (i => Assert.AreEqual (reff [i], copy [i]));
		}

		[Test]
		public void CircleEntry_Implicit_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);

			// []-Operator
			count.Repeat (i => Assert.AreEqual (circle [i], i));
			// .Value
			count.Repeat (i => Assert.AreEqual ((circle + i).Value, i));
			// Implicit Cast
			count.Repeat (i => Assert.AreEqual ((int)(circle + i), i));
		}

		[Test]
		public void CircleEntry_IndexOf_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);

			count.Repeat (i => Assert.AreEqual (i, circle.IndexOf (i)));
			count.Repeat (i => Assert.AreEqual (i, circle.IndexOf (j => j == i)));
		}

		[Test]
		public void CircleEntry_Set_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);

			count.Repeat (i => Assert.AreEqual (i + 50, circle [i] = i + 50));
		}

		[Test]
		public void CircleEntry_InterfaceNonsense_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);
			Assert.IsFalse (circle.IsReadOnly);
		}

		[Test]
		public void CircleEntry_Find_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);

			foreach (int searchFor in count.Range ()) {
				Assert.IsTrue (circle.Contains (searchFor));

				Assert.AreEqual (searchFor, circle.Find (searchFor).At (0));
				Assert.AreEqual (searchFor, circle.Find (t => t == searchFor).At (0));

				IEnumerable<CircleEntry<int>> found1;
				Assert.IsTrue (circle.Contains (searchFor, out found1));
				Assert.AreEqual (searchFor, found1.At (0));
				Assert.IsTrue (circle.Contains (t => t == searchFor, out found1));
				Assert.AreEqual (searchFor, found1.At (0));

				CircleEntry<int> found2;
				Assert.IsTrue (circle.Contains (searchFor, out found2));
				Assert.AreEqual (searchFor, found2);
				Assert.IsTrue (circle.Contains (t => t == searchFor, out found2));
				Assert.AreEqual (searchFor, found2);

				if (found2 != circle) {
					circle.Remove (searchFor);
					Assert.IsFalse (circle.Contains (searchFor, out found1));
					Assert.IsNull (found1.At (0));
					Assert.IsFalse (circle.Contains (t => t == searchFor, out found1));
					Assert.IsNull (found1.At (0));
				}
			}
			foreach (int searchFor in count.Range ().Skip (1)) {
				circle.Remove (searchFor);
			}

			Assert.AreEqual (1, circle.Count);
		}

		[Test]
		public void CircleEntry_RemoveAt_Test ()
		{
			int count = 100;
			int[] reff = count.Repeat (i => i).ToArray ();
			CircleEntry<int> circle = new CircleEntry<int> (reff);

			Assert.IsTrue (circle.Contains (5));
			circle.RemoveAt (5);
			Assert.IsFalse (circle.Contains (5));
		}

		[Test]
		public void CircleEntry_RandomAccess_Test ()
		{
			int[] reff = new int[] { 2, 6, 4, 5, 8, 7, 3, 1, 0, 9 };
			CircleEntry<int> circle = new CircleEntry<int> (reff);
			foreach (int n in reff) {
				Assert.AreEqual (circle [n], reff [n]);
				Assert.AreEqual ((circle + n).Value, reff [n]);
			}
		}

		[Test]
		public void CircleEntry_ToString_Test ()
		{
			int[] reff = new int[] { 2, 6, 4, 5, 8, 7, 3, 1, 0, 9 };
			CircleEntry<int> circle = new CircleEntry<int> (reff);
			Assert.IsNotEmpty (circle.ToString ());
			circle = CircleEntry<int>.Empty;
			Assert.IsNotEmpty (circle.ToString ());
		}

		[Test]
		public void CircleEntry_ToCircle_Test ()
		{
			int[] reff = new int[] { 2, 6, 4, 5, 8, 7, 3, 1, 0, 9 };
			CircleEntry<int> circle1 = new CircleEntry<int> (reff);
			CircleEntry<int> circle2 = reff.ToCircle ();
			reff.Length.Repeat (i => Assert.AreEqual (circle1 [i], circle2 [i]));
		}

		[Test]
		public void CircleEntry_Insert_Test ()
		{
			CircleEntry<int> start = new CircleEntry<int> (new int[] {1});
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
