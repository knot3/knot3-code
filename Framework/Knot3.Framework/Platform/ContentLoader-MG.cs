using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Platform;
using Knot3.Framework.Core;

namespace Knot3.Framework.Platform
{
    public static partial class ContentLoader
    {
        public static Effect LoadEffect (this IScreen screen, string name)
        {
            string[] filenames = {
                SystemInfo.RelativeContentDirectory + "Shader/" + name + ".mgfx",
                SystemInfo.RelativeContentDirectory + "Shader/" + name + "_3.0.mgfx",
                SystemInfo.RelativeContentDirectory + "Shader/" + name + "_3.1.mgfx"
            };
            Exception lastException = new Exception ("Could not find shader: " + name);
            foreach (string filename in filenames) {
                try {
                    Effect effect = new Effect (screen.GraphicsDevice, System.IO.File.ReadAllBytes (filename), name);
                    return effect;
                }
                catch (Exception ex) {
                    lastException = ex;
                }
            }
            throw lastException;
        }
    }
}

