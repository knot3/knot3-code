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

using Knot3.Framework.Utilities;

namespace Knot3.Framework.Primitives
{
    public class Cylinder : Primitive
    {
        public Cylinder (GraphicsDevice device)
        : this (device, 1, 1, 32)
        {
        }

        public Cylinder (GraphicsDevice device, float height, float diameter, int tessellation)
        {
            if (tessellation < 3) {
                throw new ArgumentOutOfRangeException ("cylinder tessellation");
            }

            float halfHeight = height / 2;
            float radius = diameter / 2;

            for (int i = 0; i < tessellation; i++) {
                Vector3 normal = GetCircleVector (i, tessellation);
                float textureU = (float)i / (float)tessellation;

                AddVertex (position: normal * radius + Vector3.Forward * halfHeight, normal: normal, texCoord: new Vector2 (textureU, 0));
                AddVertex (position: normal * radius + Vector3.Backward * halfHeight, normal: normal, texCoord: new Vector2 (textureU, 1));

                AddIndex (i * 2);
                AddIndex (i * 2 + 1);
                AddIndex ((i * 2 + 2) % (tessellation * 2));

                AddIndex (i * 2 + 1);
                AddIndex ((i * 2 + 3) % (tessellation * 2));
                AddIndex ((i * 2 + 2) % (tessellation * 2));
            }

            CreateCap (tessellation, halfHeight, radius, Vector3.Forward);
            CreateCap (tessellation, halfHeight, radius, Vector3.Backward);

            InitializePrimitive (device);

            Center = Vector3.Zero;
        }

        void CreateCap (int tessellation, float height, float radius, Vector3 normal)
        {
            // create cap indices.
            for (int i = 0; i < tessellation - 2; i++) {
                if (normal.Y > 0) {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                }
                else {
                    AddIndex (CurrentVertex);
                    AddIndex (CurrentVertex + (i + 2) % tessellation);
                    AddIndex (CurrentVertex + (i + 1) % tessellation);
                }
            }

            // create cap vertices.
            for (int i = 0; i < tessellation; i++) {
                Vector3 position = GetCircleVector (i, tessellation) * radius + normal * height;
                AddVertex (position, normal, Vector2.Zero);
            }
        }

        static Vector3 GetCircleVector (int i, int tessellation)
        {
            float angle = i * MathHelper.TwoPi / tessellation;
            float dx = (float)System.Math.Cos (angle);
            float dz = (float)System.Math.Sin (angle);
            return new Vector3 (dx, dz, 0);
        }
    }
}
