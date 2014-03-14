/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Math;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Models
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
            rotation = rotation ?? Angles3.Zero;
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
                vertexBuffer = new VertexBuffer (device, typeof(VertexPositionNormalTexture), vertices.Count, BufferUsage.None);
                vertexBuffer.SetData (vertices.ToArray ());
                indexBuffer = new IndexBuffer (device, typeof(ushort), indices.Count, BufferUsage.None);
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

        public static FloatOption ModelQualityOption
        {
            get {
                return _modelQualityOption = _modelQualityOption ?? new FloatOption (
                    section: "video",
                    name: "model-quality",
                    defaultValue: 0.200f,
                    validValues: 1f.Range (step: 0.001f),
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
