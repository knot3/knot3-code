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
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.KnotData;
using Knot3.Widgets;

namespace Knot3.GameObjects
{
	/// <summary>
	/// Repräsentiert einen Übergang zwischen zwei Kanten.
	/// </summary>
	public interface IJunction
	{
		#region Properties

		/// <summary>
		/// Die Kante vor dem Übergang.
		/// </summary>
		Edge EdgeFrom { get; }

		/// <summary>
		/// Die Kante nach dem Übergang.
		/// </summary>
		Edge EdgeTo { get; }

		Node Node { get; }

		int Index { get; }

		JunctionType Type { get; }

		#endregion
	}

	public enum JunctionType {
		Angled,
		Straight
	}
}