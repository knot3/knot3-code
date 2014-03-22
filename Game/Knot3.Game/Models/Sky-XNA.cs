using System;
using System.Diagnostics.CodeAnalysis;
using Knot3.Framework.Core;
using Knot3.Game.Models;
using Microsoft.Xna.Framework;
using Knot3.Framework.Models;
using System.Collections.Generic;
using System.Collections;

namespace Knot3.Game
{
    [ExcludeFromCodeCoverageAttribute]
    public class Sky : GameObject, IEnumerable<IGameObject>
    {
        private SkyCube cube;

        public Sky (IScreen screen, Vector3 position)
        {
            cube = new SkyCube (screen: screen, position: position, distance: 5000);
        }

        public override void Update (GameTime time)
        {
            cube.World = World;
            cube.Update (time);
        }

        public override void Draw (GameTime time)
        {
            cube.Draw (time);
        }

        public override GameObjectDistance Intersects (Ray ray)
        {
            return null;
        }

        /// <summary>
        /// Gibt einen Enumerator der aktuell vorhandenen 3D-Modelle zurück.
        /// [returntype=IEnumerator<IGameObject>]
        /// </summary>
        public IEnumerator<IGameObject> GetEnumerator ()
        {
            yield return cube;
        }
        // Explicit interface implementation for nongeneric interface
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator (); // Just return the generic version
        }
    }
}

