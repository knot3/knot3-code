using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Knot3.KnotData;
using Knot3.UnitTests.MockObjects;

namespace Knot3.UnitTests.Tests.KnotData
{
	[TestFixture]
	class Test_KnotStringIO
	{
		[Test]
		public void test_EncodeDecodeEdge ()
		{
			KnotStringIO knotStringIO = new KnotStringIO(KnotGenerator.generateValidSquaredKnot(10));

			Edge[] edges = { Edge.Left, Edge.Right, Edge.Up, Edge.Down, Edge.Forward, Edge.Backward };

			foreach (Edge edge in edges) {
				// EncodeEdge(edge);
			}
		}
	}
}
