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
    public class Parallelogram : Primitive
    {
        public Parallelogram (GraphicsDevice device, Vector3 left, float width, Vector3 up, float height, Vector3 origin, bool normalToCenter)
        : this (device: device,
                topLeft: origin + left * width / 2 + up * height / 2,
                topRight: origin - left * width / 2 + up * height / 2,
                bottomLeft: origin + left * width / 2 - up * height / 2,
                bottomRight: origin - left * width / 2 - up * height / 2,
                normalToCenter: normalToCenter)
        {
        }

        public Parallelogram (GraphicsDevice device, Vector3 topLeft, Vector3 topRight, Vector3 bottomLeft, Vector3 bottomRight, bool normalToCenter)
        {
            Vector3 left = topLeft - topRight;
            Vector3 up = topLeft - bottomLeft;
            float width = left.Length ();
            float height = up.Length ();
            Vector3 normal = Vector3.Cross (left, up);
            normal.Normalize ();

            if (normalToCenter) {
                Vector3 center = topLeft + (bottomRight - topLeft) / 2 - normal * (width / 2 + height / 2);
                AddVertex (position: topLeft, normal: Vector3.Normalize (center - topLeft), texCoord: new Vector2 (0, 0));
                AddVertex (position: topRight, normal: Vector3.Normalize (center - topRight), texCoord: new Vector2 (1, 0));
                AddVertex (position: bottomLeft, normal: Vector3.Normalize (center - bottomLeft), texCoord: new Vector2 (0, 1));
                AddVertex (position: bottomRight, normal: Vector3.Normalize (center - bottomRight), texCoord: new Vector2 (1, 1));

                center = topLeft + (bottomRight - topLeft) / 2 + normal * (width / 2 + height / 2);
                AddVertex (position: topLeft, normal: Vector3.Normalize (center - topLeft), texCoord: new Vector2 (0, 0));
                AddVertex (position: topRight, normal: Vector3.Normalize (center - topRight), texCoord: new Vector2 (1, 0));
                AddVertex (position: bottomLeft, normal: Vector3.Normalize (center - bottomLeft), texCoord: new Vector2 (0, 1));
                AddVertex (position: bottomRight, normal: Vector3.Normalize (center - bottomRight), texCoord: new Vector2 (1, 1));
            }
            else {
                normal *= -1;
                AddVertex (position: topLeft, normal: normal, texCoord: new Vector2 (0, 0));
                AddVertex (position: topRight, normal: normal, texCoord: new Vector2 (1, 0));
                AddVertex (position: bottomLeft, normal: normal, texCoord: new Vector2 (0, 1));
                AddVertex (position: bottomRight, normal: normal, texCoord: new Vector2 (1, 1));

                normal *= -1;
                AddVertex (position: topLeft, normal: normal, texCoord: new Vector2 (0, 0));
                AddVertex (position: topRight, normal: normal, texCoord: new Vector2 (1, 0));
                AddVertex (position: bottomLeft, normal: normal, texCoord: new Vector2 (0, 1));
                AddVertex (position: bottomRight, normal: normal, texCoord: new Vector2 (1, 1));
            }

            AddIndex (0);
            AddIndex (1);
            AddIndex (2);
            AddIndex (2);
            AddIndex (1);
            AddIndex (3);

            AddIndex (6);
            AddIndex (5);
            AddIndex (4);
            AddIndex (7);
            AddIndex (5);
            AddIndex (6);

            InitializePrimitive (device);

            Center = topLeft + (bottomRight - topLeft) / 2;
        }
    }
}
