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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Data
{
	public sealed class RectangleMap
	{
		#region Properties

		private INodeMap NodeMap;
		private Dictionary<Vector3, List<PossibleRectanglePosition>> positions
		    = new Dictionary<Vector3, List<PossibleRectanglePosition>> ();

		#endregion

		#region Constructors

		public RectangleMap (INodeMap nodeMap)
		{
			NodeMap = nodeMap;
		}

		#endregion

		#region Methods

		public void AddEdge (Edge edge, bool isVirtual)
		{
			Node a = NodeMap.NodeBeforeEdge (edge);
			Node b = NodeMap.NodeAfterEdge (edge);
			AddEdge (edge, a, b, isVirtual);
		}

		public void AddEdge (Edge edge, Node nodeA, Node nodeB, bool isVirtual)
		{
			Vector3 edgeCenter = nodeA.CenterBetween (nodeB);
			foreach (Direction direction in Direction.Values) {
				if (direction.Axis != edge.Direction.Axis) {
					Vector3 rectangleCenter = edgeCenter + direction * Node.Scale / 2;
					PossibleRectanglePosition rectanglePosition = new PossibleRectanglePosition {
						Edge = edge,
						NodeA = nodeA,
						NodeB = nodeB,
						Position = rectangleCenter,
						IsVirtual = isVirtual
					};
					positions.Add (rectangleCenter, rectanglePosition);
				}
			}
		}

		public bool ContainsEdge (Node a, Node b)
		{
			foreach (List<PossibleRectanglePosition> many in positions.Values) {
				foreach (PossibleRectanglePosition position in many) {
					if ((position.NodeA == a && position.NodeB == b) || (position.NodeA == b && position.NodeB == a)) {
						return true;
					}
				}
			}
			return false;
		}

		public IEnumerable<ValidRectanglePosition> ValidPositions ()
		{
			foreach (List<PossibleRectanglePosition> many in positions.Values) {
				foreach (var pair in many.SelectMany ((value, index) => many.Skip (index + 1),
				(first, second) => new { first, second })) {
					List<PossibleRectanglePosition> pos
					    = new PossibleRectanglePosition[] { pair.first, pair.second } .ToList ();
					if (pos.Count == 2) {
						for (int i = 0; i <= 1; ++i) {
							int first = i % 2;
							int second = (i + 1) % 2;
							Edge edgeAB = pos [first].Edge;
							Edge edgeCD = pos [second].Edge;
							Node nodeA = pos [first].NodeA;
							Node nodeB = pos [first].NodeB;
							Node nodeC = pos [second].NodeA;
							Node nodeD = pos [second].NodeB;
							if (nodeB == nodeC || (nodeA-nodeB) == (nodeC-nodeD)) {
								var valid = new ValidRectanglePosition {
									EdgeAB = edgeAB,
									EdgeCD = edgeCD,
									NodeA = nodeA,
									NodeB = nodeB,
									NodeC = nodeC,
									NodeD = nodeD,
									Position = pos[first].Position,
									IsVirtual = pos[first].IsVirtual || pos[second].IsVirtual
								};
								yield return valid;
							}
						}
					}
				}
			}
		}

		#endregion
	}

	public struct PossibleRectanglePosition {
		public Edge Edge;
		public Node NodeA;
		public Node NodeB;
		public Vector3 Position;
		public bool IsVirtual;
	}

	public struct ValidRectanglePosition {
		public Edge EdgeAB;
		public Edge EdgeCD;
		public Node NodeA;
		public Node NodeB;
		public Node NodeC;
		public Node NodeD;
		public Vector3 Position;
		public bool IsVirtual;
	}
}