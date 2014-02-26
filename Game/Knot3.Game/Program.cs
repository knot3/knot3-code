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
using System.Linq;
using System.Windows.Forms;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;

namespace Knot3.Game
{
    [ExcludeFromCodeCoverageAttribute]
    static class Program
    {
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
                         "M. Retzlaff, F. Kalka, G. Hoffmann, T. Schmidt, G. Mückl, Torsten Pelzer"
                        );
            Log.Message ();

            try {
                GameLoop ();
            }
            catch (DllNotFoundException ex) {
                Log.Message ();
                Log.Error (ex);
                Log.Message ();
                if (ex.ToString ().ToLower ().Contains ("sdl2.dll")) {
                    Log.ShowMessageBox ("This game requires SDL2. It will be downloaded now.", "Dependency missing");
                    if (Dependencies.DownloadSDL2 ()) {
                        System.Diagnostics.Process.Start (Application.ExecutablePath); // to start new instance of application
                        Application.Exit ();
                    }
                    else {
                        Log.ShowMessageBox ("SDL2 could not be downloaded.", "Dependency missing");
                    }
                }
            }
        }

        private static void GameLoop ()
        {
            using (Knot3Game game = new Knot3Game ()) {
                game.Run ();
            }
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
