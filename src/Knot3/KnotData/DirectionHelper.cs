using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Widgets;

namespace Knot3.KnotData
{
	public static class DirectionHelper
	{
		public static Direction ToDirection (this Vector3 vector)
		{
			foreach (Direction direction in Direction.Values) {
				if (direction.Vector == vector) {
					return direction;
				}
			}
			return Direction.Zero;
		}

		public static Axis[] Axes = new Axis[] { Axis.X, Axis.Y, Axis.Z };
	}
}