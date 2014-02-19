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







#endregion

namespace Knot3.Core
{
	/// <summary>
	/// Diese Klasse repräsentiert eine Option, die einen Wert aus einer distinkten Werteliste annehmen kann.
	/// </summary>
	public class DistinctOptionInfo : OptionInfo
	{
		#region Properties

		/// <summary>
		/// Eine Menge von Texten, welche die für die Option gültigen Werte beschreiben.
		/// </summary>
		public HashSet<string> ValidValues { get; private set; }

		public virtual Dictionary<string,string> DisplayValidValues { get; private set; }
		/// <summary>
		/// Eine Eigenschaft, die den aktuell abgespeicherten Wert zurück gibt.
		/// </summary>
		public override string Value
		{
			get {
				return base.Value;
			}
			set {
				if (ValidValues.Contains (value)) {
					base.Value = value;
				}
				else {
					base.Value = DefaultValue;
				}
			}
		}
		public virtual string DisplayValue
		{
			get {
				return Value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt eine neue Option, die einen der angegebenen Werte aus validValues annehmen kann, mit dem angegebenen Namen in dem
		/// angegebenen Abschnitt der angegebenen Einstellungsdatei.
		/// [base=section, name, defaultValue, configFile]
		/// </summary>
		public DistinctOptionInfo (string section, string name, string defaultValue, IEnumerable<string> validValues, ConfigFile configFile)
		: base (section, name, defaultValue, configFile)
		{
			ValidValues = new HashSet<string> (validValues);
			ValidValues.Add (defaultValue);
			DisplayValidValues = new Dictionary<string,string> (ValidValues.ToDictionary (x=>x,x=>x));
		}

		#endregion
	}
}
