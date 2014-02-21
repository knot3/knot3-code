using System;
using System.Net;
using Knot3.Framework.Platform;
using Ionic.Zip;
using System.IO;

namespace Knot3.Framework.Platform
{
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

