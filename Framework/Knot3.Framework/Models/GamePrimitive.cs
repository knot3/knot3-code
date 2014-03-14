using System;
using System.Diagnostics.CodeAnalysis;
using Primitives;
using Knot3.Framework.Core;
using Microsoft.Xna.Framework;
using Knot3.Framework.Math;
using System.Linq;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Models
{
    [ExcludeFromCodeCoverageAttribute]
    public class GamePrimitive : IGameObject
    {
        GameObjectInfo IGameObject.Info { get { return Info; } }
        
        /// <summary>
        /// Die Farbe des Modells.
        /// </summary>
        public ModelColoring Coloring { get; set; }

        /// <summary>
        /// Die Modellinformationen wie Position, Skalierung und der Dateiname des 3D-Modells.
        /// </summary>
        public GamePrimitiveInfo Info { get; protected set; }

        /// <summary>
        /// Die Klasse des XNA-Frameworks, die ein 3D-Modell repräsentiert.
        /// </summary>
        public Primitive Primitive { get; private set; }

        /// <summary>
        /// Die Spielwelt, in der sich das 3D-Modell befindet.
        /// </summary>
        public World World
        {
            get { return _world; }
            set {
                _world = value;
                _world.Camera.OnViewChanged -= OnViewChanged;
                _world.Camera.OnViewChanged += OnViewChanged;
                OnViewChanged ();
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

        protected IScreen Screen;

        /// <summary>
        /// Erstellt ein neues 3D-Modell in dem angegebenen Spielzustand mit den angegebenen Modellinformationen.
        /// </summary>
        public GamePrimitive (IScreen screen, GamePrimitiveInfo info, Primitive primitive)
        {
            Screen = screen;
            Info = info;
            Primitive = primitive;

            // default values
            Coloring = new SingleColor (Color.Transparent);
        }

        /// <summary>
        /// Gibt die Mitte des 3D-Modells zurück.
        /// </summary>
        public virtual Vector3 Center ()
        {
            return Primitive.Center / Info.Scale + Info.Position;
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public virtual void Update (GameTime time)
        {
            if (Info != null && Info.Position.Length () > World.Camera.MaxPositionDistance) {
                World.Camera.MaxPositionDistance = Info.Position.Length () + 250;
            }
        }

        /// <summary>
        /// Zeichnet das 3D-Modell in der angegebenen Spielwelt mit dem aktuellen Rendereffekt der Spielwelt.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public virtual void Draw (GameTime time)
        {
            if (Info.IsVisible) {
                if (InCameraFrustum) {
                    Screen.CurrentRenderEffects.CurrentEffect.DrawPrimitive (this, time);
                }
            }
        }

        /// <summary>
        /// Überprüft, ob der Mausstrahl das 3D-Modell schneidet.
        /// </summary>
        public virtual GameObjectDistance Intersects (Ray ray)
        {
            foreach (BoundingSphere sphere in Bounds) {
                float? distance = ray.Intersects (sphere);
                if (distance != null) {
                    GameObjectDistance intersection = new GameObjectDistance () {
                        Object = this, Distance = distance.Value
                    };
                    return intersection;
                }
            }
            return null;
        }

        private Vector3 _scale;
        private Angles3 _rotation;
        private Vector3 _position;
        private Matrix _worldMatrix;
        private BoundingSphere[] _bounds;
        private bool _inFrustum;

        public virtual BoundingSphere[] Bounds
        {
            get {
                UpdatePrecomputed ();
                return _bounds;
            }
        }

        protected bool InCameraFrustum
        {
            get {
                return _inFrustum;
            }
        }

        private void UpdatePrecomputed ()
        {
            if (Info.Scale != _scale || Info.Rotation != _rotation || Info.Position != _position) {
                // world matrix
                _worldMatrix = Matrix.CreateScale (Info.Scale)
                    * Matrix.CreateFromYawPitchRoll (Info.Rotation.Y, Info.Rotation.X, Info.Rotation.Z)
                        * Matrix.CreateTranslation (Info.Position);

                // bounding spheres
                _bounds = Primitive.Bounds;
                for (int i = 0; i < _bounds.Length; ++i) {
                    _bounds [i] = _bounds [i].Scale (Info.Scale).Rotate (Info.Rotation).Translate ((Vector3)Info.Position);
                }

                // attrs
                _scale = Info.Scale;
                _rotation = Info.Rotation;
                _position = Info.Position;
            }
        }

        private void OnViewChanged ()
        {
            // camera frustum
            _inFrustum = false;
            UpdatePrecomputed ();
            foreach (BoundingSphere _sphere in _bounds) {
                var sphere = _sphere;
                if (World.Camera.ViewFrustum.FastIntersects (ref sphere)) {
                    _inFrustum = true;
                    break;
                }
            }
        }
    }
}

