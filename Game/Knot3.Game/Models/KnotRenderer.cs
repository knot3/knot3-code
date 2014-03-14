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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Data;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Erstellt aus einem Knoten-Objekt die zu dem Knoten gehörenden 3D-Modelle sowie die 3D-Modelle der Pfeile,
    /// die nach einer Auswahl von Kanten durch den Spieler angezeigt werden. Ist außerdem ein IGameObject und ein
    /// Container für die erstellten Spielobjekte.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class KnotRenderer : IGameObject, IEnumerable<IGameObject>
    {
        private IScreen screen;

        /// <summary>
        /// Enthält Informationen über die Position des Knotens.
        /// </summary>
        public GameObjectInfo Info { get; private set; }

        /// <summary>
        /// Die Spielwelt, in der die 3D-Modelle erstellt werden sollen.
        /// </summary>
        public World World { get; set; }

        public Matrix WorldMatrix { get { return Matrix.Identity; } }

        /// <summary>
        /// Die Liste der 3D-Modelle der Pfeile,
        /// die nach einer Auswahl von Kanten durch den Spieler angezeigt werden.
        /// </summary>
        private List<ArrowModel> arrows;

        /// <summary>
        /// Die Liste der 3D-Modelle der Kantenübergänge.
        /// </summary>
        private List<JunctionModel> nodes;

        /// <summary>
        /// Die Liste der 3D-Modelle der Kanten.
        /// </summary>
        private List<PipeModel> pipes;

        /// <summary>
        /// Die Liste der Flächen zwischen den Kanten.
        /// </summary>
        private HashSet<TexturedRectangle> rectangles;
        private List<GameModel> debugModels;

        /// <summary>
        /// Der Knoten, für den 3D-Modelle erstellt werden sollen.
        /// </summary>
        public Knot Knot
        {
            get {
                return knot;
            }
            set {
                knot = value;
                knot.EdgesChanged += OnEdgesChanged;
                knot.SelectionChanged += OnSelectionChanged;
                OnEdgesChanged ();
            }
        }

        private Knot knot;

        public Knot VirtualKnot
        {
            get {
                return virtualKnot;
            }
            set {
                if (virtualKnot != value) {
                    virtualKnot = value;
                    OnVirtualKnotAssigned ();
                }
            }
        }

        private Knot virtualKnot;

        /// <summary>
        /// Der Zwischenspeicher für die 3D-Modelle der Kanten. Hier wird das Fabrik-Entwurfsmuster verwendet.
        /// </summary>
        private ModelFactory pipeFactory;

        /// <summary>
        /// Der Zwischenspeicher für die 3D-Modelle der Kantenübergänge. Hier wird das Fabrik-Entwurfsmuster verwendet.
        /// </summary>
        private ModelFactory nodeFactory;

        /// <summary>
        /// Der Zwischenspeicher für die 3D-Modelle der Pfeile. Hier wird das Fabrik-Entwurfsmuster verwendet.
        /// </summary>
        private ModelFactory arrowFactory;

        /// <summary>
        /// Die Zuordnung zwischen Kanten und den dreidimensionalen Rasterpunkten, an denen sich die die Kantenübergänge befinden.
        /// </summary>
        private INodeMap nodeMap;

        /// <summary>
        /// Gibt an, ob Pfeile anzuzeigen sind. Wird aus der Einstellungsdatei gelesen.
        /// </summary>
        private bool showArrows { get { return Config.Default ["video", "arrows", false]; } }

        /// <summary>
        /// Erstellt ein neues KnotRenderer-Objekt für den angegebenen Spielzustand mit den angegebenen
        /// Spielobjekt-Informationen, die unter Anderem die Position des Knotenursprungs enthalten.
        /// </summary>
        public KnotRenderer (IScreen screen, Vector3 position)
        {
            this.screen = screen;
            Info = new GameObjectInfo (position: position);
            pipes = new List<PipeModel> ();
            nodes = new List<JunctionModel> ();
            arrows = new List<ArrowModel> ();
            rectangles = new HashSet<TexturedRectangle> ();
            debugModels = new List<GameModel> ();
            pipeFactory = new ModelFactory ((s, i) => new PipeModel (s, i as Pipe));
            nodeFactory = new ModelFactory ((s, i) => new JunctionModel (s, i as Junction));
            arrowFactory = new ModelFactory ((s, i) => new ArrowModel (s, i as Arrow));
            nodeMap = new NodeMap ();
        }

        /// <summary>
        /// Gibt den Ursprung des Knotens zurück.
        /// </summary>
        public Vector3 Center { get { return Info.Position; } }

        /// <summary>
        /// Ruft die Intersects (Ray)-Methode der Kanten, Übergänge und Pfeile auf und liefert das beste Ergebnis zurück.
        /// </summary>
        public GameObjectDistance Intersects (Ray ray)
        {
            GameObjectDistance nearest = null;
            if (!screen.InputManager.GrabMouseMovement) {
                foreach (PipeModel pipe in pipes) {
                    GameObjectDistance intersection = pipe.Intersects (ray);
                    if (intersection != null) {
                        if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
                            nearest = intersection;
                        }
                    }
                }
                foreach (ArrowModel arrow in arrows) {
                    GameObjectDistance intersection = arrow.Intersects (ray);
                    if (intersection != null) {
                        if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
                            nearest = intersection;
                        }
                    }
                }
            }
            return nearest;
        }

        /// <summary>
        /// Wird mit dem EdgesChanged-Event des Knotens verknüft.
        /// </summary>
        private void OnEdgesChanged ()
        {
            nodeMap.Edges = knot;
            nodeMap.Offset = Info.Position + knot.OffSet;
            nodeMap.OnEdgesChanged ();

            //Log.Debug ("=> render Knot #", knot.Count (), " = ", string.Join (", ", from c in knot select c.Direction));

            CreatePipes (knot);
            if (Config.Default ["debug", "show-startedge-direction", false]) {
                CreateStartArrow ();
            }
            CreateNodes ();
            if (showArrows) {
                CreateArrows ();
            }
            CreateRectangles ();

            World.Redraw = true;
        }

        private void OnVirtualKnotAssigned ()
        {
            nodeMap.Edges = virtualKnot;
            nodeMap.Offset = Info.Position + virtualKnot.OffSet;
            nodeMap.OnEdgesChanged ();

            CreatePipes (virtualKnot);
            CreateNodes ();

            World.Redraw = true;
        }

        private void OnSelectionChanged ()
        {
            nodeMap.Edges = knot;
            nodeMap.Offset = knot.OffSet;
            if (showArrows) {
                CreateArrows ();
            }
            World.Redraw = true;
        }

        private void CreatePipes (Knot knot)
        {
            foreach (PipeModel pipemodel in pipes) {
                pipemodel.World=null;
            }
            pipes.Clear ();
            foreach (Edge edge in knot) {
                Pipe info = new Pipe (nodeMap, knot, edge);
                PipeModel pipe = pipeFactory [screen, info] as PipeModel;
                pipe.Info.Knot = knot;
                pipe.Info.IsVisible = true;
                pipe.IsVirtual = !knot.Contains (edge);
                pipe.World = World;
                pipes.Add (pipe);
            }
        }

        private void CreateStartArrow ()
        {
            Edge edge = knot.ElementAt (0);
            Vector3 towardsCamera = World.Camera.PositionToTargetDirection;
            Arrow info = new Arrow (
                position: Vector3.Zero - 10 * towardsCamera - 10 * towardsCamera.PrimaryDirection (),
                direction: edge
            );
            ArrowModel arrow = arrowFactory [screen, info] as ArrowModel;
            arrow.Info.IsVisible = true;
            arrow.World = World;
            debugModels.Add (arrow);
        }

        private void CreateNodes ()
        {
            foreach (JunctionModel junctionmodel in nodes) {
                junctionmodel.World=null;
            }
            nodes.Clear ();

            foreach (Node node in nodeMap.Nodes) {
                List<IJunction> junctions = nodeMap.JunctionsAtNode (node);
                // zeige zwischen zwei Kanten in der selben Richtung keinen Übergang an,
                // wenn sie alleine an dem Kantenpunkt sind
                if (junctions.Count == 1 && junctions [0].EdgeFrom.Direction == junctions [0].EdgeTo.Direction) {
                    continue;
                }

                foreach (Junction junction in junctions.OfType<Junction>()) {
                    JunctionModel model = nodeFactory [screen, junction] as JunctionModel;
                    model.IsVirtual = !knot.Contains (junction.EdgeFrom) || !knot.Contains (junction.EdgeTo);
                    model.World = World;
                    nodes.Add (model);
                }
            }
        }

        private void CreateArrows ()
        {
            foreach (ArrowModel arrowmodel in arrows) {
                arrowmodel.World=null;
            }
            arrows.Clear ();
            int selectedEdgesCount = knot.SelectedEdges.Count ();
            if (selectedEdgesCount > 0) {
                CreateArrow (knot.SelectedEdges.ElementAt ((int)selectedEdgesCount / 2));
            }
        }

        private void CreateArrow (Edge edge)
        {
            try {
                Node node1 = nodeMap.NodeBeforeEdge (edge);
                Node node2 = nodeMap.NodeAfterEdge (edge);
                foreach (Direction direction in Direction.Values) {
                    if (knot.IsValidDirection (direction)) {
                        Vector3 towardsCamera = World.Camera.PositionToTargetDirection;
                        Arrow info = new Arrow (
                            position: node1.CenterBetween (node2) - 25 * towardsCamera - 25 * towardsCamera.PrimaryDirection (),
                            direction: direction
                        );
                        ArrowModel arrow = arrowFactory [screen, info] as ArrowModel;
                        arrow.Info.IsVisible = true;
                        arrow.World = World;
                        arrows.Add (arrow);
                    }
                }
            }
            catch (NullReferenceException ex) {
                Log.Debug (ex);
            }
        }

        private void CreateRectangles ()
        {
            foreach (TexturedRectangle rectangle in rectangles) {
                rectangle.Dispose ();
            }
            rectangles.Clear ();

            RectangleMap rectMap = new RectangleMap (nodeMap);
            foreach (Edge edge in knot) {
                rectMap.AddEdge (edge: edge, isVirtual: false);
            }

            int newRectangles;
            do {
                newRectangles = 0;
                ValidRectanglePosition[] validPositions = rectMap.ValidPositions ().ToArray ();
                foreach (ValidRectanglePosition validPosition in validPositions) {
                    //Log.Debug ("validPosition=", validPosition);
                    newRectangles += CreateRectangle (validPosition, ref rectMap) ? 1 : 0;
                }
            }
            while (newRectangles > 0);
        }

        private bool CreateRectangle (ValidRectanglePosition rect, ref RectangleMap rectMap)
        {
            Edge edgeAB = rect.EdgeAB;
            Edge edgeCD = rect.EdgeCD;
            Node nodeA = rect.NodeA;
            Node nodeB = rect.NodeB;
            Node nodeC = rect.NodeC;
            Node nodeD = rect.NodeD;

            if (rect.IsVirtual || edgeAB.Rectangles.Intersect (edgeCD.Rectangles).Any ()) {
                Texture2D texture;
                if (rect.NodeB == rect.NodeC) {
                    texture = CreateDiagonalRectangleTexture (edgeAB.Color, edgeCD.Color);
                }
                else {
                    texture = CreateParallelRectangleTexture (edgeAB.Color, edgeCD.Color);
                }

                TexturedRectangleInfo info = new TexturedRectangleInfo (
                    texture: texture,
                    origin: rect.Position,
                    left: edgeAB.Direction,
                    width: Node.Scale,
                    up: edgeCD.Direction.Reverse,
                    height: Node.Scale
                );
                TexturedRectangle rectangle = new TexturedRectangle (screen: screen, info: info);
                rectangle.World = World;

                if (SystemInfo.IsRunningOnLinux ()) {
                    Log.Debug ("rectangle=", rectangle);
                }

                if (!rectangles.Contains (rectangle)) {
                    rectangles.Add (rectangle);

                    if (rect.NodeB == rect.NodeC) {
                        if (!rectMap.ContainsEdge (nodeB - edgeAB, nodeB + edgeAB + edgeCD)) {
                            rectMap.AddEdge (edge: edgeCD, nodeA: nodeB - edgeAB, nodeB: nodeB + edgeAB + edgeCD, isVirtual: true);
                        }
                        if (!rectMap.ContainsEdge (nodeB - edgeAB + edgeCD, nodeB + edgeCD)) {
                            rectMap.AddEdge (edge: edgeAB, nodeA: nodeB - edgeAB + edgeCD, nodeB: nodeB + edgeCD, isVirtual: true);
                        }
                    }
                    else {
                        Edge edgeAC = new Edge ((rect.NodeC - rect.NodeA).ToDirection ());
                        Edge edgeBD = new Edge ((rect.NodeD - rect.NodeB).ToDirection ());
                        if (!rectMap.ContainsEdge (nodeA + edgeAC, nodeC)) {
                            rectMap.AddEdge (edge: edgeAC, nodeA: nodeA + edgeAC, nodeB: nodeC, isVirtual: true);
                        }
                        if (!rectMap.ContainsEdge (nodeB + edgeBD, nodeD)) {
                            rectMap.AddEdge (edge: edgeBD, nodeA: nodeB + edgeBD, nodeB: nodeD, isVirtual: true);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private Texture2D CreateParallelRectangleTexture (Color fromColor, Color toColor)
        {
            int width = 50;
            int height = 50;
            Texture2D texture = new Texture2D (screen.GraphicsDevice, width, height);
            Color[] colors = new Color [width * height];
            for (int w = 0; w < width; ++w) {
                for (int h = 0; h < height; ++h) {
                    colors [h * width + w] = toColor.Mix (fromColor, 0.5f + (float)(width / 2 - w) / (float)(width / 2) * 0.9f);
                }
            }
            texture.SetData (colors);
            return texture;
        }

        private Texture2D CreateDiagonalRectangleTexture (Color fromColor, Color toColor)
        {
            int width = 50;
            int height = 50;
            Texture2D texture = new Texture2D (screen.GraphicsDevice, width, height);
            Color[] colors = new Color [width * height];
            for (int w = 0; w < width; ++w) {
                for (int h = 0; h < height; ++h) {
                    colors [h * width + w] = toColor.Mix (fromColor, 0.5f + (float)(w - h) / (float)Math.Max (width, height)) * 0.9f;
                }
            }
            texture.SetData (colors);
            return texture;
        }

        /// <summary>
        /// Ruft die Update ()-Methoden der Kanten, Übergänge und Pfeile auf.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public void Update (GameTime time)
        {
            foreach (PipeModel pipe in pipes) {
                pipe.Update (time);
            }
            foreach (JunctionModel node in nodes) {
                node.Update (time);
            }
            foreach (ArrowModel arrow in arrows) {
                arrow.Update (time);
            }
            foreach (TexturedRectangle rectangle in rectangles) {
                rectangle.Update (time);
            }

            if (time.TotalGameTime.Seconds % 10 == 0) {
                int count = pipeFactory.Count + nodeFactory.Count + arrowFactory.Count;
                if (count > 0) {
                    Profiler.Values ["# Cached Models"] = count;
                    if (time.TotalGameTime.Seconds % 20 == 0) {
                        Log.Debug ("Clear Model Cache: ", count, " Models deleted");
                        pipeFactory.Clear ();
                        nodeFactory.Clear ();
                        arrowFactory.Clear ();
                    }
                }
            }
        }

        /// <summary>
        /// Ruft die Draw ()-Methoden der Kanten, Übergänge und Pfeile auf.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public void Draw (GameTime time)
        {
            if (Info.IsVisible) {
                Profiler.Values ["# InFrustum"] = 0;
                Profiler.Values ["RenderEffect"] = 0;
                Profiler.ProfileDelegate ["Pipes"] = () => {
                    foreach (PipeModel pipe in pipes) {
                        pipe.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Nodes"] = () => {
                    foreach (JunctionModel node in nodes) {
                        node.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Arrows"] = () => {
                    foreach (ArrowModel arrow in arrows) {
                        arrow.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Rectangles"] = () => {
                    foreach (TexturedRectangle rectangle in rectangles) {
                        rectangle.Draw (time);
                    }
                };
                foreach (GameModel model in debugModels) {
                    model.Draw (time);
                }
                Profiler.Values ["# Pipes"] = pipes.Count;
                Profiler.Values ["# Nodes"] = nodes.Count;
            }
        }

        /// <summary>
        /// Gibt einen Enumerator der aktuell vorhandenen 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            foreach (PipeModel pipe in pipes) {
                yield return pipe;
            }
            foreach (JunctionModel node in nodes) {
                yield return node;
            }
            foreach (ArrowModel arrow in arrows) {
                yield return arrow;
            }
            foreach (TexturedRectangle rectangle in rectangles) {
                yield return rectangle;
            }
        }

        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }
    }
}
