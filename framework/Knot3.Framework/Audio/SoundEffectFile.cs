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
using System.IO;
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

using Knot3.Development;

using Knot3.Input;


using Knot3.Utilities;


#endregion

namespace Knot3.Audio
{
	/// <summary>
	/// Ein Wrapper um die SoundEffect-Klasse des XNA-Frameworks.
	/// </summary>
	public class SoundEffectFile : IAudioFile
	{
		/// <summary>
		/// Der Anzeigename des SoundEffects.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gibt an, ob die Wiedergabe läuft oder gestoppt bzw. pausiert ist.
		/// </summary>
		public SoundState State { get { return Instance.State; } }

		public SoundEffect SoundEffect { get; private set; }

		private SoundEffectInstance Instance;

		private Sound SoundType;
		private float volume;

		/// <summary>
		/// Erstellt eine neue SoundEffect-Datei mit dem angegebenen Anzeigenamen und des angegebenen SoundEffect-Objekts.
		/// </summary>
		public SoundEffectFile (string name, SoundEffect soundEffect, Sound soundType)
		{
			Name = name;
			SoundEffect = soundEffect;
			Instance = soundEffect.CreateInstance ();
			SoundType = soundType;
		}

		public void Play ()
		{
			Log.Debug ("Play: ", Name);
			Instance.Volume = volume = AudioManager.Volume (SoundType);
			Instance.Play ();
		}

		public void Stop ()
		{
			Log.Debug ("Stop: ", Name);
			Instance.Stop ();
		}

		[ExcludeFromCodeCoverageAttribute]
		public void Update (GameTime time)
		{
			if (volume != AudioManager.Volume (SoundType)) {
				Instance.Volume = volume = AudioManager.Volume (SoundType);
			}
		}
	}
}
