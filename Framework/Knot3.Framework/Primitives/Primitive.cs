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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Math;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Primitives
{
    public abstract class Primitive : IDisposable
    {
        private List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture> ();
        private List<ushort> indices = new List<ushort> ();
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        protected Matrix vertexTransform = Matrix.Identity;

        public Vector3 Center { get; protected set; }

        public Primitive (Vector3 translation, Angles3 rotation)
        {
            vertexTransform = Matrix.CreateTranslation (translation) * Matrix.CreateFromYawPitchRoll (rotation.Y, rotation.X, rotation.Z);
        }

        public Primitive ()
        : this (translation: Vector3.Zero, rotation: Angles3.Zero)
        {
        }

        protected void AddVertex (Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            position = Vector3.Transform (position, vertexTransform);
            normal = Vector3.TransformNormal (normal, vertexTransform);
            vertices.Add (new VertexPositionNormalTexture (position, normal, texCoord));
        }

        protected void AddIndex (int index)
        {
            if (index > ushort.MaxValue) {
                throw new ArgumentOutOfRangeException ("index");
            }

            indices.Add ((ushort)index);
        }

        protected int CurrentVertex { get { return vertices.Count; } }

        protected void InitializePrimitive (GraphicsDevice device)
        {
            // the graphics device is null during the unit tests
            if (device != null) {
                vertexBuffer = new VertexBuffer (device, typeof (VertexPositionNormalTexture), vertices.Count, BufferUsage.None);
                vertexBuffer.SetData (vertices.ToArray ());
                indexBuffer = new IndexBuffer (device, typeof (ushort), indices.Count, BufferUsage.None);
                indexBuffer.SetData (indices.ToArray ());
            }
        }

        ~Primitive ()
        {
            Dispose (false);
        }

        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (disposing) {
                if (vertexBuffer != null) {
                    vertexBuffer.Dispose ();
                }
                if (indexBuffer != null) {
                    indexBuffer.Dispose ();
                }
            }
        }

        public void Draw (Effect effect)
        {
            GraphicsDevice device = effect.GraphicsDevice;
            device.SetVertexBuffer (vertexBuffer);
            device.Indices = indexBuffer;

            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes) {
                effectPass.Apply ();
                int primitiveCount = indices.Count / 3;
                device.DrawIndexedPrimitives (PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, primitiveCount);
            }
        }

        public void DrawInstances (Effect effect, ref VertexBuffer instanceBuffer, int instanceCount)
        {
            GraphicsDevice device = effect.GraphicsDevice;
            VertexBufferBinding[] bindings = new VertexBufferBinding [2];
            bindings [0] = new VertexBufferBinding (vertexBuffer);
            bindings [1] = new VertexBufferBinding (instanceBuffer, 0, 1);
            device.SetVertexBuffers (bindings);
            device.Indices = indexBuffer;

            foreach (EffectPass effectPass in effect.CurrentTechnique.Passes) {
                effectPass.Apply ();
                int primitiveCount = indices.Count / 3;
                device.DrawInstancedPrimitives (PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, primitiveCount, instanceCount);
            }
        }

        public static FloatOption ModelQualityOption
        {
            get {
                return _modelQualityOption = _modelQualityOption ?? new FloatOption (
                                                 section: "video",
                                                 name: "model-quality",
                                                 defaultValue: 0.200f,
                                                 validValues: 1.001f.Range (step: 0.001f),
                                                 configFile: Config.Default
                ) { Verbose = false };
            }
        }

        private static FloatOption _modelQualityOption;
        public static Action<GameTime> OnModelQualityChanged = (time) => {};

        public static int CurrentCircleTessellation
        {
            get {
                return 3 + (int)(ModelQualityOption.Value * 64);
            }
        }
    }
}
