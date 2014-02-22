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
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.GameObjects;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.Audio
{
	public class AudioManager : DrawableGameScreenComponent
	{
		/// <summary>
		/// Eine Zuordnung zwischen dem Typ der Audiodateien und den Ordnern unter "Content/",
		/// in denen sich die Audiodateien befinden.
		/// </summary>
		private static readonly Dictionary<Sound, string> AudioDirectories
		= new Dictionary<Sound, string> {
			{ Sound.CreativeMusic,			"Music/Creative" },
			{ Sound.ChallengeMusic,			"Music/Challenge" },
			{ Sound.MenuMusic,				"Music/Menu" },
			{ Sound.PipeMoveSound,			"Sound/Pipe/Move" },
			{ Sound.PipeInvalidMoveSound,	"Sound/Pipe/Invalid-Move" },
		};

		// Enthält alle gefunden Audiodateien, sortiert nach ihrem Zweck
		private static Dictionary<Sound, HashSet<IAudioFile>> AudioFiles
		    = new Dictionary<Sound, HashSet<IAudioFile>> ();

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

		private static Sound _backgroundMusic = Sound.None;

		/// <summary>
		/// Enthält die Playlist, die aktuell abgespielt wird,
		/// oder null, falls keine Playlist abgespielt wird.
		/// </summary>
		public static IPlaylist Playlist { get; set; }

		private static Dictionary<Sound, float> VolumeMap = new Dictionary<Sound, float> ();

		/// <summary>
		/// Erstellt einen neuen AudioManager für den angegebenen Spielzustand.
		/// </summary>
		public AudioManager (IGameScreen screen, string directory)
		: base (screen, DisplayLayer.None)
		{
			if (AudioFiles.Count == 0) {
				// Erstelle für alle Enum-Werte von Sound ein HashSet
				foreach (Sound soundType in typeof (Sound).ToEnumValues<Sound>()) {
					AudioFiles [soundType] = new HashSet<IAudioFile> ();
					VolumeMap [soundType] = ValidVolume (Options.Default ["volume", soundType.ToString (), 1]);
				}

				// Suche nach XNA-Audio-Dateien
				FileUtility.SearchFiles (directory, new string[] {".xnb"}, AddXnaAudioFile);

				// Suche nach OGG-Dateien
				FileUtility.SearchFiles (directory, new string[] {".ogg"}, AddOggAudioFile);
			}
		}

		public AudioManager (IGameScreen screen)
		: this (screen, SystemInfo.RelativeContentDirectory)
		{
		}

		public static void Reset ()
		{
			AudioFiles.Clear ();
			VolumeMap.Clear ();
		}

		private void AddXnaAudioFile (string filepath)
		{
			filepath = filepath.Replace (".xnb", String.Empty).Replace (@"Content\", String.Empty).Replace ("Content/", String.Empty).Replace (@"\", "/");

			foreach (KeyValuePair<Sound,string> pair in AudioDirectories) {
				Sound soundType = pair.Key;
				string directory = pair.Value;
				if (filepath.ToLower ().Contains (directory.ToLower ())) {
					string name = Path.GetFileName (filepath);
					LoadXnaSoundEffect (filepath, name, soundType);
					break;
				}
			}
		}

		private void LoadXnaSoundEffect (string filepath, string name, Sound soundType)
		{
			try {
				// versuche, die Audiodatei als "SoundEffect" zu laden
				SoundEffect soundEffect = Screen.Content.Load<SoundEffect> (filepath);
				AudioFiles [soundType].Add (new SoundEffectFile (name, soundEffect, soundType));
				Log.Debug ("Load sound effect (", soundType, "): ", filepath);
			}
			catch (Exception ex) {
				Log.Debug (ex);
			}
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
				// erstelle ein AudioFile-Objekt
				Log.Debug ("Load ogg audio file (", soundType, "): ", filepath);
				AudioFiles [soundType].Add (new OggVorbisFile (name, filepath, soundType));
			}
			catch (Exception ex) {
				// egal, warum das laden nicht klappt; mehr als die Fehlermeldung anzeigen
				// macht wegen einer fehlenden Musikdatei keinen Sinn
				Log.Debug ("Failed to load ffmpeg audio file (", soundType, "): ", filepath);
				Log.Debug (ex);
			}
		}

		private void StartBackgroundMusic ()
		{
			if (Playlist != null) {
				Playlist.Stop ();
			}
			Log.Debug ("Background Music: ", BackgroundMusic);
			Playlist = new LoopPlaylist (AudioFiles [BackgroundMusic]);
			Playlist.Shuffle ();
			Playlist.Start ();
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

		protected override void UnloadContent ()
		{
			Log.Debug ("UnloadContent ()");
			Playlist.Stop ();
			base.UnloadContent ();
		}

		public static float Volume (Sound soundType)
		{
			return VolumeMap [soundType];
		}

		public static void SetVolume (Sound soundType, float volume)
		{
			volume = ValidVolume (volume);
			VolumeMap [soundType] = volume;
			Options.Default ["volume", soundType.ToString (), 1] = volume;
			Log.Debug ("Set Volume (", soundType, "): ", volume);
		}

		public static float ValidVolume (float volume)
		{
			return MathHelper.Clamp (volume, 0.0f, 2.0f);
		}
	}
}