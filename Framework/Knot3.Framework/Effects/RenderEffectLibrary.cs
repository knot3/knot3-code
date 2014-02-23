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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Input;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.Effects
{
	[ExcludeFromCodeCoverageAttribute]
	public class RenderEffectLibrary
	{
		public static List<EffectFactory> EffectLibrary = new List<EffectFactory> (new EffectFactory[] {
			new EffectFactory (
			    name: "default",
			    displayName: "Default",
			    createInstance: (screen) => new StandardEffect (screen)
			),
		}
		                                                                          );

		public static Action<string, GameTime> RenderEffectChanged = (e, t) => {};

		public static IEnumerable<string> Names
		{
			get {
				foreach (EffectFactory factory in EffectLibrary) {
					yield return factory.Name;
				}
			}
		}

		public static string DisplayName (string name)
		{
			return Factory (name).DisplayName;
		}

		public static IRenderEffect CreateEffect (IGameScreen screen, string name)
		{
			return Factory (name).CreateInstance (screen);
		}

		private static EffectFactory Factory (string name)
		{
			foreach (EffectFactory factory in EffectLibrary) {
				if (factory.Name == name) {
					return factory;
				}
			}
			return EffectLibrary [0];
		}

		[ExcludeFromCodeCoverageAttribute]
		public class EffectFactory
		{
			public string Name { get; private set; }

			public string DisplayName { get; private set; }

			public Func<IGameScreen, IRenderEffect> CreateInstance { get; private set; }

			public EffectFactory (string name, string displayName, Func<IGameScreen, IRenderEffect> createInstance)
			{
				Name = name;
				DisplayName = displayName;
				CreateInstance = createInstance;
			}
		}
	}
}
