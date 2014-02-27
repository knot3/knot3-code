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

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;

using Knot3.Game.Core;
using Knot3.Game.Data;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Eine Einführung in das Spielen von Challenges.
    /// Der Spieler wird dabei durch Anweisungen an das Lösen von Challenges herangeführt.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class TutorialChallengeModeScreen : ChallengeModeScreen
    {
        public TutorialChallengeModeScreen (Knot3Game game, Challenge challenge)
        : base (game, challenge)
        {
            throw new System.NotImplementedException ();
        }

        /// <summary>
        /// Fügt die Tutoriellanzeige in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IGameScreen previousScreen, GameTime GameTime)
        {
            throw new System.NotImplementedException ();
        }
    }
}
