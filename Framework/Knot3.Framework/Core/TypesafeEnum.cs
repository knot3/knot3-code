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

using Knot3.Framework.Utilities;

namespace Knot3.Framework.Core
{
    public abstract class TypesafeEnum<T> : IEquatable<T>, IEquatable<TypesafeEnum<T>>
        where T : TypesafeEnum<T>
    {
        private static Dictionary<string, ISet<TypesafeEnum<T>>> _values = new Dictionary<string, ISet<TypesafeEnum<T>>> ();

        public static T[] Values { get { return _values [Typename].Select (value => value as T).ToArray (); } }

        private static string Typename { get { return typeof (T).ToString (); } }

        public string Name { get; private set; }

        public TypesafeEnum (string name)
        {
            Name = name;
            if (!string.IsNullOrWhiteSpace (name) && name != "Zero" && name != "Null" && name != "Empty") {
                _values.Add (Typename, this);
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            return Name;
        }

        public static implicit operator string (TypesafeEnum<T> instance)
        {
            return instance.Name;
        }

        public static bool operator == (TypesafeEnum<T> a, TypesafeEnum<T> b)
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

        public static bool operator != (TypesafeEnum<T> d1, TypesafeEnum<T> d2)
        {
            return !(d1 == d2);
        }

        public bool Equals (TypesafeEnum<T> other)
        {
            return other != null && Name == other.Name;
        }

        public override bool Equals (object other)
        {
            return other != null && Equals (other as TypesafeEnum<T>);
        }

        public bool Equals (T other)
        {
            return other != null && Equals (other as TypesafeEnum<T>);
        }

        public static T FromString (string str)
        {
            foreach (T value in Values) {
                if (str.ToLower () == value.Name.ToLower ()) {
                    return value;
                }
            }
            return null;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return Name.GetHashCode ();
        }
    }
}
