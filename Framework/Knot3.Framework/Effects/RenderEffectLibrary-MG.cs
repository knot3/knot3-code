using System;

namespace Knot3.Framework.Effects
{
    public partial class RenderEffectLibrary
    {
        private static void AddDefaultGLShaders ()
        {
            EffectLibrary.Add (new EffectFactory (
                name: "simple-gl",
                displayName: "Simple GL Shader",
                createInstance: (screen) => new SimpleGLEffect (screen)
            ));
        }
    }
}

