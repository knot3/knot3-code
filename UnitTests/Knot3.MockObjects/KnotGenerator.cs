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

using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Knot3.Game.Data;

namespace Knot3.MockObjects
{
    public class KnotGenerator
    {
        public static readonly string FakeName = "FakeKnot";

        // TODO (jemand): Wir brauchen hier noch eine bessere Lösung / Überladungen / Umgang mit "FakeKnots"

        public static Knot generateSquareKnot (int EdgeLength, string name)
        {
            Edge[] edgeList = new Edge[EdgeLength * 4];

            for (int i = 0; i < EdgeLength; i++) {
                edgeList[i] = Edge.Up;
            }
            for (int i = EdgeLength; i < EdgeLength*2; i++) {
                edgeList[i] = Edge.Right;
            }
            for (int i = EdgeLength *2; i < EdgeLength*3; i++) {
                edgeList[i] = Edge.Down;
            }
            for (int i = EdgeLength *3; i < EdgeLength*4; i++) {
                edgeList[i] = Edge.Left;
            }
            KnotMetaData metaData = new KnotMetaData (name, edgeList.Count<Edge>, null, null);
            Knot squareKnot = new Knot (metaData, edgeList);

            return squareKnot;
        }

        public static Knot generateInvalidKnot ()
        {
            Edge[] edgeList = new Edge[] {
                Edge.Up, Edge.Up, Edge.Up, Edge.Up
            };
            KnotMetaData metaData = new KnotMetaData ("Invalid", edgeList.Count<Edge>, null, null);
            Knot invalidKnot = new Knot (metaData, edgeList);
            return invalidKnot;
        }

        public static Knot generateComplexKnot (string name)
        {
            Edge[] edgeList = new Edge[] {
                Edge.Up, Edge.Backward, Edge.Right, Edge.Forward, Edge.Down, Edge.Left
            };
            KnotMetaData metaData = new KnotMetaData (name, edgeList.Count<Edge>, null, null);
            Knot complexKnot = new Knot (metaData, edgeList);
            return complexKnot;
        }

        public static Knot coloredKnot (string name)
        {
            string content = "\nY#FF0000FF#\nZ#FF0000FF#\ny#FF0000FF#\nz#FF0000FF#";
            content = name + content;

            KnotStringIO stringIO = new KnotStringIO (content);

            KnotMetaData metaData = new KnotMetaData (name, stringIO.Edges.ToList ().Count<Edge>, null, null);
            Knot coloredKnot = new Knot (metaData, stringIO.Edges.ToList ());
            return coloredKnot;
        }
    }
}
