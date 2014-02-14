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
using System.Linq;

using Knot3.Core;
using Knot3.Development;

namespace Knot3
{
	static class Program
	{
		private static Knot3Game game;

		/// <summary>
		/// The main entry point for the application.
		///
		/// </summary>
		///
		[STAThread]
		static void Main ()
		{
			Log.Message ("Knot" + Char.ConvertFromUtf32 ('\u00B3').ToString () + " " + Version);
			Log.Message ("Copyright (C) 2013-2014 Tobias Schulz, Maximilian Reuter,\n" +
			             "Pascal Knodel, Gerd Augsburg, Christina Erler, Daniel Warzel,\n" +
			             "M. Retzlaff, F. Kalka, G. Hoffmann, T. Schmidt, G. Mückl, Torsten Pelzer");

			game = new Knot3Game ();
			game.Run ();
		}

		/// <summary>
		/// Gibt die Versionsnummer zurück.
		/// </summary>
		/// <returns></returns>
		public static string Version
		{
			get {
				return System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
			}
		}
	}
}
