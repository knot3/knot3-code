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







#endregion

namespace Knot3.Core
{
	/// <summary>
	/// Eine zeichenbare Spielkomponente, die in einem angegebenen Spielzustand verwendet wird und eine bestimmte Priorität hat.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public abstract class DrawableGameScreenComponent : DrawableGameComponent, IGameScreenComponent
	{
		#region Properties

		/// <summary>
		/// Der zugewiesene Spielzustand.
		/// </summary>
		public IGameScreen Screen { get; set; }

		private DisplayLayer _index;

		/// <summary>
		/// Die Zeichen- und Eingabepriorität.
		/// </summary>
		public DisplayLayer Index
		{
			get { return _index; }
			set {
				_index = value;
				DrawOrder = (int)value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines DrawableGameScreenComponent-Objekts und ordnet dieser ein IGameScreen-Objekt zu.
		/// index bezeichnet die Zeichenebene, auf welche die Komponente zu zeichnen ist.
		/// </summary>
		public DrawableGameScreenComponent (IGameScreen screen, DisplayLayer index)
		: base (screen.Game)
		{
			this.Screen = screen;
			this.Index = index;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gibt Spielkomponenten zurück, die in dieser Spielkomponente enthalten sind.
		/// [returntype=IEnumerable<IGameScreenComponent>]
		/// </summary>
		public virtual IEnumerable<IGameScreenComponent> SubComponents (GameTime GameTime)
		{
			yield break;
		}

		#endregion
	}
}
