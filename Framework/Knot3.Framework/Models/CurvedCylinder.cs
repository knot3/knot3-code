/*
 * Copyright (c) 2013-2014 Tobias Schulz
 *
 * Copying, redistribution and use of the source code in this file in source
 * and binary forms, with or without modification, are permitted provided
 * that the conditions of the MIT license are met.
 */
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Utilities;

namespace Primitives
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

            Vector3 bumpDirection = Vector3.Right*height;
            Vector3 top = Vector3.Forward * halfHeight;
            Vector3 bottom = Vector3.Backward * halfHeight;
            var bumpOffsets = new [] {
                new { h = 0.00f, bump = 0.00f },
                new { h = 0.20f, bump = 0.25f },
                new { h = 0.25f, bump = 0.35f },
                new { h = 0.30f, bump = 0.45f },
                new { h = 0.50f, bump = 0.55f },
                new { h = 0.70f, bump = 0.45f },
                new { h = 0.75f, bump = 0.35f },
                new { h = 0.80f, bump = 0.25f },
                new { h = 1.00f, bump = 0.00f },
            };

            for (int k = 0; k+1 < bumpOffsets.Length; ++k) {
                var bumpOffset1 = bumpOffsets [k];
                var bumpOffset2 = bumpOffsets [k+1];

                for (int i = 0; i < tessellation; i++) {
                    Vector3 normal = GetCircleVector (i, tessellation);
                    float textureU = (float)i / (float)tessellation;
                    float textureV = bumpOffset1.h;

                    AddVertex (
                        position: normal * radius + top + (bottom - top) * bumpOffset1.h + bumpDirection * bumpOffset1.bump,
                        normal: normal,
                        texCoord: new Vector2 (textureU, textureV)
                    );
                    AddVertex (
                        position: normal * radius + top + (bottom - top) * bumpOffset2.h + bumpDirection * bumpOffset2.bump,
                        normal: normal,
                        texCoord: new Vector2 (textureU, textureV)
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
            float dx = (float)Math.Cos (angle);
            float dz = (float)Math.Sin (angle);
            return new Vector3 (dx, dz, 0);
        }
    }
}
