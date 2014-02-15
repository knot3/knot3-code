#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

<<<<<<< HEAD
using Knot3.Data;
using Knot3.MockObjects;



namespace Knot3.ExtremeTests
{



    [TestFixture]
    public class ExtremeKnot
    {


        [SetUp]
        public void Init() {

            Knot knot = null;
            String knotString = null;
            int squareKnot_100 = 25;

            knot = KnotGenerator.generateSquareKnot(squareKnot_100);
            knotString = knot.ToString();

            Console.WriteLine("Knoten: " + knotString);

        }



        [Test]
        public void SquareKnotWith_100_Edges()
        {

        }



        [Test]
        public void SquareKnotWith_1000_Edges()
        {

        }




        [Test]
        public void SquareKnotWith_2000_Edges()
        {

        }






    }
}
=======
<<<<<<< HEAD
namespace Knot3.ExtremeTests
{
	public class ExtremeKnot
	{
	}
	=======
	    using Knot3.Data;
	using Knot3.MockObjects;

	namespace Knot3.ExtremeTests
	{
		[TestFixture]
		public class ExtremeKnot
		{
			[SetUp]
			public void Init ()
			{
				Knot knot = null;
				String knotString = null;
				int squareKnot_100 = 25;

				knot = KnotGenerator.generateSquareKnot (squareKnot_100);
				knotString = knot.ToString ();

				Console.WriteLine ("Knoten: " + knotString);
			}

			[Test]
			public void SquareKnotWith_100_Edges ()
			{
			}

			[Test]
			public void SquareKnotWith_1000_Edges ()
			{
			}

			[Test]
			public void SquareKnotWith_2000_Edges ()
			{
			}
		}
		>>>>>>> Tests, Extremtests
	}
>>>>>>> 51233972b8768d1f53236bea7e0e520e0c8c62f8
