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

using Knot3.Framework.Math;

namespace Knot3.Framework.Models
{
    /// <summary>
    /// Enthält Informationen über ein 3D-Modell wie den Dateinamen, die Rotation und die Skalierung.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class GameModelInfo : GameObjectInfo
    {
        /// <summary>
        /// Der Dateiname des Modells.
        /// </summary>
        public string Modelname { get; set; }

        /// <summary>
        /// Die Rotation des Modells.
        /// </summary>
        public Angles3 Rotation { get; set; }

        /// <summary>
        /// Die Skalierung des Modells.
        /// </summary>
        public Vector3 Scale { get; set; }

        /// <summary>
        /// Erstellt ein neues Informations-Objekt eines 3D-Modells mit den angegebenen Informationen zu
        /// Dateiname, Rotation und Skalierung.
        /// </summary>
        public GameModelInfo (string modelname, Angles3 rotation, Vector3 scale)
        : base (position: Vector3.Zero)
        {
            Modelname = modelname;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Erzeugt eine neue Instanz eines GameModelInfo-Objekts.
        /// In modelname wird der Name der Datei angegeben, welche das Model repräsentiert.
        /// </summary>
        public GameModelInfo (string modelname)
        : base (position: Vector3.Zero)
        {
            Modelname = modelname;
            Rotation = Angles3.Zero;
            Scale = Vector3.One;
        }
    }
}
