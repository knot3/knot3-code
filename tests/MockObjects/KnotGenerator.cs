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

﻿using Knot3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

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
