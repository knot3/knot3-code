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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Widgets;
using Knot3.Development;
using Knot3.Utilities;

#endregion

namespace Knot3.Platform
{
	public static partial class SystemInfo
	{
		#region Properties

		/// <summary>
		/// Das Einstellungsverzeichnis.
		/// </summary>
		public static string SettingsDirectory
		{
			get {
				string directory;
				if (SystemInfo.IsRunningOnLinux ()) {
					directory = Environment.GetEnvironmentVariable ("HOME") + "/.knot3/";
				}
				else {
					directory = Environment.GetFolderPath (System.Environment.SpecialFolder.Personal) + "\\Knot3\\";
				}
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		/// <summary>
		/// Das Spielstandverzeichnis.
		/// </summary>
		public static string SavegameDirectory
		{
			get {
				string directory = SettingsDirectory + PathSeparator.ToString () + "Savegames";
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		/// <summary>
		/// Das Bildschirmfotoverzeichnis.
		/// </summary>
		public static string ScreenshotDirectory
		{
			get {
				string directory;
				if (SystemInfo.IsRunningOnLinux ()) {
					directory = Environment.GetEnvironmentVariable ("HOME");
				}
				else {
					directory = Environment.GetFolderPath (System.Environment.SpecialFolder.MyPictures) + "\\Knot3\\";
				}
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		public static string DecodedMusicCache
		{
			get {
				string directory;
				if (SystemInfo.IsRunningOnLinux ()) {
					directory = "/var/tmp/knot3/";
				}
				else {
					directory = Environment.GetFolderPath (System.Environment.SpecialFolder.MyMusic) + "\\Knot3\\";
				}
				Directory.CreateDirectory (directory);
				return directory;
			}
		}

		public static string BaseDirectory
		{
			get {
				if (baseDirectory != null) {
					return baseDirectory;
				}
				else {
					string cwd = Directory.GetCurrentDirectory ();
					string[] binDirectories = new string[] {
						"Debug",
						"Release",
						"x86",
						"bin"
					};
					foreach (string dir in binDirectories) {
						if (cwd.ToLower ().EndsWith (dir.ToLower ())) {
							cwd = cwd.Substring (0, cwd.Length - dir.Length - 1);
						}
					}
					// Environment.CurrentDirectory = cwd;
					Log.Debug (cwd);
					baseDirectory = cwd;
					return cwd;
				}
			}
		}

		private static string baseDirectory = null;

		public static char PathSeparator { get { return Path.DirectorySeparatorChar; } }

		#endregion
	}
}