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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Knot3.Framework.Primitives
{
    public class Sphere : Primitive
    {
        public Sphere (GraphicsDevice device)
        : this (device, 1, 16)
        {
        }

        public Sphere (GraphicsDevice device, float diameter, int tessellation)
        {
            if (tessellation < 3) {
                throw new ArgumentOutOfRangeException ("tessellation");
            }

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            float radius = diameter / 2;

            // start with a single vertex at the bottom of the sphere.
            AddVertex (position: Vector3.Down * radius, normal: Vector3.Down, texCoord: Vector2.Zero);

            // create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; i++) {
                float latitude = ((i + 1) * MathHelper.Pi / verticalSegments) - MathHelper.PiOver2;

                float dy = (float)System.Math.Sin (latitude);
                float dxz = (float)System.Math.Cos (latitude);

                // create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++) {
                    float longitude = j * MathHelper.TwoPi / horizontalSegments;

                    float dx = (float)System.Math.Cos (longitude) * dxz;
                    float dz = (float)System.Math.Sin (longitude) * dxz;

                    Vector3 normal = new Vector3 (dx, dy, dz);

                    AddVertex (position: normal * radius, normal: normal, texCoord: new Vector2 (i / (verticalSegments - 1), j / horizontalSegments));
                }
            }

            // finish with a single vertex at the top of the sphere.
            AddVertex (position: Vector3.Up * radius, normal: Vector3.Up, texCoord: Vector2.One);

            // create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; i++) {
                AddIndex (0);
                AddIndex (1 + (i + 1) % horizontalSegments);
                AddIndex (1 + i);
            }

            // fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; i++) {
                for (int j = 0; j < horizontalSegments; j++) {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    AddIndex (1 + i * horizontalSegments + j);
                    AddIndex (1 + i * horizontalSegments + nextJ);
                    AddIndex (1 + nextI * horizontalSegments + j);

                    AddIndex (1 + i * horizontalSegments + nextJ);
                    AddIndex (1 + nextI * horizontalSegments + nextJ);
                    AddIndex (1 + nextI * horizontalSegments + j);
                }
            }

            // create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; i++) {
                AddIndex (CurrentVertex - 1);
                AddIndex (CurrentVertex - 2 - (i + 1) % horizontalSegments);
                AddIndex (CurrentVertex - 2 - i);
            }

            InitializePrimitive (device);
        }
    }
}
