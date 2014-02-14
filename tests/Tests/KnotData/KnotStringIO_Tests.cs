using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using NUnit.Framework;

using Knot3.KnotData;
using Knot3.UnitTests.MockObjects;

namespace Knot3.UnitTests.Tests.KnotData
{
	[TestFixture]
	class Test_KnotStringIO
	{
		[Test]
		public void KnotStringIO_Test ()
		{
			KnotStringIO knotStringIO = new KnotStringIO (KnotGenerator.generateValidSquaredKnot (10));
			KnotStringIO other = new KnotStringIO (knotStringIO.Content);

			Assert.AreEqual (knotStringIO.Content, other.Content, "Contetnt equal");
			KnotStringIO invalidContent = null;

			invalidContent = new KnotStringIO ("Name \n" + "Invalid Line \n");
			Assert.Catch<IOException> (() => {
				// damit der Compiler den Aufruf der Decode...-Methoden nicht wegoptimiert, muss man
				// das Attribut Edges aufrufen (das ist ein Iterator mit lazy evaluation)
				// und den in eine Liste umwandeln
				Console.WriteLine (invalidContent.Edges.ToList ());
			}
			);
			Assert.AreEqual (knotStringIO.Content, other.Content, "Contetnt equal");
		}
	}
}
