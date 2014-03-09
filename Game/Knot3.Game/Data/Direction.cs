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

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Eine Wertesammlung der möglichen Richtungen in einem dreidimensionalen Raum.
    /// Wird benutzt, damit keine ungültigen Kantenrichtungen angegeben werden können.
    /// Dies ist eine Klasse und kein Enum, kann aber
    /// uneingeschränkt wie eines benutzt werden (Typesafe Enum Pattern).
    /// </summary>
    public sealed class Direction : TypesafeEnum<Direction>
    {
        /// <summary>
        /// Links.
        /// </summary>
        public static readonly Direction Left = new Direction (Vector3.Left, "Left");
        /// <summary>
        /// Rechts.
        /// </summary>
        public static readonly Direction Right = new Direction (Vector3.Right, "Right");
        /// <summary>
        /// Hoch.
        /// </summary>
        public static readonly Direction Up = new Direction (Vector3.Up, "Up");
        /// <summary>
        /// Runter.
        /// </summary>
        public static readonly Direction Down = new Direction (Vector3.Down, "Down");
        /// <summary>
        /// Vorwärts.
        /// </summary>
        public static readonly Direction Forward = new Direction (Vector3.Forward, "Forward");
        /// <summary>
        /// Rückwärts.
        /// </summary>
        public static readonly Direction Backward = new Direction (Vector3.Backward, "Backward");
        /// <summary>
        /// Keine Richtung.
        /// </summary>
        public static readonly Direction Zero = new Direction (Vector3.Zero, "Zero");

        /// <summary>
        /// Gibt alle Richtungswerte zurück.
        /// </summary>
        public new static readonly Direction[] Values = {
            Left, Right, Up, Down, Forward,	Backward
        };
        private static readonly Dictionary<Direction, Direction> ReverseMap
            = new Dictionary<Direction, Direction> ()
        {
            { Left, Right }, { Right, Left },
            { Up, Down }, { Down, Up },
            { Forward, Backward }, { Backward, Forward },
            { Zero, Zero }
        };

        private static readonly Dictionary<Direction, Axis> AxisMap
            = new Dictionary<Direction, Axis> ()
        {
            { Left, Axis.X }, { Right, Axis.X },
            { Up, Axis.Y }, { Down, Axis.Y },
            { Forward, Axis.Z }, { Backward, Axis.Z },
            { Zero, Axis.Zero }
        };

        public Vector3 Vector { get; private set; }

        public Direction Reverse { get { return ReverseMap [this]; } }

        public Axis Axis { get { return AxisMap [this]; } }

        private Direction (Vector3 vector, string description)
        : base (description)
        {
            Vector = vector;
        }

        public static Direction FromAxis (Axis axis)
        {
            return axis == Axis.X ? Right : axis == Axis.Y ? Up : axis == Axis.Z ? Backward : Zero;
        }

        public static Vector3 operator + (Vector3 v, Direction d)
        {
            return v + d.Vector;
        }

        public static Vector3 operator - (Vector3 v, Direction d)
        {
            return v - d.Vector;
        }

        public static Vector3 operator / (Direction d, float i)
        {
            return d.Vector / i;
        }

        public static Vector3 operator * (Direction d, float i)
        {
            return d.Vector * i;
        }

        public static bool operator == (Direction a, Direction b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Vector == b.Vector;
        }

        public static bool operator != (Direction d1, Direction d2)
        {
            return !(d1 == d2);
        }

        public bool Equals (Direction other)
        {
            return other != null && Vector == other.Vector;
        }

        public override bool Equals (object other)
        {
            if (other == null) {
                return false;
            } else if (other is Direction) {
                return Equals (other as Direction);
            } else if (other is Vector3) {
                return Vector.Equals ((Vector3)other);
            } else {
                return base.Equals (other);
            }
        }

        public static implicit operator Vector3 (Direction direction)
        {
            return direction.Vector;
        }
    }
}
