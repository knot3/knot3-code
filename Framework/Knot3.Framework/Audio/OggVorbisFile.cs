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

using Knot3.Framework.Platform;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using OggSharp;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Knot3.Framework.Audio
{
    public class OggVorbisFile : IAudioFile
    {
        public string Name { get; private set; }

        public SoundState State { get { return internalFile.State; } }

        private SoundEffectFile internalFile;

        public OggVorbisFile (string name, string filepath, Sound soundType)
        {
            Name = name;
            string cachefile = SystemInfo.DecodedMusicCache
                + SystemInfo.PathSeparator.ToString ()
                + soundType.ToString ()
                + "_"
                + name.GetHashCode ().ToString ()
                + ".wav";

            Log.BlockList (id: 33, before: "  - ", after: "", begin: "Load ogg audio files:", end: "");
            Log.BlockList (id: 34, before: "  - ", after: "", begin: "Decode ogg audio files:", end: "");

            byte[] data;
            try {
                Log.ListElement (33, "[", soundType, "] ", name);
                data = File.ReadAllBytes (cachefile);
            }
            catch (Exception) {
                Log.ListElement (34, "[", soundType, "] ", name);
                OggDecoder decoder = new OggDecoder ();
                decoder.Initialize (TitleContainer.OpenStream (filepath));
                data = decoder.SelectMany (chunk => chunk.Bytes.Take (chunk.Length)).ToArray ();
                using (MemoryStream stream = new MemoryStream ())
                using (BinaryWriter writer = new BinaryWriter (stream)) {
                    WriteWave (writer, decoder.Stereo ? 2 : 1, decoder.SampleRate, data);
                    stream.Position = 0;
                    data = stream.ToArray ();
                }
                File.WriteAllBytes (cachefile, data);
            }

            using (MemoryStream stream = new MemoryStream (data)) {
                stream.Position = 0;
                SoundEffect soundEffect = SoundEffect.FromStream (stream);
                internalFile = new SoundEffectFile (name, soundEffect, soundType);
            }
        }

        public void Play ()
        {
            internalFile.Play ();
        }

        public void Stop ()
        {
            internalFile.Stop ();
        }

        [ExcludeFromCodeCoverageAttribute]
        public void Update (GameTime time)
        {
            internalFile.Update (time);
        }

        private static void WriteWave (BinaryWriter writer, int channels, int rate, byte[] data)
        {
            writer.Write (new char[4] { 'R', 'I', 'F', 'F' });
            writer.Write ((int)(36 + data.Length));
            writer.Write (new char[4] { 'W', 'A', 'V', 'E' });

            writer.Write (new char[4] { 'f', 'm', 't', ' ' });
            writer.Write ((int)16);
            writer.Write ((short)1);
            writer.Write ((short)channels);
            writer.Write ((int)rate);
            writer.Write ((int)(rate * ((16 * channels) / 8)));
            writer.Write ((short)((16 * channels) / 8));
            writer.Write ((short)16);

            writer.Write (new char[4] { 'd', 'a', 't', 'a' });
            writer.Write ((int)data.Length);
            writer.Write (data);
        }
    }
}
