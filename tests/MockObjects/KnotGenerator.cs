using Knot3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knot3.UnitTests.MockObjects
{
	class KnotGenerator
	{
		public static Knot generateValidSquaredKnot (int EdgeLength)
		{
			Edge[] edgeList = new Edge[EdgeLength * 4];

			for (int i = 0; i < EdgeLength; i++) {
				edgeList[i] = Edge.Up;
			}
			for (int i = EdgeLength; i < EdgeLength*2; i++) {
				edgeList[i] = Edge.Right;
			}
			for (int i = EdgeLength *2; i < EdgeLength*3; i++) {
				edgeList[i] = Edge.Down;
			}
			for (int i = EdgeLength *3; i < EdgeLength*4; i++) {
				edgeList[i] = Edge.Left;
			}
			KnotMetaData metaData = new KnotMetaData ("SquareKnot", edgeList.Count<Edge>, null, null);
			Knot squareKnot = new Knot (metaData, edgeList);

			return squareKnot;
		}

		public static Knot generateInvalidKnot ()
		{
			Edge[] edgeList = new Edge[] {
				Edge.Up, Edge.Up, Edge.Up, Edge.Up
			};
			KnotMetaData metaData = new KnotMetaData ("Invalid", edgeList.Count<Edge>, null, null);
			Knot invalidKnot = new Knot (metaData, edgeList);
			return invalidKnot;
		}
	}
}
