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

using System.Diagnostics.CodeAnalysis;

namespace Knot3.Framework.Models
{
    /// <summary>
    /// Die Klasse GameObjectDistance beschreibt den Abstand zu einem Spielobjekt.
    /// Dies ist z.B. hilfreich, um Schnittstellen, Verdeckungen, ... zu berechnen.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class GameObjectDistance
    {
        /// <summary>
        /// Ein Spielobjekt.
        /// </summary>
        public IGameObject Object { get; set; }

        /// <summary>
        /// Distance hält den Abstand als Gleitkommawert.
        /// </summary>
        public float Distance { get; set; }
    }
}
