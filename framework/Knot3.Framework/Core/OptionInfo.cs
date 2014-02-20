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
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

#endregion

namespace Knot3.Framework.Core
{
	/// <summary>
	/// Enthält Informationen über einen Eintrag in einer Einstellungsdatei.
	/// </summary>
	public class OptionInfo
	{
		#region Properties

		/// <summary>
		/// Die Einstellungsdatei.
		/// </summary>
		private ConfigFile configFile;

		/// <summary>
		/// Der Abschnitt der Einstellungsdatei.
		/// </summary>
		public string Section { get; private set; }

		/// <summary>
		/// Der Name der Option.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Der Standardwert der Option.
		/// </summary>
		public string DefaultValue { get; private set; }

		/// <summary>
		/// Der Wert der Option.
		/// </summary>
		public virtual string Value
		{
			get {
				Log.Debug ("OptionInfo: ", Section, ".", Name, " => ", configFile [Section, Name, DefaultValue]);
				return configFile [Section, Name, DefaultValue];
			}
			set {
				Log.Debug ("OptionInfo: ", Section, ".", Name, " <= ", value);
				configFile [Section, Name, DefaultValue] = value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt ein neues OptionsInfo-Objekt aus den übergegebenen Werten.
		/// </summary>
		public OptionInfo (string section, string name, string defaultValue, ConfigFile configFile)
		{
			Section = section;
			Name = name;
			DefaultValue = defaultValue;
			this.configFile = configFile != null ? configFile : Options.Default;
		}

		#endregion
	}
}
