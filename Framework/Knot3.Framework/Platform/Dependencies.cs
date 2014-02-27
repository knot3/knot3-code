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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

using Ionic.Zip;

namespace Knot3.Framework.Platform
{
    [ExcludeFromCodeCoverageAttribute]
    public static class Dependencies
    {
        public static string DOWNLOAD_URL_SDL2 = "http://www.libsdl.org/release/SDL2-2.0.1-win32-x86.zip";

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
                            // extract the file to the current directory
                            entry.Extract (".", ExtractExistingFileAction.OverwriteSilently);
                            // downloading was obviously sucessful
                            ++ extractedFiles;
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
    }
}
