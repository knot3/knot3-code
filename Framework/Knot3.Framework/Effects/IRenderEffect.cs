/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * This source code file is part of Knot3. Copying, redistribution and
 * use of the source code in this file in source and binary forms,
 * with or without modification, are permitted provided that the conditions
 * of the MIT license are met:
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 *
 * See the LICENSE file for full license details of the Knot3 project.
 */

using System;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Models;

namespace Knot3.Framework.Effects
{
    /// <summary>
    /// Stellt eine Schnittstelle für Klassen bereit, die Rendereffekte ermöglichen.
    /// </summary>
    public interface IRenderEffect : IDisposable
    {
        /// <summary>
        /// Das Rendertarget, in das zwischen dem Aufruf der Begin ()- und der End ()-Methode gezeichnet wird,
        /// weil es in Begin () als primäres Rendertarget des MonoGame-Frameworks gesetzt wird.
        /// </summary>
        RenderTarget2D RenderTarget { get; }

        bool SelectiveRendering { get; }

        /// <summary>
        /// In der Methode Begin () wird das aktuell von MonoGame genutzte Rendertarget auf einem Stapel gesichert
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
        /// Zeichnet das Spielprimitiv primitive mit diesem Rendereffekt.
        /// </summary>
        void DrawPrimitive (GamePrimitive primitive, GameTime time);

        /// <summary>
        /// Beim Laden des Modells wird von der XNA-Content-Pipeline jedem ModelMeshPart ein Shader der Klasse
        /// BasicEffect zugewiesen. Für die Nutzung des Modells in diesem Rendereffekt kann jedem ModelMeshPart
        /// ein anderer Shader zugewiesen werden.
        /// </summary>
        void RemapModel (Model model);

        void DrawLastFrame (GameTime time);
    }
}
