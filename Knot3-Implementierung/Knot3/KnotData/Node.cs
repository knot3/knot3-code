using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Widgets;

namespace Knot3.KnotData
{
    /// <summary>
    /// Eine Position im 3D-Raster. Die Werte für alle drei Koordinaten sind Integer, wobei 1 die Breite der Raster-Abschnitte angibt.
    /// Eine Skalierung auf Koordinaten im 3D-Raum und damit einhergehend eine Konvertierung in ein Vector3-Objekt des XNA-Frameworks kann mit der Methode ToVector() angefordert werden.
    /// </summary>
    public class Node : IEquatable<Node>, ICloneable
    {
        #region Properties

        /// <summary>
        /// X steht für eine x-Koordinate im dreidimensionalen Raster.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Y steht für eine y-Koordinate im dreidimensionalen Raster.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Z steht für eine z-Koordinate im dreidimensionalen Raster.
        /// </summary>
        public int Z { get; private set; }

        /// <summary>
        /// Ein Skalierungswert.
        /// </summary>
		public static readonly int Scale = 100;

        #endregion

        #region Constructors

        /// <summary>
        /// Erzeugt eine neue Instanz eines Node-Objekts und initialisiert diese mit Werten
        /// für die x-, y- und z-Koordinate.
        /// </summary>
        public Node (int x, int y, int z)
        {
			X = x;
			Y = y;
			Z = z;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Liefert die x-, y- und z-Koordinaten im 3D-Raum als ein Vektor3 der Form (x, y, z).
        /// </summary>
        public virtual Vector3 ToVector ( )
        {
			return new Vector3 (X * Scale, Y * Scale, Z * Scale);
        }

		public Vector3 CenterBetween (Node other)
		{
			Vector3 positionFrom = this.ToVector ();
			Vector3 positionTo = other.ToVector ();
			return positionFrom + (positionTo - positionFrom) / 2;
		}

		public static Node operator + (Node a, Vector3 b)
		{
			return new Node (a.X + (int)b.X, a.Y + (int)b.Y, a.Z + (int)b.Z);
		}

		public static Vector3 operator - (Node a, Node b)
		{
			return new Vector3 (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public override int GetHashCode ()
		{
			return X * 10000 + Y * 100 + Z;
		}

		public override string ToString ()
		{
			return "(" + X + "," + Y + "," + Z + ")";
		}

		public object Clone ()
		{
			return new Node (X, Y, Z);
		}

		public static bool operator == (Node a, Node b)
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
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator != (Node a, Node b)
		{
			return !(a == b);
		}
		
		public bool Equals (Node other)
		{
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
		}
		
		public override bool Equals (object obj)
		{
			if (obj is Node) {
				return Equals ((Node)obj);
			} else {
				return false;
			}
		}

        #endregion

    }
}

