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

namespace Knot3.Game.Models
{
    /// <summary>
    /// Ein 3D-Modell, das eine Kante darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class PipeModel : GameModel
    {
        /// <summary>
        /// Enthält Informationen über die darzustellende Kante.
        /// </summary>
        public new Pipe Info { get { return base.Info as Pipe; } set { base.Info = value; } }

        private BoundingSphere[] _bounds;

        public override BoundingSphere[] Bounds
        {
            get { return _bounds; }
        }

        public bool IsVirtual { get; set; }

        /// <summary>
        /// Erstellt ein neues 3D-Modell mit dem angegebenen Spielzustand und den angegebenen Spielinformationen.
        /// [base=screen, info]
        /// </summary>
        public PipeModel (IGameScreen screen, Pipe info)
        : base (screen, info)
        {
            float length = (info.PositionTo - info.PositionFrom).Length ();
            float radius = 5.1f;
            _bounds = VectorHelper.CylinderBounds (
                          length: length,
                          radius: radius,
                          direction: Info.Edge.Direction.Vector,
                          position: info.PositionFrom
                      );
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            Coloring = new SingleColor (Info.Edge);
            if (World.SelectedObject == this) {
                Coloring.Highlight (intensity: 0.40f, color: Color.White);
            }
            else if (IsVirtual) {
                Coloring.Highlight (intensity: 0.5f, color: Color.White);
            }
            else if (Info.Knot != null && Info.Knot.SelectedEdges.Contains (Info.Edge)) {
                Coloring.Highlight (intensity: 0.80f, color: Color.White);
            }
            else {
                Coloring.Unhighlight ();
            }

            base.Draw (time);
        }

        /// <summary>
        /// Prüft, ob der angegebene Mausstrahl das 3D-Modell schneidet.
        /// </summary>
        public override GameObjectDistance Intersects (Ray ray)
        {
            foreach (BoundingSphere sphere in Bounds) {
                float? distance = ray.Intersects (sphere);
                if (distance != null) {
                    GameObjectDistance intersection = new GameObjectDistance () {
                        Object=this, Distance=distance.Value
                    };
                    return intersection;
                }
            }
            return null;
        }
    }
}
