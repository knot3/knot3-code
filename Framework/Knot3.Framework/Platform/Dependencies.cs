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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Windows.Forms;

using Ionic.Zip;

namespace Knot3.Framework.Platform
{
    [ExcludeFromCodeCoverageAttribute]
    public static class Dependencies
    {
        public static string DOWNLOAD_URL_SDL2 = "http://www.libsdl.org/release/SDL2-2.0.2-win32-x86.zip";
        public static string DOWNLOAD_URL_SDL2_image = "https://www.libsdl.org/projects/SDL_image/release/SDL2_image-2.0.0-win32-x86.zip";
        public static string DOWNLOAD_URL_OPENAL_INSTALLER = "https://rallyraid.googlecode.com/files/oalinst.zip";

        public static bool DownloadSDL2 ()
        {
            string zipFilename = "SDL2.zip";
            bool success = false;
            try {
                int extractedFiles = 0;
                // try to download the zip file
                if (Download (DOWNLOAD_URL_SDL2, zipFilename)) {
                    // read the zip file
                    using (ZipFile zip = ZipFile.Read (zipFilename)) {
                        // iterate over files in the zip file
                        foreach (ZipEntry entry in zip) {
                            CatchExtractExceptions (() => {
                                // extract the file to the current directory
                                entry.Extract (".", ExtractExistingFileAction.OverwriteSilently);
                                // downloading was obviously sucessful
                                ++ extractedFiles;
                            });
                        }
                    }
                }

                // if all files were extracted
                success = extractedFiles > 0;
            }
            catch (Exception ex) {
                // an error occurred
                Log.Error (ex);
                success = false;
            }

            // remove the zip file
            try {
                File.Delete (zipFilename);
            }
            catch (Exception ex) {
                Log.Error (ex);
            }

            return success;
        }

        public static bool DownloadSDL2_image ()
        {
            string zipFilename = "SDL2_image.zip";
            bool success = false;
            try {
                int extractedFiles = 0;
                // try to download the zip file
                if (Download (DOWNLOAD_URL_SDL2_image, zipFilename)) {
                    // read the zip file
                    using (ZipFile zip = ZipFile.Read (zipFilename)) {
                        // iterate over files in the zip file
                        foreach (ZipEntry entry in zip) {
                            CatchExtractExceptions (() => {
                                // extract the file to the current directory
                                entry.Extract (".", ExtractExistingFileAction.OverwriteSilently);
                                // downloading was obviously sucessful
                                ++ extractedFiles;
                            });
                        }
                    }
                }

                // if all files were extracted
                success = extractedFiles > 0;
            }
            catch (Exception ex) {
                // an error occurred
                Log.Error (ex);
                success = false;
            }

            // remove the zip file
            try {
                File.Delete (zipFilename);
            }
            catch (Exception ex) {
                Log.Error (ex);
            }

            return success;
        }

        public static bool DownloadOpenAL ()
        {
            string zipFilename = "openal32.zip";
            bool success = false;
            try {
                int extractedFiles = 0;
                // try to download the zip file
                if (Download (DOWNLOAD_URL_OPENAL_INSTALLER, zipFilename)) {
                    // read the zip file
                    using (ZipFile zip = ZipFile.Read (zipFilename)) {
                        // iterate over files in the zip file
                        foreach (ZipEntry entry in zip) {
                            CatchExtractExceptions (() => {
                                // extract the file to the current directory
                                entry.Extract (".", ExtractExistingFileAction.OverwriteSilently);
                                // downloading was obviously sucessful
                                ++ extractedFiles;
                            });
                        }
                    }
                }

                // if all files were extracted
                success = extractedFiles > 0;
            }
            catch (Exception ex) {
                // an error occurred
                Log.Error (ex);
                success = false;
            }

            // remove the zip file
            try {
                File.Delete (zipFilename);
            }
            catch (Exception ex) {
                Log.Error (ex);
            }

            return success;
        }

        private static bool Download (string httpUrl, string saveAs)
        {
            try {
                Log.Message ("Download:");
                Log.Message ("  HTTP URL: ", httpUrl);
                Log.Message ("  Save as:  ", saveAs);

                WebClient webClient = new WebClient ();
                webClient.DownloadFile (httpUrl, saveAs);

                return true;
            }
            catch (Exception ex) {
                Log.Error (ex);
                return false;
            }
        }

        public static void CatchExtractExceptions (Action action)
        {
            try {
                action ();
            }
            catch (Exception ex) {
                Log.Error (ex);
            }
        }

        public static void CatchDllExceptions (Action action)
        {
            Application.EnableVisualStyles ();

            if (!File.Exists ("oalinst.exe") && Dependencies.DownloadOpenAL ()) {
                System.Diagnostics.Process.Start ("oalinst.exe"); // to start the openal installer
                Log.ShowMessageBox ("Please install OpenAL and restart the game afterwards.", "Dependency missing");
                Application.Exit ();
                return;
            }

            try {
                action ();
            }
            catch (DllNotFoundException ex) {
                Log.Message ();
                Log.Error (ex);
                Log.Message ();
                if (ex.ToString ().ToLower ().Contains ("sdl2.dll")) {
                    Log.ShowMessageBox ("This game requires SDL2. It will be downloaded now.", "Dependency missing");
                    if (Dependencies.DownloadSDL2 () && (File.Exists ("SDL2_image.dll") || Dependencies.DownloadSDL2_image ())) {
                        if (!File.Exists ("oalinst.exe") && Dependencies.DownloadOpenAL ()) {
                            System.Diagnostics.Process.Start ("oalinst.exe"); // to start the openal installer
                            Log.ShowMessageBox ("Please install OpenAL and start the game afterwards.", "Dependency missing");
                            Application.Exit ();
                        }
                        else {
                            System.Diagnostics.Process.Start (Application.ExecutablePath); // to start new instance of application
                            Application.Exit ();
                        }
                    }
                    else {
                        Log.ShowMessageBox ("SDL2 could not be downloaded.", "Dependency missing");
                    }
                }
                if (ex.ToString ().ToLower ().Contains ("sdl2_image.dll")) {
                    Log.ShowMessageBox ("This game requires SDL2_image. It will be downloaded now.", "Dependency missing");
                    if (Dependencies.DownloadSDL2_image ()) {
                        System.Diagnostics.Process.Start (Application.ExecutablePath); // to start new instance of application
                        Application.Exit ();
                    }
                    else {
                        Log.ShowMessageBox ("SDL2_image could not be downloaded.", "Dependency missing");
                    }
                }
                if (ex.ToString ().ToLower ().Contains ("openal32.dll")) {
                    Log.ShowMessageBox ("This game requires OpenAL (openal32.dll). It will be downloaded now. Please restart the Game afterwards.", "Dependency missing");
                    if (File.Exists ("oalinst.exe") || Dependencies.DownloadOpenAL ()) {
                        System.Diagnostics.Process.Start ("oalinst.exe"); // to start the openal installer
                        Application.Exit ();
                    }
                    else {
                        Log.ShowMessageBox ("OpenAL could not be downloaded.", "Dependency missing");
                    }
                }
            }
        }
    }
}
