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

using Microsoft.Xna.Framework;

namespace Knot3.Framework.Math
{
    [ExcludeFromCodeCoverageAttribute]
    public static class FrustumExtensions
    {
        private static Vector3 GetNegativeVertex (this BoundingBox aabb, ref Vector3 normal)
        {
            Vector3 p = aabb.Max;
            if (normal.X >= 0) {
                p.X = aabb.Min.X;
            }
            if (normal.Y >= 0) {
                p.Y = aabb.Min.Y;
            }
            if (normal.Z >= 0) {
                p.Z = aabb.Min.Z;
            }

            return p;
        }

        private static Vector3 GetPositiveVertex (this BoundingBox aabb, ref Vector3 normal)
        {
            Vector3 p = aabb.Min;
            if (normal.X >= 0) {
                p.X = aabb.Max.X;
            }
            if (normal.Y >= 0) {
                p.Y = aabb.Max.Y;
            }
            if (normal.Z >= 0) {
                p.Z = aabb.Max.Z;
            }

            return p;
        }

        public static bool FastIntersects (this BoundingFrustum boundingfrustum, ref BoundingSphere aabb)
        {
            if (boundingfrustum == null) {
                return false;
            }
            var box = BoundingBox.CreateFromSphere (aabb);
            return boundingfrustum.FastIntersects (ref box);
        }

        public static bool FastIntersects (this BoundingFrustum boundingfrustum, ref BoundingBox aabb)
        {
            if (boundingfrustum == null) {
                return false;
            }

            Plane plane;
            Vector3 normal, p;

            plane = boundingfrustum.Bottom;
            normal = plane.Normal;
            normal.X = -normal.X;
            normal.Y = -normal.Y;
            normal.Z = -normal.Z;
            p = aabb.GetPositiveVertex (ref normal);
            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0) {
                return false;
            }

            plane = boundingfrustum.Far;
            normal = plane.Normal;
            normal.X = -normal.X;
            normal.Y = -normal.Y;
            normal.Z = -normal.Z;
            p = aabb.GetPositiveVertex (ref normal);
            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0) {
                return false;
            }

            plane = boundingfrustum.Left;
            normal = plane.Normal;
            normal.X = -normal.X;
            normal.Y = -normal.Y;
            normal.Z = -normal.Z;
            p = aabb.GetPositiveVertex (ref normal);
            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0) {
                return false;
            }

            plane = boundingfrustum.Near;
            normal = plane.Normal;
            normal.X = -normal.X;
            normal.Y = -normal.Y;
            normal.Z = -normal.Z;
            p = aabb.GetPositiveVertex (ref normal);
            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0) {
                return false;
            }

            plane = boundingfrustum.Right;
            normal = plane.Normal;
            normal.X = -normal.X;
            normal.Y = -normal.Y;
            normal.Z = -normal.Z;
            p = aabb.GetPositiveVertex (ref normal);
            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0) {
                return false;
            }

            plane = boundingfrustum.Top;
            normal = plane.Normal;
            normal.X = -normal.X;
            normal.Y = -normal.Y;
            normal.Z = -normal.Z;
            p = aabb.GetPositiveVertex (ref normal);
            if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0) {
                return false;
            }

            return true;
        }
    }
}
