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
using Knot3.Framework.Math;

namespace Knot3.Framework.Models
{
    public class Torus : Primitive
    {
        public Torus (GraphicsDevice device)
        : this (device, 1, 0.25f, 32)
        {
        }

        public Torus (GraphicsDevice device, float diameter, float thickness, int tessellation, float circlePercent = 1f,
                      Vector3 translation = default(Vector3), Angles3 rotation = default(Angles3))
            : base (translation: translation, rotation: rotation)
        {
            if (tessellation < 3) {
                throw new ArgumentOutOfRangeException ("cylinder tessellation");
            }

            circlePercent = MathHelper.Clamp (circlePercent, 0f, 1f);

            for (int i = 0; i < tessellation; i++) {
                float outerAngle = i * MathHelper.TwoPi * circlePercent / (tessellation-1);
                float textureU = (float)i / (float)(tessellation - 1);
                if (circlePercent < 1f)
                    textureU = MathHelper.Clamp (textureU, 0.25f, 0.75f);

                Matrix transform = Matrix.CreateTranslation (diameter / 2, 0, 0) * Matrix.CreateRotationY (outerAngle);

                // Now we loop along the other axis, around the side of the tube.
                for (int j = 0; j < tessellation; j++) {
                    float innerAngle = j * MathHelper.TwoPi / tessellation;
                    float textureV = 0;//(float)j / (float)tessellation;

                    float dx = (float)System.Math.Cos (innerAngle);
                    float dy = (float)System.Math.Sin (innerAngle);

                    // Create a vertex.
                    Vector3 normal = new Vector3 (dx, dy, 0);
                    Vector3 position = normal * thickness / 2;

                    position = Vector3.Transform (position, transform);
                    normal = Vector3.TransformNormal (normal, transform);

                    AddVertex (position: position, normal: normal, texCoord: new Vector2 (textureU, textureV));

                    // And create indices for two triangles.
                    if (i + 1 < tessellation || circlePercent == 1f) {
                        int nextI = (i + 1) % tessellation;
                        int nextJ = (j + 1) % tessellation;

                        if (nextI < tessellation) {
                            AddIndex (i * tessellation + j);
                            AddIndex (i * tessellation + nextJ);
                            AddIndex (nextI * tessellation + j);

                            AddIndex (i * tessellation + nextJ);
                            AddIndex (nextI * tessellation + nextJ);
                            AddIndex (nextI * tessellation + j);
                        }
                    }
                }
            }

            InitializePrimitive (device);
        }
    }
}
