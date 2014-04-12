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
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Data;
using Knot3.Game.Models;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Eine Zuordnung zwischen Kanten und den dreidimensionalen Rasterpunkten, an denen sich die die Kantenübergänge befinden.
    /// </summary>
    public sealed class Grid : IGrid
    {
        private IScreen Screen;

        private sealed class NodeContent
        {
            public List<Junction> Junctions = new List<Junction> ();
            private Dictionary<Edge, Pipe> PipesOut = new Dictionary<Edge, Pipe> ();
            private Dictionary<Edge, Pipe> PipesIn = new Dictionary<Edge, Pipe> ();

            public IEnumerable<Edge> EdgesOutgoing { get { return PipesOut.Keys; } }

            public IEnumerable<Edge> EdgesIncoming { get { return PipesIn.Keys; } }

            public IEnumerable<Pipe> PipesOutgoing { get { return PipesOut.Values; } }

            public IEnumerable<Pipe> PipesIncoming { get { return PipesIn.Values; } }

            public void AddPipeOutgoing (Edge edge, Pipe _pipe)
            {
                PipesOut [edge] = _pipe;
            }

            public void AddPipeIncoming (Edge edge, Pipe _pipe)
            {
                PipesIn [edge] = _pipe;
            }

            public bool ContainsEdgeWithDirection (Direction direction)
            {
                return PipesIn.Keys.Any (edge => edge.Direction == direction) || PipesOut.Keys.Any (edge => edge.Direction == direction);
            }

            public bool ContainsEdgeWithAxis (Axis axis)
            {
                return PipesIn.Keys.Any (edge => edge.Direction.Axis == axis) || PipesOut.Keys.Any (edge => edge.Direction.Axis == axis);
            }

            public Edge EdgeWithAxis (Axis axis)
            {
                foreach (Edge edge in PipesIn.Keys.Where (edge => edge.Direction.Axis == axis)) {
                    return edge;
                }
                foreach (Edge edge in PipesOut.Values.Where (pipe => pipe.Edge.Direction.Axis == axis).Select (pipe => pipe.Edge)) {
                    return edge;
                }
                return null;
            }

            public Edge EdgeWithDirection (Direction direction)
            {
                foreach (Edge edge in PipesIn.Keys.Where (edge => edge.Direction == direction.Reverse)) {
                    return edge;
                }
                foreach (Edge edge in PipesOut.Keys.Where (edge => edge.Direction == direction)) {
                    return edge;
                }
                return null;
            }

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

            public void SetTick (Edge edgeFrom, Edge edgeTo, int tick)
            {
                Junctions.Where (junction => junction.EdgeFrom == edgeFrom && junction.EdgeTo == edgeTo)
                .ForEach (obj => obj.LastTick = tick);
            }

            public void Update (Edge edgeFrom, Edge edgeTo)
            {
                foreach (Junction junction in Junctions.Where (junction => junction.EdgeFrom == edgeFrom && junction.EdgeTo == edgeTo)) {
                    junction.OnGridUpdated ();
                }
            }

            private static Edge[] removedEdges = new Edge [100];

            public void RemoveOldStuff (int tick)
            {
                RemoveOldPipes (PipesIn, tick);
                RemoveOldPipes (PipesOut, tick);

                Junctions.Where (obj => obj.LastTick != tick).ForEach (obj => obj.World = null);
                Junctions.RemoveAll (obj => obj.LastTick != tick);
            }

            private void RemoveOldPipes (Dictionary<Edge, Pipe> table, int tick)
            {
                int i = 0;
                foreach (Edge edge in table.Keys) {
                    Pipe pipe = (table [edge] as Pipe);
                    if (pipe.LastTick != tick) {
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
        private Dictionary<Vector3, Surface> surfaceGrid = new Dictionary<Vector3, Surface> ();
        private int CurrentTick = 0;

        private NodeContent AtNode (Node node)
        {
            if (!grid.ContainsKey (node)) {
                return grid [node] = new NodeContent ();
            }
            else {
                return grid [node];
            }
        }

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
                if (pipeCache.Count == 0) {
                    foreach (Node node in grid.Keys) {
                        NodeContent content = grid [node];
                        foreach (Pipe pipe in content.PipesOutgoing) {
                            if (pipe.LastTick == CurrentTick) {
                                pipeCache.Add (pipe);
                            }
                        }
                    }
                }
                for (int i = 0; i < pipeCache.Count; ++i) {
                    yield return pipeCache [i];
                }
            }
        }

        private ArrayList<Pipe> pipeCache = new ArrayList<Pipe> (capacity: 100, step: 200);

        public IEnumerable<Junction> Junctions
        {
            get {
                if (junctionCache.Count == 0) {
                    foreach (Node node in grid.Keys) {
                        NodeContent content = grid [node];
                        // zeige zwischen zwei Kanten in der selben Richtung keinen Übergang an,
                        // wenn sie alleine an dem Kantenpunkt sind
                        if (content.Junctions.Count == 1 && content.Junctions [0].EdgeFrom.Direction == content.Junctions [0].EdgeTo.Direction) {
                            continue;
                        }
                        foreach (Junction junction in content.Junctions) {
                            if (junction.LastTick == CurrentTick) {
                                junctionCache.Add (junction);
                            }
                        }
                    }
                }
                for (int i = 0; i < junctionCache.Count; ++i) {
                    yield return junctionCache [i];
                }
            }
        }

        private ArrayList<Junction> junctionCache = new ArrayList<Junction> (capacity: 100, step: 200);

        public IEnumerable<Surface> Surfaces
        {
            get {
                if (surfaceCache.Count == 0) {
                    foreach (Surface surface in surfaceGrid.Values) {
                        if (surface.LastTick == CurrentTick) {
                            surfaceCache.Add (surface);
                        }
                    }
                }
                for (int i = 0; i < surfaceCache.Count; ++i) {
                    yield return surfaceCache [i];
                }
            }
        }

        private ArrayList<Surface> surfaceCache = new ArrayList<Surface> (capacity: 100, step: 200);

        public List<Junction> JunctionsAtNode (Node node)
        {
            return AtNode (node).Junctions;
        }

        public List<Junction> JunctionsBeforeEdge (Edge edge)
        {
            return JunctionsAtNode (edge.StartNode);
        }

        public List<Junction> JunctionsAfterEdge (Edge edge)
        {
            return JunctionsAtNode (edge.EndNode);
        }

        public Junction JunctionBeforeEdge (Edge edge)
        {
            foreach (Junction junction in JunctionsAtNode (edge.StartNode)) {
                if (junction.EdgeTo == edge) {
                    return junction;
                }
            }
            return null;
        }

        public Junction JunctionAfterEdge (Edge edge)
        {
            foreach (Junction junction in JunctionsAtNode (edge.EndNode)) {
                if (junction.EdgeFrom == edge) {
                    return junction;
                }
            }
            return null;
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
        public void Update (bool withSurfaces)
        {
            UpdateIndex (withSurfaces: withSurfaces);
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

        private Surface newSurface (SurfaceLocation location)
        {
            Surface surface = new Surface (screen: Screen, location: location);
            surface.IsVisible = true;
            surface.World = World;
            return surface;
        }

        private void UpdateIndex (bool withSurfaces)
        {
            ++CurrentTick;

            Action executeLater = () => {};

            HashSet<Node> updatedNodes = new HashSet<Node> ();
            Profiler.ProfileDelegate ["Grid.Pipes"] = () => {
                // update the pipes
                UpdatePipes (updatedNodes: updatedNodes, executeLater: ref executeLater);
            };

            Profiler.ProfileDelegate ["Grid.Junctions"] = () => {
                // update the junctions
                UpdateJunctions (updatedNodes: updatedNodes, executeLater: ref executeLater);
            };

            Profiler.ProfileDelegate ["Grid.Remove"] = () => {
                // Remove old pipes and junctions
                foreach (NodeContent content in grid.Values) {
                    content.RemoveOldStuff (CurrentTick);
                }
            };

            Profiler.ProfileDelegate ["Grid.Later"] = executeLater;

            pipeCache.Clear ();
            junctionCache.Clear ();
            surfaceCache.Clear ();

            // ------------------------------

            if (Config.Default ["video", "surfaces", false]) {
                if (withSurfaces) {
                    UpdateSurfaces ();
                }
                else {
                    foreach (Surface surface in surfaceGrid.Values) {
                        surface.LastTick = CurrentTick;
                    }
                }
            }
        }

        private void UpdatePipes (HashSet<Node> updatedNodes, ref Action executeLater)
        {
            foreach (Edge edge in Knot) {
                NodeContent content1 = AtNode (edge.StartNode);
                NodeContent content2 = AtNode (edge.EndNode);
                if (!content1.ContainsEdge (edge) || !content2.ContainsEdge (edge)) {
                    Pipe _pipe = newPipe (edge, edge.StartNode, edge.EndNode);
                    content1.AddPipeOutgoing (edge, _pipe);
                    content2.AddPipeIncoming (edge, _pipe);
                    updatedNodes.Add (edge.StartNode);
                    updatedNodes.Add (edge.EndNode);
                }
                Pipe pipe = content1.GetPipe (edge);
                pipe.LastTick = CurrentTick;
                executeLater += () => {
                    pipe.Knot = Knot;
                    pipe.OnGridUpdated ();
                };
            }
        }

        private void UpdateJunctions (HashSet<Node> updatedNodes, ref Action executeLater)
        {
            Edge[] edgeArray = Knot.ToArray ();
            for (int n = 0; n < edgeArray.Length; n++) {
                Edge edgeA = edgeArray.At (n);
                Edge edgeB = edgeArray.At (n + 1);
                Node node = edgeA.EndNode;
                NodeContent content = AtNode (node);
                if (updatedNodes.Contains (node)) {
                    if (!content.ContainsJunction (edgeA, edgeB)) {
                        content.DeleteOldJunctions (edgeA, edgeB);
                        content.Junctions.Add (newJunction (edgeA, edgeB, node, n));
                    }

                    executeLater += () => {
                        content.Update (edgeA, edgeB);
                    };
                }
                content.SetTick (edgeA, edgeB, CurrentTick);
            }
        }

        private void UpdateSurfaces ()
        {
            // Finde den kleinsten und den größten Rasterpunkt
            Node minNode = Node.Zero, maxNode = Node.Zero;
            foreach (Edge edge in Knot) {
                minNode = Node.Min (minNode, edge.StartNode);
                maxNode = Node.Max (maxNode, edge.StartNode);
            }
            minNode -= Node.One;
            maxNode += Node.One;
            Node sizeNode = maxNode - minNode;

            Dictionary<Vector3, SurfaceLocation> surfaceLocations = new Dictionary<Vector3, SurfaceLocation> ();

            // Finde alle Flächenpositionen
            Profiler.ProfileDelegate ["Grid.SurfLocs"] = () => {
                // für alle drei Achsen -> "axis"...
                foreach (Axis axis in Axis.Values) {
                    Axis side1 = axis.Side1;
                    Axis side2 = axis.Side2;
                    Axis[] otherSides = new Axis[] { side1, side2 };

                    // wähle eine Position auf der "Startebene"
                    for (Node tmp0 = Node.Zero; tmp0 [side1] < sizeNode [side1]; tmp0 += side1) {
                        for (Node tmp1 = tmp0; tmp1 [side2] < sizeNode [side2]; tmp1 += side2) {
                            Log.Debug ("plane=", (minNode, tmp1), ", towards=", axis, ", side1=", side1);

                            // für alle anderen Achsen -> "otherSide"
                            foreach (Axis otherSide in otherSides) {
                                Edge previousEdge = null;
                                ArrayList<Node> nodesBetweenEdges = new ArrayList<Node> ();

                                // iteriere entlang der Achse
                                for (Node tmp2 = tmp1; tmp2 [axis] < sizeNode [axis]; tmp2 += axis) {
                                    // die aktuelle Node
                                    Node currentNode = minNode + tmp2;
                                    // die Kante an der Node, die entlang der "otherSide"-Achse verläuft, falls vorhanden
                                    Edge currentEdge = AtNode (currentNode).EdgeWithDirection (Direction.FromAxis (otherSide));

                                    if (currentEdge != null) {
                                        Log.Debug ("  node with edge: node=", currentNode, ", edge=", currentEdge, ", edge.Axis=", otherSide);
                                    }

                                    // wenn wir eine Kante gefunden haben und das nicht das erste Mal ist
                                    if (previousEdge != null && currentEdge != null) {
                                        Log.Debug ("  creating surfaces: (!!!)");
                                        Log.Debug ("    nodes between edges: ", string.Join (", ", nodesBetweenEdges));

                                        // iteriere über die Rasterpunkte dazwischen
                                        foreach (Node node in nodesBetweenEdges) {
                                            // berechne die Mitte der Fläche
                                            Vector3 surfaceCenter = node.Vector + Direction.FromAxis (axis) * Node.Scale / 2 + Direction.FromAxis (otherSide) * Node.Scale / 2;
                                            Log.Debug ("    surface at node: node=", node, ", middle=", surfaceCenter);

                                            // erstelle ein SurfaceLocation-Objekt, falls noch nicht vorhanden
                                            if (!surfaceLocations.ContainsKey (surfaceCenter)) {
                                                surfaceLocations [surfaceCenter] = new SurfaceLocation (location: surfaceCenter);
                                            }
                                            SurfaceLocation location = surfaceLocations [surfaceCenter];

                                            // die vorherige Kante
                                            location.Sides.Add (new SurfaceSide {
                                                Edge = previousEdge,
                                                Direction = previousEdge.Direction,
                                                NodeA = previousEdge.StartNode,
                                                NodeB = previousEdge.EndNode,
                                            });
                                            // die aktuelle Kante
                                            location.Sides.Add (new SurfaceSide {
                                                Edge = currentEdge,
                                                Direction = currentEdge.Direction,
                                                NodeA = currentEdge.StartNode,
                                                NodeB = currentEdge.EndNode,
                                            });
                                            // die Richtung dazwischen
                                            location.Sides.Add (new SurfaceSide {
                                                Edge = null,
                                                Direction = Direction.FromAxis (axis),
                                                NodeA = previousEdge.StartNode,
                                                NodeB = previousEdge.StartNode + axis,
                                            });
                                        }

                                        nodesBetweenEdges.Clear ();
                                    }

                                    if (currentEdge != null) {
                                        previousEdge = currentEdge;
                                    }

                                    if (previousEdge != null) {
                                        nodesBetweenEdges.Add (currentNode);
                                    }
                                }
                            }
                        }
                    }
                }
            };

            Profiler.ProfileDelegate ["Grid.Surfaces"] = () => {
                foreach (SurfaceLocation location in surfaceLocations.Values) {
                    if (location.Sides.Count >= 2) {
                        Log.Debug ("location: ", location.Location);

                        if (!surfaceGrid.ContainsKey (location.Location)) {
                            try {
                                Surface surface = newSurface (location: location);
                                surfaceGrid [location.Location] = surface;
                            }
                            catch (ArgumentException) {
                                continue;
                            }
                        }
                        surfaceGrid [location.Location].LastTick = CurrentTick;
                    }
                }
            };

            Profiler.ProfileDelegate ["Grid.RemoveSurf"] = () => {
                // Remove old surfaces
                Vector3[] locations = surfaceGrid.Keys.ToArray ();
                foreach (Vector3 location in locations) {
                    if (surfaceGrid [location].LastTick != CurrentTick) {
                        surfaceGrid.Remove (location);
                    }
                }
            };
        }
    }

    public class SurfaceLocation
    {
        public ArrayList<SurfaceSide> Sides = new ArrayList<SurfaceSide> (12);
        public Vector3 Location;

        public SurfaceLocation (Vector3 location)
        {
            Location = location;
        }
    }

    public struct SurfaceSide {
        public Node NodeA;
        public Node NodeB;
        public Edge Edge;
        public Direction Direction;

        public Node[] Nodes { get { return new Node[] { NodeA, NodeB }; } }

        public Vector3 Center { get { return NodeA.CenterBetween (NodeB); } }
    }
}
