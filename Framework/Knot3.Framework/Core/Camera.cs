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
using System.Diagnostics.CodeAnalysis;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Development;
using Knot3.Framework.Math;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Core
{
    /// <summary>
    /// Jede Instanz der World-Klasse hält eine für diese Spielwelt verwendete Kamera als Attribut.
    /// Die Hauptfunktion der Kamera-Klasse ist das Berechnen der drei Matrizen, die für die Positionierung
    /// und Skalierung von 3D-Objekten in einer bestimmten Spielwelt benötigt werden, der View-, World- und Projection-Matrix.
    /// Um diese Matrizen zu berechnen, benötigt die Kamera unter Anderem Informationen über die aktuelle Kamera-Position,
    /// das aktuelle Kamera-Ziel und das Field of View.
    /// </summary>
    public sealed class Camera : ScreenComponent
    {
        private Vector3 _position;

        /// <summary>
        /// Die Position der Kamera.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set {
                if (_position != value) {
                    OnViewChanged ();
                    _position = value;
                }
            }
        }

        private Vector3 _target;

        /// <summary>
        /// Das Ziel der Kamera.
        /// </summary>
        public Vector3 Target
        {
            get { return _target; }
            set {
                if (_target != value) {
                    OnViewChanged ();
                    _target = value;
                }
            }
        }

        private float _foV;

        /// <summary>
        /// Das Sichtfeld.
        /// </summary>
        public float FoV
        {
            get { return _foV; }
            set {
                _foV = MathHelper.Clamp (value, 10, 70);
                OnViewChanged ();
            }
        }

        /// <summary>
        /// Die View-Matrix wird über die statische Methode CreateLookAt der Klasse Matrix des MonoGame-Frameworks
        /// mit Matrix.CreateLookAt (Position, Target, Vector3.Up) berechnet.
        /// </summary>
        public Matrix ViewMatrix { get; private set; }

        /// <summary>
        /// Die World-Matrix wird mit Matrix.CreateFromYawPitchRoll und den drei Rotationswinkeln berechnet.
        /// </summary>
        public Matrix WorldMatrix { get; private set; }

        /// <summary>
        /// Die Projektionsmatrix wird über die statische MonoGame-Methode Matrix.CreatePerspectiveFieldOfView berechnet.
        /// </summary>
        public Matrix ProjectionMatrix { get; private set; }

        /// <summary>
        /// Berechnet ein Bounding-Frustum, das benötigt wird, um festzustellen, ob ein 3D-Objekt sich im Blickfeld des Spielers befindet.
        /// </summary>
        public BoundingFrustum ViewFrustum { get; private set; }

        /// <summary>
        /// Eine Referenz auf die Spielwelt, für welche die Kamera zuständig ist.
        /// </summary>
        private World World { get; set; }

        /// <summary>
        /// Die Rotationswinkel.
        /// </summary>
        public Angles3 Rotation { get; set; }

        /// <summary>
        /// Der Vektor, der bestimmt, wo "oben" ist.
        /// </summary>
        public Vector3 UpVector { get; private set; }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Kamera-Parameter wie Kamera-Position oder Kamera-Target geändert haben.
        /// </summary>
        public Action OnViewChanged = () => {};

        /// <summary>
        /// Das Seitenverhältnis des Screens.
        /// </summary>
        private float aspectRatio;

        /// <summary>
        /// Die Near Plane.
        /// </summary>
        private float nearPlane;

        /// <summary>
        /// Die Far Plane.
        /// </summary>
        public float FarPlane { get; private set; }

        /// <summary>
        /// Die Standard-Kamera-Position
        /// </summary>
        private Vector3 defaultPosition = new Vector3 (400, 400, 700);

        /// <summary>
        /// Die aktuelle Position der Sonne.
        /// </summary>
        public Vector3 SunPosition { get; set; }

        /// <summary>
        /// Die aktuelle Richtung des Lichts, das von der Sonne ausgeht.
        /// </summary>
        public Vector3 LightDirection { get; private set; }

        /// <summary>
        /// Die Länge eines Tageszyklus in Sekunden.
        /// </summary>
        public float DayCycleSeconds
        {
            get { return Config.Default ["video", "day-cycle-seconds", 60]; }
            set { Config.Default ["video", "day-cycle-seconds", 60] = value; }
        }

        /// <summary>
        /// Erstellt eine neue Kamera in einem bestimmten IGameScreen für eine bestimmte Spielwelt.
        /// </summary>
        public Camera (IScreen screen, World world)
        : base (screen, DisplayLayer.None)
        {
            World = world;
            Position = defaultPosition;
            Target = Vector3.Zero;
            UpVector = Vector3.Up;
            Rotation = Angles3.Zero;

            FoV = 60;
            nearPlane = 0.5f;
            FarPlane = 15000.0f;

            UpdateMatrices (null);
        }

        /// <summary>
        /// Die Blickrichtung von der Kamera-Position zum Kamera-Ziel.
        /// </summary>
        public Vector3 PositionToTargetDirection
        {
            get {
                return Vector3.Normalize (Target - Position);
            }
        }

        /// <summary>
        /// Die Blickrichtung von der Kamera-Position zum Rotations-Ziel.
        /// </summary>
        public Vector3 PositionToArcballTargetDirection
        {
            get {
                return Vector3.Normalize (ArcballTarget - Position);
            }
        }

        /// <summary>
        /// Der Abstand zwischen der Kamera-Position und dem Kamera-Ziel.
        /// </summary>
        public float PositionToTargetDistance
        {
            get {
                return Position.DistanceTo (Target);
            }
            set {
                Position = Position.SetDistanceTo (Target, value);
            }
        }

        /// <summary>
        /// Der Abstand zwischen der Kamera-Position und dem Rotations-Ziel.
        /// </summary>
        public float PositionToArcballTargetDistance
        {
            get {
                return Position.DistanceTo (ArcballTarget);
            }
            set {
                Position = Position.SetDistanceTo (ArcballTarget, value);
            }
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            // Setze den Viewport auf den der aktuellen Spielwelt
            Viewport original = Screen.Viewport;
            Screen.Viewport = World.Viewport;

            UpdateSun (time);
            UpdateMatrices (time);
            UpdateSmoothMove (time);

            // Setze den Viewport wieder auf den ganzen Screen
            Screen.Viewport = original;
        }

        /// <summary>
        /// Aktualiert die Sonnen-Position und die Lichtrichtung.
        /// </summary>
        private void UpdateSun (GameTime time)
        {
            if (DayCycleSeconds > 0) {
                Vector3 sunStartPosition = Vector3.Up * 1000 + Vector3.Left * FarPlane / 4;
                Vector3 sunNormal = Vector3.Up; //Vector3.Cross (sunStartPosition, UpVector);
                SunPosition = sunStartPosition.RotateAroundVector (axis: sunNormal, angleRadians: (float)(time.TotalGameTime.TotalMilliseconds / (DayCycleSeconds * 1000f) * MathHelper.TwoPi));
            }
            LightDirection = Vector3.Normalize (-SunPosition);
        }

        /// <summary>
        /// Aktualisiert die Matrizen.
        /// </summary>
        private void UpdateMatrices (GameTime time)
        {
            aspectRatio = Screen.Viewport.AspectRatio;
            FarPlane = 500000 + MathHelper.Max (Position.Length (), Target.Length ()) * 3.6f;
            ViewMatrix = Matrix.CreateLookAt (Position, Target, UpVector);
            WorldMatrix = Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z);
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MathHelper.ToRadians (FoV), aspectRatio, nearPlane, FarPlane);
            ViewFrustum = new BoundingFrustum (ViewMatrix * ProjectionMatrix);
        }

        /// <summary>
        /// Berechnet einen Strahl für die angegebenene 2D-Mausposition.
        /// </summary>
        public Ray GetMouseRay (ScreenPoint mousePosition)
        {
            Viewport viewport = World.Viewport;

            Vector3 nearPoint = new Vector3 (mousePosition.AbsoluteVector, 0);
            Vector3 farPoint = new Vector3 (mousePosition.AbsoluteVector, 1);

            nearPoint = viewport.Unproject (nearPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);
            farPoint = viewport.Unproject (farPoint, ProjectionMatrix, ViewMatrix, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize ();

            return new Ray (nearPoint, direction);
        }

        /// <summary>
        /// Eine Position, um die rotiert werden soll, wenn der User die rechte Maustaste gedrückt hält und die Maus bewegt.
        /// </summary>
        public Vector3 ArcballTarget
        {
            get {
                if (World.SelectedObject != null) {
                    return World.SelectedObject.Center;
                }
                else {
                    return Vector3.Zero;
                }
            }
        }

        /// <summary>
        /// Setzt die Kamera auf die Standard-Position zurück.
        /// </summary>
        public void ResetCamera ()
        {
            Position = defaultPosition;
            Target = new Vector3 (0, 0, 0);
            Rotation = Angles3.Zero;
            FoV = 45;
        }

        private Vector3? smoothTarget = null;
        private float smoothDistance = 0f;
        private float smoothProgress = 0f;

        public void StartSmoothMove (Vector3 target, GameTime time)
        {
            if (!InSmoothMove) {
                smoothTarget = target;
                smoothDistance = System.Math.Abs (Target.DistanceTo (target));
                smoothProgress = 0f;
            }
        }

        public bool InSmoothMove { get { return smoothTarget.HasValue && smoothProgress <= 1f; } }

        private void UpdateSmoothMove (GameTime time)
        {
            if (InSmoothMove) {
                float distance = MathHelper.SmoothStep (0, smoothDistance, smoothProgress);

                smoothProgress += 0.05f;

                //Log.Debug ("distance = ", distance);
                Target = Target.SetDistanceTo (target: smoothTarget.Value, distance: System.Math.Max (0, smoothDistance - distance));
                World.Redraw = true;
            }
        }

        /// <summary>
        /// Berechne aus einer 2D-Positon (z.b. Mausposition) die entsprechende Position im 3D-Raum.
        /// Für die fehlende dritte Koordinate wird eine Angabe einer weiteren 3D-Position benötigt,
        /// mit der die 3D-(Maus-)Position auf der selben Ebene liegen soll.
        /// </summary>
        public Vector3 To3D (ScreenPoint position, Vector3 nearTo)
        {
            if (Config.Default ["debug", "unproject", "SelectedObject"] == "NearFarAverage") {
                Vector3 nearScreenPoint = new Vector3 (position.AbsoluteVector, 0);
                Vector3 farScreenPoint = new Vector3 (position.AbsoluteVector, 1);
                Vector3 nearWorldPoint = World.Viewport.Unproject (
                                             source: nearScreenPoint,
                                             projection: World.Camera.ProjectionMatrix,
                                             view: World.Camera.ViewMatrix,
                                             world: Matrix.Identity
                                         );
                Vector3 farWorldPoint = World.Viewport.Unproject (
                                            source: farScreenPoint,
                                            projection: World.Camera.ProjectionMatrix,
                                            view: World.Camera.ViewMatrix,
                                            world: Matrix.Identity
                                        );

                Vector3 direction = farWorldPoint - nearWorldPoint;

                float zFactor = -nearWorldPoint.Y / direction.Y;
                Vector3 zeroWorldPoint = nearWorldPoint + direction * zFactor;
                return zeroWorldPoint;
            }
            else {
                Vector3 screenLocation = World.Viewport.Project (
                                             source: nearTo,
                                             projection: World.Camera.ProjectionMatrix,
                                             view: World.Camera.ViewMatrix,
                                             world: World.Camera.WorldMatrix
                                         );
                Vector3 currentMousePosition = World.Viewport.Unproject (
                                                   source: new Vector3 (position.AbsoluteVector, screenLocation.Z),
                                                   projection: World.Camera.ProjectionMatrix,
                                                   view: World.Camera.ViewMatrix,
                                                   world: Matrix.Identity
                                               );
                return currentMousePosition;
            }
        }

        public Vector2 To2D (Vector3 position)
        {
            Vector3 screenLocation = World.Viewport.Project (
                                         source: position,
                                         projection: World.Camera.ProjectionMatrix,
                                         view: World.Camera.ViewMatrix,
                                         world: World.Camera.WorldMatrix
                                     );
            return new Vector2 (screenLocation.X, screenLocation.Y);
        }
    }
}
