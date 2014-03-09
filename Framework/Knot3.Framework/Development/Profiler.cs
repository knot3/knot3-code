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
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
