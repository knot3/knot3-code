
#region Using
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Core;
using Knot3.Data;
using Knot3.GameObjects;
using Knot3.RenderEffects;

#endregion

namespace Knot3.UnitTests
{
	[TestFixture]
	public class DisplayLayer_Tests
	{

		[Test]
		public void DisplayLayer_Equals_Tests ()
		{
			foreach (DisplayLayer lay1 in DisplayLayer.Values) {
				Assert.AreEqual (lay1, lay1);
				Assert.IsTrue (lay1.Equals (lay1));
				Assert.IsTrue (lay1.Equals ((object)lay1));
				foreach (DisplayLayer lay2 in DisplayLayer.Values) {
					if (lay1.Index == lay2.Index)
						Assert.AreEqual (lay1, lay2);
					else
						Assert.AreNotEqual (lay1, lay2);
				}
			}
		}

		[Test]
		public void DisplayLayer_Implicit_Tests ()
		{
			foreach (DisplayLayer lay in DisplayLayer.Values) {
				int index = lay;
				Assert.AreEqual (lay.Index, index);
			}
		}

		[Test]
		public void DisplayLayer_Math_Tests ()
		{
			foreach (DisplayLayer lay1 in DisplayLayer.Values) {
				foreach (DisplayLayer lay2 in DisplayLayer.Values) {
					Assert.AreEqual (lay1 + lay2, lay2 + lay1);
				}
				Assert.AreEqual ((lay1 * 99).Index, lay1.Index * 99);
			}
		}
	}
}

