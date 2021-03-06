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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Math;
using Knot3.Framework.Widgets;

namespace Knot3.Framework.Utilities
{
    [ExcludeFromCodeCoverageAttribute]
    public static class VectorHelper
    {
        private static readonly float MinAngleY = 0.1f;
        private static readonly float MaxAngleY = MathHelper.Pi - 0.1f;

        public static Vector3 ArcBallMove (this Vector3 position, Vector2 mouse, Vector3 up, Vector3 forward)
        {
            Vector3 side = Vector3.Normalize (Vector3.Cross (up, forward));
            //Vector3 relUp = Vector3.Normalize (Vector3.Cross (side, forward));

            // horizontal rotation
            float diffAngleX = MathHelper.Pi / 300f * mouse.X;
            Vector3 rotated = position.RotateAroundVector (up, diffAngleX);

            // vertical rotation
            float currentAngleY = position.AngleBetween (up);
            float diffAngleY = MathHelper.Pi / 200f * mouse.Y;
            if (currentAngleY + diffAngleY > MinAngleY && currentAngleY + diffAngleY < MaxAngleY) {
                rotated = rotated.RotateAroundVector (-side, diffAngleY);
            }

            return rotated;
        }

        public static Vector3 MoveLinear (this Vector3 vectorToMove, Vector3 mouse, Vector3 up, Vector3 forward)
        {
            /*Vector3 side = Vector3.Cross (up, forward);
            side.Normalize ();
            Vector3 relUp = Vector3.Cross (side, forward);
            relUp.Normalize ();
            Vector3 movedVector = vectorToMove - side * mouse.X - relUp * mouse.Y - forward * mouse.Z;
            return movedVector;*/
            Vector3 movedVector = Vector3.Zero;
            Profiler.ProfileDelegate ["gesamt"] = () => {
                Vector3 side = Vector3.Zero;
                Profiler.ProfileDelegate ["Cross1"] = () => {
                    side = Vector3.Cross (up, forward);
                    side.Normalize ();
                };
                Vector3 relUp = Vector3.Zero;
                Profiler.ProfileDelegate ["Cross2"] = () => {
                    relUp = Vector3.Cross (side, forward);
                    relUp.Normalize ();
                };

                Profiler.ProfileDelegate ["PlusMinux"] = () => {
                    movedVector = vectorToMove - side * mouse.X - relUp * mouse.Y - forward * mouse.Z;
                };
            };
            return movedVector;
        }

        public static Vector3 MoveLinear (this Vector3 vectorToMove, Vector2 mouse, Vector3 up, Vector3 forward)
        {
            return vectorToMove.MoveLinear (new Vector3 (mouse.X, mouse.Y, 0), up, forward);
        }

        public static float AngleBetween (this Vector2 a, Vector2 b)
        {
            return ((b.X - a.X) > 0 ? 1 : -1) * (float)System.Math.Acos ((double)Vector2.Dot (Vector2.Normalize (a), Vector2.Normalize (b)));
        }

        public static float AngleBetween (this Vector3 a, Vector3 b)
        {
            return //((b.X - a.X) > 0 ? 1 : -1) *
                (float)System.Math.Acos ((double)Vector3.Dot (Vector3.Normalize (a), Vector3.Normalize (b)));
        }

        public static Vector3 RotateX (this Vector3 vectorToRotate, float angleRadians)
        {
            return Vector3.Transform (vectorToRotate, Matrix.CreateRotationX (angleRadians));
        }

        public static Vector3 RotateY (this Vector3 vectorToRotate, float angleRadians)
        {
            return Vector3.Transform (vectorToRotate, Matrix.CreateRotationY (angleRadians));
        }

        public static Vector3 RotateZ (this Vector3 vectorToRotate, float angleRadians)
        {
            return Vector3.Transform (vectorToRotate, Matrix.CreateRotationZ (angleRadians));
        }

        public static Vector3 RotateAroundVector (this Vector3 vectorToRotate, Vector3 axis, float angleRadians)
        {
            return Vector3.Transform (vectorToRotate, Quaternion.CreateFromAxisAngle (Vector3.Normalize (axis), angleRadians));
        }

        public static Vector3 Clamp (this Vector3 v, Vector3 lower, Vector3 higher)
        {
            return new Vector3 (
                       MathHelper.Clamp (v.X, lower.X, higher.X),
                       MathHelper.Clamp (v.Y, lower.Y, higher.Y),
                       MathHelper.Clamp (v.Z, lower.Z, higher.Z)
                   );
        }

        public static Vector3 Clamp (this Vector3 v, int minLength, int maxLength)
        {
            if (v.Length () < minLength) {
                return v * minLength / v.Length ();
            }
            else if (v.Length () > maxLength) {
                return v * maxLength / v.Length ();
            }
            else {
                return v;
            }
        }

        public static Vector2 PrimaryVector (this Vector2 v)
        {
            if (v.X.Abs () > v.Y.Abs ()) {
                return new Vector2 (v.X, 0);
            }
            else if (v.Y.Abs () > v.X.Abs ()) {
                return new Vector2 (0, v.Y);
            }
            else {
                return new Vector2 (v.X, 0);
            }
        }

        public static Vector3 PrimaryVector (this Vector3 v)
        {
            if (v.X.Abs () > v.Y.Abs () && v.X.Abs () > v.Z.Abs ()) {
                return new Vector3 (v.X, 0, 0);
            }
            else if (v.Y.Abs () > v.X.Abs () && v.Y.Abs () > v.Z.Abs ()) {
                return new Vector3 (0, v.Y, 0);
            }
            else if (v.Z.Abs () > v.Y.Abs () && v.Z.Abs () > v.X.Abs ()) {
                return new Vector3 (0, 0, v.Z);
            }
            else {
                return new Vector3 (v.X, 0, 0);
            }
        }

        public static Vector2 PrimaryDirection (this Vector2 v)
        {
            Vector2 vector = v.PrimaryVector ();
            return new Vector2 (System.Math.Sign (vector.X), System.Math.Sign (vector.Y));
        }

        public static Vector3 PrimaryDirection (this Vector3 v)
        {
            Vector3 vector = v.PrimaryVector ();
            return new Vector3 (System.Math.Sign (vector.X), System.Math.Sign (vector.Y), System.Math.Sign (vector.Z));
        }

        public static Vector3 PrimaryDirectionExcept (this Vector3 v, Vector3 wrongDirection)
        {
            Vector3 copy = v;
            if (wrongDirection.X != 0) {
                copy.X = 0;
            }
            else if (wrongDirection.Y != 0) {
                copy.Y = 0;
            }
            else if (wrongDirection.Z != 0) {
                copy.Z = 0;
            }
            return copy.PrimaryDirection ();
        }

        public static float Abs (this float v)
        {
            return System.Math.Abs (v);
        }

        public static float Clamp (this float v, float min, float max)
        {
            return MathHelper.Clamp (v, min, max);
        }

        public static BoundingBox Bounds (this Vector3 a, Vector3 diff)
        {
            return new BoundingBox (a, a + diff);
        }

        public static BoundingSphere Scale (this BoundingSphere sphere, float scale)
        {
            return new BoundingSphere (sphere.Center, sphere.Radius * scale);
        }

        public static BoundingSphere Scale (this BoundingSphere sphere, Vector3 scale)
        {
            return new BoundingSphere (sphere.Center, sphere.Radius * scale.PrimaryVector ().Length ());
        }

        public static BoundingSphere Translate (this BoundingSphere sphere, Vector3 position)
        {
            return new BoundingSphere (Vector3.Transform (sphere.Center, Matrix.CreateTranslation (position)), sphere.Radius);
        }

        public static BoundingSphere Rotate (this BoundingSphere sphere, Angles3 rotation)
        {
            return new BoundingSphere (Vector3.Transform (sphere.Center, Matrix.CreateFromYawPitchRoll (rotation.Y, rotation.X, rotation.Z)), sphere.Radius);
        }

        public static BoundingBox Scale (this BoundingBox box, float scale)
        {
            return new BoundingBox (box.Min * scale, box.Max * scale);
        }

        public static BoundingBox Translate (this BoundingBox box, Vector3 position)
        {
            Matrix translation = Matrix.CreateTranslation (position);
            return new BoundingBox (Vector3.Transform (box.Min, translation), Vector3.Transform (box.Max, translation));
        }

        public static ScreenPoint ToScreenPoint (this MouseState mouse, IScreen screen)
        {
            return new ScreenPoint (screen, (float)mouse.X / (float)screen.Viewport.Width, (float)mouse.Y / (float)screen.Viewport.Height);
        }

        public static ScreenPoint Center (this Viewport viewport, IScreen screen)
        {
            Vector2 center = new Vector2 (viewport.X + viewport.Width / 2, viewport.Y + viewport.Height / 2);
            return new ScreenPoint (screen, center / screen.Viewport.Size ());
        }

        public static Vector2 Size (this Viewport viewport)
        {
            return new Vector2 (viewport.Width, viewport.Height);
        }

        public static string Join (this string delimiter, List<int> list)
        {
            StringBuilder builder = new StringBuilder ();
            foreach (int elem in list) {
                // Append each int to the StringBuilder overload.
                builder.Append (elem).Append (delimiter);
            }
            return builder.ToString ();
        }

        public static Vector2 Scale (this Vector2 v, Viewport viewport)
        {
            Vector2 max = viewport.Size ();
            if (v.X > 1 || v.Y > 1) {
                return v / 1000f * max;
            }
            else {
                return v * max;
            }
        }

        public static Rectangle Grow (this Rectangle rect, int x, int y)
        {
            return new Rectangle (rect.X - x, rect.Y - y, rect.Width + x * 2, rect.Height + y * 2);
        }

        public static Rectangle Shrink (this Rectangle rect, int x, int y)
        {
            return Grow (rect, -x, -y);
        }

        public static Rectangle Grow (this Rectangle rect, int xy)
        {
            return Grow (rect, xy, xy);
        }

        public static Rectangle Shrink (this Rectangle rect, int xy)
        {
            return Grow (rect, -xy, -xy);
        }

        public static Rectangle Translate (this Rectangle rect, int x, int y)
        {
            return new Rectangle (rect.X + x, rect.Y + y, rect.Width, rect.Height);
        }

        public static Rectangle Resize (this Rectangle rect, int w, int h)
        {
            return new Rectangle (rect.X, rect.Y, rect.Width + w, rect.Height + h);
        }

        public static BoundingSphere[] CylinderBounds (float length, float radius, Vector3 direction, Vector3 position)
        {
            float distance = radius / 4;
            BoundingSphere[] bounds = new BoundingSphere [(int)(length / distance)];
            for (int offset = 0; offset < (int)(length / distance); ++offset) {
                bounds [offset] = new BoundingSphere (position + direction * offset * distance, radius);
                //Log.Debug ("sphere [", offset, "]=", Bounds [offset]);
            }
            return bounds;
        }

        public static Rectangle CreateRectangle (this Vector2 topLeft, Vector2 size)
        {
            return CreateRectangle (0, topLeft.X, topLeft.Y, size.X, size.Y);
        }

        public static Rectangle CreateRectangle (this Vector2 topLeft, Vector2 size, int lineWidth)
        {
            return CreateRectangle (lineWidth, topLeft.X, topLeft.Y, size.X, size.Y);
        }

        public static Rectangle CreateRectangle (int lineWidth, float x, float y, float w, float h)
        {
            if (w == 0) {
                return new Rectangle ((int)x - lineWidth / 2, (int)y - lineWidth / 2, lineWidth, (int)h + lineWidth);
            }
            else if (h == 0) {
                return new Rectangle ((int)x - lineWidth / 2, (int)y - lineWidth / 2, (int)w + lineWidth, lineWidth);
            }
            else {
                return new Rectangle ((int)x, (int)y, (int)w, (int)h);
            }
        }

        public static T At<T> (this List<T> list, int index)
        {
            if (list.Count == 0) {
                return default (T);
            }
            else {
                while (index < 0) {
                    index += list.Count;
                }
                if (index >= list.Count) {
                    index = index % list.Count;
                }
                return list [index];
            }
        }

        public static T At<T> (this IEnumerable<T> list, int index)
        {
            int count = list.Count ();
            if (count == 0) {
                return default (T);
            }
            else {
                while (index < 0) {
                    index += count;
                }
                if (index >= count) {
                    index = index % count;
                }
                return list.ElementAt (index);
            }
        }

        public static T At<T> (this Tuple<T,T> tuple, int i)
        {
            return i == 0 ? tuple.Item1 : i == 1 ? tuple.Item2 : default (T);
        }

        public static T At<T> (this Tuple<T,T,T> tuple, int i)
        {
            return i == 0 ? tuple.Item1 : i == 1 ? tuple.Item2 : i == 2 ? tuple.Item3 : default (T);
        }

        public static IEnumerable<T> ToEnumerable<T> (this Tuple<T,T> tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
        }

        public static IEnumerable<T> ToEnumerable<T> (this Tuple<T,T,T> tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
            yield return tuple.Item3;
        }

        private static Random random = new Random (Guid.NewGuid ().GetHashCode ());

        public static int RandomIndex<T> (this IEnumerable<T> list)
        {
            int index = random.Next (list.Count ());
            return index;
        }

        public static T RandomElement<T> (this IEnumerable<T> list)
        {
            return list.At (list.RandomIndex ());
        }

        public static void SetCoordinates (this Widget widget, float left, float top, float right, float bottom)
        {
            widget.Bounds.Position = new ScreenPoint (widget.Screen, left, top);
            widget.Bounds.Size = new ScreenPoint (widget.Screen, right - left, bottom - top);
        }

        public static Dictionary<A, B> ReverseDictionary<A,B> (this Dictionary<B,A> dict)
        {
            return dict.ToDictionary (x => x.Value, x => x.Key);
        }

        public static float DistanceTo (this Vector3 origin, Vector3 target)
        {
            Vector3 toPosition = origin - target;
            return toPosition.Length ();
        }

        public static Vector3 SetDistanceTo (this Vector3 origin, Vector3 target, float distance)
        {
            Vector3 to = origin - target;
            float oldDistance = to.Length ();
            double scale = (double)distance / (double)to.Length ();
            if (System.Math.Abs (oldDistance) > 1 && System.Math.Abs (oldDistance - distance) > 1 && System.Math.Abs (distance) > 1) {
                return target + to * (float)scale;
            }
            else {
                return origin;
            }
        }

        public static IEnumerable<T> Shuffle<T> (this IEnumerable<T> source)
        {
            Random rnd = new Random ();
            return source.OrderBy<T, int> ((item) => rnd.Next ());
        }

        public static void Repeat (this int count, Action action)
        {
            for (int i = 0; i < count; i++) {
                action ();
            }
        }

        public static void Repeat (this int count, Action<int> action)
        {
            for (int i = 0; i < count; i++) {
                action (i);
            }
        }

        public static IEnumerable<T> Repeat<T> (this int count, Func<int, T> func)
        {
            List<T> list = new List<T> ();
            for (int i = 0; i < count; i++) {
                list.Add (func (i));
            }
            return list;
        }

        public static IEnumerable<int> Range (this int count)
        {
            for (int i = 0; i < count; i++) {
                yield return i;
            }
        }

        public static IEnumerable<float> Range (this float count, float step)
        {
            for (float f = 0; f < count; f += step) {
                yield return f;
            }
        }

        public static void ForEach<U> (this IEnumerable<U> enumerable, Action<U> action)
        {
            foreach (U item in enumerable) {
                action (item);
            }
        }

        public static bool Swap<T> (ref T x, ref T y)
        {
            try {
                T t = y;
                y = x;
                x = t;
                return true;
            }
            catch {
                return false;
            }
        }

        public static Quaternion RotateToFaceTarget (Vector3 sourceDirection, Vector3 destDirection, Vector3 up)
        {
            float dot = Vector3.Dot (sourceDirection, destDirection);

            if (System.Math.Abs (dot - (-1.0f)) < 0.000001f) {
                // vector a and b point exactly in the opposite direction,
                // so it is a 180 degrees turn around the up-axis
                return new Quaternion (up, MathHelper.ToRadians (180.0f));
            }
            if (System.Math.Abs (dot - (1.0f)) < 0.000001f) {
                // vector a and b point exactly in the same direction
                // so we return the identity quaternion
                return Quaternion.Identity;
            }

            float rotAngle = (float)System.Math.Acos (dot);
            Vector3 rotAxis = Vector3.Cross (sourceDirection, destDirection);
            rotAxis = Vector3.Normalize (rotAxis);
            return Quaternion.CreateFromAxisAngle (rotAxis, rotAngle);
        }
    }
}
