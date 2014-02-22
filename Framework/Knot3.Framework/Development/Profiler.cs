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
using System.Diagnostics;
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
using Knot3.Framework.Platform;
using Knot3.Framework.RenderEffects;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

#endregion

namespace Knot3.Framework.Development
{
	[ExcludeFromCodeCoverageAttribute]
	public static class Profiler
	{
		public static TimeSpan Time (Action action)
		{
			Stopwatch stopwatch = Stopwatch.StartNew ();
			action ();
			stopwatch.Stop ();
			return stopwatch.Elapsed;
		}

		public static Hashtable ProfilerMap = new Hashtable ();

		public static HashtableActionWrapper ProfileDelegate = new HashtableActionWrapper ();
		public static HashtableWrapper Values = new HashtableWrapper ();

		[ExcludeFromCodeCoverageAttribute]
		public class HashtableWrapper
		{
			public double this [string str]
			{
				get {
					return (double)ProfilerMap [str];
				}
				set {
					ProfilerMap [str] = value;
				}
			}

			public bool ContainsKey (string str)
			{
				return ProfilerMap.ContainsKey (str);
			}
		}

		[ExcludeFromCodeCoverageAttribute]
		public class HashtableActionWrapper
		{
			public Action this [string str]
			{
				set {
					ProfilerMap [str] = Time (value).TotalMilliseconds;
				}
			}
		}
	}
}