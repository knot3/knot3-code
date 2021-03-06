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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Implementiert das Speicherformat für Knoten.
    /// </summary>
    public sealed class KnotFileIO : IKnotIO
    {
        /// <summary>
        /// Die für eine Knoten-Datei gültigen Dateiendungen.
        /// </summary>
        public IEnumerable<string> FileExtensions
        {
            get {
                yield return ".knot";
                yield return ".knt";
            }
        }

        private Dictionary<string, Knot> KnotCache = new Dictionary<string, Knot> ();
        private Dictionary<string, KnotMetaData> KnotMetaDataCache = new Dictionary<string, KnotMetaData> ();

        /// <summary>
        /// Erstellt ein KnotFileIO-Objekt.
        /// </summary>
        public KnotFileIO ()
        {
        }

        public void ResetCache ()
        {
            KnotCache.Clear ();
            KnotMetaDataCache.Clear ();
        }

        /// <summary>
        /// Speichert einen Knoten in dem Dateinamen, der in dem Knot-Objekt enthalten ist.
        /// </summary>
        public void Save (Knot knot,bool force)
        {
            KnotStringIO parser = new KnotStringIO (knot);
            Log.Debug ("KnotFileIO.Save (", knot, ") = #", parser.Content.Length);
            if (knot.MetaData.Filename == null) {
                throw new NoFilenameException ("Error! knot has no filename: " + knot);
            }
            else if (!force && File.Exists (knot.MetaData.Filename)) {
                throw new FileAlreadyExistsException ("Error! Knot already exists!");
            }
            else {
                File.WriteAllText (knot.MetaData.Filename, parser.Content);
            }
        }

        /// <summary>
        /// Lädt eines Knotens aus einer angegebenen Datei.
        /// </summary>
        public Knot Load (string filename)
        {
            if (KnotCache.ContainsKey (filename)) {
                return KnotCache [filename];
            }
            else {
                Log.Debug ("Load knot from ", filename);
                KnotStringIO parser = new KnotStringIO (content: string.Join ("\n", FileUtility.ReadFrom (filename)));
                return KnotCache [filename] = new Knot (
                    new KnotMetaData (parser.Name, () => parser.CountEdges, this, filename),
                    parser.Edges
                );
            }
        }

        /// <summary>
        /// Lädt die Metadaten eines Knotens aus einer angegebenen Datei.
        /// </summary>
        public KnotMetaData LoadMetaData (string filename)
        {
            if (KnotMetaDataCache.ContainsKey (filename)) {
                return KnotMetaDataCache [filename];
            }
            else {
                KnotStringIO parser = new KnotStringIO (content: string.Join ("\n", FileUtility.ReadFrom (filename)));
                return KnotMetaDataCache [filename] = new KnotMetaData (
                    name: parser.Name,
                    countEdges: () => parser.CountEdges,
                    format: this,
                    filename: filename
                );
            }
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            return "KnotFileIO";
        }
    }
}
