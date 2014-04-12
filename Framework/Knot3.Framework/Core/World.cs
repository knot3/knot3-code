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
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Effects;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Core
{
    /// <summary>
    /// Repräsentiert eine Spielwelt, in der sich 3D-Modelle befinden und gezeichnet werden können.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class World : DrawableScreenComponent, IEnumerable<IGameObject>
    {
        /// <summary>
        /// Die Kamera dieser Spielwelt.
        /// </summary>
        public Camera Camera
        {
            get {
                return _camera;
            }
            set {
                _camera = value;
                useInternalCamera = false;
            }
        }

        private Camera _camera;

        /// <summary>
        /// Gibt an, ob die interne Kamera, die im Konstruktor erstellt wurde, genutzt werden soll oder ob eine
        /// externe Kamera zugewiesen wurde, die stattdessen verwendet werden soll.
        /// </summary>
        private bool useInternalCamera = true;

        /// <summary>
        /// Die Liste von Spielobjekten.
        /// </summary>
        public HashSet<IGameObject> Objects { get; set; }

        private IGameObject _selectedObject;

        /// <summary>
        /// Das aktuell ausgewählte Spielobjekt. "Ausgewählt" bedeutet hier, dass sich die Maus über den Objekt befindet.
        /// </summary>
        public IGameObject SelectedObject
        {
            get {
                return _selectedObject;
            }
            set {
                if (_selectedObject != value) {
                    _selectedObject = value;
                    SelectionChanged (_selectedObject);
                    Redraw = true;
                }
            }
        }

        /// <summary>
        /// Der Abstand von der aktuellen Kameraposition und dem ausgewählten Spielobjekt.
        /// Gibt 0 zurück, wenn kein Spielobjekt ausgewählt ist.
        /// </summary>
        public float SelectedObjectDistance
        {
            get {
                if (SelectedObject != null) {
                    Vector3 toTarget = SelectedObject.Center - Camera.Position;
                    return toTarget.Length ();
                }
                else {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Der aktuell angewendete Rendereffekt.
        /// </summary>
        public IRenderEffect CurrentEffect { get; set; }

        /// <summary>
        /// Gibt an, ob der aktuell angewendete Rendereffekt dispost werden soll.
        /// </summary>
        private bool disposeEffect = false;

        /// <summary>
        /// Wird ausgelöst, wenn das selektierte Spielobjekt geändert wurde.
        /// </summary>
        public Action<IGameObject> SelectionChanged = (o) => {};

        /// <summary>
        /// Gibt an, ob die Spielwelt im folgenden Frame neugezeichnet wird
        /// oder nur der letzte Frame wiedergegeben wird.
        /// </summary>
        public bool Redraw { get; set; }

        /// <summary>
        /// Wird ausgelöst, wenn die Spielwelt neu gezeichnet wird.
        /// </summary>
        public Action OnRedraw = () => {};

        /// <summary>
        /// Die Ausmaße der Welt auf dem Screen.
        /// </summary>
        public Bounds Bounds { get; private set; }

        /// <summary>
        /// Erstellt eine neue Spielwelt im angegebenen Spielzustand, dem angegebenen Rendereffekt und dem
        /// angegebenen Bounds-Objekt.
        /// </summary>
        public World (IScreen screen, DisplayLayer drawOrder, IRenderEffect effect, Bounds bounds)
        : base (screen, drawOrder)
        {
            // die Kamera für diese Spielwelt
            _camera = new Camera (screen, this);

            // die Liste der Spielobjekte
            Objects = new HashSet<IGameObject> ();

            // der Rendereffekt
            CurrentEffect = effect;

            // die relative Standard-Position und Größe
            Bounds = bounds;

            if (Screen.Game != null) {
                Screen.Game.FullScreenChanged += () => viewportCache.Clear ();
            }
        }

        /// <summary>
        /// Erstellt eine neue Spielwelt im angegebenen Spielzustand und dem
        /// angegebenen Bounds-Objekt.
        /// Als Rendereffekt wird der in der Konfigurationsdatei festgelegte Standardeffekt festgelegt.
        /// Falls während der Existenz dieses Objektes der Standardeffekt geändert wird,
        /// wird der neue Effekt übernommen.
        /// </summary>
        public World (IScreen screen, DisplayLayer drawOrder, Bounds bounds)
        : this (screen: screen, drawOrder: drawOrder, effect: null, bounds: bounds)
        {
        }

        /// <summary>
        /// Liest den Standard-Effekt aus der Konfigurationsdatei aus und liefert ein entsprechendes RenderEffekt-Objekt zurück.
        /// </summary>
        private static IRenderEffect DefaultEffect (IScreen screen)
        {
            // suche den eingestellten Standardeffekt heraus
            string effectName = Config.Default ["video", "current-world-effect", "default"];
            IRenderEffect effect = RenderEffectLibrary.CreateEffect (screen: screen, name: effectName);
            return effect;
        }

        /// <summary>
        /// Lade die Inhalte der Spielwelt. Falls kein Rendereffekt im Konstruktor übergeben wurde,
        /// wird der Standard-Effekt als zu verwendender Rendereffekt zugewiesen.
        /// </summary>
        public override void LoadContent (GameTime time)
        {
            if (CurrentEffect == null) {
                CurrentEffect = DefaultEffect (screen: Screen);

                disposeEffect = true;
                RenderEffectLibrary.RenderEffectChanged += (newEffectName, time2) => {
                    if (CurrentEffect != null) {
                        CurrentEffect.Dispose ();
                    }
                    CurrentEffect = RenderEffectLibrary.CreateEffect (screen: Screen, name: newEffectName);
                };
            }
        }

        /// <summary>
        /// Entlade die Inhalte. Falls das aktuell verwendete Rendereffekt-Objekt in dieser Klasse erstellt werde, dispose es auch hier.
        /// </summary>
        public override void UnloadContent (GameTime time)
        {
            if (disposeEffect && CurrentEffect != null) {
                CurrentEffect.Dispose ();
                CurrentEffect = null;
            }
        }

        /// <summary>
        /// Füge ein Spielobjekt zur Spielwelt hinzu.
        /// </summary>
        public void Add (IGameObject obj)
        {
            if (obj != null) {
                Objects.Add (obj);
                obj.World = this;
            }
            Redraw = true;
        }

        /// <summary>
        /// Entferne ein Spielobjekt aus der Spielwelt.
        /// </summary>
        public void Remove (IGameObject obj)
        {
            if (obj != null) {
                Objects.Remove (obj);
                obj.World = null;
            }
            Redraw = true;
        }

        /// <summary>
        /// Ruft auf allen Spielobjekten die Update ()-Methode auf.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            if (!Config.Default ["video", "selectiveRendering", false]) {
                Redraw = true;
            }
            if (!Screen.PostProcessingEffect.SelectiveRendering) {
                Redraw = true;
            }

            // run the update method on all game objects
            foreach (IGameObject obj in Objects) {
                obj.Update (time);
            }
        }

        private Dictionary<Point, Dictionary<Vector4, Viewport>> viewportCache = new Dictionary<Point, Dictionary<Vector4, Viewport>> ();

        /// <summary>
        /// Gibt den aktuellen Viewport zurück.
        /// </summary>
        public Viewport Viewport
        {
            get {
                // when we have a graphics device
                if (Screen.GraphicsDevice != null) {
                    PresentationParameters pp = Screen.GraphicsDevice.PresentationParameters;
                    Point resolution = new Point (pp.BackBufferWidth, pp.BackBufferHeight);
                    Vector4 key = Bounds.Vector4;
                    if (!viewportCache.ContainsKey (resolution)) {
                        viewportCache [resolution] = new Dictionary<Vector4, Viewport> ();
                    }
                    if (!viewportCache [resolution].ContainsKey (key)) {
                        Rectangle subScreen = Bounds.Rectangle;
                        viewportCache [resolution] [key] = new Viewport (subScreen.X, subScreen.Y, subScreen.Width, subScreen.Height) {
                            MinDepth = 0,
                            MaxDepth = 1
                        };
                    }
                    return viewportCache [resolution] [key];
                }
                // for unit tests
                else {
                    return Screen.Viewport;
                }
            }
        }

        /// <summary>
        /// Ruft auf allen Spielobjekten die Draw ()-Methode auf.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            if (Redraw) {
                OnRedraw ();
                Redraw = false;

                //Screen.BackgroundColor = CurrentEffect is CelShadingEffect ? Color.CornflowerBlue : _Color.Black;

                // begin the knot render effect
                CurrentEffect.Begin (time);

                foreach (IGameObject obj in Objects) {
                    obj.World = this;
                    obj.Draw (time);
                }

                // end of the knot render effect
                CurrentEffect.End (time);
            }
            else {
                CurrentEffect.DrawLastFrame (time);
            }
        }

        /// <summary>
        /// Liefert einen Enumerator über die Spielobjekte dieser Spielwelt.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            foreach (IGameObject obj in flat (Objects)) {
                yield return obj;
            }
        }

        private IEnumerable<IGameObject> flat (IEnumerable<IGameObject> enumerable)
        {
            foreach (IGameObject obj in enumerable) {
                if (obj is IEnumerable<IGameObject>) {
                    foreach (IGameObject subobj in flat (obj as IEnumerable<IGameObject>)) {
                        yield return subobj;
                    }
                }
                else {
                    yield return obj;
                }
            }
        }
        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            if (useInternalCamera) {
                yield return Camera;
            }
        }

        /// <summary>
        /// Gibt einen Iterator über alle Spielobjekte zurück, der so sortiert ist, dass die
        /// Spielobjekte, die der angegebenen 2D-Position am nächsten sind, am Anfang stehen.
        /// Dazu wird die 2D-Position in eine 3D-Position konvertiert.
        /// </summary>
        public IEnumerable<IGameObject> FindNearestObjects (ScreenPoint nearTo)
        {
            Dictionary<float, IGameObject> distances = new Dictionary<float, IGameObject> ();
            foreach (IGameObject obj in this) {
                if (obj.IsSelectable) {
                    // Berechne aus der angegebenen 2D-Position eine 3D-Position
                    Vector3 position3D = Camera.To3D (position: nearTo, nearTo: obj.Center);
                    // Berechne die Distanz zwischen 3D-Mausposition und dem Spielobjekt
                    float distance = System.Math.Abs ((position3D - obj.Center).Length ());
                    distances [distance] = obj;
                }
            }
            if (distances.Count > 0) {
                IEnumerable<float> sorted = distances.Keys.OrderBy (k => k);
                foreach (float where in sorted) {
                    yield return distances [where];
                    // Log.Debug ("where=", where, " = ", distances [where].Center ());
                }
            }
            else {
                yield break;
            }
        }

        /// <summary>
        /// Gibt einen Iterator über alle Spielobjekte zurück, der so sortiert ist, dass die
        /// Spielobjekte, die der angegebenen 3D-Position am nächsten sind, am Anfang stehen.
        /// </summary>
        public IEnumerable<IGameObject> FindNearestObjects (Vector3 nearTo)
        {
            Dictionary<float, IGameObject> distances = new Dictionary<float, IGameObject> ();
            foreach (IGameObject obj in this) {
                if (obj.IsSelectable) {
                    // Berechne die Distanz zwischen 3D-Mausposition und dem Spielobjekt
                    float distance = System.Math.Abs ((nearTo - obj.Center).Length ());
                    distances [distance] = obj;
                }
            }
            if (distances.Count > 0) {
                IEnumerable<float> sorted = distances.Keys.OrderBy (k => k);
                foreach (float where in sorted) {
                    yield return distances [where];
                }
            }
            else {
                yield break;
            }
        }
    }
}
