﻿using System;

using NUnit.Framework;

//namespace Knot3.UnitTests
//{
[TestFixture]
public class NUnit_Beispiel_Tests
{
	[Test]
	public void Test_Methode1 ()
	{
		Assert.IsNull (null);
	}

	[Test]
	public void Test_Methode2 ()
	{
		Assert.IsNotNull (null);
	}

	public static void DoSomething()
	{
		try {
			Console.WriteLine("I Random!");
		}
		catch (Exception ex) {
			Console.WriteLine(ex.Message);
		}
	}
}
//}