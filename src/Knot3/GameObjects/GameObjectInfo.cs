#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion

#region Using

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
using Knot3.Data;
using Knot3.Input;
using Knot3.RenderEffects;
using Knot3.Screens;
using Knot3.Widgets;

#endregion

namespace Knot3.GameObjects
{
	/// <summary>
	/// Enthält Informationen über ein 3D-Objekt wie die Position, Sichtbarkeit, Verschiebbarkeit und Auswählbarkeit.
	/// </summary>
	public class GameObjectInfo : IEquatable<GameObjectInfo>
	{
		#region Properties

		/// <summary>
		/// Die Verschiebbarkeit des Spielobjektes.
		/// </summary>
		public Boolean IsMovable { get; set; }

		/// <summary>
		/// Die Auswählbarkeit des Spielobjektes.
		/// </summary>
		public Boolean IsSelectable { get; set; }

		/// <summary>
		/// Die Sichtbarkeit des Spielobjektes.
		/// </summary>
		public Boolean IsVisible { get; set; }

		/// <summary>
		/// Die Position des Spielobjektes.
		/// </summary>
		public Vector3 Position { get; set; }

		#endregion

		#region Constructors

		public GameObjectInfo (Vector3 position, bool isVisible = true, bool isSelectable = false, bool isMovable = false)
		{
			Position = position;
			IsVisible = isVisible;
			IsSelectable = isSelectable;
			IsMovable = isMovable;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Vergleicht zwei Informationsobjekte für Spielobjekte.
		/// [parameters=GameObjectInfo other]
		/// </summary>
		public virtual bool Equals (GameObjectInfo other)
		{
			if (other == null) {
				return false;
			}

			if (this.Position == other.Position) {
				return true;
			}
			else {
				return false;
			}
		}

		public override bool Equals (Object obj)
		{
			GameObjectInfo infoObj = obj as GameObjectInfo;
			return Equals (infoObj);
		}

		[ExcludeFromCodeCoverageAttribute]
		public override int GetHashCode ()
		{
			return Position.GetHashCode ();
		}

		public static bool operator == (GameObjectInfo o1, GameObjectInfo o2)
		{
			if ((object)o1 == null || ((object)o2) == null) {
				return Object.Equals (o1, o2);
			}

			return o2.Equals (o2);
		}

		public static bool operator != (GameObjectInfo o1, GameObjectInfo o2)
		{
			return !(o1 == o2);
		}

		#endregion
	}
}
