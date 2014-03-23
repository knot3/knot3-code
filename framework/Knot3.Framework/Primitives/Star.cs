using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Knot3.Framework.Primitives
{
    public class Star:Primitive
    {

        public Star (GraphicsDevice device)
        {
            AddVertex (position: new Vector3(0.5f,0f,0f), normal: new Vector3(0f,0f,1f), texCoord: new Vector2 (0.5f,0f));
            AddVertex (position: new Vector3(0.8f,0.75f,0f), normal:new Vector3(0f,0f,1f), texCoord: new Vector2 (0.8f,0.75f));
            AddVertex (position: new Vector3(0.2f,0.75f,0f), normal:new Vector3(0f,0f,1f), texCoord: new Vector2 (0.2f,0.75f));
            AddVertex (position: new Vector3(0.5f,1f,0f), normal:new Vector3(0f,0f,1f), texCoord: new Vector2 (0.5f,1f));
            AddVertex (position: new Vector3(0.8f,0.25f,0f), normal:new Vector3(0f,0f,1f), texCoord: new Vector2 (0.8f,0.25f));
            AddVertex (position: new Vector3(0.2f,0.25f,0f), normal:new Vector3(0f,0f,1f), texCoord: new Vector2 (0.2f,0.25f));
            AddVertex (position: new Vector3(0.5f,0f,0f), normal: new Vector3(0f,0f,-1f), texCoord: new Vector2 (0.5f,0f));
            AddVertex (position: new Vector3(0.8f,0.75f,0f), normal:new Vector3(0f,0f,-1f), texCoord: new Vector2 (0.8f,0.75f));
            AddVertex (position: new Vector3(0.2f,0.75f,0f), normal:new Vector3(0f,0f,-1f), texCoord: new Vector2 (0.2f,0.75f));
            AddVertex (position: new Vector3(0.5f,1f,0f), normal:new Vector3(0f,0f,-1f), texCoord: new Vector2 (0.5f,1f));
            AddVertex (position: new Vector3(0.8f,0.25f,0f), normal:new Vector3(0f,0f,-1f), texCoord: new Vector2 (0.8f,0.25f));
            AddVertex (position: new Vector3(0.2f,0.25f,0f), normal:new Vector3(0f,0f,-1f), texCoord: new Vector2 (0.2f,0.25f));



            AddIndex(1);
            AddIndex(0);
            AddIndex(2);

            AddIndex(3);
            AddIndex(4);
            AddIndex(5);

            AddIndex(6);
            AddIndex(7);
            AddIndex(8);

            AddIndex(9);
            AddIndex(11);
            AddIndex(10);

            InitializePrimitive (device);
        }

    }
}

