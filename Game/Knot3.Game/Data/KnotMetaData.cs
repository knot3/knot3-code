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
using System.Linq;

using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Enthält Metadaten eines Knotens, die aus einer Spielstand-Datei schneller eingelesen werden können,
    /// als der vollständige Knoten. Dieses Objekt enthält keine Datenstruktur zur Repräsentation der Kanten,
    /// sondern nur Informationen über den Namen des Knoten und die Anzahl seiner Kanten. Es kann ohne ein
    /// dazugehöriges Knoten-Objekt existieren, aber jedes Knoten-Objekt enthält genau ein Knoten-Metadaten-Objekt.
    /// </summary>
    public class KnotMetaData : IEquatable<KnotMetaData>
    {
        /// <summary>
        /// Der Anzeigename des Knotens, welcher auch leer sein kann.
        /// Beim Speichern muss der Spieler in diesem Fall zwingend
        /// einen nichtleeren Namen wählen. Wird ein neuer Anzeigename festgelegt,
        /// dann wird der Dateiname ebenfalls auf einen neuen Wert gesetzt, unabhängig davon
        /// ob er bereits einen Wert enthält oder \glqq null\grqq~ist.
        /// Diese Eigenschaft kann öffentlich gelesen und gesetzt werden.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
            set {
                name = value;
                if (Format == null) {
                    Format = new KnotFileIO ();
                }
                if (name != null && name.Length > 0) {
                    string extension;
                    if (Format.FileExtensions.Any ()) {
                        extension = Format.FileExtensions.ElementAt (0);
                    }
                    else {
                        throw new ArgumentException ("Every implementation of IKnotIO must have at least one file extension.");
                    }
                    Filename = SystemInfo.SavegameDirectory + SystemInfo.PathSeparator + FileUtility.ConvertToFileName (name) + extension;
                }
            }
        }

        private string name;

        /// <summary>
        /// Das Format, aus dem die Metadaten geladen wurden.
        /// Es ist genau dann \glqq null\grqq, wenn die Metadaten nicht aus einer Datei gelesen wurden. Nur lesbar.
        /// </summary>
        public IKnotIO Format { get; private set; }

        /// <summary>
        /// Ein Delegate, das die Anzahl der Kanten zurückliefert.
        /// Falls dieses Metadaten-Objekt Teil eines Knotens ist, gibt es dynamisch die Anzahl der
        /// Kanten des Knoten-Objektes zurück. Anderenfalls gibt es eine statische Zahl zurück,
        /// die beim Einlesen der Metadaten vor dem Erstellen dieses Objektes gelesen wurde. Nur lesbar.
        /// </summary>
        public int CountEdges { get { return countEdges (); } }

        private Func<int> countEdges;

        /// <summary>
        /// Falls die Metadaten aus einer Datei eingelesen wurden, enthält dieses Attribut den Dateinamen,
        /// sonst \glqq null\grqq.
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Erstellt ein neues Knoten-Metadaten-Objekt mit einem angegebenen Knotennamen
        /// und einer angegebenen Funktion, welche eine Kantenanzahl zurück gibt.
        /// Zusätzlich wird der Dateiname oder das Speicherformat angegeben, aus dem die Metadaten gelesen wurden.
        /// </summary>
        public KnotMetaData (string name, Func<int> countEdges, IKnotIO format, string filename)
        {
            Name = name;
            this.countEdges = countEdges;
            Format = format ?? Format;
            Filename = filename ?? Filename;
        }

        /// <summary>
        /// Erstellt ein neues Knoten-Metadaten-Objekt mit einem angegebenen Knotennamen
        /// und einer angegebenen Funktion, welche eine Kantenanzahl zurück gibt.
        /// </summary>
        public KnotMetaData (string name, Func<int> countEdges)
        {
            this.name = name;
            this.countEdges = countEdges;
            Format = null;
            Filename = null;
        }

        public bool Equals (KnotMetaData other)
        {
            return other != null && name == other.name && countEdges () == other.countEdges ();
        }

        public override bool Equals (object other)
        {
            return other != null && Equals (other as KnotMetaData);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return (countEdges ().ToString () + (name ?? String.Empty)).GetHashCode ();
        }

        public static bool operator == (KnotMetaData a, KnotMetaData b)
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

        public static bool operator != (KnotMetaData a, KnotMetaData b)
        {
            return !(a == b);
        }
    }
}
