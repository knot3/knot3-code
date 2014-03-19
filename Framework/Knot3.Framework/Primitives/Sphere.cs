using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException ("tessellation");

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

