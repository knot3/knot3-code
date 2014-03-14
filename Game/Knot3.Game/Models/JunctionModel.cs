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
using Microsoft.Xna.Framework;
using Knot3.Framework.Core;
using Knot3.Framework.Models;
using Primitives;
using System.Collections.Generic;
using Knot3.Framework.Math;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Ein 3D-Modell, das einen Kantenübergang darstellt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class JunctionModel : GamePrimitive
    {
        /// <summary>
        /// Enthält Informationen über den darzustellende 3D-Modell des Kantenübergangs.
        /// </summary>
        public new Junction Info { get { return base.Info as Junction; } set { base.Info = value; } }

        public bool IsVirtual { get; set; }
        
        public override BoundingSphere[] Bounds { get { return _bounds; } }

        private BoundingSphere[] _bounds;

        private static Dictionary<string, Primitive> _primitiveSingleton = new Dictionary<string, Primitive>();

        /// <summary>
        /// Erstellt ein neues 3D-Modell mit dem angegebenen Spielzustand und dem angegebenen Informationsobjekt.
        /// [base=screen, info]
        /// </summary>
        public JunctionModel (IScreen screen, Junction info)
        : base (screen, info, PrimitiveSingleton(screen: screen, info: info))
        {
            _bounds = new BoundingSphere[] { new BoundingSphere(Info.Position, radius: 100f) };
        }

        private static Primitive PrimitiveSingleton (IScreen screen, Junction info)
        {
            string name = info.Modelname;
            if (name == "pipe-angled") {
                return _primitiveSingleton [name] = _primitiveSingleton.ContainsKey (name) ? _primitiveSingleton [name] : new Torus (
                    device: screen.GraphicsDevice,
                    diameter: 4f,
                    thickness: 1f,
                    tessellation: 64,
                    circlePercent: 0.25f,
                    translation: Vector3.Left*2+Vector3.Backward*2,
                    rotation: Angles3.FromDegrees(90, 0, 90)
                );
            }
            else if (name == "pipe-straight") {
                return _primitiveSingleton [name] = _primitiveSingleton.ContainsKey (name) ? _primitiveSingleton [name] : new Cylinder (
                    device: screen.GraphicsDevice,
                    height: 1f,
                    diameter: 1f,
                    tessellation: 64
                    );
            }
            else {
                return _primitiveSingleton [name] = _primitiveSingleton.ContainsKey (name) ? _primitiveSingleton [name] : new CurvedCylinder (
                    device: screen.GraphicsDevice,
                    height: 1f,
                    diameter: 1f,
                    tessellation: 64
                    );
            }
        }

        /// <summary>
        /// Zeichnet das 3D-Modell mit dem aktuellen Rendereffekt.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            Coloring = new GradientColor (Info.EdgeFrom, Info.EdgeTo);
            if (IsVirtual) {
                Coloring.Highlight (intensity: 0.5f, color: Color.White);
            }
            else {
                Coloring.Unhighlight ();
            }
            base.Draw (time);
        }
    }
}
