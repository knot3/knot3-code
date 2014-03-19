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
using Knot3.Framework.Primitives;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Erstellt aus einem Knoten-Objekt die zu dem Knoten gehörenden 3D-Modelle sowie die 3D-Modelle der Pfeile,
    /// die nach einer Auswahl von Kanten durch den Spieler angezeigt werden. Ist außerdem ein IGameObject und ein
    /// Container für die erstellten Spielobjekte.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class KnotRenderer : GameObject, IEnumerable<IGameObject>
    {
        private IScreen Screen;
        /// <summary>
        /// Die Liste der 3D-Modelle der Pfeile,
        /// die nach einer Auswahl von Kanten durch den Spieler angezeigt werden.
        /// </summary>
        private List<Arrow> arrows;
        /// <summary>
        /// Die Liste der Flächen zwischen den Kanten.
        /// </summary>
        private HashSet<RectangleModel> rectangles;
        private List<GameModel> debugModels;

        /// <summary>
        /// Der Knoten, für den 3D-Modelle erstellt werden sollen.
        /// </summary>
        private Knot knot
        {
            get {
                return _knot;
            }
            set {
                if (_knot != null) {
                    _knot.SelectionChanged -= OnSelectionChanged;
                }
                if (value != null) {
                    _knot = value;
                    _knot.SelectionChanged += OnSelectionChanged;
                }
            }
        }

        private Knot _knot;
        /// <summary>
        /// Die Zuordnung zwischen Kanten und den dreidimensionalen Rasterpunkten, an denen sich die die Kantenübergänge befinden.
        /// </summary>
        private Grid grid;

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
            Screen = screen;
            Position = position;
            arrows = new List<Arrow> ();
            rectangles = new HashSet<RectangleModel> ();
            debugModels = new List<GameModel> ();
            grid = new Grid (screen: Screen);
        }

        /// <summary>
        /// Ruft die Intersects (Ray)-Methode der Kanten, Übergänge und Pfeile auf und liefert das beste Ergebnis zurück.
        /// </summary>
        public override GameObjectDistance Intersects (Ray ray)
        {
            GameObjectDistance nearest = null;
            if (!Screen.InputManager.GrabMouseMovement) {
                foreach (Pipe pipe in grid.Pipes) {
                    GameObjectDistance intersection = pipe.Intersects (ray);
                    if (intersection != null) {
                        if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
                            nearest = intersection;
                        }
                    }
                }
                foreach (Arrow arrow in arrows) {
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
        public void RenderKnot (Knot newKnot)
        {
            if (knot != newKnot) {
                knot = newKnot;

                grid.Knot = knot;
                grid.World = World;
                grid.Offset = Position + knot.OffSet;
                Profiler.ProfileDelegate ["Grid"] = () => grid.Update ();

                if (Config.Default ["debug", "show-startedge-direction", false]) {
                    CreateStartArrow ();
                }
                if (showArrows) {
                    CreateArrows ();
                }
                CreateRectangles ();

                World.Redraw = true;
            }
        }

        private void OnSelectionChanged ()
        {
            grid.Knot = knot;
            grid.Offset = knot.OffSet;
            if (showArrows) {
                CreateArrows ();
            }
            World.Redraw = true;
        }

        private void CreateStartArrow ()
        {
            Edge edge = knot.ElementAt (0);
            Vector3 towardsCamera = World.Camera.PositionToTargetDirection;
            Arrow arrow = new Arrow (
                screen: Screen,
                position: Vector3.Zero - 10 * towardsCamera - 10 * towardsCamera.PrimaryDirection (),
                direction: edge
            );
            arrow.IsVisible = true;
            arrow.World = World;
            debugModels.Add (arrow);
        }

        private void CreateArrows ()
        {
            foreach (Arrow Arrow in arrows) {
                Arrow.World = null;
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
                Node node1 = grid.NodeBeforeEdge (edge);
                Node node2 = grid.NodeAfterEdge (edge);
                foreach (Direction direction in Direction.Values) {
                    if (knot.IsValidDirection (direction)) {
                        Vector3 towardsCamera = World.Camera.PositionToTargetDirection;
                        Arrow arrow = new Arrow (
                            screen: Screen,
                            position: node1.CenterBetween (node2) - 25 * towardsCamera - 25 * towardsCamera.PrimaryDirection (),
                            direction: direction
                        );
                        arrow.IsVisible = true;
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
            rectangles.Clear ();

            RectangleMap rectMap = new RectangleMap (grid);
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

                Parallelogram parallelogram = new Parallelogram (
                    device: Screen.GraphicsDevice,
                    origin: Vector3.Zero,
                    left: edgeAB.Direction,
                    width: Node.Scale,
                    up: edgeCD.Direction.Reverse,
                    height: Node.Scale,
                    normalToCenter: false
                );
                RectangleModel rectangle = new RectangleModel (
                    screen: Screen,
                    texture: texture,
                    parallelogram: parallelogram,
                    position: rect.Position
                );
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
            Texture2D texture = new Texture2D (Screen.GraphicsDevice, width, height);
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
            Texture2D texture = new Texture2D (Screen.GraphicsDevice, width, height);
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
        public override void Update (GameTime time)
        {
            foreach (Pipe pipe in grid.Pipes) {
                pipe.Update (time);
            }
            foreach (Junction junction in grid.Junctions) {
                junction.Update (time);
            }
            foreach (Arrow arrow in arrows) {
                arrow.Update (time);
            }
            foreach (RectangleModel rectangle in rectangles) {
                rectangle.Update (time);
            }

            if (time.TotalGameTime.Seconds % 10 == 0) {
                int count = grid.Pipes.Count () + grid.Junctions.Count () + arrows.Count;
                if (count > 0) {
                    Profiler.Values ["# Cached Models"] = count;
                }
            }
        }

        /// <summary>
        /// Ruft die Draw ()-Methoden der Kanten, Übergänge und Pfeile auf.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (IsVisible) {
                Profiler.Values ["# InFrustum"] = 0;
                Profiler.Values ["RenderEffect"] = 0;
                Profiler.ProfileDelegate ["Pipes"] = () => {
                    foreach (Pipe pipe in grid.Pipes) {
                        pipe.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Nodes"] = () => {
                    foreach (Junction junction in grid.Junctions) {
                        //Log.Debug ("junction=", junction, ", LastTick=", junction.LastTick, ", modelname=",junction.Scale);
                        junction.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Arrows"] = () => {
                    foreach (Arrow arrow in arrows) {
                        arrow.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Rectangles"] = () => {
                    foreach (RectangleModel rectangle in rectangles) {
                        rectangle.Draw (time);
                    }
                };
                foreach (GameModel model in debugModels) {
                    model.Draw (time);
                }
                Profiler.Values ["# Pipes"] = grid.Pipes.Count ();
                Profiler.Values ["# Nodes"] = grid.Nodes.Count ();
            }
        }

        /// <summary>
        /// Gibt einen Enumerator der aktuell vorhandenen 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            foreach (Pipe pipe in grid.Pipes) {
                yield return pipe;
            }
            foreach (Junction junction in grid.Junctions) {
                yield return junction;
            }
            foreach (Arrow arrow in arrows) {
                yield return arrow;
            }
            foreach (RectangleModel rectangle in rectangles) {
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
