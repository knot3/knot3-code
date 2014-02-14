using System;
using System.Collections;
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
using Knot3.KnotData;
using Knot3.Widgets;
using Knot3.Development;

namespace Knot3.Utilities
{
	/// <summary>
	/// Eine Hilfsklasse für Dateioperationen.
	/// </summary>
	public static class FileUtility
	{
		#region Methods

		/// <summary>
		/// Konvertiert einen Namen eines Knotens oder einer Challenge in einen gültigen Dateinamen durch Weglassen ungültiger Zeichen.
		/// </summary>
		public static string ConvertToFileName (string name)
		{
			char[] arr = name.ToCharArray ();
			arr = Array.FindAll<char> (arr, (c => (char.IsLetterOrDigit (c)
			                                       || char.IsWhiteSpace (c)
			                                       || c == '-'))
			                          );
			return new string (arr);
		}

		/// <summary>
		/// Liefert einen Hash-Wert zu der durch filename spezifizierten Datei.
		/// </summary>
		public static string GetHash (string filename)
		{
			return string.Join ("\n", FileUtility.ReadFrom (filename)).ToMD5Hash ();
		}

		public static string ToMD5Hash (this string TextToHash)
		{
			if (string.IsNullOrEmpty (TextToHash)) {
				return string.Empty;
			}

			MD5 md5 = new MD5CryptoServiceProvider ();
			byte[] textToHash = Encoding.Default.GetBytes (TextToHash);
			byte[] result = md5.ComputeHash (textToHash);

			return System.BitConverter.ToString (result);
		}

		public static IEnumerable<string> ReadFrom (string file)
		{
			string line;
			using (var reader = File.OpenText (file)) {
				while ((line = reader.ReadLine ()) != null) {
					yield return line;
				}
			}
		}

		public static void SearchFiles (IEnumerable<string> directories, IEnumerable<string> extensions, Action<string> add)
		{
			foreach (string directory in directories) {
				SearchFiles (directory, extensions, add);
			}
		}

		public static void SearchFiles (string directory, IEnumerable<string> extensions, Action<string> add)
		{
			Directory.CreateDirectory (directory);
			var files = Directory.GetFiles (directory, "*.*", SearchOption.AllDirectories)
			            .Where (s => extensions.Any (e => s.EndsWith (e)));
			foreach (string file in files) {
				add (file);
			}
		}

		#endregion
	}
}
