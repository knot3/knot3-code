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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Knot3.Framework.Storage
{
    public sealed class IniFile : IDisposable
    {
        private string Filename;
        public Dictionary<string, Dictionary<string, string>> Data;

        public IniFile (string filename)
        {
            Data = new Dictionary<string, Dictionary<string, string>> ();
            Filename = filename;
            if (File.Exists (filename)) {
                using (StreamReader reader = new StreamReader (filename)) {
                    string section = null;
                    while (reader.Peek () != -1) {
                        string line = StripComments (reader.ReadLine ().Trim ());
                        if (line.StartsWith ("[") && line.EndsWith ("]")) {
                            section = line.Substring (1, line.Length - 2);
                            if (!Data.ContainsKey (section)) {
                                Data [section] = new Dictionary<string,string> ();
                            }
                        }
                        else if (line.Contains ("=")) {
                            string[] parts = line.Split ('=');
                            if (section != null) {
                                Data [section] [Decode (parts [0].Trim ())] = Decode (parts [1].Trim ());
                            }
                        }
                    }
                }
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        [ExcludeFromCodeCoverageAttribute]
        private void Dispose (bool disposing)
        {
            if (disposing) {
                Save ();
            }
        }

        public void Save ()
        {
            using (StreamWriter writer = new StreamWriter (Filename)) {
                foreach (string section in Data.Keys.OrderBy (x => x)) {
                    writer.WriteLine ("[" + section + "]");
                    foreach (string key  in Data [section].Keys.OrderBy (x => x)) {
                        writer.WriteLine (Encode (key) + "=" + Encode (Data [section] [key]));
                    }
                }
            }
        }

        private static string StripComments (string line)
        {
            if (line != null) {
                if (line.Contains ("//")) {
                    return line.Remove (line.IndexOf ("//")).Trim ();
                }
                return line.Trim ();
            }
            return string.Empty;
        }

        public string this [string section, string key, string defaultValue = null]
        {
            get {
                if (!Data.ContainsKey (section)) {
                    Data [section] = new Dictionary<string,string> ();
                }
                if (!Data [section].ContainsKey (key)) {
                    Data [section] [key] = defaultValue;
                    Save ();
                }
                string value = Data [section] [key];
                return value;
            }
            set {
                if (!Data.ContainsKey (section)) {
                    Data [section] = new Dictionary<string,string> ();
                }
                Data [section] [key] = value;
                Save ();
            }
        }

        private string Encode (string text)
        {
            return text.Replace ("\r", "").Replace ("\n", "\\n");
        }

        private string Decode (string text)
        {
            return text.Replace ("\\n", "\n");
        }
    }
}
