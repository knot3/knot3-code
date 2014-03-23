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
using Knot3.Framework.Math;

namespace Knot3.Framework.Models
{
    /// <summary>
    /// Diese Schnittstelle repräsentiert ein Spielobjekt und enthält eine Referenz auf die Spielwelt, in der sich dieses
    /// Game befindet, sowie Informationen zu dem Game.
    /// </summary>
    public interface IGameObject : IEquatable<IGameObject>
    {
        /// <summary>
        /// Die Verschiebbarkeit des Spielobjektes.
        /// </summary>
        bool IsMovable { get; }

        /// <summary>
        /// Die Auswählbarkeit des Spielobjektes.
        /// </summary>
        bool IsSelectable { get; }

        /// <summary>
        /// Die Sichtbarkeit des Spielobjektes.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Die Position des Spielobjektes.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Die Rotation des Spielobjektes.
        /// </summary>
        Angles3 Rotation { get; set; }

        /// <summary>
        /// Die Skalierung des Spielobjektes.
        /// </summary>
        Vector3 Scale { get; set; }

        /// <summary>
        /// Eine Referenz auf die Spielwelt, in der sich das Spielobjekt befindet.
        /// </summary>
        World World { get; set; }

        /// <summary>
        /// Die Weltmatrix des 3D-Modells in der angegebenen Spielwelt.
        /// </summary>
        Matrix WorldMatrix { get; }

        /// <summary>
        /// Die Mitte des Spielobjektes im 3D-Raum.
        /// </summary>
        Vector3 Center { get; }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        void Update (GameTime time);

        /// <summary>
        /// Zeichnet das Spielobjekt.
        /// </summary>
        void Draw (GameTime time);

        /// <summary>
        /// Überprüft, ob der Mausstrahl das Spielobjekt schneidet.
        /// </summary>
        GameObjectDistance Intersects (Ray ray);

        /// <summary>
        /// Gibt die Ausmaße des Spielobjekts zurück.
        /// </summary>
        BoundingSphere[] Bounds { get; }

        /// <summary>
        /// Gibt an, ob das Spielobjekt innerhalb des Frustums ist, das den sichtbaren Bereich enthält.
        /// </summary>
        bool InCameraFrustum { get; }

        /// <summary>
        /// Gibt an, ob das Spielobjekt mit Beleuchtung gezeichnet werden soll oder nicht.
        /// </summary>
        bool IsLightingEnabled { get; }

        /// <summary>
        /// Gibt an, ob das Spielobjekt zur SkyBox oder etwas vergleichbarem gehört.
        /// Falls diese Property true ist, wird eine veränderte View-Matrix verwendet.
        /// </summary>
        bool IsSkyObject { get; }

        string UniqueKey { get; }

        /// <summary>
        /// Gibt eine Kategorie an, die dabei hilft, Spielobjekte nach gleichartigen Objekten zu ordnen.
        /// Wird beim Hardware-Instancing verwendet.
        /// </summary>
        string GameObjectCategory  { get; }

        /// <summary>
        /// Die Farbe des Modells.
        /// </summary>
        ModelColoring Coloring { get; }

        /// <summary>
        /// Die Textur des Modells.
        /// </summary>
        Texture2D Texture { get; }
    }
}
