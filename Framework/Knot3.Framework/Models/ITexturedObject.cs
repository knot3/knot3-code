using System;
using Microsoft.Xna.Framework.Graphics;

namespace Knot3.Framework.Models
{
    public interface ITexturedObject : IGameObject
    {
        /// <summary>
        /// Die Textur des Modells.
        /// </summary>
        Texture2D Texture { get; }
    }
}

