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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Primitives;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Models
{
    [ExcludeFromCodeCoverageAttribute]
    public abstract class GamePrimitive : GameObject
    {
        /// <summary>
        /// Der Dateiname des Modells.
        /// </summary>
        protected string Modelname { get; set; }

        /// <summary>
        /// Die Klasse des MonoGame-Frameworks, die ein automatisch generiertes 3D-Modell repräsentiert.
        /// </summary>
        public Primitive Primitive
        {
            get {
                string key = this.GetType ().Name + ":" + Modelname;
                if (_primitiveCache.ContainsKey (key)) {
                    return _primitiveCache [key];
                }
                else {
                    _primitiveCache [key] = CreativePrimitive ();
                    UpdateCategory ();
                    return _primitiveCache [key];
                }
            }
        }

        protected IScreen Screen;
        private static Dictionary<string, Primitive> _primitiveCache = new Dictionary<string, Primitive> ();

        static GamePrimitive ()
        {
            Primitive.OnModelQualityChanged += (time) => {
                foreach (Primitive primitive in _primitiveCache.Values) {
                    primitive.Dispose ();
                }
                _primitiveCache.Clear ();
            };
        }

        /// <summary>
        /// Erstellt ein neues 3D-Modell in dem angegebenen Spielzustand mit den angegebenen Modellinformationen.
        /// </summary>
        public GamePrimitive (IScreen screen)
        {
            Screen = screen;
            UpdateCategory ();
        }

        protected abstract Primitive CreativePrimitive ();

        protected override void UpdateCategory ()
        {
            base.UpdateCategory (category: Primitive != null ? Primitive.GetType ().Name : "?");
        }

        /// <summary>
        /// Gibt die Mitte des 3D-Modells zurück.
        /// </summary>
        public override Vector3 Center
        {
            get {
                return Primitive.Center / Scale + Position;
            }
        }

        /// <summary>
        /// Zeichnet das 3D-Modell in der angegebenen Spielwelt mit dem aktuellen Rendereffekt der Spielwelt.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (IsVisible && InCameraFrustum && Primitive != null) {
                Screen.CurrentRenderEffects.CurrentEffect.DrawPrimitive (this, time);
            }
        }

        public void Dispose ()
        {
            if (Texture != null) {
                Texture.Dispose ();
                Texture = null;
            }
        }
    }
}
