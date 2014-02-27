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

namespace Knot3.Framework.Storage
{
    /// <summary>
    /// Eine statische Klasse, die eine Referenz auf die zentrale Einstellungsdatei des Spiels enthält.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Die zentrale Einstellungsdatei des Spiels.
        /// </summary>
        public static ConfigFile Default
        {
            get {
                if (_default == null) {
                    _default = new ConfigFile (SystemInfo.SettingsDirectory + "knot3.ini");
                }
                return _default;
            }
            set {
                _default = value;
            }
        }

        private static ConfigFile _default;

        public static ConfigFile Models
        {
            get {
                if (_models == null) {
                    _models = new ConfigFile (SystemInfo.RelativeContentDirectory + "models.ini");
                }
                return _models;
            }
        }

        private static ConfigFile _models;
    }
}
