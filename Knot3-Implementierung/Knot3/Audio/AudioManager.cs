using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.KnotData;
using Knot3.Widgets;
using Knot3.Utilities;
using Knot3.Audio.XNA;
using Knot3.Audio.FFmpeg;

namespace Knot3.Audio
{
	public class AudioManager : DrawableGameScreenComponent
	{
		private static readonly Dictionary<Sound, string> AudioDirectories
		= new Dictionary<Sound, string> {
			{ Sound.CreativeMusic,		"Music-Creative" },
			{ Sound.ChallengeMusic,		"Music-Challenge" },
			{ Sound.MenuMusic,			"Music-Menu" },
			{ Sound.PipeSound,			"Sound-Pipe" },
		};
		private static Dictionary<Sound, HashSet<IAudioFile>> AudioFiles
			= new Dictionary<Sound, HashSet<IAudioFile>> ();
		private static Sound _backgroundMusic = Sound.None;

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

		public static IPlaylist Playlist { get; set; }

		public AudioManager (GameScreen screen)
			: base(screen, DisplayLayer.None)
		{
			// Erstelle für alle Enum-Werte von Sound ein HashSet
			foreach (Sound soundType in typeof(Sound).ToEnumValues<Sound>()) {
				AudioFiles [soundType] = new HashSet<IAudioFile> ();
			}

			// Suche nach XNA-Audio-Dateien
			FileUtility.SearchFiles (".", new string[]{".xnb"}, AddXnaAudioFile);

			// Suche nach FFmpeg-Audio-Dateien
			FileUtility.SearchFiles (".", new string[]{".mp3", ".ogg", ".wav", ".wma"}, AddFFmpegAudioFile);
		}

		private void AddXnaAudioFile (string filepath)
		{
			filepath = filepath.Replace (".xnb", "").Replace (@"Content\", "").Replace ("Content/", "");

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
				AudioFiles [soundType].Add (new SoundEffectFile (name, soundEffect));
				Console.WriteLine ("Load sound effect (" + soundType + "): " + filepath);
			}
			catch (Exception ex) {
				// wenn man versucht, einen "Song" als "SoundEffect" zu laden,
				// dann bekommt man unter Windows eine "ContentLoadException"
				// und unter Linux eine "InvalidCastException"
				if (ex is ContentLoadException || ex is InvalidCastException) {
					LoadXnaSong (filepath, name, soundType);
				}
				else {
					throw;
				}
			}
		}

		private void LoadXnaSong (string filepath, string name, Sound soundType)
		{
			// nur unter Windows
			if (MonoHelper.IsRunningOnMono ())
				return;

			try {
				// versuche, die Audiodatei als "Song" zu laden
				Song song = Screen.Content.Load<Song> (filepath);
				AudioFiles [soundType].Add (new SongFile (name, song));
				Console.WriteLine ("Load song (" + soundType + "): " + filepath);
			}
			catch (Exception ex) {
				// egal, warum das laden nicht klappt; mehr als die Fehlermeldung anzeigen
				// macht wegen einer fehlenden Musikdatei keinen Sinn
				
				Console.WriteLine ("Failed to load audio file (" + soundType + "): " + filepath);
				Console.WriteLine (ex.ToString ());
			}
		}

		private void AddFFmpegAudioFile (string filepath)
		{
			filepath = filepath.Replace (@"\", "/");

			foreach (KeyValuePair<Sound,string> pair in AudioDirectories) {
				Sound soundType = pair.Key;
				string directory = pair.Value;
				if (filepath.ToLower ().Contains (directory.ToLower ())) {
					string name = Path.GetFileName (filepath);
					LoadFFmpegAudioFile (filepath, name, soundType);
					break;
				}
			}
		}

		private void LoadFFmpegAudioFile (string filepath, string name, Sound soundType)
		{
			// nur unter Linux
			if (!MonoHelper.IsRunningOnMono ())
				return;

			try {
				// erstelle ein AudioFile-Objekt
				AudioFiles [soundType].Add (new AudioFile (name, filepath));
				Console.WriteLine ("Load ffmpeg audio file (" + soundType + "): " + filepath);
			}
			catch (Exception ex) {
				// egal, warum das laden nicht klappt; mehr als die Fehlermeldung anzeigen
				// macht wegen einer fehlenden Musikdatei keinen Sinn
				Console.WriteLine ("Failed to load ffmpeg audio file (" + soundType + "): " + filepath);
				Console.WriteLine (ex.ToString ());
			}
		}

		private void StartBackgroundMusic ()
		{
			if (Playlist != null)
				Playlist.Stop ();
			Console.WriteLine ("Background Music: " + BackgroundMusic);
			Playlist = new StandardPlaylist (AudioFiles [BackgroundMusic]);
			Playlist.Start ();
		}

		public override void Update (GameTime time)
		{
			if (Playlist != null)
				Playlist.Update (time);
			base.Update (time);
		}

		protected override void UnloadContent ()
		{
			Console.WriteLine("UnloadContent ()");
			Playlist.Stop ();
			base.UnloadContent();
		}
	}
}

