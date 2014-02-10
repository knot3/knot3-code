using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Core;
using Knot3.Utilities;
using Knot3.RenderEffects;

namespace Knot3.UnitTests.Test_Camera
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für Test_Camera
	/// </summary>
	[TestFixture]
	public class Test_Camera
	{
		FakeScreen screen;
		Camera cam1;
		IRenderEffect effect;
		World world;

		[SetUp]
		public void Init ()
		{
			screen = new FakeScreen ();
			effect = new FakeEffect (screen);
			world = new World (screen: screen, effect: effect);
			cam1 = new Camera (screen: screen, world: world);
		}

		[Test]
		public void PositionTargetTest ()
		{
			Vector3 pos = cam1.Position;
			cam1.Position += Vector3.Left;
			Assert.AreEqual (pos + Vector3.Left, cam1.Position);

			Vector3 target = cam1.Target;
			cam1.Target += Vector3.Up;
			Assert.AreEqual (target + Vector3.Up, cam1.Target);

			pos = cam1.Position;
			target = cam1.Target;
			cam1.PositionToTargetDistance -= 50;
			cam1.PositionToTargetDistance += 100;
			cam1.PositionToTargetDistance -= 50;
			Assert.True (Equals (pos, cam1.Position));
			Assert.True (Equals (target, cam1.Target));
		}

		[Test]
		public void MatrixTest ()
		{
			Assert.IsNotNull (cam1.WorldMatrix);
			Assert.IsNotNull (cam1.ViewMatrix);
			Assert.IsNotNull (cam1.ProjectionMatrix);
			Assert.IsNotNull (cam1.ViewFrustum);
		}

		[Test]
		public void MouseTest ()
		{
			Vector2 mouse = screen.Bounds.FromTop (0.5f).FromLeft (0.5f).Position;
			Vector3 mouse3D = cam1.To3D (position: mouse, nearTo: cam1.Target);
			Vector2 mouse2D = cam1.To2D (position: mouse3D);
			Assert.True (Equals (mouse, mouse2D));
		}

		private static bool Equals (Vector3 a, Vector3 b)
		{
			return (a - b).Length ().Abs () < 0.1f;
		}

		private static bool Equals (Vector2 a, Vector2 b)
		{
			return (a - b).Length ().Abs () < 0.1f;
		}
	}
}
