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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

namespace Knot3.Framework.Math
{
    /// <summary>
    /// Diese Klasse repräsentiert die Rollwinkel der drei Achsen X, Y und Z.
    /// Sie bietet Möglichkeit vordefinierte Winkelwerte zu verwenden, z.B. stellt Zero den Nullvektor dar.
    /// Die Umwandlung zwischen verschiedenen Winkelmaßen wie Grad- und Bogenmaß unterstützt sie durch entsprechende Methoden.
    /// </summary>
    public struct Angles3 : IEquatable<Angles3> {
        /// <summary>
        /// Der Winkel im Bogenmaß für das Rollen um die X-Achse. Siehe statische Methode Matrix.CreateRotationX (float) des MonoGame-Frameworks.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Der Winkel im Bogenmaß für das Rollen um die Y-Achse. Siehe statische Methode Matrix.CreateRotationY (float) des MonoGame-Frameworks.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Der Winkel im Bogenmaß für das Rollen um die Z-Achse. Siehe statische Methode Matrix.CreateRotationZ (float) des MonoGame-Frameworks.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Eine statische Eigenschaft mit dem Wert X = 0, Y = 0, Z = 0.
        /// </summary>
        public static Angles3 Zero { get { return new Angles3 (0f, 0f, 0f); } }

        /// <summary>
        /// Konstruiert ein neues Angles3-Objekt mit drei gegebenen Winkeln im Bogenmaß.
        /// </summary>
        public Angles3 (float x, float y, float z)
        : this ()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Angles3 (Vector3 v)
        : this ()
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Eine statische Methode, die Grad in Bogenmaß konvertiert.
        /// </summary>
        public static Angles3 FromDegrees (float x, float y, float z)
        {
            return new Angles3 (
                       MathHelper.ToRadians (x),
                       MathHelper.ToRadians (y),
                       MathHelper.ToRadians (z)
                   );
        }

        /// <summary>
        /// Konvertiert Bogenmaß in Grad.
        /// </summary>
        public void ToDegrees (out float x, out float y, out float z)
        {
            x = (int)MathHelper.ToDegrees (X) % 360;
            y = (int)MathHelper.ToDegrees (Y) % 360;
            z = (int)MathHelper.ToDegrees (Z) % 360;
        }

        public override bool Equals (object obj)
        {
            return (obj is Angles3) ? this == (Angles3)obj : false;
        }

        public bool Equals (Angles3 other)
        {
            return this == other;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return (int)(this.X + this.Y + this.Z);
        }

        public static bool operator == (Angles3 value1, Angles3 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }

        public static bool operator != (Angles3 value1, Angles3 value2)
        {
            return !(value1 == value2);
        }

        public static Angles3 operator + (Angles3 value1, Angles3 value2)
        {
            return new Angles3 (value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
        }

        public static Angles3 operator - (Angles3 value)
        {
            value = new Angles3 (-value.X, -value.Y, -value.Z);
            return value;
        }

        public static Angles3 operator - (Angles3 value1, Angles3 value2)
        {
            return new Angles3 (value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
        }

        public static Angles3 operator * (Angles3 value1, Angles3 value2)
        {
            return new Angles3 (value1.X * value2.X, value1.Y * value2.Y, value1.Z * value2.Z);
        }

        public static Angles3 operator * (Angles3 value, float scaleFactor)
        {
            return new Angles3 (value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
        }

        public static Angles3 operator * (float scaleFactor, Angles3 value)
        {
            return new Angles3 (value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
        }

        public static Angles3 operator / (Angles3 value1, Angles3 value2)
        {
            return new Angles3 (value1.X / value2.X, value1.Y / value2.Y, value1.Z / value2.Z);
        }

        public static Angles3 operator / (Angles3 value, float divider)
        {
            float scaleFactor = 1 / divider;
            return new Angles3 (value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            float x, y, z;
            ToDegrees (out x, out y, out z);

            return   "Angles3 ("
                     + x.ToString ()
                     + ","
                     + y.ToString ()
                     + ","
                     + z.ToString ()
                     + ")";
        }
    }
}
