using System;
using System.IO;

namespace Knot3.Framework.Storage
{
    public class NoFilenameException:IOException
    {
        public NoFilenameException(string message):base(message)
        {
        }
    }
}

