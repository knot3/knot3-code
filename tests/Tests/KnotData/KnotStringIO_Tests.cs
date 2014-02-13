using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			KnotStringIO knotStringIO = new KnotStringIO(KnotGenerator.generateValidSquaredKnot(10));
			KnotStringIO other = new KnotStringIO(knotStringIO.Content);

			Assert.AreEqual(knotStringIO.Content, other.Content, "Contetnt equal");

            
            
            KnotStringIO invalidContent = new KnotStringIO("Name \n" + "Invalid Line \n");

            Assert.AreEqual(knotStringIO.Content, other.Content, "Contetnt equal");

		}
	}
}
