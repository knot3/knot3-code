using System;
using Knot3.Framework.Math;
using Microsoft.Xna.Framework;
using Primitives;

namespace Knot3.Framework.Models
{
    public abstract class GamePrimitiveInfo : GameModelInfo
    {
        public GamePrimitiveInfo (Angles3 rotation, Vector3 scale)
            : base ("", rotation, scale)
        {
        }
    }
}

