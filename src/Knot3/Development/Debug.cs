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
using System.Diagnostics;
using System.Linq;

using Knot3.Platform;
using Knot3.Utilities;

#endregion

namespace Knot3.Development
{
	public static class Log
	{
		private static string lastDebugStr = "";
		private static int lastDebugTimes = 0;

		[Conditional ("DEBUG")]
		public static void Debug (params object[] message)
		{
			string str = string.Join ("", message);
			if (SystemInfo.IsRunningOnLinux ()) {
				Console.WriteLine (str);
			}
			else {
				if (str == lastDebugStr) {
					++lastDebugTimes;
					if (lastDebugTimes > 100) {
						Console.WriteLine ("[" + lastDebugTimes + "x] " + lastDebugStr);
						lastDebugTimes = 0;
					}
				}
				else {
					if (lastDebugTimes > 0) {
						Console.WriteLine (lastDebugTimes + "x " + lastDebugStr);
					}
					Console.WriteLine (str);
					lastDebugStr = str;
					lastDebugTimes = 0;
				}
			}
		}

		public static void Message (params object[] message)
		{
			Console.WriteLine (string.Join ("", message));
		}

		public static void Error (Exception ex)
		{
			Console.WriteLine (ex.ToString ());
		}
	}
}
