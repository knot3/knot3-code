using System;
using Knot3.Framework.Models;

namespace Knot3.Framework.Models
{
    public interface IColoredObject : IGameObject
    {
        /// <summary>
        /// Die Farbe des Spielobjektes.
        /// </summary>
        ModelColoring Coloring { get; }
    }
}

