/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 *
 * See the LICENSE file for full license details of the Knot3 project.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Models;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Eine Zuordnung zwischen Kanten und den dreidimensionalen Rasterpunkten, an denen sich die die Kantenübergänge befinden.
    /// </summary>
    public sealed class Grid : IGrid
    {
        private IScreen Screen;

        private class NodeContent
        {
            public List<Junction> Junctions = new List<Junction> ();
            public Hashtable PipesOut = new Hashtable ();
            public Hashtable PipesIn = new Hashtable ();

            public bool ContainsEdge (Edge edge)
            {
                return PipesIn.ContainsKey (edge) || PipesOut.ContainsKey (edge);
            }

            public bool ContainsJunction (Edge edgeFrom, Edge edgeTo)
            {
                return Junctions.Count > 0 && Junctions.Where (junction => junction.EdgeFrom == edgeFrom && junction.EdgeTo == edgeTo).Any ();
            }

            public void DeleteOldJunctions (Edge edgeFrom, Edge edgeTo)
            {
                Junctions.RemoveAll (j => j.EdgeFrom.Direction == edgeFrom.Direction && j.EdgeTo.Direction == edgeTo.Direction);
            }

            public Pipe GetPipe (Edge edge)
            {
                return PipesIn.ContainsKey (edge) ? PipesIn [edge] as Pipe : PipesOut [edge] as Pipe;
            }

            public void SetTick (Edge edgeFrom, Edge edgeTo)
            {
                Junctions.Where (junction => junction.EdgeFrom == edgeFrom && junction.EdgeTo == edgeTo)
                .ForEach (obj => obj.LastTick = CurrentTick);
            }

            public void Update (Edge edgeFrom, Edge edgeTo)
            {
                foreach (Junction junction in Junctions.Where (junction => junction.EdgeFrom == edgeFrom && junction.EdgeTo == edgeTo)) {
                    junction.OnGridUpdated ();
                }
            }

            private static Edge[] removedEdges = new Edge [100];

            public void RemoveOldStuff ()
            {
                RemoveOldPipes (PipesIn);
                RemoveOldPipes (PipesOut);

                Junctions.Where (obj => obj.LastTick != CurrentTick).ForEach (obj => obj.World = null);
                Junctions.RemoveAll (obj => obj.LastTick != CurrentTick);
            }

            private void RemoveOldPipes (Hashtable table)
            {
                int i = 0;
                foreach (Edge edge in table.Keys) {
                    Pipe pipe = (table [edge] as Pipe);
                    if (pipe.LastTick != CurrentTick) {
                        pipe.World = null;
                        removedEdges [i++] = edge;
                    }
                }
                for (--i; i >= 0; --i) {
                    table.Remove (removedEdges [i]);
                }
            }
        };

        private Dictionary<Node, NodeContent> grid = new Dictionary<Node, NodeContent> ();
        private Dictionary<Edge, Node> nodeBeforeEdge = new Dictionary<Edge, Node> ();
        private Dictionary<Edge, Node> nodeAfterEdge = new Dictionary<Edge, Node> ();
        private static int CurrentTick = 0;

        private NodeContent AtNode (Node node)
        {
            if (!grid.ContainsKey (node)) {
                return grid [node] = new NodeContent ();
            }
            else {
                return grid [node];
            }
        }

        /// <summary>
        /// Die Skalierung, die bei einer Konvertierung in einen Vector3 des XNA-Frameworks durch die ToVector ()-Methode der Node-Objekte verwendet wird.
        /// </summary>
        public int Scale { get; set; }

        public Knot Knot { get; set; }

        public World World { get; set; }

        public Vector3 Offset { get; set; }

        public Grid (IScreen screen)
        {
            Screen = screen;
        }

        public IEnumerable<Pipe> Pipes
        {
            get {
                foreach (NodeContent content in grid.Values) {
                    foreach (Pipe pipe in content.PipesOut.Values) {
                        if (pipe.LastTick == CurrentTick) {
                            yield return pipe;
                        }
                    }
                }
            }
        }

        public IEnumerable<Junction> Junctions
        {
            get {
                foreach (NodeContent content in grid.Values) {
                    // zeige zwischen zwei Kanten in der selben Richtung keinen Übergang an,
                    // wenn sie alleine an dem Kantenpunkt sind
                    if (content.Junctions.Count == 1 && content.Junctions [0].EdgeFrom.Direction == content.Junctions [0].EdgeTo.Direction) {
                        continue;
                    }
                    foreach (Junction junction in content.Junctions) {
                        if (junction.LastTick == CurrentTick) {
                            yield return junction;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gibt die Rasterposition des Übergangs am Anfang der Kante zurück.
        /// </summary>
        public Node NodeBeforeEdge (Edge edge)
        {
            return nodeBeforeEdge.ContainsKey (edge) ? nodeBeforeEdge [edge] : null;
        }

        /// <summary>
        /// Gibt die Rasterposition des Übergangs am Ende der Kante zurück.
        /// </summary>
        public Node NodeAfterEdge (Edge edge)
        {
            return nodeAfterEdge.ContainsKey (edge) ? nodeAfterEdge [edge] : null;
        }

        public List<Junction> JunctionsAtNode (Node node)
        {
            return AtNode (node).Junctions;
        }

        public List<Junction> JunctionsBeforeEdge (Edge edge)
        {
            return JunctionsAtNode (NodeBeforeEdge (edge));
        }

        public List<Junction> JunctionsAfterEdge (Edge edge)
        {
            return JunctionsAtNode (NodeAfterEdge (edge));
        }

        public IEnumerable<Node> Nodes
        {
            get {
                return grid.Keys;
            }
        }

        /// <summary>
        /// Aktualisiert die Zuordnung, wenn sich die Kanten geändert haben.
        /// </summary>
        public void Update ()
        {
            BuildIndex ();
        }

        private Pipe newPipe (Edge edge, Node node1, Node node2)
        {
            Pipe pipe = new Pipe (screen: Screen, grid: this, knot: Knot, edge: edge, node1: node1, node2: node2);
            pipe.Knot = Knot;
            pipe.IsVisible = true;
            pipe.World = World;
            return pipe;
        }

        private Junction newJunction (Edge edgeA, Edge edgeB, Node node, int index)
        {
            Junction junction = new Junction (screen: Screen, grid: this, from: edgeA, to: edgeB, node: node, index: index);
            junction.IsVisible = true;
            junction.World = World;
            return junction;
        }

        private void BuildIndex ()
        {
            ++CurrentTick;

            Action later = () => {};

            HashSet<Node> updatedNodes = new HashSet<Node> ();
            Profiler.ProfileDelegate ["Grid.Pipes"] = () => {
                float x = Offset.X, y = Offset.Y, z = Offset.Z;

                foreach (Edge edge in Knot) {
                    Node node1 = new Node ((int)x, (int)y, (int)z);
                    Vector3 v = edge.Direction.Vector;
                    x += v.X;
                    y += v.Y;
                    z += v.Z;
                    Node node2 = new Node ((int)x, (int)y, (int)z);

                    NodeContent content1 = AtNode (node1);
                    NodeContent content2 = AtNode (node2);
                    if (!content1.ContainsEdge (edge) || !content2.ContainsEdge (edge)) {
                        Pipe _pipe = newPipe (edge, node1, node2);
                        content1.PipesOut [edge] = _pipe;
                        content2.PipesIn [edge] = _pipe;
                        updatedNodes.Add (node1);
                        updatedNodes.Add (node2);
                    }
                    Pipe pipe = content1.GetPipe (edge);
                    pipe.LastTick = CurrentTick;
                    later += () => {
                        pipe.Knot = Knot;
                        pipe.OnGridUpdated ();
                    };

                    nodeBeforeEdge [edge] = node1;
                    nodeAfterEdge [edge] = node2;
                }
            };

            Profiler.ProfileDelegate ["Grid.Junctions"] = () => {
                List<Edge> EdgeList = Knot.ToList ();
                for (int n = 0; n < EdgeList.Count; n++) {
                    Edge edgeA = EdgeList.At (n);
                    Edge edgeB = EdgeList.At (n + 1);
                    Node node = NodeAfterEdge (edgeA);
                    NodeContent content = AtNode (node);
                    if (updatedNodes.Contains (node)) {
                        if (!content.ContainsJunction (edgeA, edgeB)) {
                            content.DeleteOldJunctions (edgeA, edgeB);
                            content.Junctions.Add (newJunction (edgeA, edgeB, node, n));
                        }

                        later += () => {
                            content.Update (edgeA, edgeB);
                        };
                    }
                    content.SetTick (edgeA, edgeB);
                }
            };

            Profiler.ProfileDelegate ["Grid.Remove"] = () => {
                foreach (NodeContent content in grid.Values) {
                    content.RemoveOldStuff ();
                }
            };

            Profiler.ProfileDelegate ["Grid.Later"] = () => later ();
        }
    }
}
