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

using Knot3.Framework.Platform;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Core
{
    [ExcludeFromCodeCoverageAttribute]
    public abstract class GameClass : Microsoft.Xna.Framework.Game
    {
        private static readonly Vector2 defaultSize = SystemInfo.IsRunningOnLinux ()
                ? new Vector2 (1024, 600) : new Vector2 (1280, 720);

        public Action FullScreenChanged { get; set; }

        protected string lastResolution;
        protected bool isFullscreen;

        /// <summary>
        /// Wird dieses Attribut ausgelesen, dann gibt es einen Wahrheitswert zurück, der angibt,
        /// ob sich das Spiel im Vollbildmodus befindet. Wird dieses Attribut auf einen Wert gesetzt,
        /// dann wird der Modus entweder gewechselt oder beibehalten, falls es auf denselben Wert gesetzt wird.
        /// </summary>
        public bool IsFullScreen
        {
            get {
                return isFullscreen;
            }
            set {
                if (value != isFullscreen) {
                    Log.Debug ("Fullscreen Toggle");
                    if (value) {
                        Graphics.PreferredBackBufferWidth = Graphics.GraphicsDevice.DisplayMode.Width;
                        Graphics.PreferredBackBufferHeight = Graphics.GraphicsDevice.DisplayMode.Height;
                    }
                    else {
                        string currentResolution = Graphics.GraphicsDevice.DisplayMode.Width.ToString ()
                                                   + "x"
                                                   + Graphics.GraphicsDevice.DisplayMode.Height.ToString ();

                        Config.Default ["video", "resolution", currentResolution] = "1280x720";
                    }
                    Graphics.ToggleFullScreen ();
                    Graphics.ApplyChanges ();
                    isFullscreen = value;
                    Graphics.ApplyChanges ();
                    FullScreenChanged ();
                    toDefaultSize (isFullscreen);
                }
            }
        }

        /// <summary>
        /// Enthält als oberste Element den aktuellen Spielzustand und darunter die zuvor aktiven Spielzustände.
        /// </summary>
        public Stack<IGameScreen> Screens { get; set; }

        /// <summary>
        /// Dieses Attribut dient sowohl zum Setzen des Aktivierungszustandes der vertikalen Synchronisation,
        /// als auch zum Auslesen dieses Zustandes.
        /// </summary>
        public bool VSync
        {
            get {
                return Graphics.SynchronizeWithVerticalRetrace;
            }
            set {
                Graphics.SynchronizeWithVerticalRetrace = value;
                this.IsFixedTimeStep = value;
                Graphics.ApplyChanges ();
            }
        }

        /// <summary>
        /// Der aktuelle Grafikgeräteverwalter des XNA-Frameworks.
        /// </summary>
        public GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Erstellt ein neues zentrales Spielobjekt und setzt die Auflösung des BackBuffers auf
        /// die in der Einstellungsdatei gespeicherte Auflösung oder falls nicht vorhanden auf die aktuelle
        /// Bildschirmauflösung und wechselt in den Vollbildmodus.
        /// </summary>
        public GameClass ()
        {
            Graphics = new GraphicsDeviceManager (this);

            Graphics.PreferredBackBufferWidth = (int)defaultSize.X;
            Graphics.PreferredBackBufferHeight = (int)defaultSize.Y;

            Graphics.IsFullScreen = false;
            isFullscreen = false;
            Graphics.ApplyChanges ();

            if (SystemInfo.IsRunningOnLinux ()) {
                IsMouseVisible = true;
            }
            else if (SystemInfo.IsRunningOnWindows ()) {
                IsMouseVisible = false;
                System.Windows.Forms.Cursor.Hide ();
            }
            else {
                throw new Exception ("Unsupported Plattform Exception");
            }

            Content.RootDirectory = SystemInfo.RelativeContentDirectory;
            Window.Title = "";

            FullScreenChanged = () => {};
        }

        private void toDefaultSize (bool fullscreen)
        {
            if (!fullscreen) {
                Graphics.PreferredBackBufferWidth = (int)GameClass.defaultSize.X;
                Graphics.PreferredBackBufferHeight = (int)GameClass.defaultSize.Y;
                Graphics.ApplyChanges ();
            }
        }

        protected void updateResolution ()
        {
            int width;
            int height;
            string currentResolution = Graphics.GraphicsDevice.DisplayMode.Width.ToString ()
                                       + "x"
                                       + Graphics.GraphicsDevice.DisplayMode.Height.ToString ();
            if (lastResolution != Config.Default ["video", "resolution", currentResolution] && !isFullscreen) {
                String strReso = Config.Default ["video", "resolution", currentResolution];
                string[] reso = strReso.Split ('x');
                width = int.Parse (reso [0]);
                height = int.Parse (reso [1]);
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.ApplyChanges ();
            }
            lastResolution = Config.Default ["video", "resolution", currentResolution];
        }
    }
}
