using System;
using System.IO;

namespace Knot3.Framework.Storage
{
    public class FileAlreadyExistsException:IOException
    {
        public FileAlreadyExistsException(string message):base(message)
        {
        }
    }
}

