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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Development;
using Knot3.Framework.Input;
using Knot3.Framework.Models;
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
    /// Der Spielzustand, der während dem Erstellen und Bearbeiten eines Knotens aktiv ist und für den Knoten eine 3D-Welt zeichnet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public class CreativeModeScreen : Screen
    {
        /// <summary>
        /// Die Spielwelt in der die 3D-Objekte des dargestellten Knotens enthalten sind.
        /// </summary>
        private World world;
        /// <summary>
        /// Der Controller, der aus dem Knoten die 3D-Modelle erstellt.
        /// </summary>
        private KnotRenderer knotRenderer;

        /// <summary>
        /// Der Undo-Stack.
        /// </summary>
        public Stack<Knot> Undo { get; set; }

        /// <summary>
        /// Der Redo-Stack.
        /// </summary>
        public Stack<Knot> Redo { get; set; }

        /// <summary>
        /// Der Knoten, der vom Spieler bearbeitet wird.
        /// </summary>
        public Knot Knot
        {
            get {
                return knot;
            }
            set {
                knot = value;
                // Undo- und Redo-Stacks neu erstellen
                Redo = new Stack<Knot> ();
                Undo = new Stack<Knot> ();
                Undo.Push (knot.Clone () as Knot);
                // den Knoten den Inputhandlern und Renderern zuweisen
                registerCurrentKnot ();
                // die Events registrieren
                knot.EdgesChanged += OnEdgesChanged;
                knot.StartEdgeChanged += knotInput.OnStartEdgeChanged;
            }
        }

        private Knot knot;
        private KnotInputHandler knotInput;
        private ModelMouseHandler modelMouseHandler;
        private EdgeMovement edgeMovement;
        private EdgeColoring edgeColoring;
        private EdgeRectangles edgeRectangles;
        private MousePointer pointer;
        private Overlay overlay;
        private Button invisible;
        private DebugBoundings debugBoundings;
        // Undo-Button
        private Button undoButton;
        private Border undoButtonBorder;
        // Redo-Button
        private Button redoButton;
        private Border redoButtonBorder;

        /// <summary>
        /// Erzeugt eine neue Instanz eines CreativeModeScreen-Objekts und initialisiert diese mit einem Knot3Game-Objekt game, sowie einem Knoten knot.
        /// </summary>
        public CreativeModeScreen (GameCore game, Knot knot)
        : base (game)
        {
            // die Spielwelt
            world = new World (screen: this, drawOrder: DisplayLayer.GameWorld, bounds: Bounds);
            // der Input-Handler
            knotInput = new KnotInputHandler (screen: this, world: world);
            // das Overlay zum Debuggen
            overlay = new Overlay (screen: this, world: world);
            // der Mauszeiger
            pointer = new MousePointer (screen: this);
            // der Maus-Handler für die 3D-Modelle
            modelMouseHandler = new ModelMouseHandler (screen: this, world: world);

            // der Knoten-Renderer
            knotRenderer = new KnotRenderer (screen: this, position: Vector3.Zero);
            world.Add (knotRenderer);

            // visualisiert die BoundingSpheres
            debugBoundings = new DebugBoundings (screen: this, position: Vector3.Zero);
            world.Add (debugBoundings);

            // der Input-Handler zur Kanten-Verschiebung
            edgeMovement = new EdgeMovement (screen: this, world: world, knotRenderer : knotRenderer, position: Vector3.Zero);
            edgeMovement.KnotMoved = OnKnotMoved;

            // der Input-Handler zur Kanten-Einfärbung
            edgeColoring = new EdgeColoring (screen: this);

            // Flächen zwischen Kanten
            edgeRectangles = new EdgeRectangles (screen: this);

            // assign the specified knot
            Knot = knot;

            // Hintergrund
            Sky skyCube = new Sky (screen: this, position: Vector3.Zero);
            world.Add (skyCube);

            // Sonne
            Sun sun = new Sun (screen: this);
            world.Add (sun);

            // Undo-Button
            undoButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Undo",
                onClick: (time) => OnUndo ()
            );
            undoButton.SetCoordinates (left: 0.05f, top: 0.900f, right: 0.15f, bottom: 0.95f);
            undoButton.AlignX = HorizontalAlignment.Center;
            undoButton.IsVisible = false;

            undoButtonBorder = new Border (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                                           widget: undoButton, lineWidth: 2, padding: 0);

            // Redo-Button
            redoButton = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "Redo",
                onClick: (time) => OnRedo ()
            );
            redoButton.SetCoordinates (left: 0.20f, top: 0.900f, right: 0.30f, bottom: 0.95f);
            redoButton.AlignX = HorizontalAlignment.Center;
            redoButton.IsVisible = false;

            redoButtonBorder = new Border (screen: this, drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                                           widget: redoButton, lineWidth: 2, padding: 0);

            invisible = new Button (
                screen: this,
                drawOrder: DisplayLayer.ScreenUI + DisplayLayer.MenuItem,
                name: "menu",
            onClick: (time) => {
                // erstelle einen neuen Pausedialog
                knotInput.IsEnabled = false;
                Dialog pauseDialog = new CreativePauseDialog (screen: this, drawOrder: DisplayLayer.Dialog, knot: Knot);
                // füge ihn in die Spielkomponentenliste hinzu
                pauseDialog.Close += (t) => knotInput.IsEnabled = true;
                AddGameComponents (time, pauseDialog);
            }
            );
            invisible.SetCoordinates (left: 1.00f, top: 1.00f, right: 1.10f, bottom: 1.10f);
            invisible.IsVisible = true;
            invisible.AddKey (Keys.Escape);
        }
        /*
        public void Dispose ()
        {
            // Undo, Redo, knot ...
        }
        */
        private void OnKnotMoved (Knot newKnot)
        {
            if (!knot.Equals (newKnot)) {
                knot = newKnot;
                OnEdgesChanged ();
                Log.Debug ("=> set Knot #", knot.Count (), " = ", string.Join (", ", from c in knot select c.Direction));
            }
            registerCurrentKnot ();
        }

        private void OnEdgesChanged ()
        {
            Knot push = knot.Clone () as Knot;
            Undo.Push (push);
            Redo.Clear ();
            redoButton.IsVisible = false;
            undoButton.IsVisible = true;
        }

        private void OnUndo ()
        {
            Log.Debug ("Undo: Undo.Count=", Undo.Count);
            if (Undo.Count >= 2) {
                Knot current = Undo.Pop ();
                Knot prev = Undo.Peek ();
                Knot previous = prev.Clone () as Knot;
                Knot curr = current.Clone () as Knot;
                Redo.Push (curr);
                knot = previous;
                // den Knoten den Inputhandlern und Renderern zuweisen
                registerCurrentKnot ();
                knot.EdgesChanged += OnEdgesChanged;
                redoButton.IsVisible = true;
            }
            if (Undo.Count == 1) {
                undoButton.IsVisible = false;
            }
        }

        private void OnRedo ()
        {
            Log.Debug ("Redo: Redo.Count=", Redo.Count);
            if (Redo.Count >= 1) {
                Knot next = Redo.Pop ();
                Knot push = next.Clone ()as Knot;
                Undo.Push (push);
                knot = next;
                knot.EdgesChanged += OnEdgesChanged;
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
            knotRenderer.RenderKnot (knot);
            // den Knoten dem Kantenverschieber zuweisen
            edgeMovement.Knot = knot;
            // den Knoten dem Kanteneinfärber zuweisen
            edgeColoring.Knot = knot;
            // den Knoten dem Flächendings zuweisen
            edgeRectangles.Knot = knot;
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
        }

        /// <summary>
        /// Fügt die 3D-Welt und den Inputhandler in die Spielkomponentenliste ein.
        /// </summary>
        public override void Entered (IScreen previousScreen, GameTime time)
        {
            base.Entered (previousScreen, time);
            AddGameComponents (time, knotInput, overlay, pointer, world, modelMouseHandler,
                               edgeColoring, edgeRectangles, edgeMovement,
                               undoButton, undoButtonBorder, redoButton, redoButtonBorder, invisible);
            AudioManager.BackgroundMusic = Knot3Sound.CreativeMusic;

            // Einstellungen anwenden
            debugBoundings.IsVisible = Config.Default ["debug", "show-boundings", false];
        }
    }
}
