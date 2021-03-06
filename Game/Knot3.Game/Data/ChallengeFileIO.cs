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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Ionic.Zip;

using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Implementiert das Speicherformat für Challenges.
    /// </summary>
    public sealed class ChallengeFileIO : IChallengeIO
    {
        /// <summary>
        /// Die für eine Knoten-Datei gültigen Dateiendungen.
        /// </summary>
        public IEnumerable<string> FileExtensions
        {
            get {
                yield return ".challenge";
                yield return ".chl";
                yield return ".chn";
                yield return ".chg";
                yield return ".chlng";
                yield return ".chal";
                yield return ".chall";
                yield return ".chllng";
            }
        }

        /// <summary>
        /// Erstellt ein ChallengeFileIO-Objekt.
        /// </summary>
        public ChallengeFileIO ()
        {
        }

        /// <summary>
        /// Speichert eine Challenge in dem Dateinamen, der in dem Challenge-Objekt enthalten ist.
        /// </summary>
        public void Save (Challenge challenge, bool force)
        {
            if (!force && File.Exists (challenge.MetaData.Filename)) {
                throw new FileAlreadyExistsException ("Error! Challenge already exists!");
            }
            else {
                using (ZipFile zip = new ZipFile ()) {
                    // Namen
                    zip.AddEntry ("name.txt", challenge.Name);
                    // Startknoten
                    KnotStringIO parser = new KnotStringIO (challenge.Start);
                    zip.AddEntry ("start.knot", parser.Content);
                    // Zielknoten
                    parser = new KnotStringIO (challenge.Target);
                    zip.AddEntry ("target.knot", parser.Content);
                    // Highscore
                    zip.AddEntry ("highscore.txt", string.Join ("\n", printHighscore (challenge.Highscore)));
                    // ZIP-Datei speichern
                    zip.Save (challenge.MetaData.Filename);
                }
            }
        }

        /// <summary>
        /// Lädt eine Challenge aus einer angegebenen Datei.
        /// </summary>
        public Challenge Load (string filename)
        {
            ChallengeMetaData meta = LoadMetaData (filename: filename);
            Knot start = null;
            Knot target = null;

            using (ZipFile zip = ZipFile.Read (filename)) {
                foreach (ZipEntry entry in zip) {
                    string content = entry.ReadContent ();

                    // für die Datei mit dem Startknoten
                    if (entry.FileName.ToLower ().Contains ("start")) {
                        KnotStringIO parser = new KnotStringIO (content: content);
                        start = new Knot (
                            new KnotMetaData (parser.Name, () => parser.CountEdges, null, null),
                            parser.Edges
                        );
                    }

                    // für die Datei mit dem Zielknoten
                    else if (entry.FileName.ToLower ().Contains ("target")) {
                        KnotStringIO parser = new KnotStringIO (content: content);
                        target = new Knot (
                            new KnotMetaData (parser.Name, () => parser.CountEdges, null, null),
                            parser.Edges
                        );
                    }
                }
            }

            if (meta != null && start != null && target != null) {
                return new Challenge (meta, start, target);
            }
            else {
                throw new IOException (
                    "Error! Invalid challenge file: " + filename
                    + " (meta=" + meta + ",start=" + start + ",target=" + target + ")"
                );
            }
        }

        /// <summary>
        /// Lädt die Metadaten einer Challenge aus einer angegebenen Datei.
        /// </summary>
        public ChallengeMetaData LoadMetaData (string filename)
        {
            string name = null;
            KnotMetaData start = null;
            KnotMetaData target = null;
            IEnumerable<KeyValuePair<string, int>> highscore = null;
            using (ZipFile zip = ZipFile.Read (filename)) {
                foreach (ZipEntry entry in zip) {
                    string content = entry.ReadContent ();

                    // für die Datei mit dem Startknoten
                    if (entry.FileName.ToLower ().Contains ("start")) {
                        KnotStringIO parser = new KnotStringIO (content: content);
                        start = new KnotMetaData (parser.Name, () => parser.CountEdges, null, null);
                    }

                    // für die Datei mit dem Zielknoten
                    else if (entry.FileName.ToLower ().Contains ("target")) {
                        KnotStringIO parser = new KnotStringIO (content: content);
                        target = new KnotMetaData (parser.Name, () => parser.CountEdges, null, null);
                    }

                    // für die Datei mit dem Namen
                    else if (entry.FileName.ToLower ().Contains ("name")) {
                        name = content.Trim ();
                    }

                    // für die Datei mit den Highscores
                    else if (entry.FileName.ToLower ().Contains ("highscore")) {
                        highscore = parseHighscore (content.Split (new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries));
                    }
                }
            }
            if (name != null && start != null && target != null) {
                Log.Debug ("Load challenge file: ", filename, " (name=", name, ",start=", start, ",target=", target, ",highscore=", highscore);
                return new ChallengeMetaData (
                           name: name,
                           start: start,
                           target: target,
                           filename: filename,
                           format: this,
                           highscore: highscore
                       );
            }
            else {
                throw new IOException (
                    "Error! Invalid challenge file: " + filename
                    + " (name=" + name + ",start=" + start + ",target=" + target + ",highscore=" + highscore + ")"
                );
            }
        }

        IEnumerable<string> printHighscore (IEnumerable<KeyValuePair<string, int>> highscore)
        {
            foreach (KeyValuePair<string, int> entry in highscore) {
                Log.Debug (
                    "Save Highscore: "
                    + entry.Value.ToString ()
                    + ":"
                    + entry.Key.ToString ()
                );

                yield return entry.Value + ":" + entry.Key;
            }
        }

        IEnumerable<KeyValuePair<string, int>> parseHighscore (IEnumerable<string> highscore)
        {
            foreach (string line in highscore) {
                Log.Debug ("Load Highscore: ",line);
                if (line.Contains (":")) {
                    string[] entry = line.Split (new char[] {':'}, 2, StringSplitOptions.None);
                    string name = entry [1].Trim ();
                    int time;
                    if (Int32.TryParse (entry [0], out time)) {
                        Log.Debug ("=> ", name, ":", time);
                        yield return new KeyValuePair<string, int> (name, time);
                    }
                }
            }
        }
    }

    [ExcludeFromCodeCoverageAttribute]
    static class ZipHelper
    {
        public static string ReadContent (this ZipEntry entry)
        {
            MemoryStream memory = new MemoryStream ();
            entry.Extract (memory);
            memory.Position = 0;
            var sr = new StreamReader (memory);
            return sr.ReadToEnd ();
        }
    }
}
