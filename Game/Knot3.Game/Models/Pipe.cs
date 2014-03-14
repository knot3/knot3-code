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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Math;
using Knot3.Framework.Models;

using Knot3.Game.Data;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Enthält Informationen über ein 3D-Modell, das eine Kante darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class Pipe : GameModelInfo
    {
        /// <summary>
        /// Die Kante, die durch das 3D-Modell dargestellt wird.
        /// </summary>
        public Edge Edge { get; set; }

        /// <summary>
        /// Der Knoten, der die Kante enthält.
        /// </summary>
        public Knot Knot { get; set; }

        /// <summary>
        /// Die Position, an der die Kante beginnt.
        /// </summary>
        public Vector3 PositionFrom { get; set; }

        /// <summary>
        /// Die Position, an der die Kante endet.
        /// </summary>
        public Vector3 PositionTo { get; set; }

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
        public Pipe (INodeMap nodeMap, Knot knot, Edge edge)
        : base ("pipe-straight", Angles3.Zero, new Vector3 (12.5f, 12.5f, 50f))
        {
            // Weise Knoten und Kante zu
            Knot = knot;
            Edge = edge;

            // Berechne die beiden Positionen, zwischen denen die Kante gezeichnet wird
            Node node1 = nodeMap.NodeBeforeEdge (edge);
            Node node2 = nodeMap.NodeAfterEdge (edge);
            PositionFrom = node1;
            PositionTo = node2;
            Position = node1.CenterBetween (node2);

            // Kanten sind verschiebbar und auswählbar
            IsMovable = true;
            IsSelectable = true;

            // Berechne die Drehung
            Rotation += RotationMap [Edge.Direction];

            // Berechne die Skalierung bei abgeschnittenen Übergängen
            List<IJunction> junctions1 = nodeMap.JunctionsBeforeEdge (edge);
            List<IJunction> junctions2 = nodeMap.JunctionsAfterEdge (edge);

            // Berechne die Skalierung bei überlangen Kanten
            if (junctions1.Count == 1 && junctions1 [0].EdgeFrom.Direction == junctions1 [0].EdgeTo.Direction) {
                Scale += new Vector3 (0, 0, 25f);
                Position -= edge.Direction * 12.5f;
            }
            if (junctions2.Count == 1 && junctions2 [0].EdgeFrom.Direction == junctions2 [0].EdgeTo.Direction) {
                Scale += new Vector3 (0, 0, 25f);
                Position += edge.Direction * 12.5f;
            }
        }

        public override bool Equals (GameObjectInfo other)
        {
            if (other == null) {
                return false;
            }

            if (other is Pipe) {
                if (this.Edge == (other as Pipe).Edge && this.Scale == (other as Pipe).Scale && base.Equals (other)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                return base.Equals (other);
            }
        }
    }
}
