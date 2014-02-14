using System;
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
