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

using Knot3.Data;
using Knot3.GameObjects;
using Knot3.RenderEffects;
using Knot3.Screens;
using Knot3.Widgets;

#endregion

namespace Knot3.Core
{
	/// <summary>
	/// Diese Klasse repräsentiert eine Option, welche die Werte \glqq Wahr\grqq~oder \glqq Falsch\grqq~annehmen kann.
	/// </summary>
	public sealed class BooleanOptionInfo : DistinctOptionInfo
	{
		#region Properties

		/// <summary>
		/// Eine Eigenschaft, die den aktuell abgespeicherten Wert zurückgibt.
		/// </summary>
		public new bool Value
		{
			get {
				return base.Value == ConfigFile.True ? true : false;
			}
			set {
				base.Value = value ? ConfigFile.True : ConfigFile.False;
			}
		}

		public new static string[] ValidValues = new string[] {
			ConfigFile.True,
			ConfigFile.False
		};

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt eine neue Option, welche die Werte \glqq Wahr\grqq~oder \glqq Falsch\grqq~annehmen kann. Mit dem angegebenen Namen, in dem
		/// angegebenen Abschnitt der angegebenen Einstellungsdatei.
		/// [base=section, name, defaultValue?ConfigFile.True:ConfigFile.False, ValidValues, configFile]
		/// </summary>
		public BooleanOptionInfo (string section, string name, bool defaultValue, ConfigFile configFile)
		: base (section, name, defaultValue?ConfigFile.True:ConfigFile.False, ValidValues, configFile)
		{
		}

		#endregion
	}
}
