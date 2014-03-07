using System;
using Knot3.Framework.Core;

namespace Knot3.Game.Data
{
    public class Axis : TypesafeEnum<Axis>
    {
        public static readonly Axis X = new Axis ("X");
        public static readonly Axis Y = new Axis ("Y");
        public static readonly Axis Z = new Axis ("Z");
        public static readonly Axis Zero = new Axis ("Zero");

        private Axis (string name)
            : base (name)
        {
        }
    }
}

