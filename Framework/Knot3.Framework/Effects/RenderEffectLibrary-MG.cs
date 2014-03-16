using System;

namespace Knot3.Framework.Effects
{
    public partial class RenderEffectLibrary
    {
        private static void AddDefaultGLShaders ()
        {
            EffectLibrary.Add (new EffectFactory (
                name: "simple-gl",
                displayName: "Simple OpenGL Shader",
                createInstance: (screen) => new SimpleGLEffect (screen)
            ));
            EffectLibrary.Add (new EffectFactory (
                name: "hardware-instancing-gl",
                displayName: "Hardware Instancing (OpenGL)",
                createInstance: (screen) => new HardwareInstancingEffect (screen)
            ));
        }
    }
}

