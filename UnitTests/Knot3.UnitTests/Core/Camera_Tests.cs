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

#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Microsoft.Xna.Framework;

using NUnit.Framework;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;

using Knot3.Game.Core;
using Knot3.Game.RenderEffects;
using Knot3.Game.Utilities;

using Knot3.MockObjects;

#endregion

namespace Knot3.UnitTests.Core
{
	/// <summary>
	/// Zusammenfassungsbeschreibung für Test_Camera
	/// </summary>
	[TestFixture]
	public class Camera_Tests
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
			world = new World (screen: screen, drawIndex: DisplayLayer.GameWorld, effect: effect, bounds: screen.Bounds);
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
			cam1.PositionToTargetDistance = cam1.PositionToTargetDistance;
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
			string orig = Options.Default ["debug", "unproject", "SelectedObject"];
			foreach (string optionValue in new string[] { "SelectedObject", "NearFarAverage" }) {
				Options.Default ["debug", "unproject", "SelectedObject"] = optionValue;
				Vector2 mouse = screen.Bounds.FromTop (0.5f).FromLeft (0.5f).Position;
				Vector3 mouse3D = cam1.To3D (position: mouse, nearTo: cam1.Target);
				Vector2 mouse2D = cam1.To2D (position: mouse3D);
				Assert.True (Equals (mouse, mouse2D));
			}
			Options.Default ["debug", "unproject", "SelectedObject"] = orig;
		}

		[Test]
		public void ResetTest ()
		{
			cam1.ResetCamera ();
			Vector3 pos = cam1.Position;
			Vector3 target = cam1.Target;
			float fov = cam1.FoV;
			cam1.Position += new Vector3 (43, 24, 64);
			cam1.Target += new Vector3 (29, 24, 234);
			cam1.FoV += 10;
			Assert.AreNotEqual (pos, cam1.Position);
			Assert.AreNotEqual (target, cam1.Target);
			Assert.AreNotEqual (fov, cam1.FoV);
			cam1.ResetCamera ();
			Assert.AreEqual (pos, cam1.Position);
			Assert.AreEqual (target, cam1.Target);
			Assert.AreEqual (fov, cam1.FoV);
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
