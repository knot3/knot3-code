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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Audio;
using Knot3.Framework.Effects;
using Knot3.Framework.Platform;
using Knot3.Framework.Screens;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Core
{
    [ExcludeFromCodeCoverageAttribute]
    public abstract class GameCore : Microsoft.Xna.Framework.Game
    {
        private static readonly Point defaultSize = SystemInfo.IsRunningOnLinux ()
                ? new Point (1024, 600) : new Point (1280, 720);

        public Action FullScreenChanged { get; set; }

        protected Point lastResolution;
        protected bool isFullscreen;

        public bool SkipNextScreenEffect { get; set; }

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
                    Graphics.ToggleFullScreen ();
                    Graphics.ApplyChanges ();
                    isFullscreen = value;
                    Config.Default ["video", "fullscreen", false] = value;
                    Graphics.ApplyChanges ();
                }
            }
        }

        protected Point ScreenResolution
        {
            get {
                return new Point (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
        }

        protected Point WindowResolution
        {
            get {
                String strReso = Config.Default ["video", "resolution", defaultSize.X + "x" + defaultSize.Y];
                string[] reso = strReso.Split ('x');
                int width = int.Parse (reso [0]);
                int height = int.Parse (reso [1]);
                return new Point (width, height);
            }
            set {
                Config.Default ["video", "resolution", defaultSize.X + "x" + defaultSize.Y] = value.X + "x" + value.Y;
            }
        }

        protected Point BackbufferResolution
        {
            get {
                return new Point (Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }
            set {
                Graphics.PreferredBackBufferWidth = value.X;
                Graphics.PreferredBackBufferHeight = value.Y;
                Graphics.ApplyChanges ();
            }
        }

        /// <summary>
        /// Enthält als oberste Element den aktuellen Spielzustand und darunter die zuvor aktiven Spielzustände.
        /// </summary>
        public Stack<IScreen> Screens { get; set; }

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
        /// Der AudioManager des Spiels.
        /// </summary>
        public AudioManager AudioManager { get; protected set; }

        /// <summary>
        /// Erstellt ein neues zentrales Spielobjekt und setzt die Auflösung des BackBuffers auf
        /// die in der Einstellungsdatei gespeicherte Auflösung oder falls nicht vorhanden auf die aktuelle
        /// Bildschirmauflösung und wechselt in den Vollbildmodus.
        /// </summary>
        public GameCore ()
        {
            Graphics = new GraphicsDeviceManager (this);

            Graphics.PreferredBackBufferWidth = (int)defaultSize.X;
            Graphics.PreferredBackBufferHeight = (int)defaultSize.Y;

            Graphics.IsFullScreen = false;
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

            ErrorScreenEnabled = true;
        }

        protected override void Initialize ()
        {
            base.Initialize ();

            IsFullScreen = Config.Default ["video", "fullscreen", false];
        }

        public abstract IScreen DefaultScreen { get; }

        public Func<IScreen, IScreen, IRenderEffect> ScreenTransitionEffect;

        public IScreen CurrentScreen { get { return Screens.Peek (); } }

        private void InitializeScreens ()
        {
            // screens
            Screens = new Stack<IScreen> ();
            Screens.Push (DefaultScreen);
            Screens.Peek ().Entered (null, null);
        }

        protected void updateResolution ()
        {
            Point currentResolution = IsFullScreen ? ScreenResolution : WindowResolution;
            if (lastResolution != currentResolution || BackbufferResolution != currentResolution) {
                Log.Message ("Resolution (Full Screen Mode): ", ScreenResolution);
                Log.Message ("Resolution (Window Mode): ", WindowResolution);
                Log.Message ("Resolution (Current Mode): ", currentResolution);
                Log.Message ("Resolution (BackBuffer - previously): ", BackbufferResolution);
                BackbufferResolution = currentResolution;
                lastResolution = currentResolution;
                FullScreenChanged ();
                Log.Message ("Resolution (BackBuffer - now): ", BackbufferResolution);
            }
            if (Config.Default ["debug", "projector-mode", false]) {
                WindowResolution = new Point (1024, 768);
                IsFullScreen = false;
            }
        }

        public bool ErrorScreenEnabled { get; protected set; }

        private Exception firstException = null;

        public void ShowError (Exception ex)
        {
            if (firstException == null) {
                Screens = new Stack<IScreen> ();
                Screens.Push (new ErrorScreen (this, ex));
                Screens.Peek ().Entered (null, null);
                firstException = ex;
            }
            else {
                Log.ShowMessageBox (firstException.ToString (), "Exception");
                Exit ();
            }
        }

        /// <summary>
        /// Ruft die Draw ()-Methode des aktuellen Spielzustands auf.
        /// </summary>
        protected override void Draw (GameTime time)
        {
            if (Screens == null) {
                InitializeScreens ();
            }

            try {
                // Lade den aktuellen Screen
                IScreen current = Screens.Peek ();

                // Starte den Post-Processing-Effekt des Screens
                current.PostProcessingEffect.Begin (time);
                Graphics.GraphicsDevice.Clear (current.BackgroundColor);

                try {
                    // Rufe Draw () auf dem aktuellen Screen auf
                    current.Draw (time);

                    // Rufe Draw () auf den Spielkomponenten auf
                    base.Draw (time);
                }
                catch (Exception ex) {
                    if (ErrorScreenEnabled) {
                        // Error Screen
                        ShowError (ex);
                    }
                    else {
                        throw;
                    }
                }

                // Beende den Post-Processing-Effekt des Screens
                current.PostProcessingEffect.End (time);
            }
            catch (Exception ex) {
                if (ErrorScreenEnabled) {
                    // Error Screen
                    ShowError (ex);
                }
                else {
                    throw;
                }
            }
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        protected override void Update (GameTime time)
        {
            if (Screens == null) {
                InitializeScreens ();
            }

            try {
                updateResolution ();
                // falls der Screen gewechselt werden soll...
                IScreen current = Screens.Peek ();
                IScreen next = current.NextScreen;
                if (current != next && !(current is ErrorScreen)) {
                    if (ScreenTransitionEffect != null && !SkipNextScreenEffect) {
                        next.PostProcessingEffect = ScreenTransitionEffect (current, next);
                    }
                    SkipNextScreenEffect = false;
                    current.BeforeExit (next, time);
                    current.NextScreen = current;
                    next.NextScreen = next;
                    if (next.ClearScreenHistory) {
                        Screens.Clear ();
                    }
                    Screens.Push (next);
                    next.Entered (current, time);
                }

                // Rufe Update () auf dem aktuellen Screen auf
                Screens.Peek ().Update (time);

                // base method
                base.Update (time);
            }
            catch (Exception ex) {
                if (ErrorScreenEnabled) {
                    // Error Screen
                    ShowError (ex);
                }
                else {
                    throw;
                }
            }
        }

        /// <summary>
        /// Macht nichts. Das Freigeben aller Objekte wird von der automatischen Speicherbereinigung übernommen.
        /// </summary>
        protected override void UnloadContent ()
        {
            base.UnloadContent ();
        }
    }
}
