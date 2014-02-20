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

using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;

#endregion

namespace Knot3.Game.Widgets
{
	/// <summary>
	/// Repräsentiert einen Eintrag in einem Dropdown-Menü.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public class DropDownEntry
	{
		#region Properties

		/// <summary>
		/// Der Text des Eintrags.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Die Aktion, welche bei der Auswahl ausgeführt wird.
		/// </summary>
		public Action OnSelect { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines DropDownEntry-Objekts.
		/// text bezeichnet den Text für den Eintrag,
		/// onSelect ist die Aktion, welche bei der Auswahl des Eintrags auzuführen ist (s. Action).
		/// </summary>
		public DropDownEntry (string text, Action onSelect)
		{
			throw new System.NotImplementedException ();
		}

		#endregion
	}
}
