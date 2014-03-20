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
    /// Eine Spielkomponente, die in einem IGameScreen verwendet wird und eine bestimmte Priorität hat.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class ScreenComponent : GameComponent, IScreenComponent
    {
        /// <summary>
        /// Die Zeichen- und Eingabepriorität.
        /// </summary>
        public DisplayLayer Index { get; set; }

        /// <summary>
        /// Der zugewiesene Spielzustand.
        /// </summary>
        public IScreen Screen { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz eines IGameScreenComponent-Objekts und initialisiert diese mit dem zugehörigen IGameScreen und der zugehörigen Zeichenreihenfolge. Diese Spielkomponente kann nur in dem zugehörigen IGameScreen verwendet werden.
        /// </summary>
        public ScreenComponent (IScreen screen, DisplayLayer index)
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
    public static class GameComponentExtensions
    {
        public static ScreenComponent ToScreenComponent (this GameComponent component, IScreen screen, DisplayLayer index)
        {
            return new GameComponentWrapper (component, screen, index);
        }

        public static ScreenComponent ToScreenComponent (this GameComponent component, IScreen screen)
        {
            return component.ToScreenComponent (screen, DisplayLayer.None);
        }

        [ExcludeFromCodeCoverage]
        class GameComponentWrapper : ScreenComponent
        {
            private GameComponent Wrapped;

            public GameComponentWrapper (GameComponent component, IScreen screen, DisplayLayer index)
            : base (screen, index)
            {
                Wrapped = component;
            }

            public override void Update (GameTime time)
            {
                Wrapped.Update (time);
            }
        }
    }
}
