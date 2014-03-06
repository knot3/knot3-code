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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Knot3.Framework.Core;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Audio
{
    public abstract class AudioManager : DrawableGameComponent
    {
        /// <summary>
        /// Eine Zuordnung zwischen dem Typ der Audiodateien und den Ordnern unter "Content/",
        /// in denen sich die Audiodateien befinden.
        /// </summary>
        public Dictionary<Sound, string> AudioDirectories { get; private set; }

        // Enthält alle gefunden Audiodateien, sortiert nach ihrem Zweck
        private Dictionary<Sound, HashSet<IAudioFile>> AudioFiles = new Dictionary<Sound, HashSet<IAudioFile>> ();

        /// <summary>
        /// Die aktuell verwendete Hintergrundmusik.
        /// </summary>
        public Sound BackgroundMusic
        {
            get {
                return _backgroundMusic;
            }
            set {
                if (value != Sound.None && value != _backgroundMusic) {
                    _backgroundMusic = value;
                    StartBackgroundMusic ();
                }
            }
        }

        private Sound _backgroundMusic = Sound.None;

        /// <summary>
        /// Enthält die Playlist, die aktuell abgespielt wird,
        /// oder null, falls keine Playlist abgespielt wird.
        /// </summary>
        public IPlaylist Playlist { get; set; }

        private static Dictionary<Sound, float> VolumeMap = new Dictionary<Sound, float> ();

        /// <summary>
        /// Erstellt einen neuen AudioManager für den angegebenen Spielzustand.
        /// </summary>
        public AudioManager (Game game)
        : base (game)
        {
            AudioDirectories = new Dictionary<Sound, string> ();
        }

        public override void Initialize ()
        {
            Initialize (SystemInfo.RelativeContentDirectory);
        }

        public virtual void Initialize (string directory)
        {
            if (SystemInfo.IsRunningOnMonogame())
            base.Initialize ();

            if (AudioFiles.Count == 0) {
                // Erstelle für alle Enum-Werte von Sound ein HashSet
                foreach (Sound soundType in Sound.Values) {
                    AudioFiles [soundType] = new HashSet<IAudioFile> ();
                    VolumeMap [soundType] = ValidVolume (Config.Default ["volume", soundType.ToString (), 1]);
                }

                // Suche nach OGG-Dateien
                FileUtility.SearchFiles (directory, new string[] {".ogg"}, AddOggAudioFile);
            }
        }

        public void Reset ()
        {
            AudioFiles.Clear ();
            VolumeMap.Clear ();
        }

        private void AddOggAudioFile (string filepath)
        {
            filepath = filepath.Replace (@"\", "/");

            foreach (KeyValuePair<Sound,string> pair in AudioDirectories) {
                Sound soundType = pair.Key;
                string directory = pair.Value;
                if (filepath.ToLower ().Contains (directory.ToLower ())) {
                    string name = Path.GetFileName (filepath);
                    LoadOggAudioFile (filepath, name, soundType);
                    break;
                }
            }
        }

        private void LoadOggAudioFile (string filepath, string name, Sound soundType)
        {
            try {
                // erstelle ein OggVorbisFile-Objekt
                AudioFiles [soundType].Add (new OggVorbisFile (name, filepath, soundType));
            }
            catch (Exception ex) {
                // egal, warum das laden nicht klappt; mehr als die Fehlermeldung anzeigen
                // macht wegen einer fehlenden Musikdatei keinen Sinn
                Log.Debug ("Failed to load ogg audio file (", soundType, "): ", filepath);
                Log.Debug (ex);
            }
        }

        private void StartBackgroundMusic ()
        {
            if (Playlist != null) {
                Playlist.Stop ();
            }
            Log.Debug ("Background Music: ", BackgroundMusic);
            if (AudioFiles.ContainsKey (BackgroundMusic)) {
                Playlist = new LoopPlaylist (AudioFiles [BackgroundMusic]);
                Playlist.Shuffle ();
                Playlist.Start ();
            }
            else {
                Log.Message ("Warning: ", BackgroundMusic, ": no sound files available!");
            }
        }

        public void PlaySound (Sound sound)
        {
            Log.Debug ("Sound: ", sound);
            if (AudioFiles [sound].Count > 0) {
                AudioFiles [sound].RandomElement ().Play ();
            }
            else {
                Log.Debug ("There are no audio files for: ", sound);
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            if (Playlist != null) {
                Playlist.Update (time);
            }
            base.Update (time);
        }

        public static float Volume (Sound soundType)
        {
            return VolumeMap [soundType];
        }

        public static void SetVolume (Sound soundType, float volume)
        {
            volume = ValidVolume (volume);
            VolumeMap [soundType] = volume;
            Config.Default ["volume", soundType.ToString (), 1] = volume;
            Log.Debug ("Set Volume (", soundType, "): ", volume);
        }

        public static float ValidVolume (float volume)
        {
            return MathHelper.Clamp (volume, 0.0f, 2.0f);
        }
    }
}
