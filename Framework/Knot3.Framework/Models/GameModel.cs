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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Models
{
    /// <summary>
    /// Repräsentiert ein 3D-Modell in einer Spielwelt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class GameModel : GameObject
    {
        /// <summary>
        /// Der Dateiname des Modells.
        /// </summary>
        public string Modelname { get; set; }

        /// <summary>
        /// Die Klasse des MonoGame-Frameworks, die ein 3D-Modell repräsentiert.
        /// </summary>
        public virtual Model Model { get { return Screen.LoadModel (Modelname); } }

        protected IScreen Screen;

        /// <summary>
        /// Erstellt ein neues 3D-Modell in dem angegebenen Spielzustand mit den angegebenen Modellinformationen.
        /// </summary>
        public GameModel (IScreen screen, string modelname)
        {
            Screen = screen;
            Modelname = modelname;

            // default values
            UniqueKey = modelname;
            UpdateCategory ();
        }

        protected override void UpdateCategory ()
        {
            base.UpdateCategory (category: Modelname);
        }

        /// <summary>
        /// Gibt die Mitte des 3D-Modells zurück.
        /// </summary>
        public override Vector3 Center
        {
            get {
                Vector3 center = Vector3.Zero;
                int count = Model.Meshes.Count;
                foreach (ModelMesh mesh in Model.Meshes) {
                    center += mesh.BoundingSphere.Center / count;
                }
                return center / Scale + Position;
            }
        }

        /// <summary>
        /// Zeichnet das 3D-Modell in der angegebenen Spielwelt mit dem aktuellen Rendereffekt der Spielwelt.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (IsVisible) {
                if (InCameraFrustum) {
                    Screen.CurrentRenderEffects.CurrentEffect.DrawModel (this, time);
                }
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
