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
using Knot3.Input;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.Data;
using Knot3.Widgets;
using Knot3.Platform;

#endregion

namespace Knot3.Utilities
{
	public static class ShaderHelper
	{
		public static Effect LoadEffect (this IGameScreen screen, string name)
		{
			if (SystemInfo.IsRunningOnMono () || SystemInfo.IsRunningOnMonogame ()) {
				return LoadEffectMono (screen, name);
			}
			else {
				return LoadEffectDotnet (screen, name);
			}
		}

		private static Effect LoadEffectMono (IGameScreen screen, string name)
		{
			string[] filenames = {
				"Content/Shader/" + name + ".mgfx",
				"Content/Shader/" + name + "_3.0.mgfx",
				"Content/Shader/" + name + "_3.1.mgfx"
			};
			Exception lastException = new Exception ("Could not find shader: " + name);
			foreach (string filename in filenames) {
				try {
					Effect effect = new Effect (screen.Device, System.IO.File.ReadAllBytes (filename));
					return effect;
				}
				catch (Exception ex) {
					lastException = ex;
				}
			}
			throw lastException;
		}

		private static Effect LoadEffectDotnet (IGameScreen screen, string name)
		{
			return screen.Content.Load<Effect> ("Shader/" + name);
		}
	}
}
