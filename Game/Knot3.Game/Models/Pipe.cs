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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Primitives;
using Knot3.Framework.Utilities;

using Knot3.Game.Data;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Enthält Informationen über ein 3D-Modell, das eine Kante darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class Pipe : GamePrimitive
    {
        /// <summary>
        /// Die Kante, die durch das 3D-Modell dargestellt wird.
        /// </summary>
        public Edge Edge { get; set; }

        /// <summary>
        /// Der Knoten, der die Kante enthält.
        /// </summary>
        public Knot Knot { get; set; }

        public IGrid Grid;

        /// <summary>
        /// Die Position, an der die Kante beginnt.
        /// </summary>
        public Vector3 PositionFrom { get; set; }

        /// <summary>
        /// Die Position, an der die Kante endet.
        /// </summary>
        public Vector3 PositionTo { get; set; }

        public bool IsVirtual { get; set; }

        private Dictionary<Direction, Angles3> RotationMap = new Dictionary<Direction, Angles3> ()
        {
            { Direction.Up, 		Angles3.FromDegrees (90, 0, 0) },
            { Direction.Down, 		Angles3.FromDegrees (270, 0, 0) },
            { Direction.Right, 		Angles3.FromDegrees (0, 90, 0) },
            { Direction.Left, 		Angles3.FromDegrees (0, 270, 0) },
            { Direction.Forward, 	Angles3.FromDegrees (0, 0, 0) },
            { Direction.Backward, 	Angles3.FromDegrees (0, 0, 0) },
        };

        /// <summary>
        /// Erstellt ein neues Informationsobjekt für ein 3D-Modell, das eine Kante darstellt.
        /// [base="pipe1", Angles3.Zero, new Vector3 (10,10,10)]
        /// </summary>
        public Pipe (IScreen screen, IGrid grid, Knot knot, Edge edge, Node node1, Node node2)
        : base (screen: screen)
        {
            UniqueKey = edge.ToString ();

            // Weise Knoten und Kante zu
            Knot = knot;
            Edge = edge;
            Grid = grid;

            // Berechne die beiden Positionen, zwischen denen die Kante gezeichnet wird
            PositionFrom = node1;
            PositionTo = node2;

            // Kanten sind verschiebbar und auswählbar
            IsMovable = true;
            IsSelectable = true;

            // Berechne die Drehung
            Rotation += RotationMap [Edge.Direction];

            // Aktualisiere die Kategorie
            Coloring.OnColorChanged += UpdateCategory;
            IsSingleColored = true;

            incomplete = true;
        }

        private bool incomplete;

        private void initialize ()
        {
            incomplete = false;

            // die Position
            Position = (PositionFrom + PositionTo) / 2;
            // die Skalierung
            Scale = new Vector3 (12.5f, 12.5f, 50f);

            // berechne die Skalierung bei abgeschnittenen Übergängen
            if (Grid != null) {
                List<Junction> junctions1 = Grid.JunctionsBeforeEdge (Edge);
                List<Junction> junctions2 = Grid.JunctionsAfterEdge (Edge);

                // berechne die Skalierung bei überlangen Kanten
                if (junctions1.Count == 1 && junctions1 [0].EdgeFrom.Direction == junctions1 [0].EdgeTo.Direction) {
                    Scale += new Vector3 (0, 0, 25f);
                    Position -= Edge.Direction * 12.5f;
                }
                if (junctions2.Count == 1 && junctions2 [0].EdgeFrom.Direction == junctions2 [0].EdgeTo.Direction) {
                    Scale += new Vector3 (0, 0, 25f);
                    Position += Edge.Direction * 12.5f;
                }
            }

            // Bounds
            float length = (PositionTo - PositionFrom).Length ();
            float radius = 6.25f;
            Bounds = VectorHelper.CylinderBounds (
                         length: length,
                         radius: radius,
                         direction: Edge.Direction.Vector,
                         position: PositionFrom
                     );

            OnViewChanged ();
        }

        public void OnGridUpdated ()
        {
            initialize ();
        }

        protected override Primitive CreativePrimitive ()
        {
            int tessellation = Primitive.CurrentCircleTessellation;
            return new Cylinder (
                       device: Screen.GraphicsDevice,
                       height: 1f,
                       diameter: 1f,
                       tessellation: tessellation
                   );
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (incomplete) {
                initialize ();
            }

            (Coloring as SingleColor).BaseColor = Edge.Color;

            if (World.SelectedObject == this) {
                Coloring.Highlight (intensity: 0.40f, color: Color.White);
            }
            else if (Knot != null && Knot.SelectedEdges.Contains (Edge)) {
                Coloring.Highlight (intensity: 0.80f, color: Color.White);
            }
            else {
                Coloring.Unhighlight ();
            }

            base.Draw (time);
        }

        public override bool Equals (IGameObject other)
        {
            if (other != null && other is Pipe && Edge == (other as Pipe).Edge) {
                return true;
            }
            else {
                return base.Equals (other);
            }
        }
    }
}
