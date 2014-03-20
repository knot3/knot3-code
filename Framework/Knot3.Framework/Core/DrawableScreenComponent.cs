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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

namespace Knot3.Framework.Core
{
    /// <summary>
    /// Eine zeichenbare Spielkomponente, die in einem angegebenen Spielzustand verwendet wird und eine bestimmte Priorität hat.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class DrawableScreenComponent : DrawableGameComponent, IScreenComponent
    {
        /// <summary>
        /// Der zugewiesene Spielzustand.
        /// </summary>
        public IScreen Screen { get; set; }

        private DisplayLayer _index;

        /// <summary>
        /// Die Zeichen- und Eingabepriorität.
        /// </summary>
        public DisplayLayer Index
        {
            get { return _index; }
            set {
                _index = value;
                DrawOrder = (int)value;
            }
        }

        /// <summary>
        /// Erzeugt eine neue Instanz eines DrawableGameScreenComponent-Objekts und ordnet dieser ein IGameScreen-Objekt zu.
        /// index bezeichnet die Zeichenebene, auf welche die Komponente zu zeichnen ist.
        /// </summary>
        public DrawableScreenComponent (IScreen screen, DisplayLayer index)
        : base (screen.Game)
        {
            this.Screen = screen;
            this.Index = index;
        }

        /// <summary>
        /// Gibt Spielkomponenten zurück, die in dieser Spielkomponente enthalten sind.
        /// [returntype=IEnumerable<IGameScreenComponent>]
        /// </summary>
        public virtual IEnumerable<IScreenComponent> SubComponents (GameTime GameTime)
        {
            yield break;
        }

        /// <summary>
        /// Lade die Inhalte des Spielkomponents.
        /// </summary>
        public virtual void LoadContent (GameTime time)
        {
        }

        /// <summary>
        /// Entlade die Inhalte des Spielkomponents.
        /// </summary>
        public virtual void UnloadContent (GameTime time)
        {
        }
    }

    [ExcludeFromCodeCoverage]
    public static class DrawableGameComponentExtensions
    {
        public static DrawableScreenComponent ToScreenComponent (this DrawableGameComponent component, IScreen screen, DisplayLayer index)
        {
            return new DrawableGameComponentWrapper (component, screen, index);
        }

        public static DrawableScreenComponent ToScreenComponent (this DrawableGameComponent component, IScreen screen)
        {
            return component.ToScreenComponent (screen, DisplayLayer.None);
        }

        [ExcludeFromCodeCoverage]
        class DrawableGameComponentWrapper : DrawableScreenComponent
        {
            private DrawableGameComponent Wrapped;

            public DrawableGameComponentWrapper (DrawableGameComponent component, IScreen screen, DisplayLayer index)
            : base (screen, index)
            {
                Wrapped = component;
            }

            public override void Update (GameTime time)
            {
                Wrapped.Update (time);
            }

            public override void Draw (GameTime time)
            {
                Wrapped.Draw (time);
            }
        }
    }
}
