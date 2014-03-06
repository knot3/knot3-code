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

using Microsoft.Xna.Framework;

using Knot3.Framework.Platform;

namespace Knot3.Game.Data
{
    /// <summary>
    /// Diese Klasse repräsentiert einen Parser für das Knoten-Austauschformat und enthält die
    /// eingelesenen Informationen wie den Namen des Knotens und die Kantenliste als Eigenschaften.
    /// </summary>
    public sealed class KnotStringIO
    {
        /// <summary>
        /// Der Name der eingelesenen Knotendatei oder des zugewiesenen Knotenobjektes.
        /// </summary>
        public string Name { get; set; }

        private IEnumerable<string> edgeLines;

        /// <summary>
        /// Die Kanten der eingelesenen Knotendatei oder des zugewiesenen Knotenobjektes.
        /// </summary>
        public IEnumerable<Edge> Edges
        {
            get {
                Log.Debug ("KnotStringIO.Edges[get] = ", edgeLines.Count ());
                foreach (string _line in edgeLines) {
                    string line = _line;
                    Edge edge = DecodeEdge (line [0]);
                    line = line.Substring (1);
                    if (line.StartsWith ("#")) {
                        line = line.Substring (1);
                    }
                    edge.Color = DecodeColor (line.Substring (0, 8));
                    line = line.Substring (8);
                    if (line.StartsWith ("#")) {
                        line = line.Substring (1);
                    }
                    if (line.Length > 0) {
                        foreach (int rect in line.Split (',').Select (int.Parse).ToList ()) {
                            edge.Rectangles.Add (rect);
                        }
                    }
                    yield return edge;
                }
            }
            set {
                Log.Debug ("KnotStringIO.Edges[set] = #", value.Count ());
                try {
                    edgeLines = ToLines (value);
                }
                catch (Exception ex) {
                    Log.Debug (ex);
                }
            }
        }

        /// <summary>
        /// Die Anzahl der Kanten der eingelesenen Knotendatei oder des zugewiesenen Knotenobjektes.
        /// </summary>
        public int CountEdges
        {
            get {
                return edgeLines.Where ((l) => l.Trim ().Length > 0).Count ();
            }
        }

        /// <summary>
        /// Erstellt aus den \glqq Name\grqq~- und \glqq Edges\grqq~-Eigenschaften einen neue Zeichenkette,
        /// die als Dateiinhalt in einer Datei eines Spielstandes einen gültigen Knoten repräsentiert.
        /// </summary>
        public string Content
        {
            get {
                return Name + "\n" + string.Join ("\n", edgeLines);
            }
            set {
                if (value.Trim ().Contains ("\n")) {
                    string[] parts = value.Split (new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
                    Name = parts [0];
                    edgeLines = parts.Skip (1);
                }
                else {
                    Name = value;
                    edgeLines = new string[] {};
                }
            }
        }

        /// <summary>
        /// Liest das in der angegebenen Zeichenkette enthaltene Dateiformat ein. Enthält es einen gültigen Knoten,
        /// so werden die \glqq Name\grqq~- und \glqq Edges\grqq~-Eigenschaften auf die eingelesenen Werte gesetzt.
        /// Enthält es einen ungültigen Knoten, so wird eine IOException geworfen und das Objekt wird nicht erstellt.
        /// </summary>
        public KnotStringIO (string content)
        {
            Content = content;
        }

        /// <summary>
        /// Erstellt ein neues Objekt und setzt die \glqq Name\grqq~- und \glqq Edge\grqq~-Eigenschaften auf die
        /// im angegebenen Knoten enthaltenen Werte.
        /// </summary>
        public KnotStringIO (Knot knot)
        {
            Name = knot.Name;
            try {
                edgeLines = ToLines (knot);
            }
            catch (Exception ex) {
                Log.Debug (ex);
            }
        }

        private static IEnumerable<string> ToLines (IEnumerable<Edge> edges)
        {
            foreach (Edge edge in edges) {
                yield return EncodeEdge (edge) + "#" + EncodeColor (edge.Color) + "#" + string.Join (",", edge.Rectangles);
            }
        }

        private static Edge DecodeEdge (char c)
        {
            switch (c) {
            case 'X':
                return Edge.Right;
            case 'x':
                return Edge.Left;
            case 'Y':
                return Edge.Up;
            case 'y':
                return Edge.Down;
            case 'Z':
                return Edge.Backward;
            case 'z':
                return Edge.Forward;
            default:
                throw new IOException ("Failed to decode Edge: '" + c + "'!");
            }
        }

        private static char EncodeEdge (Edge edge)
        {
            if (edge.Direction == Direction.Right) {
                return 'X';
            }
            else if (edge.Direction == Direction.Left) {
                return  'x';
            }
            else if (edge.Direction == Direction.Up) {
                return  'Y';
            }
            else if (edge.Direction == Direction.Down) {
                return  'y';
            }
            else if (edge.Direction == Direction.Backward) {
                return  'Z';
            }
            else if (edge.Direction == Direction.Forward) {
                return  'z';
            }
            else {
                throw new IOException ("Failed to encode Edge: '" + edge + "'!");
            }
        }

        private static String EncodeColor (Color c)
        {
            return c.R.ToString ("X2") + c.G.ToString ("X2") + c.B.ToString ("X2") + c.A.ToString ("X2");
        }

        private static Color DecodeColor (string hexString)
        {
            if (hexString.StartsWith ("#")) {
                hexString = hexString.Substring (1);
            }
            uint hex = unchecked ( uint.Parse (hexString, System.Globalization.NumberStyles.HexNumber) );
            Color color = Color.White;
            if (hexString.Length == 8) {
                unchecked {
                    color.R = (byte)(hex >> 24);
                    color.G = (byte)(hex >> 16);
                    color.B = (byte)(hex >> 8);
                    color.A = (byte)(hex);
                }
            }
            else if (hexString.Length == 6) {
                unchecked {
                    color.R = (byte)(hex >> 16);
                    color.G = (byte)(hex >> 8);
                    color.B = (byte)(hex);
                }
            }
            else {
                throw new IOException ("Invalid hex representation of an ARGB or RGB color value.");
            }
            return color;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override string ToString ()
        {
            return "KnotStringIO (length=" + Content.Length + ")";
        }
    }
}
