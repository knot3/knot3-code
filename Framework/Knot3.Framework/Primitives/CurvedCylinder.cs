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
    public class CurvedCylinder : Primitive
    {
        public CurvedCylinder (GraphicsDevice device)
        : this (device, 1, 1, 32)
        {
        }

        public CurvedCylinder (GraphicsDevice device, float height, float diameter, int tessellation)
        {
            if (tessellation < 3) {
                throw new ArgumentOutOfRangeException ("cylinder tessellation");
            }

            float halfHeight = height / 2;
            float radius = diameter / 2;

            Vector3 bumpDirection = Vector3.Right * height;
            Vector3 top = Vector3.Forward * halfHeight;
            Vector3 bottom = Vector3.Backward * halfHeight;
            var bumpOffsets = new[] {
                new { h = 0.00f, bump = 0.00f },
                new { h = 0.05f, bump = 0.05f },
                new { h = 0.10f, bump = 0.10f },
                new { h = 0.15f, bump = 0.175f },
                new { h = 0.20f, bump = 0.25f },
                new { h = 0.25f, bump = 0.35f },
                new { h = 0.30f, bump = 0.40f },
                new { h = 0.40f, bump = 0.45f },
                new { h = 0.50f, bump = 0.50f },
                new { h = 0.60f, bump = 0.45f },
                new { h = 0.70f, bump = 0.40f },
                new { h = 0.75f, bump = 0.35f },
                new { h = 0.80f, bump = 0.25f },
                new { h = 0.85f, bump = 0.175f },
                new { h = 0.90f, bump = 0.10f },
                new { h = 0.95f, bump = 0.05f },
                new { h = 1.00f, bump = 0.00f },
            };

            for (int k = 0; k+1 < bumpOffsets.Length; ++k) {
                var bumpOffset1 = bumpOffsets [k];
                var bumpOffset2 = bumpOffsets [k + 1];

                for (int i = 0; i < tessellation; i++) {
                    Vector3 normal = GetCircleVector (i, tessellation);
                    float textureU1 = bumpOffset1.h;//(float)i / (float)tessellation;
                    float textureU2 = bumpOffset2.h;//(float)i / (float)tessellation;
                    textureU1 = 0.25f + textureU1 / 2;
                    textureU2 = 0.25f + textureU2 / 2;
                    float textureV = 0;

                    AddVertex (
                        position: normal * radius + top + (bottom - top) * bumpOffset1.h + bumpDirection * bumpOffset1.bump,
                        normal: normal,
                        texCoord: new Vector2 (textureU1, textureV)
                    );
                    AddVertex (
                        position: normal * radius + top + (bottom - top) * bumpOffset2.h + bumpDirection * bumpOffset2.bump,
                        normal: normal,
                        texCoord: new Vector2 (textureU2, textureV)
                    );

                    int vertexOffset = tessellation * k * 2;
                    int vertexIndex = i * 2;

                    AddIndex (vertexOffset + vertexIndex);
                    AddIndex (vertexOffset + vertexIndex + 1);
                    AddIndex (vertexOffset + (vertexIndex + 2) % (tessellation * 2));

                    AddIndex (vertexOffset + vertexIndex + 1);
                    AddIndex (vertexOffset + (vertexIndex + 3) % (tessellation * 2));
                    AddIndex (vertexOffset + (vertexIndex + 2) % (tessellation * 2));
                }
            }

            InitializePrimitive (device);

            Center = Vector3.Zero;
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
