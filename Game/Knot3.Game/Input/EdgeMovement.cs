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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;

using Knot3.Framework.Audio;
using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

using Knot3.Game.Audio;
using Knot3.Game.Data;
using Knot3.Game.Models;

namespace Knot3.Game.Input
{
    /// <summary>
    /// Ein Inputhandler, der für das Verschieben der Kanten zuständig ist.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class EdgeMovement : IGameObject, IEnumerable<IGameObject>
    {
        private IScreen Screen;

        /// <summary>
        /// Enthält Informationen über die Position des Knotens.
        /// </summary>
        public GameObjectInfo Info { get; private set; }

        /// <summary>
        /// Der Knoten, dessen Kanten verschoben werden können.
        /// </summary>
        public Knot Knot { get; set; }

        public Action<Knot> KnotMoved = (k) => {};

        /// <summary>
        /// Die Spielwelt, in der sich die 3D-Modelle der Kanten befinden.
        /// </summary>
        public World World { get; set; }

        private Vector3 previousMousePosition = Vector3.Zero;
        private List<ShadowModel> shadowModels;
        private KnotRenderer knotRenderer;
        private Dictionary<Vector3, Knot> knotCache = new Dictionary<Vector3, Knot> ();

        /// <summary>
        /// Erzeugt eine neue Instanz eines EdgeMovement-Objekts und initialisiert diese
        /// mit ihrem zugehörigen IGameScreen-Objekt screen, der Spielwelt world und
        /// Objektinformationen info.
        /// </summary>
        public EdgeMovement (IScreen screen, World world, KnotRenderer knotRenderer, Vector3 position)
        {
            Screen = screen;
            World = world;
            this.knotRenderer = knotRenderer;
            Info = new GameObjectInfo (position: position);
            shadowModels = new List<ShadowModel> ();
        }

        /// <summary>
        /// Gibt den Ursprung des Knotens zurück.
        /// </summary>
        public Vector3 Center ()
        {
            return Info.Position;
        }

        /// <summary>
        /// Gibt immer \glqq null\grqq~zurück.
        /// </summary>
        public GameObjectDistance Intersects (Ray Ray)
        {
            return null;
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public void Update (GameTime time)
        {
            SelectEdges (time);
            MoveEdges (time);
        }

        /// <summary>
        /// Führt die Auswahl von Kanten mit Linksklick und evtl. Shift/Ctrl aus.
        /// </summary>
        private void SelectEdges (GameTime time)
        {
            // Überprüfe, ob das Objekt über dem die Maus liegt, eine Pipe ist
            if (World.SelectedObject is PipeModel) {
                PipeModel pipe = World.SelectedObject as PipeModel;

                // Bei einem Linksklick...
                if (Screen.InputManager.LeftMouseButton == ClickState.SingleClick) {
                    // Zeichne im nächsten Frame auf jeden Fall neu
                    World.Redraw = true;

                    try {
                        Edge selectedEdge = pipe.Info.Edge;
                        Log.Debug ("knot.Count () = ", Knot.Count ());

                        // Ctrl gedrückt
                        if (Screen.InputManager.KeyHeldDown (KnotInputHandler.LookupKey (Knot3PlayerAction.AddToEdgeSelection))) {
                            Knot.AddToSelection (selectedEdge);
                        }
                        // Shift gedrückt
                        else if (Screen.InputManager.KeyHeldDown (KnotInputHandler.LookupKey (Knot3PlayerAction.AddRangeToEdgeSelection))) {
                            Knot.AddRangeToSelection (selectedEdge);
                        }
                        // keine Taste gedrückt
                        else {
                            Knot.ClearSelection ();
                            Knot.AddToSelection (selectedEdge);
                        }
                    }
                    catch (ArgumentOutOfRangeException exp) {
                        Log.Debug (exp);
                    }
                }
            }

            // Wenn das selektierte Objekt weder Kante noch Pfeil ist...
            else if (!(World.SelectedObject is ArrowModel)) {
                // dann leert ein Linksklick die Selektion
                if (Screen.InputManager.LeftMouseButton == ClickState.SingleClick) {
                    Knot.ClearSelection ();
                }
            }
        }

        /// <summary>
        /// Führt das Verschieben der Kanten aus.
        /// </summary>
        private void MoveEdges (GameTime time)
        {
            // Wenn die Maus über einer Kante oder einem Pfeil liegt
            if (World.SelectedObject is PipeModel || World.SelectedObject is ArrowModel) {
                GameModel selectedModel = World.SelectedObject as GameModel;

                // Berechne die Mausposition in 3D
                Vector3 currentMousePosition = World.Camera.To3D (
                                                   position: Screen.InputManager.CurrentMousePosition,
                                                   nearTo: selectedModel.Center ()
                                               );

                // Wenn die Maus gedrückt gehalten ist und wir mitten im Ziehen der Kante
                // an die neue Position sind
                if (Screen.InputManager.CurrentInputAction == InputAction.SelectedObjectShadowMove) {
                    // Wenn dies der erste Frame ist...
                    if (previousMousePosition == Vector3.Zero) {
                        previousMousePosition = currentMousePosition;
                        // dann erstelle die Shadowobjekte und fülle die Liste
                        CreateShadowModels ();
                    }

                    // Setze die Positionen der Shadowobjekte abhängig von der Mausposition
                    if (selectedModel is ArrowModel) {
                        // Wenn ein Pfeil selektiert wurde, ist die Verschiebe-Richtung eindeutig festgelegt
                        UpdateShadowPipes (currentMousePosition, (selectedModel as ArrowModel).Info.Direction, time);
                    }
                    else {
                        // Wenn etwas anderes (eine Kante) selektiert wurde,
                        // muss die Verschiebe-Richtung berechnet werden
                        UpdateShadowPipes (currentMousePosition, time);
                    }

                    // Zeichne im nächsten Frame auf jeden Fall neu
                    World.Redraw = true;
                }

                // Wenn die Verschiebe-Aktion beendet ist (wenn die Maus losgelassen wurde)
                else if (Screen.InputManager.CurrentInputAction == InputAction.SelectedObjectMove) {
                    // Führe die finale Verschiebung durch
                    if (selectedModel is ArrowModel) {
                        // Wenn ein Pfeil selektiert wurde, ist die Verschiebe-Richtung eindeutig festgelegt
                        MovePipes (currentMousePosition, (selectedModel as ArrowModel).Info.Direction, time);
                    }
                    else {
                        // Wenn etwas anderes (eine Kante) selektiert wurde,
                        // muss die Verschiebe-Richtung berechnet werden
                        MovePipes (currentMousePosition, time);
                    }
                    DestroyShadowModels ();
                    // Zeichne im nächsten Frame auf jeden Fall neu
                    World.Redraw = true;
                }

                // Keine Verschiebeaktion
                else {
                    previousMousePosition = Vector3.Zero;

                    // Wenn die Shadowobjekt-Liste nicht leer ist...
                    if (shadowModels.Count > 0) {
                        // dann leere die Liste
                        DestroyShadowModels ();
                        // Zeichne im nächsten Frame auf jeden Fall neu
                        World.Redraw = true;
                    }
                }
            }
        }

        /// <summary>
        /// Bestimme die Richtung und die Länge in Rasterpunkt-Einheiten
        /// und verschiebe die ausgewählten Kanten.
        /// </summary>
        private void MovePipes (Vector3 currentMousePosition, Direction direction, GameTime time)
        {
            int distance = (int)Math.Round (ComputeLength (currentMousePosition));
            if (distance > 0) {
                try {
                    Knot newKnot;
                    Knot.TryMove (direction, distance, out newKnot);

                    if (newKnot != null) {
                        KnotMoved (newKnot);
                        Screen.AudioManager.PlaySound (Knot3Sound.PipeMoveSound);
                    }
                    else {
                        KnotMoved (Knot);
                        Screen.AudioManager.PlaySound (Knot3Sound.PipeInvalidMoveSound);
                    }
                    previousMousePosition = currentMousePosition;
                }
                catch (ArgumentOutOfRangeException exp) {
                    Log.Debug (exp);
                }
            }
            knotCache.Clear ();
        }

        private void MovePipes (Vector3 currentMousePosition, GameTime time)
        {
            Direction direction = ComputeDirection (currentMousePosition);
            MovePipes (currentMousePosition, direction, time);
        }

        /// <summary>
        /// Berechne aus der angegebenen aktuellen Mausposition
        /// die hauptsächliche Richtung und die Länge in Rasterpunkt-Einheiten.
        /// </summary>
        private Direction ComputeDirection (Vector3 currentMousePosition)
        {
            Vector3 mouseMove = currentMousePosition - previousMousePosition;
            return mouseMove.PrimaryDirection ().ToDirection ();
        }

        /// <summary>
        /// Berechne aus der angegebenen aktuellen Mausposition
        /// die hauptsächliche Richtung und die gerundete Länge in Rasterpunkt-Einheiten.
        /// </summary>
        private float ComputeLength (Vector3 currentMousePosition)
        {
            Vector3 mouseMove = currentMousePosition - previousMousePosition;
            return mouseMove.Length () / Node.Scale;
        }

        /// <summary>
        /// Erstellt für die selektierten Kantenmodelle und die Pfeile jeweils Shadowobjekte.
        /// </summary>
        private void CreateShadowModels ()
        {
            DestroyShadowModels ();
            foreach (PipeModel pipe in World.OfType<PipeModel>()) {
                if (Knot.SelectedEdges.Contains (pipe.Info.Edge)) {
                    pipe.Info.IsVisible = false;
                    shadowModels.Add (new ShadowModel (Screen, pipe));
                }
            }
            foreach (ArrowModel arrow in World.OfType<ArrowModel>()) {
                arrow.Info.IsVisible = false;
                shadowModels.Add (new ShadowModel (Screen, arrow));
            }
        }

        /// <summary>
        /// Entfernt alle Shadowobjekte.
        /// </summary>
        private void DestroyShadowModels ()
        {
            shadowModels.Clear ();
            foreach (PipeModel pipe in World.OfType<PipeModel>()) {
                pipe.Info.IsVisible = true;
            }
            foreach (ArrowModel arrow in World.OfType<ArrowModel>()) {
                arrow.Info.IsVisible = true;
            }
        }

        /// <summary>
        /// Setze die Position der Shadowobjekte der selektierten Kantenmodelle
        /// auf die von der aktuellen Mausposition abhängende Position.
        /// </summary>
        private void UpdateShadowPipes (Vector3 currentMousePosition, Direction direction, float count, GameTime time)
        {
            if (Config.Default ["video", "auto-camera-move", true]) {
                ScreenPoint currentPosition = Screen.InputManager.CurrentMousePosition;
                Bounds worldBounds = World.Bounds;
                var bounds = new[] {
                    new { Bounds = worldBounds.FromLeft (0.1f), Side = new Vector2 (-1, 0) },
                    new { Bounds = worldBounds.FromRight (0.1f), Side = new Vector2 (1, 0) },
                    new { Bounds = worldBounds.FromTop (0.1f), Side = new Vector2 (0, 1) },
                    new { Bounds = worldBounds.FromBottom (0.1f), Side = new Vector2 (0, -1) }
                };
                Vector2[] sides = bounds.Where (x => x.Bounds.Contains (currentPosition)).Select (x => x.Side).ToArray ();

                if (sides.Length == 1) {
                    InputAction action = Screen.InputManager.CurrentInputAction;
                    World.Camera.Position += direction * 2.5f;
                    World.Camera.Target += direction * 2.5f;
                    //KnotInput.MoveCameraAndTarget (new Vector3 (sides [0].X, sides [0].Y, 0) * 0.5f, time);
                    Screen.InputManager.CurrentInputAction = action;
                    World.Redraw = true;
                }
            }

            if (Knot.IsValidDirection (direction)) {
                int distance = (int)Math.Round (count);
                Knot shadowKnot;
                if (knotCache.ContainsKey (direction * distance)) {
                    shadowKnot = knotCache [direction * distance];
                }
                else {
                    Knot.TryMove (direction, distance, out shadowKnot);
                    knotCache [direction * distance] = shadowKnot;
                }

                if (shadowKnot != null) {
                    knotRenderer.VirtualKnot = shadowKnot;
                }

                foreach (ShadowModel shadowObj in shadowModels) {
                    shadowObj.ShadowPosition = shadowObj.OriginalPosition + direction * count * Node.Scale;
                    shadowObj.ShadowAlpha = 1f;
                    shadowObj.ShadowColor = Color.White;
                }
            }
        }

        private void UpdateShadowPipes (Vector3 currentMousePosition, Direction direction, GameTime time)
        {
            //Log.Debug ("XXX: ", direction);
            float count = ComputeLength (currentMousePosition);
            UpdateShadowPipes (currentMousePosition, direction, count, time);
        }

        private void UpdateShadowPipes (Vector3 currentMousePosition, GameTime time)
        {
            float count = ComputeLength (currentMousePosition);
            Direction direction = ComputeDirection (currentMousePosition);
            UpdateShadowPipes (currentMousePosition, direction, count, time);
        }

        /// <summary>
        /// Gibt einen Enumerator über die während einer Verschiebeaktion dynamisch erstellten 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            foreach (IGameObject shadowObj in shadowModels) {
                yield return shadowObj;
            }
        }

        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }

        /// <summary>
        /// Zeichnet die während einer Verschiebeaktion dynamisch erstellten 3D-Modelle.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public void Draw (GameTime time)
        {
            foreach (IGameObject shadowObj in shadowModels) {
                shadowObj.Draw (time);
            }
        }
    }
}
