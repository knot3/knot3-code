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

using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

using Knot3.Game.Data;

namespace Knot3.Game.Utilities
{
    public class SavegameLoader<Savegame, SavegameMetaData>
    {
        public ISavegameIO<Savegame, SavegameMetaData> FileFormat { get; set; }

        public FileIndex fileIndex { get; private set; }

        public string IndexName;
        private Action<string, SavegameMetaData> OnSavegameFound;

        public SavegameLoader (ISavegameIO<Savegame, SavegameMetaData> fileFormat, string indexName)
        {
            FileFormat = fileFormat;
            IndexName = indexName;
        }

        public void FindSavegames (Action<string, SavegameMetaData> onSavegameFound)
        {
            // Erstelle einen neuen Index, der eine Datei mit dem angegeben Indexnamen im Spielstandverzeichnis einliest
            fileIndex = new FileIndex (SystemInfo.SavegameDirectory + SystemInfo.PathSeparator.ToString () + IndexName + ".txt");

            // Diese Verzeichnisse werden nach Spielständen durchsucht
            string[] searchDirectories = new string[] {
                SystemInfo.BaseDirectory,
                SystemInfo.SavegameDirectory
            };
            Log.Debug ("Search for Savegames: ", string.Join (", ", searchDirectories));

            // Suche nach Spielstanddateien und fülle das Menü auf
            OnSavegameFound = onSavegameFound;
            FileUtility.SearchFiles (searchDirectories, FileFormat.FileExtensions, AddFileToList);
        }

        /// <summary>
        /// Diese Methode wird für jede gefundene Spielstanddatei aufgerufen
        /// </summary>
        private void AddFileToList (string filename)
        {
            // Lese die Datei ein und erstelle einen Hashcode
            string hashcode = FileUtility.GetHash (filename);

            // Ist dieser Hashcode im Index enthalten?
            // Dann wäre der Spielstand gültig, sonst ungültig oder unbekannt.
            bool isValid = fileIndex.Contains (hashcode);

            // Wenn der Spielstand ungültig oder unbekannt ist...
            if (!isValid) {
                try {
                    // Lade den Knoten und prüfe, ob Exceptions auftreten
                    FileFormat.Load (filename);
                    // Keine Exceptions? Dann ist enthält die Datei einen gültigen Knoten!
                    isValid = true;
                    fileIndex.Add (hashcode);
                }
                catch (Exception ex) {
                    // Es ist eine Exception aufgetreten, der Knoten ist offenbar ungültig.
                    Log.Debug (ex);
                    isValid = false;
                }
            }

            // Falls der Knoten gültig ist, entweder laut Index oder nach Überprüfung, dann...
            if (isValid) {
                // Lade die Metadaten
                SavegameMetaData meta = FileFormat.LoadMetaData (filename);

                // Rufe die Callback-Funktion auf
                OnSavegameFound (filename, meta);
            }
        }
    }
}
