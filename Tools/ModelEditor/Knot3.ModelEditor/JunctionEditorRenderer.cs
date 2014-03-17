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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Effects;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

namespace Knot3.ModelEditor
{
    [ExcludeFromCodeCoverageAttribute]
    public class JunctionEditorRenderer : GameObject, IEnumerable<IGameObject>
    {
        private IScreen screen;
        /// <summary>
        /// Die Liste der 3D-Modelle der Kantenübergänge.
        /// </summary>
        private List<Junction> junctions;
        /// <summary>
        /// Die Liste der 3D-Modelle der Kanten.
        /// </summary>
        private List<Pipe> pipes;
        /// <summary>
        /// Die Zuordnung zwischen Kanten und den dreidimensionalen Rasterpunkten, an denen sich die die Kantenübergänge befinden.
        /// </summary>
        private JunctionEditorGrid grid;

        /// <summary>
        /// Erstellt ein neues KnotRenderer-Objekt für den angegebenen Spielzustand mit den angegebenen
        /// Spielobjekt-Informationen, die unter Anderem die Position des Knotenursprungs enthalten.
        /// </summary>
        public JunctionEditorRenderer (IScreen screen, Vector3 position)
        {
            this.screen = screen;
            pipes = new List<Pipe> ();
            junctions = new List<Junction> ();
            grid = new JunctionEditorGrid (screen: screen);
        }

        /// <summary>
        /// Ruft die Intersects (Ray)-Methode der Kanten, Übergänge und Pfeile auf und liefert das beste Ergebnis zurück.
        /// </summary>
        public override GameObjectDistance Intersects (Ray ray)
        {
            GameObjectDistance nearest = null;
            if (!screen.InputManager.GrabMouseMovement) {
                foreach (Pipe pipe in pipes) {
                    GameObjectDistance intersection = pipe.Intersects (ray);
                    if (intersection != null) {
                        if (intersection.Distance > 0 && (nearest == null || intersection.Distance < nearest.Distance)) {
                            nearest = intersection;
                        }
                    }
                }
            }
            return nearest;
        }

        public void Render (Direction[] directions)
        {
            bool valid = true;
            HashSet<Axis> axes = new HashSet<Axis> ();
            foreach (Direction direction in directions) {
                if (axes.Contains (direction.Axis)) {
                    valid = false;
                    break;
                }
                else {
                    axes.Add (direction.Axis);
                }
            }

            if (valid) {
                grid.Render (directions);
                grid.Offset = Position;

                CreatePipes ();
                CreateJunctions ();

                World.Redraw = true;
            }
            else {
                pipes.Clear ();
                junctions.Clear ();

                World.Redraw = true;
            }
        }

        private void CreatePipes ()
        {
            pipes.Clear ();
            foreach (Edge edge in grid.Edges) {
                Pipe pipe = new Pipe (screen: screen, grid: grid, knot: null, edge: edge,
                                      node1: grid.NodeBeforeEdge (edge), node2: grid.NodeAfterEdge (edge));
                pipe.IsVisible = true;
                pipe.World = World;
                pipe.OnGridUpdated ();
                pipes.Add (pipe);
            }
        }

        private void CreateJunctions ()
        {
            junctions.Clear ();

            foreach (Node node in grid.Nodes) {
                List<Junction> junctionsAtNode = grid.JunctionsAtNode (node);
                // zeige zwischen zwei Kanten in der selben Richtung keinen Übergang an,
                // wenn sie alleine an dem Kantenpunkt sind
                if (junctionsAtNode.Count == 1 && junctionsAtNode [0].EdgeFrom.Direction == junctionsAtNode [0].EdgeTo.Direction) {
                    continue;
                }

                foreach (Junction junction in junctionsAtNode) {
                    junction.World = World;
                    junction.OnGridUpdated ();
                    junctions.Add (junction);
                }
            }
        }

        /// <summary>
        /// Ruft die Update ()-Methoden der Kanten, Übergänge und Pfeile auf.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            foreach (Pipe pipe in pipes) {
                pipe.Update (time);
            }
            foreach (Junction node in junctions) {
                node.Update (time);
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
                    Log.Debug("Pipes: " + string.Join(", ", pipes.Select(pipe => pipe.Edge.ToString())));
                    foreach (Pipe pipe in pipes) {
                        pipe.Draw (time);
                    }
                };
                Profiler.ProfileDelegate ["Nodes"] = () => {
                        Log.Debug("Junction: " + string.Join(", ", junctions.Select(junction => junction.Node.ToString())));
                    foreach (Junction junction in junctions) {
                        junction.Draw (time);
                    }
                };
                Profiler.Values ["# Pipes"] = pipes.Count;
                Profiler.Values ["# Nodes"] = junctions.Count;
            }
        }

        /// <summary>
        /// Gibt einen Enumerator der aktuell vorhandenen 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            foreach (Pipe pipe in pipes) {
                yield return pipe;
            }
            foreach (Junction node in junctions) {
                yield return node;
            }
        }
        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }
    }
}
