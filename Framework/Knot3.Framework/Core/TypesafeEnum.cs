using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

using Knot3.Framework.Utilities;
using System.Linq;

namespace Knot3.Framework
{
    public abstract class TypesafeEnum<T> : IEquatable<T>, IEquatable<TypesafeEnum<T>>
    {
        private static Dictionary<string, ISet<string>> _values = new Dictionary<string, ISet<string>> ();

        public static string[] Values { get { return _values [Typename].ToArray(); } }

        private static string Typename { get { return typeof(T).ToString (); } }

        public string Name { get; private set; }

        public TypesafeEnum (string name)
        {
            Name = name;
            _values.Add(Typename, name);
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

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return Name.GetHashCode ();
        }
    }
}

