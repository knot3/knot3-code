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

using NUnit.Framework;

using Knot3.Game.Data;

using Knot3.MockObjects;

namespace Knot3.UnitTests.Data
{
    [TestFixture]
    public class KnotStringIO_Construction
    {
        KnotStringIO squaredKnotStringIO;
        KnotStringIO complexKnotStringIO;

        [SetUp]
        public void Init ()
        {
             squaredKnotStringIO = new KnotStringIO (KnotGenerator.generateSquareKnot (10, KnotGenerator.FakeName));
             complexKnotStringIO = new KnotStringIO (KnotGenerator.generateComplexKnot (KnotGenerator.FakeName));
        }
        [Test]
        public void KnotStringIO_Invalid_Test ()
        {            
            KnotStringIO other = new KnotStringIO (squaredKnotStringIO.Content);
           
            Assert.AreEqual (squaredKnotStringIO.CountEdges, 40, "Count Edges");

            Assert.AreEqual (squaredKnotStringIO.Content, other.Content, "Contetnt equal");
            KnotStringIO invalidContent = null;

            invalidContent = new KnotStringIO ("Name \n" + "Invalid Line \n");
            Assert.Catch<IOException> (() => {
                // damit der Compiler den Aufruf der Decode...-Methoden nicht wegoptimiert,
                // muss man zurück zum Konstruktur noch das eigentlich dort abgespeicherte
                // Attribut Edges abrufen (das ist ein Iterator mit lazy evaluation)
                // und das dann in eine Liste umwandeln
                Console.WriteLine (invalidContent.Edges.ToList ());
            }
                                      );
            Assert.AreEqual (squaredKnotStringIO.Content, other.Content, "Content equal");
        }

        [Test]
        public void KnotStringIO_EdgeCount_Test ()
        {
            Assert.AreEqual (squaredKnotStringIO.CountEdges, 40, "Squared Knot Edge Count");
            Assert.AreEqual (complexKnotStringIO.CountEdges, 6, "Squared Knot Edge Count");
        }

        [Test]
        public void KnotStringIO_Decode_Test ()
        {
          String content_rgba = "Start\nY#FF0000FF#\nZ#FF0000FF#\ny#FF0000FF#\nz#FF0000FF#";
          String content_rgb = "Start\nY#FF0000#\nZ#FF0000#\ny#FF0000#\nz#FF0000#";
          String content_rectangle = "Start\nY#FF0000#1000#\nZ#FF0000#1000\ny#FF0000#1000\nz#FF0000#1000";
          KnotStringIO rgba = new KnotStringIO (content_rgba);
          KnotStringIO rgb = new KnotStringIO (content_rgb);

          KnotStringIO rectangle = new KnotStringIO(content_rectangle);
          Assert.DoesNotThrow(() =>
          {
              List<Edge> squaredEdges = complexKnotStringIO.Edges.ToList();
              List<Edge> allEdges = complexKnotStringIO.Edges.ToList ();
              List<Edge> coloredRGBAEdges = rgba.Edges.ToList (); 
              List<Edge> coloredRGBEdges = rgb.Edges.ToList ();
              List<Edge> rectangleEdges = rectangle.Edges.ToList ();
         }, " Erstellung");
        }


        [Test]
        public void KnotStringIO_invalid_Decode_Test()
        {
            String content_rgba = "Start\nY#FZ0000FF#\nZ#FF0000FF#\ny#FF0000FF#\nz#FF0000FF#";
            String content_rgb = "Start\nY#FF000Z#\nZ#FF00<10#\ny#FF0000#\nz#FF0000#";
            String content_rectangle = "Start\nY#FF0000#D000#\nZ#FF0000#1!00\ny#FF0000#1000\nz#FF0000#1000";
            KnotStringIO rgba = new KnotStringIO(content_rgba);
            KnotStringIO rgb = new KnotStringIO(content_rgb);
            KnotStringIO rectangle = new KnotStringIO(content_rectangle);
            Assert.Catch( () => {
                List<Edge> squaredEdges = complexKnotStringIO.Edges.ToList();
                List<Edge> allEdges = complexKnotStringIO.Edges.ToList();
                List<Edge> coloredRGBAEdges = rgba.Edges.ToList();
                List<Edge> coloredRGBEdges = rgb.Edges.ToList();
                List<Edge> rectangleEdges = rectangle.Edges.ToList();
            }, " Erstellung");

        }
    }
}
