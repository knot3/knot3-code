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
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Audio
{
    /// <summary>
    /// Diese Klasse repräsentiert eine Playlist, deren Audiodateien der reihe nach in einer
    /// Endlosschleife abgespielt werden.
    /// </summary>
    public class LoopPlaylist : IPlaylist
    {
        private List<IAudioFile> Sounds;
        private int index;

        public SoundState State { get; private set; }

        /// <summary>
        /// Erstellt eine neue Playlist.
        /// </summary>
        /// <param name='sounds'>
        /// Die abzuspielenden Audiodateien.
        /// </param>
        public LoopPlaylist (IEnumerable<IAudioFile> sounds)
        {
            Sounds = sounds.ToList ();
            index = 0;
            State = SoundState.Stopped;

            Log.Debug ("Created new playlist (", Sounds.Count, " songs)");
            foreach (IAudioFile sound in Sounds) {
                Log.Debug ("  - ", sound.Name);
            }
        }

        public void Shuffle ()
        {
            Sounds = Sounds.Shuffle ().ToList ();
        }

        /// <summary>
        /// Starte die Wiedergabe.
        /// </summary>
        public void Start ()
        {
            if (Sounds.Count > 0) {
                State = SoundState.Playing;
                Sounds .At (index).Play ();
            }
        }

        /// <summary>
        /// Stoppe die Wiedergabe.
        /// </summary>
        public void Stop ()
        {
            if (Sounds.Count > 0) {
                State = SoundState.Stopped;
                Sounds.At (index).Stop ();
            }
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public void Update (GameTime time)
        {
            if (Sounds.Count > 0) {
                if (State == SoundState.Playing && Sounds.At (index).State != SoundState.Playing) {
                    ++index;
                    Sounds.At (index).Play ();
                }
            }
            if (index >= 0 && index < Sounds.Count) {
                Sounds.At (index).Update (time);
            }
        }
    }
}
