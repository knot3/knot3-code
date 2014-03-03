using System;
using Knot3.Framework.Audio;
using Knot3.Framework.Core;

namespace Knot3.ModelEditor
{
    public class SilentAudioManager : AudioManager
    {
        public SilentAudioManager (GameCore game)
            : base (game)
        {
        }

        public override void Initialize (string directory)
        {
            // no directories are initialized -> no audio files will be detected

            base.Initialize (directory);
        }
    }
}

