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
using System.ComponentModel;
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
using Knot3.Data;
using Knot3.Input;
using Knot3.RenderEffects;
using Knot3.Screens;
using Knot3.Utilities;
using Knot3.Widgets;

#endregion

namespace Knot3.Input
{
	/// <summary>
	/// Die Aktionen, für die der Spieler die zugewiesene Taste festlegen kann.
	/// Sie können in den Controls-Einstellungen verändert werden.
	/// </summary>
	public enum PlayerActions
	{
		[Description ("Move Up")]
		MoveUp,
		[Description ("Move Down")]
		MoveDown,
		[Description ("Move Left")]
		MoveLeft,
		[Description ("Move Right")]
		MoveRight,
		[Description ("Move Forward")]
		MoveForward,
		[Description ("Move Backward")]
		MoveBackward,
		[Description ("Rotate Up")]
		RotateUp,
		[Description ("Rotate Down")]
		RotateDown,
		[Description ("Rotate Left")]
		RotateLeft,
		[Description ("Rotate Right")]
		RotateRight,
		[Description ("Zoom In")]
		ZoomIn,
		[Description ("Zoom Out")]
		ZoomOut,
		[Description ("Reset Camera")]
		ResetCamera,
		[Description ("Move Selection to Center")]
		MoveToCenter,
		[Description ("Toggle Mouse Lock")]
		ToggleMouseLock,
		[Description ("Add to Selection")]
		AddToEdgeSelection,
		[Description ("Add Range to Selection")]
		AddRangeToEdgeSelection,
	}
}
