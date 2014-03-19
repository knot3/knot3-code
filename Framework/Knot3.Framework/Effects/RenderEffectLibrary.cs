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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;

using Knot3.Framework.Core;

namespace Knot3.Framework.Effects
{
    [ExcludeFromCodeCoverageAttribute]
    public partial class RenderEffectLibrary
    {
        public static List<EffectFactory> EffectLibrary = new List<EffectFactory> ();

        static RenderEffectLibrary ()
        {
            EffectLibrary.Add (new EffectFactory (
                                   name: "basiceffect",
                                   displayName: "Basic Effect (XNA)",
                                   createInstance: (screen) => new StandardEffect (screen)
                               ));
            AddDefaultGLShaders ();
        }

        public static Action<string, GameTime> RenderEffectChanged = (e, t) => {};

        public static IEnumerable<string> Names
        {
            get {
                foreach (EffectFactory factory in EffectLibrary) {
                    yield return factory.Name;
                }
            }
        }

        public static IEnumerable<EffectFactory> Factories
        {
            get {
                foreach (EffectFactory factory in EffectLibrary) {
                    yield return factory;
                }
            }
        }

        public static string DisplayName (string name)
        {
            return Factory (name).DisplayName;
        }

        public static IRenderEffect CreateEffect (IScreen screen, string name)
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

            public Func<IScreen, IRenderEffect> CreateInstance { get; private set; }

            public EffectFactory (string name, string displayName, Func<IScreen, IRenderEffect> createInstance)
            {
                Name = name;
                DisplayName = displayName;
                CreateInstance = createInstance;
            }
        }
    }
}
