using System;
using Knot3.Framework.Models;
using Knot3.Framework.Core;
using Knot3.Framework.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Knot3.Framework.Storage;

namespace Knot3.Framework.Models
{
    public class Sun : GamePrimitive
    {
        private BasicEffect effect;

        public Sun (IScreen screen) : base (screen: screen)
        {
            Coloring = new SingleColor (Color.Yellow);
            effect = new BasicEffect (screen.GraphicsDevice);
        }

        protected override Primitive CreativePrimitive ()
        {
            int tessellation = Primitive.CurrentCircleTessellation;
            return new Sphere (
                device: Screen.GraphicsDevice,
                diameter: 1f,
                tessellation: tessellation
            );
        }

        int j = 0;

        public override void Update (GameTime gameTime)
        {
            Position = World.Camera.SunPosition;
            Scale = Vector3.One * MathHelper.Clamp((int)(World.Camera.SunPosition.Length() / 25f), 1000f, 10000f);
            if (j % 60 == 0) {
                IsVisible = Config.Default ["video", "show-sun", true] && !Config.Default ["debug", "projector-mode", false];
                World.Redraw = true;
            }
            ++j;
            base.Update (gameTime);
        }

        public override void Draw (GameTime gameTime)
        {
            if (IsVisible && InCameraFrustum) {
                effect.World = WorldMatrix * World.Camera.WorldMatrix;
                effect.View = World.Camera.ViewMatrix;
                effect.Projection = World.Camera.ProjectionMatrix;
                effect.FogEnabled = false;
                effect.LightingEnabled = true;
                effect.PreferPerPixelLighting = true;
                effect.DirectionalLight0.DiffuseColor = Color.Yellow.ToVector3();
                effect.DirectionalLight0.SpecularColor = Color.Yellow.ToVector3();
                effect.DirectionalLight0.Direction = -World.Camera.LightDirection;
                effect.DirectionalLight0.Enabled = true;
                effect.Alpha = 0.85f;
                Primitive.Draw (effect: effect);
            }
        }
    }
}

