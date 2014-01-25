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
using Knot3.KnotData;
using Knot3.Widgets;
using Knot3.RenderEffects;

namespace Knot3.UnitTests
{
	/// <summary>
	/// Ein Stapel, der während der Draw-Aufrufe die Hierarchie der aktuell verwendeten Rendereffekte verwaltet
	/// und automatisch das aktuell von XNA verwendete Rendertarget auf das Rendertarget des obersten Rendereffekts
	/// setzt.
	/// </summary>
	public sealed class FakeEffectStack : IRenderEffectStack
	{
		#region Properties

		private static Stack<IRenderEffect> stack = new Stack<IRenderEffect> ();

		/// <summary>
		/// Der oberste Rendereffekt.
		/// </summary>
		public IRenderEffect CurrentEffect
		{
			get {
				if (stack.Count > 0) {
					return stack.Peek ();
				}
				else {
					return defaultEffect;
				}
			}
		}

		/// <summary>
		/// Der Standard-Rendereffekt, der verwendet wird, wenn der Stapel leer ist.
		/// </summary>
		private IRenderEffect defaultEffect { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt einen neuen Rendereffekt-Stapel.
		/// </summary>
		public FakeEffectStack (IGameScreen screen, IRenderEffect defaultEffect)
		{
			this.defaultEffect = defaultEffect;
			stack = new Stack<IRenderEffect> ();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Entfernt den obersten Rendereffekt vom Stapel.
		/// </summary>
		public IRenderEffect Pop ()
		{
			return stack.Pop ();
		}

		/// <summary>
		/// Legt einen Rendereffekt auf den Stapel.
		/// </summary>
		public void Push (IRenderEffect effect)
		{
			stack.Push (effect);
		}

		#endregion
	}
}
