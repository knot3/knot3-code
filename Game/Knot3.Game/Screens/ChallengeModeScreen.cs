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
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;

using Knot3.Game.Audio;
using Knot3.Game.Data;
using Knot3.Game.Development;
using Knot3.Game.Input;
using Knot3.Game.Models;
using Knot3.Game.Widgets;

namespace Knot3.Game.Screens
{
    /// <summary>
    /// Der Spielzustand, der während dem Spielen einer Challenge aktiv ist und für den Ausgangs- und Referenzknoten je eine 3D-Welt zeichnet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class ChallengeModeScreen : Screen
    {
        /// <summary>
        /// Die Spielwelt in der die 3D-Modelle des dargestellten Referenzknotens enthalten sind.
        /// </summary>
        private World ChallengeWorld;
        /// <summary>
        /// Die Spielwelt in der die 3D-Modelle des dargestellten Spielerknotens enthalten sind.
        /// </summary>
        private World PlayerWorld;
        /// <summary>
        /// Der Controller, der aus dem Referenzknoten die 3D-Modelle erstellt.
        /// </summary>
        private KnotRenderer ChallengeKnotRenderer;
        /// <summary>
        /// Der Controller, der aus dem Spielerknoten die 3D-Modelle erstellt.
        /// </summary>
        private KnotRenderer PlayerKnotRenderer;
        /// <summary>
        /// Der Inputhandler, der die Kantenverschiebungen des Spielerknotens durchführt.
        /// </summary>
        private EdgeMovement PlayerEdgeMovement;

        /// <summary>
        /// Der Undo-Stack.
        /// </summary>
        public Stack<Knot> Undo { get; set; }

        /// <summary>
        /// Der Redo-Stack.
        /// </summary>
        public Stack<Knot> Redo { get; set; }

        private bool returnFromPause;

        /// <summary>
        /// Die Challenge.
        /// </summary>
        public Challenge Challenge { get; set; }

        /// <summary>
        /// Der Spielerknoten, der durch die Tranformatsion des Spielers aus dem Ausgangsknoten entsteht.
        /// </summary>
        public Knot PlayerKnot
        {
            get {
                return _playerKnot;
            }
            set {
                _playerKnot = value;
                // Undo- und Redo-Stacks neu erstellen
                Redo = new Stack<Knot> ();
                Undo = new Stack<Knot> ();
                Undo.Push (_playerKnot.Clone () as Knot);
                // den Knoten dem KnotRenderer zuweisen
                registerCurrentKnot ();
                // Event registrieren
                _playerKnot.EdgesChanged += OnEdgesChanged;
                // coloring.Knot = knot;
            }
        }
        // der Knoten
        private Knot _playerKnot;
        // Spielkomponenten
        private KnotInputHandler playerKnotInput;
        private KnotInputHandler challengeKnotInput;
        private ModelMouseHandler playerModelMouseHandler;
        private ModelMouseHandler challengeModelMouseHandler;
        private MousePointer pointer;
        private Overlay overlay;
        private Lines lines;
        private DebugBoundings debugBoundings;
        // Zeitmessung und Zeitanzeige
        private TimeSpan playTime;
        private TextItem playTimeDisplay;
        private Border playTimeBorder;
        // Undo-Button
        private Button undoButton;
        private Border undoButtonBorder;
        // Undo-Button
        private Button redoButton;
        private Border redoButtonBorder;
        // Der Status, z.b. ist die Challenge beendet?
        private ChallengeModeState state;

        /// <summary>
        /// Erzeugt eine neue Instanz eines ChallengeModeScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt, einem Spielerknoten playerKnot und dem Knoten challengeKnot, den der Spieler nachbauen soll.
        /// </summary>
        public ChallengeModeScreen (GameCore game, Challenge challenge)
        : base (game)
        {
            // world
            PlayerWorld = new World (screen: this, drawOrder: DisplayLayer.GameWorld, bounds: Bounds.FromRight (percent: 0.5f));
            ChallengeWorld = new World (screen: this, drawOrder: DisplayLayer.GameWorld, bounds: Bounds.FromLeft (percent: 0.5f));
            ChallengeWorld.Camera = PlayerWorld.Camera;
            PlayerWorld.OnRedraw += () => ChallengeWorld.Redraw = true;
            ChallengeWorld.OnRedraw += () => PlayerWorld.Redraw = true;
            // input
            playerKnotInput = new KnotInputHandler (screen: this, world: PlayerWorld);
            challengeKnotInput = new KnotInputHandler (screen: this, world: ChallengeWorld);
            // overlay
            overlay = new Overlay (screen: this, world: PlayerWorld);
            // pointer
            pointer = new MousePointer (screen: this);
            // model mouse handler
            playerModelMouseHandler = new ModelMouseHandler (screen: this, world: PlayerWorld);
            challengeModelMouseHandler = new ModelMouseHandler (screen: this, world: ChallengeWorld);

            // knot renderer
            PlayerKnotRenderer = new KnotRenderer (screen: this, position: Vector3.Zero);
            PlayerWorld.Add (PlayerKnotRenderer);
            ChallengeKnotRenderer = new KnotRenderer (screen: this, position: Vector3.Zero);
            ChallengeWorld.Add (ChallengeKnotRenderer);

            // debug displays
            debugBoundings = new DebugBoundings (screen: this, position: Vector3.Zero);

            // edge movements
            PlayerEdgeMovement = new EdgeMovement (screen: this, world: PlayerWorld, knotRenderer: PlayerKnotRenderer, position: Vector3.Zero);
            PlayerEdgeMovement.KnotMoved = OnKnotMoved;

            // assign the specified challenge
            Challenge = challenge;
            // assign the specified player knot
            PlayerKnot = challenge.Start.Clone () as Knot;
            // assign the specified target knot
            ChallengeKnotRenderer.RenderKnot (challenge.Target);
            // assign the specified start knot
            PlayerKnotRenderer.RenderKnot (PlayerKnot);

            SkyCube playerSkyCube = new SkyCube (screen: this, position: Vector3.Zero, distance: 10000);
            PlayerWorld.Add (playerSkyCube);
            SkyCube challengeSkyCube = new SkyCube (screen: this, position: Vector3.Zero, distance: 10000);
            ChallengeWorld.Add (challengeSkyCube);

            // Die Spielzeit-Anzeige
            playTimeDisplay = new TextItem (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem, text: String.Empty);
            playTimeDisplay.Bounds.Position = new ScreenPoint (this, 0.800f, 0.01f);
            playTimeDisplay.Bounds.Size = new ScreenPoint (this, 0.15f, 0.04f);
            playTimeDisplay.BackgroundColorFunc = (s) => Design.WidgetBackground;
            playTimeDisplay.ForegroundColorFunc = (s) => Design.WidgetForeground;
            playTimeDisplay.AlignX = HorizontalAlignment.Center;
            playTimeBorder = new Border (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                                         widget: playTimeDisplay, lineWidth: 2, padding: 0);
            //Undo-Button
            undoButton = new Button (screen: this,
                                     drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                                     name: "Undo",
                                     onClick: (time) => OnUndo ());
            undoButton.SetCoordinates (left: 0.55f, top: 0.900f, right: 0.65f, bottom: 0.95f);

            undoButtonBorder = new Border (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                                           widget: undoButton, lineWidth: 2, padding: 0);
            undoButton.AlignX = HorizontalAlignment.Center;
            undoButton.IsVisible = false;

            // Redo-Button
            redoButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Redo",
                onClick: (time) => OnRedo ()
            );
            redoButton.SetCoordinates (left: 0.70f, top: 0.900f, right: 0.80f, bottom: 0.95f);

            redoButtonBorder = new Border (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                                           widget: redoButton, lineWidth: 2, padding: 0);
            redoButton.AlignX = HorizontalAlignment.Center;
            redoButton.IsVisible = false;

            // die Linien
            lines = new Lines (screen: this, drawOrder: DisplayLayer.Dialog, lineWidth: 2);
            lines.AddPoints (0.500f, 0.000f, 0.500f, 1.000f);

            // Status
            state = ChallengeModeState.Start;
        }

        private void OnKnotMoved (Knot newKnot)
        {
            OnEdgesChanged ();
            _playerKnot = newKnot;
            registerCurrentKnot ();
        }

        private void OnEdgesChanged ()
        {
            Knot push = _playerKnot.Clone ()as Knot;
            Undo.Push (push);
            Redo.Clear ();
            redoButton.IsVisible = false;
            undoButton.IsVisible = true;

            // Status
            if (state == ChallengeModeState.Start) {
                state = ChallengeModeState.Running;
            }
        }

        private void OnUndo ()
        {
            if (Undo.Count >= 2) {
                Knot current = Undo.Pop ();
                Knot prev = Undo.Peek ();
                Knot previous = prev.Clone () as Knot;
                Knot curr = current.Clone () as Knot;
                Redo.Push (curr);
                _playerKnot = previous;
                registerCurrentKnot ();
                _playerKnot.EdgesChanged += OnEdgesChanged;
                redoButton.IsVisible = true;
            }
            if (Undo.Count == 1) {
                undoButton.IsVisible = false;
            }
        }

        private void OnRedo ()
        {
            if (Redo.Count >= 1) {
                Knot next = Redo.Pop ();
                Knot push = next.Clone () as Knot;
                //Undo.Push (push);
                Undo.Push (push);
                _playerKnot = next;
                _playerKnot.EdgesChanged += OnEdgesChanged;
                // den Knoten den Inputhandlern und Renderern zuweisen
                registerCurrentKnot ();

                undoButton.IsVisible = true;
            }
            if (Redo.Count == 0) {
                redoButton.IsVisible = false;
            }
        }

        private void registerCurrentKnot ()
        {
            // den Knoten dem KnotRenderer zuweisen
            PlayerKnotRenderer.RenderKnot (_playerKnot);
            // den Knoten dem Kantenverschieber zuweisen
            PlayerEdgeMovement.Knot = _playerKnot;
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            // während die Challenge läuft...
            if (state == ChallengeModeState.Running || state == ChallengeModeState.Start) {
                ChallengeModeState oldState = state;
                // wenn zur Zeit kein Dialog vorhanden ist, und Escape gedrückt wurde...
                if (InputManager.KeyPressed (Keys.Escape) && !returnFromPause) {
                    // erstelle einen neuen Pausedialog
                    playerKnotInput.IsEnabled = false;
                    challengeKnotInput.IsEnabled = false;
                    Dialog pauseDialog = new ChallengePauseDialog (screen: this, drawOrder: DisplayLayer.Dialog);
                    // pausiere die Zeitmessung
                    state = ChallengeModeState.Paused;
                    // wenn der Dialog geschlossen wird, starte die Zeitmessung wieder
                    pauseDialog.Close += (t) => {
                        state = oldState;
                        playerKnotInput.IsEnabled = true;
                        challengeKnotInput.IsEnabled = true;
                        returnFromPause = true;
                    };
                    // füge ihn zur Spielkomponentenliste hinzu
                    AddGameComponents (time, pauseDialog);
                }
                returnFromPause = false;
            }

            // während die Challenge läuft...
            if (state == ChallengeModeState.Running) {
                // vergleiche den Spielerknoten mit dem Zielknoten
                if (PlayerKnot.Equals (Challenge.Target)) {
                    Log.Debug ("Playerknot equals Target!");
                    state = ChallengeModeState.Finished;
                    OnChallengeFinished (time);
                }

                // die Zeit, die der Spieler zum Spielen der Challenge braucht
                playTime += time.ElapsedGameTime;
                // zeige die Zeit an
                playTimeDisplay.Text = (playTime.Hours * 60 + playTime.Minutes).ToString ("D2") + ":" + playTime.Seconds.ToString ("D2");
            }
        }

        public void OnChallengeFinished (GameTime time)
        {
            playerKnotInput.IsEnabled = false;
            challengeKnotInput.IsEnabled = false;
            // erstelle einen Dialog zum Eingeben des Spielernamens
            TextInputDialog nameDialog = new TextInputDialog (screen: this, drawOrder: DisplayLayer.Dialog,
                    title: "Challenge", text: "Your name:",
                    inputText: Config.Default ["profile", "name", String.Empty]);
            // füge ihn zur Spielkomponentenliste hinzu
            nameDialog.NoCloseEmpty = true;
            nameDialog.NoWhiteSpace = true;
            nameDialog.Text = "Press Enter to submit your name. ";

            AddGameComponents (time, nameDialog);

            Action<GameTime> openHighscoreDialog = (t) => {
                // erstelle einen Highscoredialog
                Dialog highscoreDialog = new HighscoreDialog (screen: this, drawOrder: DisplayLayer.Dialog, challenge: Challenge);
                // füge ihn zur Spielkomponentenliste hinzu
                AddGameComponents (time, highscoreDialog);
            };

            // wenn der Dialog geschlossen wird...
            nameDialog.Submit += (t) => {
                Challenge.AddToHighscore (name: nameDialog.InputText, time: (int)playTime.TotalSeconds);
                openHighscoreDialog (t);
            };
            nameDialog.Cancel += (t) => {
                openHighscoreDialog (t);
            };

            Undo.Clear ();
            Redo.Clear ();

            RemoveGameComponents (time, undoButton);
            RemoveGameComponents (time, undoButtonBorder);
            RemoveGameComponents (time, redoButton);
            RemoveGameComponents (time, redoButtonBorder);
        }

        /// <summary>
        /// Fügt die 3D-Welten und den Inputhandler in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, playerKnotInput, challengeKnotInput, overlay, pointer,
                               ChallengeWorld, PlayerWorld, playerModelMouseHandler, challengeModelMouseHandler,
                               PlayerEdgeMovement,
                               lines, playTimeDisplay, playTimeBorder,
                               undoButton, undoButtonBorder, redoButton, redoButtonBorder);

            AudioManager.BackgroundMusic = Knot3Sound.ChallengeMusic;

            // Einstellungen anwenden
            debugBoundings.IsVisible = Config.Default ["debug", "show-boundings", false];
        }

        enum ChallengeModeState {
            Start,
            Running,
            Finished,
            Paused
        }
    }
}
