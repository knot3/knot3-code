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
using System.Linq;

using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Enthält Metadaten zu einer Challenge.
    /// </summary>
    public class ChallengeMetaData
    {
        /// <summary>
        /// Der Name der Challenge.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
            set {
                name = value;
                if (Format == null) {
                    Format = new ChallengeFileIO ();
                }
                string extension;
                if (Format.FileExtensions.Any ()) {
                    extension = Format.FileExtensions.ElementAt (0);
                }
                else {
                    throw new ArgumentException ("Every implementation of IChallengeIO must have at least one file extension.");
                }
                Filename = SystemInfo.SavegameDirectory + SystemInfo.PathSeparator + FileUtility.ConvertToFileName (name) + extension;
            }
        }

        private string name;

        /// <summary>
        /// Der Ausgangsknoten, den der Spieler in den Referenzknoten transformiert.
        /// </summary>
        public KnotMetaData Start { get; private set; }

        /// <summary>
        /// Der Referenzknoten, in den der Spieler den Ausgangsknoten transformiert.
        /// </summary>
        public KnotMetaData Target { get; private set; }

        /// <summary>
        /// Das Format, aus dem die Metadaten der Challenge gelesen wurden oder null.
        /// </summary>
        public IChallengeIO Format { get; private set; }

        /// <summary>
        /// Der Dateiname, aus dem die Metadaten der Challenge gelesen wurden oder in den sie abgespeichert werden.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Ein öffentlicher Enumerator, der die Bestenliste unabhängig von der darunterliegenden Datenstruktur zugänglich macht.
        /// </summary>
        public IEnumerable<KeyValuePair<string, int>> Highscore { get { return highscore; } }

        private List<KeyValuePair<string, int>> highscore;

        public float AvgTime
        {
            get {
                if (   highscore != null
                        && highscore.Any ()) {
                    float amount =0;
                    foreach (KeyValuePair<string, int> entry in highscore) {
                        amount += (float)entry.Value;
                    }
                    return amount/((float)highscore.Count);
                }
                return 0f;
            }

            private set {}
        }

        public string FormatedAvgTime
        {
            get {
                float time = AvgTime;
                Log.Debug (time);
                if (time != 0f) {
                    return formatTime (time);
                }
                return "Not yet set.";
            }
            private set {
            }
        }

        /// <summary>
        /// Erstellt ein Challenge-Metadaten-Objekt mit einem gegebenen Namen und den Metadaten des Ausgangs- und Referenzknotens.
        /// </summary>
        public ChallengeMetaData (string name, KnotMetaData start, KnotMetaData target,
                                  string filename, IChallengeIO format,
                                  IEnumerable<KeyValuePair<string, int>> highscore)
        {
            Name = name;
            Start = start;
            Target = target;
            Format = format ?? Format;
            Filename = filename ?? Filename;

            this.highscore = new List<KeyValuePair<string, int>> ();
            if (highscore != null) {
                foreach (KeyValuePair<string, int> entry in highscore) {
                    this.highscore.Add (entry);
                }
            }
        }

        /// <summary>
        /// Fügt eine neue Bestzeit eines bestimmten Spielers in die Bestenliste ein.
        /// </summary>
        public void AddToHighscore (string name, int time)
        {
            KeyValuePair<string, int> entry = new KeyValuePair<string, int> (name, time);
            if (!highscore.Contains (entry)) {
                highscore.Add (entry);
            }
        }

        public static string formatTime (float secs)
        {
            Log.Debug (secs);
            TimeSpan t = TimeSpan.FromSeconds ( secs );

            string answer = string.Format ("{0:D2}h:{1:D2}m:{2:D2}s",
                                           t.Hours,
                                           t.Minutes,
                                           t.Seconds);
            return answer;
        }

        public bool Equals (ChallengeMetaData other)
        {
            return other != null && name == other.name;
        }

        public override bool Equals (object other)
        {
            return other != null && Equals (other as ChallengeMetaData);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return (name ?? String.Empty).GetHashCode ();
        }

        public static bool operator == (ChallengeMetaData a, ChallengeMetaData b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals (a, b)) {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null)) {
                return false;
            }

            // Return true if the fields match:
            return a.Equals (b);
        }

        public static bool operator != (ChallengeMetaData a, ChallengeMetaData b)
        {
            return !(a == b);
        }
    }
}
