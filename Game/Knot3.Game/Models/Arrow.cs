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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Effects;
using Knot3.Game.Input;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;
using Knot3.Game.Widgets;

#endregion

namespace Knot3.Game.Models
{
    /// <summary>
    /// Ein Objekt der Klasse ArrowModelInfo hält alle Informationen, die zur Erstellung eines Pfeil-3D-Modelles (s. ArrowModel) notwendig sind.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class Arrow : GameModelInfo
    {
        #region Properties

        /// <summary>
        /// Gibt die Richtung, in die der Pfeil zeigen soll an.
        /// </summary>
        public Direction Direction { get; private set; }

        public float Length { get { return 40f; } }

        public float Diameter { get { return 8f; } }

        private Dictionary<Direction, Angles3> RotationMap = new Dictionary<Direction, Angles3> ()
        {
            { Direction.Up, 		Angles3.FromDegrees (90, 0, 00) },
            { Direction.Down, 		Angles3.FromDegrees (270, 0, 0) },
            { Direction.Right, 		Angles3.FromDegrees (0, 270, 0) },
            { Direction.Left, 		Angles3.FromDegrees (0, 90, 0) },
            { Direction.Forward, 	Angles3.FromDegrees (0, 0, 0) },
            { Direction.Backward, 	Angles3.FromDegrees (180, 0, 0) },
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Erstellt ein neues ArrowModelInfo-Objekt an einer bestimmten Position position im 3D-Raum. Dieses zeigt in eine durch direction bestimmte Richtung.
        /// </summary>
        public Arrow (Vector3 position, Direction direction)
        : base ("arrow")
        {
            Direction = direction;
            Position = position + Direction.Vector * Node.Scale / 3;
            Scale = new Vector3 (7,7,20);
            IsMovable = true;

            // Berechne die Drehung
            Rotation += RotationMap [direction];
        }

        #endregion
    }
}
