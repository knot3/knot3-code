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

using Knot3.Framework.Platform;
using System;
using System.IO;


namespace Knot3.UnitTests
{
    public static class TestHelper
    {
        private static Random random = new Random ();

        public static string TestResourcesDirectory
        {
            get {
                return SystemInfo.RelativeBaseDirectory + "Resources" + SystemInfo.PathSeparator;
            }
        }

        public static string TempDirectory
        {
            get {
                string directory;
                /*
                if (SystemInfo.IsRunningOnLinux ()) {
                	directory = "/var/tmp/knot3-tests/";
                }
                else {
                	directory = Path.GetTempPath () + "\\Knot3-Tests\\";
                }
                */
                directory = "tmp" + SystemInfo.PathSeparator;
                Directory.CreateDirectory (directory);
                return directory;
            }
        }

        public static string RandomFilename (string extension)
        {
            string filename = TestHelper.TempDirectory + "test" + (random.Next () % 100) + "." + extension;

            try {
                File.Delete (filename);
            }
            catch (IOException) {
            }

            return filename;
        }
    }
}
