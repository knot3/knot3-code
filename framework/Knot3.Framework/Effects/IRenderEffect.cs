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
using Knot3.Framework.GameObjects;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.RenderEffects
{
	/// <summary>
	/// Stellt eine Schnittstelle für Klassen bereit, die Rendereffekte ermöglichen.
	/// </summary>
	public interface IRenderEffect
	{
		#region Properties

		/// <summary>
		/// Das Rendertarget, in das zwischen dem Aufruf der Begin ()- und der End ()-Methode gezeichnet wird,
		/// weil es in Begin () als primäres Rendertarget des XNA-Frameworks gesetzt wird.
		/// </summary>
		RenderTarget2D RenderTarget { get; }

		bool SelectiveRendering { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// In der Methode Begin () wird das aktuell von XNA genutzte Rendertarget auf einem Stapel gesichert
		/// und das Rendertarget des Effekts wird als aktuelles Rendertarget gesetzt.
		/// </summary>
		void Begin (GameTime time);

		/// <summary>
		/// Das auf dem Stapel gesicherte, vorher genutzte Rendertarget wird wiederhergestellt und
		/// das Rendertarget dieses Rendereffekts wird, unter Umständen in Unterklassen verändert,
		/// auf dieses ubergeordnete Rendertarget gezeichnet.
		/// </summary>
		void End (GameTime time);

		/// <summary>
		/// Zeichnet das Spielmodell model mit diesem Rendereffekt.
		/// </summary>
		void DrawModel (GameModel model, GameTime time);

		/// <summary>
		/// Beim Laden des Modells wird von der XNA-Content-Pipeline jedem ModelMeshPart ein Shader der Klasse
		/// BasicEffect zugewiesen. Für die Nutzung des Modells in diesem Rendereffekt kann jedem ModelMeshPart
		/// ein anderer Shader zugewiesen werden.
		/// </summary>
		void RemapModel (Model model);

		void DrawLastFrame (GameTime time);

		#endregion
	}
}
