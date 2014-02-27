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

namespace Knot3.Framework.Audio
{
    public class Sound : IEquatable<Sound>
    {
        /// <summary>
        /// Gibt alle Sound-Werte zurück.
        /// </summary>
        public static Sound[] Values { get { return _values.Select (name => new Sound (name)).ToArray (); } }

        private static HashSet<string> _values = new HashSet<string> ();

        /// <summary>
        /// Kein Sound.
        /// </summary>
        public static readonly Sound None = new Sound ("None");

        public string Name { get; private set; }

        public Sound (string name)
        {
            Name = name;
            _values.Add (name);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            return Name;
        }

        public static implicit operator string (Sound layer)
        {
            return layer.Name;
        }

        public static bool operator == (Sound a, Sound b)
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
            return a.Name == b.Name;
        }

        public static bool operator != (Sound d1, Sound d2)
        {
            return !(d1 == d2);
        }

        public bool Equals (Sound other)
        {
            return other != null && Name == other.Name;
        }

        public override bool Equals (object other)
        {
            return other != null && Equals (other as Sound);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return Name.GetHashCode ();
        }
    }
}
