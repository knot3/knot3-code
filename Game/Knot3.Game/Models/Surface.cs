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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Primitives;

using Knot3.Game.Data;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Ein 3D-Modell, das eine Flächen zwischen Kanten darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class Surface : GamePrimitive
    {
        public SurfaceLocation Location { get; private set; }

        private Vector3 Direction0;
        private Vector3 Direction1;
        private Vector3 DirectionCross;

        /// <summary>
        /// Erstellt ein neues 3D-Modell mit dem angegebenen Spielzustand und den angegebenen Spielinformationen.
        /// [base=screen, info]
        /// </summary>
        public Surface (IScreen screen, SurfaceLocation location)
        : base (screen: screen)
        {
            Location = location;
            FindDirections (location);
            Modelname = Direction0 + "" + Direction1;
            Texture = ContentLoader.CreateTexture (graphicsDevice: screen.GraphicsDevice, color: Color.HotPink);
            (Coloring as SingleColor).BaseColor = Edge.RandomColor ();
            Position = location.Location;
            IsSelectable = true;
            BoundingBoxes = FindBounds ();
        }

        private void FindDirections (SurfaceLocation location)
        {
            Direction direction0 = location.Sides [0].Direction;
            Vector3? direction1 = null;
            for (int i = 1; i < location.Sides.Count; ++i) {
                if (direction0.Axis != location.Sides [i].Direction.Axis) {
                    direction1 = location.Sides [i].Direction;
                    break;
                }
            }
            if (!direction1.HasValue) {
                for (int i = 1; i < location.Sides.Count; ++i) {
                    foreach (Node node0 in location.Sides [0].Nodes) {
                        foreach (Node nodei in location.Sides [i].Nodes) {
                            Vector3 direction = nodei - node0;
                            //Log.Debug ("direction: ", direction, ", direction0: ", direction0);
                            if (direction.Length () == 1 && direction != direction0) {
                                direction1 = direction;
                                //break;
                            }
                        }
                    }
                }
            }
            if (!direction1.HasValue) {
                throw new ArgumentException ();
            }

            //Log.Debug ("direction0: ", direction0, ", direction1: ", direction1);
            Direction0 = direction0;
            Direction1 = direction1.Value;
            DirectionCross = Vector3.Cross (Direction0, Direction1);
            DirectionCross = Vector3.Min (DirectionCross, -DirectionCross);
        }

        private BoundingBox[] FindBounds ()
        {
            //Log.Debug ("min: ", test (-1, -1, -1), "max: ", test (,1,,1,,1));
            return new BoundingBox[] {
                new BoundingBox (test (-1, -1, -1), test (+1, +1, +1)),
                new BoundingBox (test (+1, -1, -1), test (-1, +1, +1)),
                new BoundingBox (test (-1, +1, -1), test (+1, -1, +1)),
                new BoundingBox (test (+1, +1, -1), test (-1, -1, +1)),
            };
        }

        private Vector3 test (int sign0, int sign1, int sign2)
        {
            return Position + (Node.Scale - 20) * (Direction0 / 2 * sign0 + Direction1 / 2 * sign1) + DirectionCross * 5 * sign2;
        }

        protected override Primitive CreativePrimitive ()
        {
            return new Parallelogram (
                       device: Screen.GraphicsDevice,
                       origin: Vector3.Zero,
                       left: Direction0,
                       width: Node.Scale - 10,
                       up: Direction1,
                       height: Node.Scale - 10,
                       normalToCenter: false
                   );
        }

        protected override void UpdateCategory ()
        {
            base.UpdateCategory (Direction0 + "" + Direction1);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);
        }
    }
}
