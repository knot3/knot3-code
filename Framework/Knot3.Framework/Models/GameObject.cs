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

using Knot3.Framework.Core;
using Knot3.Framework.Math;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Models
{
    /// <summary>
    /// Enthält Informationen über ein 3D-Objekt wie die Position, Sichtbarkeit, Verschiebbarkeit und Auswählbarkeit.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class GameObject : IGameObject
    {
        /// <summary>
        /// Die Verschiebbarkeit des Spielobjektes.
        /// </summary>
        public Boolean IsMovable { get; set; }

        /// <summary>
        /// Die Auswählbarkeit des Spielobjektes.
        /// </summary>
        public Boolean IsSelectable { get; set; }

        /// <summary>
        /// Die Sichtbarkeit des Spielobjektes.
        /// </summary>
        public Boolean IsVisible { get; set; }

        /// <summary>
        /// Die Position des Spielobjektes.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Die Mitte des Spielobjektes.
        /// </summary>
        public virtual Vector3 Center { get { return Position; } }

        /// <summary>
        /// Die Rotation des Spielobjektes.
        /// </summary>
        public Angles3 Rotation { get; set; }

        /// <summary>
        /// Die Skalierung des Spielobjektes.
        /// </summary>
        public Vector3 Scale { get; set; }

        public int LastTick { get; set; }

        /// <summary>
        /// Die Spielwelt, in der sich das 3D-Modell befindet.
        /// </summary>
        public World World
        {
            get { return _world; }
            set {
                if (_world != value) {
                    if (_world != null) {
                        _world.Camera.OnViewChanged -= OnViewChanged;
                    }
                    _world = value;
                    if (value != null) {
                        _world.Camera.OnViewChanged += OnViewChanged;
                        OnViewChanged ();
                    }
                }
            }
        }

        private World _world;

        /// <summary>
        /// Die Weltmatrix des 3D-Modells in der angegebenen Spielwelt.
        /// </summary>
        public Matrix WorldMatrix
        {
            get {
                UpdatePrecomputed ();
                return _worldMatrix;
            }
        }

        /// <summary>
        /// Die Weltmatrix des 3D-Modells in der angegebenen Spielwelt.
        /// </summary>
        public Matrix WorldMatrixInverseTranspose
        {
            get {
                UpdatePrecomputed ();
                return _worldMatrixInverseTranspose;
            }
        }

        /// <summary>
        /// Gibt die Ausmaße des Spielobjekts zurück.
        /// </summary>
        public virtual BoundingSphere[] Bounds { get; protected set; }

        /// <summary>
        /// Gibt an, ob das Spielobjekt innerhalb des Frustums ist, das den sichtbaren Bereich enthält.
        /// </summary>
        public bool InCameraFrustum { get { return _inFrustum; } }

        /// <summary>
        /// Gibt an, ob das Spielobjekt mit Beleuchtung gezeichnet werden soll oder nicht.
        /// </summary>
        public bool IsLightingEnabled { get; set; }

        /// <summary>
        /// Gibt an, ob das Spielobjekt zur SkyBox oder etwas vergleichbarem gehört.
        /// Falls diese Property true ist, wird eine veränderte View-Matrix verwendet.
        /// </summary>
        public bool IsSkyObject { get; set; }

        public string UniqueKey { get; protected set; }

        /// <summary>
        /// Gibt eine Kategorie an, die dabei hilft, Spielobjekte nach gleichartigen Objekten zu ordnen.
        /// Wird beim Hardware-Instancing verwendet.
        /// </summary>
        public string GameObjectCategory  { get; protected set; }

        /// <summary>
        /// Die Farbe des Modells.
        /// </summary>
        public ModelColoring Coloring { get; set; }

        public bool IsSingleColored { get; set; }

        /// <summary>
        /// Die Textur des Modells.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set {
                _texture = value;
                UpdateCategory ();
            }
        }

        private Texture2D _texture;

        public GameObject (Vector3 position = default (Vector3), Angles3 rotation = default (Angles3), Vector3 scale = default (Vector3),
                           bool isVisible = true, bool isSelectable = false, bool isMovable = false)
        {
            Position = position;
            Scale = scale.Length () != 0 ? scale : Vector3.One;
            IsVisible = isVisible;
            IsSelectable = isSelectable;
            IsMovable = isMovable;
            Bounds = new BoundingSphere [0];
            UniqueKey = GetType ().Name;
            IsLightingEnabled = true;
            IsSkyObject = false;
            Coloring = new SingleColor (Color.Transparent);
        }

        protected virtual void UpdateCategory ()
        {
            UpdateCategory ("");
        }

        protected void UpdateCategory (string category)
        {
            IsSingleColored = Coloring is SingleColor;
            GameObjectCategory = GetType ().Name + GetTextureHashCode (obj: this) + category + IsLightingEnabled + IsSkyObject;
        }

        protected int GetTextureHashCode (IGameObject obj)
        {
            if (IsSingleColored) {
                return 0;
            }
            else if (obj.Texture != null) {
                return obj.Texture.GetHashCode ();
            }
            else {
                return obj.Coloring.GetHashCode ();
            }
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        public virtual void Update (GameTime time)
        {
        }

        /// <summary>
        /// Zeichnet das Spielobjekt.
        /// </summary>
        public abstract void Draw (GameTime time);

        /// <summary>
        /// Überprüft, ob der Mausstrahl das Spielobjekt schneidet.
        /// </summary>
        public virtual GameObjectDistance Intersects (Ray ray)
        {
            if (ray.Intersects (_overallBoundingBox) != null) {
                foreach (BoundingSphere sphere in Bounds) {
                    float? distance = ray.Intersects (sphere);
                    if (distance != null) {
                        GameObjectDistance intersection = new GameObjectDistance () {
                            Object = this, Distance = distance.Value
                        };
                        return intersection;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Vergleicht zwei Spielobjekte.
        /// </summary>
        public virtual bool Equals (IGameObject other)
        {
            return other != null && Position == other.Position && UniqueKey == other.UniqueKey;
        }

        public override bool Equals (object obj)
        {
            return Equals (obj as IGameObject);
        }

        [ExcludeFromCodeCoverageAttribute]
        public override int GetHashCode ()
        {
            return (UniqueKey + Position).GetHashCode ();
        }

        public void RotateToFaceTarget (Vector3 target)
        {
            Vector3 upDirection = World != null ? World.Camera.UpVector : Vector3.Up;
            Vector3 targetDirection = Vector3.Normalize (target - Position);
            _rotationQuaternion = VectorHelper.RotateToFaceTarget (sourceDirection: Vector3.Forward, destDirection: targetDirection, up: upDirection);
            UpdatePrecomputed (overrideValues: true);
        }

        private Vector3 _scale;
        private Angles3 _rotation;
        private Vector3 _position;
        private Matrix _worldMatrix;
        private Matrix _worldMatrixInverseTranspose;
        private bool _inFrustum;
        private BoundingBox _overallBoundingBox;
        private BoundingBox _frustumBoundingBox;
        private Quaternion? _rotationQuaternion;

        private void UpdatePrecomputed (bool overrideValues = false)
        {
            if (overrideValues || Scale != _scale || Rotation != _rotation || Position != _position) {
                Matrix rotationMatrix = _rotationQuaternion.HasValue ? Matrix.CreateFromQuaternion (_rotationQuaternion.Value)
                                        : Matrix.CreateFromYawPitchRoll (Rotation.Y, Rotation.X, Rotation.Z);
                // world matrix
                _worldMatrix = Matrix.CreateScale (Scale) * rotationMatrix * Matrix.CreateTranslation (Position);
                _worldMatrixInverseTranspose = Matrix.Transpose (Matrix.Invert (_worldMatrix));

                // attrs
                _scale = Scale;
                _rotation = Rotation;
                _position = Position;
            }
        }

        protected void OnViewChanged ()
        {
            UpdatePrecomputed ();

            // bounding box which contains the whole object (and maybe more)
            Vector3 overallBoundsMin = Position + Vector3.One * 200;
            Vector3 overallBoundsMax = Position - Vector3.One * 200;
            _overallBoundingBox = new BoundingBox (overallBoundsMin, overallBoundsMax);

            // bounding box for view frustum intersects check
            Vector3 boundsMin = Position + Vector3.One;
            Vector3 boundsMax = Position - Vector3.One;
            Vector3 toCameraTarget = World.Camera.Target - Position;
            Vector3 frustumBoundsMin = boundsMin + Vector3.Normalize (toCameraTarget) * MathHelper.Min (200, toCameraTarget.Length ());
            Vector3 frustumBoundsMax = boundsMax + Vector3.Normalize (toCameraTarget) * MathHelper.Min (200, toCameraTarget.Length ());
            _frustumBoundingBox = new BoundingBox (frustumBoundsMin, frustumBoundsMax);
            _inFrustum = World.Camera.ViewFrustum.FastIntersects (ref _frustumBoundingBox);
        }
    }
}
