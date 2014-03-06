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

using Knot3.Framework.Core;
using Knot3.Framework.Models;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Die 3D-Modelle, die während einer Verschiebung von Kanten die Vorschaumodelle repräsentieren.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ShadowModel : IGameObject
    {
        private IScreen Screen;
        private GameModel DecoratedModel;

        /// <summary>
        /// Die Farbe der Vorschaumodelle.
        /// </summary>
        public Color ShadowColor { get; set; }

        /// <summary>
        /// Die Transparenz der Vorschaumodelle.
        /// </summary>
        public float ShadowAlpha { get; set; }

        /// <summary>
        /// Eine Referenz auf die Spielwelt, in der sich das Spielobjekt befindet.
        /// </summary>
        public World World
        {
            get { return DecoratedModel.World; }
            set {}
        }

        /// <summary>
        /// Die Position, an der das Vorschau-Spielobjekt gezeichnet werden soll.
        /// </summary>
        public Vector3 ShadowPosition { get; set; }

        /// <summary>
        /// Die Position, an der sich das zu dekorierende Objekt befindet.
        /// </summary>
        public Vector3 OriginalPosition
        {
            get { return DecoratedModel.Info.Position; }
        }

        GameObjectInfo IGameObject.Info { get { return Info; } }

        /// <summary>
        /// Die Modellinformationen wie Position, Skalierung und der Dateiname des 3D-Modells.
        /// </summary>
        public GameObjectInfo Info { get; private set; }

        /// <summary>
        /// Erstellt ein neues Vorschaumodell in dem angegebenen Spielzustand für das angegebene zu dekorierende Modell.
        /// </summary>
        public ShadowModel (IScreen screen, GameModel decoratedModel)
        {
            Screen = screen;
            DecoratedModel = decoratedModel;
            Info = new GameObjectInfo (position: Vector3.Zero, isVisible: true, isSelectable: false, isMovable: false);
        }

        /// <summary>
        /// Zeichnet das Vorschaumodell.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public void Draw (GameTime time)
        {
            // swap position and colors
            Vector3 originalPositon = DecoratedModel.Info.Position;
            ModelColoring originalColoring = DecoratedModel.Coloring;
            DecoratedModel.Info.Position = ShadowPosition;
            DecoratedModel.Coloring = new SingleColor (originalColoring.MixedColor, alpha: ShadowAlpha);

            // draw
            Screen.CurrentRenderEffects.CurrentEffect.DrawModel (DecoratedModel, time);

            // swap everything back
            DecoratedModel.Info.Position = originalPositon;
            DecoratedModel.Coloring = originalColoring;
        }

        /// <summary>
        /// Die Position, an der das Vorschau-Spielobjekt gezeichnet werden soll.
        /// </summary>
        public Vector3 Center ()
        {
            return ShadowPosition;
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        public void Update (GameTime GameTime)
        {
            Info.IsVisible = Math.Abs ((ShadowPosition - OriginalPosition).Length ()) > 50;
        }

        /// <summary>
        /// Prüft, ob der angegebene Mausstrahl das Vorschau-Spielobjekt schneidet.
        /// </summary>
        public GameObjectDistance Intersects (Ray Ray)
        {
            return null;
        }
    }
}
