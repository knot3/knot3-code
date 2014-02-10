using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Core;
using Knot3.Utilities;

namespace Knot3.UnitTests.Test_Camera
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für Test_Camera
	/// </summary>
	[TestFixture]
	public class Test_Camera
	{
		FakeScreen fakeScreen;
		Camera cam1;

		[SetUp]
		public void Init ()
		{
			fakeScreen = new FakeScreen();
			cam1 =  new Camera(screen: fakeScreen, world: null);
		}

		[Test]
		public void PositionTargetTest ()
		{
			Vector3 pos = cam1.Position;
			cam1.Position += Vector3.Left;
			Assert.AreEqual(pos+Vector3.Left, cam1.Position);

			Vector3 target = cam1.Target;
			cam1.Target += Vector3.Up;
			Assert.AreEqual(target+Vector3.Up, cam1.Target);

			pos = cam1.Position;
			target = cam1.Target;
			cam1.PositionToTargetDistance -= 50;
			cam1.PositionToTargetDistance += 100;
			cam1.PositionToTargetDistance -= 50;
			Assert.True(Equals (pos, cam1.Position));
			Assert.True(Equals (target, cam1.Target));
		}

		private static bool Equals (Vector3 a, Vector3 b)
		{
			return (a- b).Length().Abs() < 0.1f;
		}
	}
}
