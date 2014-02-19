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
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Development
{
	[ExcludeFromCodeCoverageAttribute]
	public class JunctionEditorNodeMap : INodeMap
	{
		#region Properties

		private Hashtable fromMap = new Hashtable ();
		private Hashtable toMap = new Hashtable ();
		private Dictionary<Node, List<IJunction>> junctionMap = new Dictionary<Node, List<IJunction>> ();

		/// <summary>
		/// Die Skalierung, die bei einer Konvertierung in einen Vector3 des XNA-Frameworks durch die ToVector ()-Methode der Node-Objekte verwendet wird.
		/// </summary>
		public int Scale { get; set; }

		public IEnumerable<Edge> Edges
		{
			get { return _edges; }
			set {}
		}

		private Edge[] _edges;

		public Vector3 Offset { get; set; }

		public Action IndexRebuilt { get; set; }

		#endregion

		#region Constructors

		public JunctionEditorNodeMap ()
		{
			IndexRebuilt = () => {};
		}

		#endregion

		#region Methods

		public void Render (Tuple<Direction, Direction, Direction> direction)
		{
			_edges = new Edge[] {
				new Edge (direction.Item1), new Edge (direction.Item1),
				new Edge (direction.Item2), new Edge (direction.Item2),
				new Edge (direction.Item3), new Edge (direction.Item3),
			};
			BuildIndex ();
		}

		/// <summary>
		/// Gibt die Rasterposition des Übergangs am Anfang der Kante zurück.
		/// </summary>
		public Node NodeBeforeEdge (Edge edge)
		{
			return (Node)fromMap [edge];
		}

		/// <summary>
		/// Gibt die Rasterposition des Übergangs am Ende der Kante zurück.
		/// </summary>
		public Node NodeAfterEdge (Edge edge)
		{
			return (Node)toMap [edge];
		}

		public List<IJunction> JunctionsAtNode (Node node)
		{
			return junctionMap [node];
		}

		public List<IJunction> JunctionsBeforeEdge (Edge edge)
		{
			Node node = NodeBeforeEdge (edge);
			return junctionMap.ContainsKey (node) ? junctionMap [node] : new List<IJunction>();
		}

		public List<IJunction> JunctionsAfterEdge (Edge edge)
		{
			Node node = NodeAfterEdge (edge);
			return junctionMap.ContainsKey (node) ? junctionMap [node] : new List<IJunction>();
		}

		public IEnumerable<Node> Nodes
		{
			get {
				return junctionMap.Keys;
			}
		}

		/// <summary>
		/// Aktualisiert die Zuordnung, wenn sich die Kanten geändert haben.
		/// </summary>
		public void OnEdgesChanged ()
		{
			BuildIndex ();
		}

		private void BuildIndex ()
		{
			fromMap.Clear ();
			toMap.Clear ();
			IndexRebuilt = () => {};
			junctionMap.Clear ();

			Node zero = new Node (0, 0, 0);
			for (int i = 0; i <= 2; ++i) {
				Edge edge1 = _edges [i * 2 + 0];
				Edge edge2 = _edges [i * 2 + 1];

				fromMap [edge1] = zero - edge1.Direction;
				toMap [edge1] = zero;
				fromMap [edge2] = zero;
				toMap [edge2] = zero + edge2.Direction;

				Node node = NodeAfterEdge (edge1);
				IJunction junction = new NodeModelInfo (nodeMap: this, from: edge1, to: edge2, node: node, index: i * 2);
				junctionMap.Add (node, junction);
			}

			IndexRebuilt ();
		}

		#endregion
	}
}
