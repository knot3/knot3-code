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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Knot3.Framework.Core;
using Knot3.Framework.Models;
using Knot3.Framework.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Ein 3D-Modell, das eine Flächen zwischen Kanten darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class RectangleModel : GamePrimitive
    {
        public override BoundingSphere[] Bounds { get { return new BoundingSphere[0]; } }

        public bool IsVirtual { get; set; }

        /// <summary>
        /// Erstellt ein neues 3D-Modell mit dem angegebenen Spielzustand und den angegebenen Spielinformationen.
        /// [base=screen, info]
        /// </summary>
        public RectangleModel (IScreen screen, Texture2D texture, Parallelogram parallelogram, Vector3 position)
            : base (screen, new GameModelInfo(modelname: ""), () => parallelogram)
        {
            Texture = texture;
            Info.Position = position;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);
        }

        public static bool operator == (RectangleModel a, RectangleModel b)
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
            return a.Info.Position == b.Info.Position;
        }

        public static bool operator != (RectangleModel a, RectangleModel b)
        {
            return !(a == b);
        }

        public bool Equals (RectangleModel other)
        {
            return this.Info.Position == other.Info.Position;
        }

        public override bool Equals (object obj)
        {
            if (obj is RectangleModel) {
                return Equals ((RectangleModel)obj);
            }
            else {
                return false;
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return Info.Position.GetHashCode ();
        }
    }
}
