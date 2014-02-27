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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Knot3.Framework.Core;
using Knot3.Framework.Models;

namespace Knot3.Game.Models
{
    /// <summary>
    /// Ein Zwischenspeicher für 3D-Modelle.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class ModelFactory
    {
        /// <summary>
        /// Die Zuordnung zwischen den Modellinformationen zu den 3D-Modellen.
        /// </summary>
        private Dictionary<GameModelInfo, GameModel> cache { get; set; }

        /// <summary>
        /// Ein Delegate, das beim Erstellen eines Zwischenspeichers zugewiesen wird und aus den
        /// angegebenen Modellinformationen und dem angegebenen Spielzustand ein 3D-Modell erstellt.
        /// </summary>
        private Func<IGameScreen, GameModelInfo, GameModel> createModel { get; set; }

        /// <summary>
        /// Erstellt einen neuen Zwischenspeicher.
        /// </summary>
        public ModelFactory (Func<IGameScreen, GameModelInfo, GameModel> createModel)
        {
            this.createModel = createModel;
            cache = new Dictionary<GameModelInfo, GameModel> ();
        }

        /// <summary>
        /// Falls das 3D-Modell zwischengespeichert ist, wird es zurückgegeben, sonst mit createModel () erstellt.
        /// </summary>
        public GameModel this [IGameScreen screen, GameModelInfo info]
        {
            get {
                if (cache.ContainsKey (info)) {
                    return cache [info];
                }
                else {
                    return cache [info] = createModel (screen, info);
                }
            }
        }
    }
}
