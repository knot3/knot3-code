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
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Knot3.Framework.Core;
using Knot3.Framework.Effects;
using Knot3.Framework.Math;
using Knot3.Framework.Models;
using Knot3.Framework.Widgets;

namespace Knot3.Framework.Platform
{
    [ExcludeFromCodeCoverageAttribute]
    public static partial class ContentLoader
    {
        private static Dictionary<string, ContentManager> contentManagers = new Dictionary<string, ContentManager> ();
        private static HashSet<string> invalidModels = new HashSet<string> ();

        public static Model LoadModel (this IScreen screen, string name)
        {
            ContentManager content;
            if (contentManagers.ContainsKey (screen.CurrentRenderEffects.CurrentEffect.ToString ())) {
                content = contentManagers [screen.CurrentRenderEffects.CurrentEffect.ToString ()];
            }
            else {
                contentManagers [screen.CurrentRenderEffects.CurrentEffect.ToString ()] = content = new ContentManager (screen.Game.Content.ServiceProvider, screen.Game.Content.RootDirectory);
            }

            Model model = LoadModel (content, screen.CurrentRenderEffects.CurrentEffect, name);
            return model;
        }

        private static Model LoadModel (ContentManager content, IRenderEffect pp, string name)
        {
            if (invalidModels.Contains (name)) {
                return null;
            }
            else {
                try {
                    Model model = content.Load<Model> ("Models/" + name);
                    pp.RemapModel (model);
                    return model;
                }
                catch (ContentLoadException) {
                    Log.Debug ("Warning: Model ", name, " does not exist!");
                    invalidModels.Add (name);
                    return null;
                }
            }
        }

        private static Dictionary<int, Texture2D> textureCache = new Dictionary<int, Texture2D> ();

        public static Texture2D LoadTexture (this IScreen screen, string name)
        {
            int key = Hash (name);
            if (textureCache.ContainsKey (key)) {
                return textureCache [key];
            }
            else {
                Texture2D tex = null;
                if (tex == null) {
                    tex = LoadTextureFromFile (screen, name + ".png");
                }
                if (tex == null) {
                    tex = LoadTextureFromContentPipeline (screen, name);
                }
                return textureCache [key] = tex;
            }
        }

        private static Texture2D LoadTextureFromContentPipeline (IScreen screen, string name)
        {
            try {
                return screen.Game.Content.Load<Texture2D> ("Textures/" + name);
            }
            catch (IOException ex) {
                Log.Debug (ex);
                return null;
            }
            catch (ContentLoadException ex) {
                Log.Debug (ex);
                return null;
            }
        }

        private static Texture2D LoadTextureFromFile (IScreen screen, string name)
        {
            try {
                Texture2D texture = null;
                Dependencies.CatchDllExceptions (() => {
                    string filename = SystemInfo.RelativeContentDirectory + "Textures" + SystemInfo.PathSeparator + name;
                    FileStream stream = new FileStream (filename, FileMode.Open);
                    texture = Texture2D.FromStream (screen.GraphicsDevice, stream);
                });
                return texture;
            }
            catch (Exception ex) {
                // hier kommen bei indexierten Bitmaps nur "Exception"-Objekte,
                // keine IOException oder ContentLoadException, daher die allgemeine Abfrage nach Exception!
                Log.Debug (ex);
                return null;
            }
        }

        public static SpriteFont LoadFont (this IScreen screen, string name)
        {
            try {
                return screen.Game.Content.Load<SpriteFont> ("Fonts/" + name);
            }
            catch (ContentLoadException ex) {
                Log.Debug (ex);
                return null;
            }
        }

        public static Texture2D CreateTexture (GraphicsDevice graphicsDevice, Color color)
        {
            return CreateTexture (graphicsDevice, 1, 1, color);
        }

        public static Texture2D CreateTexture (GraphicsDevice graphicsDevice, int width, int height, Color color)
        {
            int key = Hash (color, width, height);
            if (textureCache.ContainsKey (key)) {
                return textureCache [key];
            }
            else {
                // create a texture with the specified size
                Texture2D texture = new Texture2D (graphicsDevice, width, height);

                // fill it with the specified colors
                Color[] colors = new Color [width * height];
                for (int i = 0; i < colors.Length; i++) {
                    colors [i] = new Color (color.ToVector3 ());
                }
                texture.SetData (colors);
                textureCache [key] = texture;
                return texture;
            }
        }

        public static Texture2D CreateGradient (GraphicsDevice graphicsDevice, GradientColor coloring)
        {
            return CreateGradient (graphicsDevice: graphicsDevice, color1: coloring.Color1, color2: coloring.Color2);
        }

        public static Texture2D CreateGradient (GraphicsDevice graphicsDevice, Color color1, Color color2)
        {
            int key = Hash (color1, color2);
            if (textureCache.ContainsKey (key)) {
                return textureCache [key];
            }
            else {
                // create a texture with the specified size
                Texture2D texture = new Texture2D (graphicsDevice, 2, 1);

                // fill it with the specified colors
                Color[] colors = new Color [texture.Width * texture.Height];
                colors [0] = color1;
                colors [1] = color2;
                texture.SetData (colors);
                textureCache [key] = texture;
                return texture;
            }
        }

        public static Texture2D CreateGradient (GraphicsDevice graphicsDevice, Color topLeft, Color topRight, Color bottomLeft, Color bottomRight)
        {
            int key = Hash (topLeft, topRight, bottomLeft, bottomRight);
            if (textureCache.ContainsKey (key)) {
                return textureCache [key];
            }
            else {
                // create a texture with the specified size
                Texture2D texture = new Texture2D (graphicsDevice, 2, 2);

                // fill it with the specified colors
                Color[] colors = new Color [texture.Width * texture.Height];
                colors [0] = topLeft;
                colors [1] = topRight;
                colors [2] = bottomLeft;
                colors [3] = bottomRight;
                texture.SetData (colors);
                textureCache [key] = texture;
                return texture;
            }
        }

        public static Texture2D InvertTexture (IScreen screen, Texture2D texture)
        {
            Color[] colors = new Color [texture.Width * texture.Height];
            texture.GetData (colors);
            for (int i = 0; i < colors.Length; ++i) {
                colors [i] = new Color (Vector3.One - colors [i].ToVector3 ());
            }
            Texture2D newTexture = new Texture2D (screen.GraphicsDevice, texture.Width, texture.Height);
            newTexture.SetData (colors);
            return newTexture;
        }

        public static void DrawColoredRectangle (this SpriteBatch spriteBatch, Color color, Rectangle bounds)
        {
            Texture2D texture = ContentLoader.CreateTexture (spriteBatch.GraphicsDevice, Color.White);
            spriteBatch.Draw (
                texture, bounds, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.1f
            );
        }

        private static int Hash (params object[] parameters)
        {
            int hash = 0;
            unchecked {
                for (int i = 0; i < parameters.Length; ++i) {
                    hash = hash * 99971 + parameters [i].GetHashCode ();
                }
            }
            return hash;
        }

        public static void DrawScaledString (this SpriteBatch spriteBatch, SpriteFont font,
                                             string text, Color color, ScreenPoint position, Vector2 scale)
        {
            try {
                // zeichne die Schrift
                spriteBatch.DrawString (font, text, position.AbsoluteVector, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
            }
            catch (ArgumentException exp) {
                Log.Debug (exp);
            }
            catch (InvalidOperationException exp) {
                Log.Debug (exp);
            }
        }

        public static void DrawStringInRectangle (this SpriteBatch spriteBatch, SpriteFont font,
                string text, Color color, Bounds bounds,
                HorizontalAlignment alignX, VerticalAlignment alignY)
        {
            Vector2 scaledPosition = bounds.Position.AbsoluteVector;
            Vector2 scaledSize = bounds.Size.AbsoluteVector;
            try {
                // finde die richtige Skalierung
                Vector2 scale = spriteBatch.ScaleStringInRectangle (font, text, color, bounds, alignX, alignY);

                // finde die richtige Position
                Vector2 textPosition = TextPosition (
                                           font: font, text: text, scale: scale,
                                           position: scaledPosition, size: scaledSize,
                                           alignX: alignX, alignY: alignY
                                       );

                // zeichne die Schrift
                spriteBatch.DrawString (font, text, textPosition, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
            }
            catch (ArgumentException exp) {
                Log.Debug (exp);
            }
            catch (InvalidOperationException exp) {
                Log.Debug (exp);
            }
        }

        public static Vector2 ScaleStringInRectangle (this SpriteBatch spriteBatch, SpriteFont font,
                string text, Color color, Rectangle bounds,
                HorizontalAlignment alignX, VerticalAlignment alignY)
        {
            Vector2 scaledSize = new Vector2 (bounds.Width, bounds.Height);
            try {
                // finde die richtige Skalierung
                Vector2 scale = scaledSize / font.MeasureString (text) * 0.9f;
                if (!text.Contains ("\n")) {
                    scale.Y = scale.X = MathHelper.Min (scale.X, scale.Y);
                }
                return scale;
            }
            catch (Exception exp) {
                Log.Debug (exp);
                return Vector2.One;
            }
        }

        public static Vector2 TextPosition (SpriteFont font, string text, Vector2 scale, Vector2 position, Vector2 size,
                                            HorizontalAlignment alignX, VerticalAlignment alignY)
        {
            Vector2 textPosition = position;
            Vector2 minimumSize = font.MeasureString (text);
            switch (alignX) {
            case HorizontalAlignment.Left:
                textPosition.Y += (size.Y - minimumSize.Y * scale.Y) / 2;
                break;
            case HorizontalAlignment.Center:
                textPosition += (size - minimumSize * scale) / 2;
                break;
            case HorizontalAlignment.Right:
                textPosition.Y += (size.Y - minimumSize.Y * scale.Y) / 2;
                textPosition.X += size.X - minimumSize.X * scale.X;
                break;
            }
            return textPosition;
        }
    }
}
