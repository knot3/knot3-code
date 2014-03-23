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
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Menüeintrag, der einen Schieberegler bereitstellt, mit dem man einen Wert zwischen einem minimalen
    /// und einem maximalen Wert über Verschiebung einstellen kann.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class SliderItem : MenuItem, IMouseClickEventListener, IMouseMoveEventListener
    {
        /// <summary>
        /// Der aktuelle Wert.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { if (_value != value) { _value = value; } }
        }

        private int _value;

        /// <summary>
        /// Der minimale Wert.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Der maximale Wert.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Wird aufgerufen, wenn der Wert geändert wurde
        /// </summary>
        public Action<GameTime> OnValueChanged = (time) => {};

        /// <summary>
        /// Die Breite des Rechtecks, abhängig von der Auflösung des Viewports.
        /// </summary>
        private float SliderRectangleWidth
        {
            get {
                return new Vector2 (0, 0.020f).Scale (Screen.Viewport).Y;
            }
        }
        /// <summary>
        /// Die geringste X-Position des Rechtecks (so weit links wie möglich), abhängig von der Auflösung des Viewports.
        /// </summary>
        private float SliderRectangleMinX
        {
            get {
                return ValueBounds.Rectangle.X + SliderRectangleWidth/2;
            }
        }
        /// <summary>
        /// Die höchste X-Position des Rechtecks (so weit rechts wie möglich), abhängig von der Auflösung des Viewports.
        /// </summary>
        private float SliderRectangleMaxX
        {
            get {
                return SliderRectangleMinX + ValueBounds.Rectangle.Width - SliderRectangleWidth/2;
            }
        }
        /// <summary>
        /// Die Position und Größe des Rechtecks.
        /// </summary>
        private Rectangle SliderRectangle
        {
            get {
                Rectangle valueBounds = ValueBounds;
                Rectangle rect = new Rectangle ();
                rect.Height = valueBounds.Height;
                rect.Width = (int)SliderRectangleWidth;
                rect.Y = valueBounds.Y;
                rect.X = (int)(SliderRectangleMinX + (SliderRectangleMaxX-SliderRectangleMinX)
                               * (Value-MinValue) / (MaxValue-MinValue) - rect.Width/2);
                return rect;
            }
        }

        public override Bounds MouseClickBounds { get { return ValueBounds; } }
        public Bounds MouseMoveBounds { get { return ValueBounds; } }

        /// <summary>
        /// Erzeugt eine neue Instanz eines SliderItem-Objekts und initialisiert diese
        /// mit dem zugehörigen IGameScreen-Objekt. Zudem ist die Angabe der Zeichenreihenfolge,
        /// einem minimalen einstellbaren Wert, einem maximalen einstellbaren Wert und einem Standardwert Pflicht.
        /// </summary>
        public SliderItem (IScreen screen, DisplayLayer drawOrder, string text, int max, int min, int value)
        : base (screen, drawOrder, text)
        {
            MaxValue = max;
            MinValue = min;
            _value = value;
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            Rectangle valueBounds = ValueBounds;

            int lineWidth = valueBounds.Width;
            int lineHeight = 2;

            Texture2D lineTexture = new Texture2D (Screen.GraphicsDevice, lineWidth, lineHeight);
            Texture2D rectangleTexture = new Texture2D (Screen.GraphicsDevice, 1, 1);

            Color[] dataLine = new Color [lineWidth * lineHeight];
            for (int i = 0; i < dataLine.Length; ++i) {
                dataLine [i] = Color.White;
            }
            lineTexture.SetData (dataLine);

            Color[] dataRec = new Color [1];
            dataRec [0] = Design.DefaultLineColor;
            rectangleTexture.SetData (dataRec);

            Vector2 coordinateLine = new Vector2 (valueBounds.X, valueBounds.Y + Bounds.Size.Absolute.Y / 2);

            spriteBatch.Begin ();

            spriteBatch.Draw (lineTexture, coordinateLine, Design.WidgetForeground);
            spriteBatch.Draw (rectangleTexture, SliderRectangle, Design.DefaultLineColor);

            spriteBatch.End ();
        }

        private void UpdateSlider (ScreenPoint position, GameTime time)
        {
            float min = SliderRectangleMinX-ValueBounds.Rectangle.X;
            float max = SliderRectangleMaxX-ValueBounds.Rectangle.X;

            Log.Debug ("position=", position, ", min=", min, ", max=", max);

            float mousePositionX = ((float)(position.Absolute.X)).Clamp (min, max);
            float percent = (mousePositionX - min)/(max-min);

            Value = (int)(MinValue + percent * (MaxValue-MinValue));
            OnValueChanged (time);
        }

        public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        public void OnLeftMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
            UpdateSlider (currentPosition, time);
        }

        public void OnRightMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
        }

        public void OnMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
        }

        public void OnNoMove (ScreenPoint currentPosition, GameTime time)
        {
        }
    }
}
