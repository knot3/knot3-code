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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

#endregion

namespace Knot3.Framework.Input
{
	/// <summary>
	/// Eine Schnittstelle, die von Klassen implementiert wird, die auf Maus-Klicks reagieren.
	/// </summary>
	public interface IMouseClickEventListener
	{
		#region Properties

		/// <summary>
		/// Die Eingabepriorität.
		/// </summary>
		DisplayLayer Index { get; }

		/// <summary>
		/// Ob die Klasse zur Zeit auf Mausklicks reagiert.
		/// </summary>
		bool IsMouseClickEventEnabled { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Die Ausmaße des von der Klasse repräsentierten Objektes.
		/// </summary>
		Bounds MouseClickBounds { get; }

		/// <summary>
		/// Die Reaktion auf einen Linksklick.
		/// </summary>
		void OnLeftClick (Vector2 position, ClickState state, GameTime time);

		/// <summary>
		/// Die Reaktion auf einen Rechtsklick.
		/// </summary>
		void OnRightClick (Vector2 position, ClickState state, GameTime time);

		void SetHovered (bool hovered, GameTime time);

		#endregion
	}
}