using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Platform;
using Knot3.Framework.Core;

namespace Knot3.Framework
{
    [ExcludeFromCodeCoverageAttribute]
    public static partial class ContentLoader
    {
        public static Effect LoadEffect (this IScreen screen, string name)
        {
            return screen.Game.Content.Load<Effect> ("Shader/" + name);
        }
    }
}

