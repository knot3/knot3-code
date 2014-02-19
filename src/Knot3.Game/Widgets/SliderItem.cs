#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Output;
using Knot3.Framework.Platform;
using Knot3.Framework.Utilities;
using Knot3.Framework.Widgets;
using Knot3.Game.Core;
using Knot3.Game.Data;
using Knot3.Game.Development;
using Knot3.Game.GameObjects;
using Knot3.Game.Input;
using Knot3.Game.RenderEffects;
using Knot3.Game.Screens;
using Knot3.Game.Utilities;

#endregion

namespace Knot3.Game.Widgets
{
	/// <summary>
	/// Ein Menüeintrag, der einen Schieberegler bereitstellt, mit dem man einen Wert zwischen einem minimalen
	/// und einem maximalen Wert über Verschiebung einstellen kann.
	/// </summary>
	[ExcludeFromCodeCoverageAttribute]
	public sealed class SliderItem : MenuItem, IMouseClickEventListener, IMouseMoveEventListener
	{
		#region Properties

		/// <summary>
		/// Der aktuelle Wert.
		/// </summary>
		public int Value
		{
			get { return _value; }
			set { if (_value != value) { _value = value; OnValueChanged (); } }
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
		/// Schrittweite zwischen zwei einstellbaren Werten.
		/// </summary>
		//public int Step { get; set; }

		/// <summary>
		/// Wird aufgerufen, wenn der Wert geändert wurde
		/// </summary>
		public Action OnValueChanged = () => {};

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

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt eine neue Instanz eines SliderItem-Objekts und initialisiert diese
		/// mit dem zugehörigen IGameScreen-Objekt. Zudem ist die Angabe der Zeichenreihenfolge,
		/// einem minimalen einstellbaren Wert, einem maximalen einstellbaren Wert und einem Standardwert Pflicht.
		/// </summary>
		public SliderItem (IGameScreen screen, DisplayLayer drawOrder, string text, int max, int min, int step, int value)
		: base (screen, drawOrder, text)
		{
			MaxValue = max;
			MinValue = min;
			//Step = step;
			_value = value;
		}

		#endregion

		#region Methods

		[ExcludeFromCodeCoverageAttribute]
		public override void Draw (GameTime time)
		{
			base.Draw (time);

			Rectangle valueBounds = ValueBounds;

			int lineWidth = valueBounds.Width;
			int lineHeight = 2;

			Texture2D lineTexture = new Texture2D (Screen.Device, lineWidth, lineHeight);
			Texture2D rectangleTexture = new Texture2D (Screen.Device, 1, 1);

			Color[] dataLine = new Color[lineWidth * lineHeight];
			for (int i = 0; i < dataLine.Length; ++i) {
				dataLine [i] = Color.White;
			}
			lineTexture.SetData (dataLine);

			Color[] dataRec = new Color[1];
			dataRec [0] = Design.DefaultLineColor;
			rectangleTexture.SetData (dataRec);

			Vector2 coordinateLine = new Vector2 (valueBounds.X, valueBounds.Y + Bounds.Size.Absolute.Y / 2);

			spriteBatch.Begin ();

			spriteBatch.Draw (lineTexture, coordinateLine, Color.White);
			spriteBatch.Draw (rectangleTexture, SliderRectangle, Design.DefaultLineColor);

			spriteBatch.End ();
		}

		private void UpdateSlider (ScreenPoint position)
		{
			float min = SliderRectangleMinX-ValueBounds.Rectangle.X;
			float max = SliderRectangleMaxX-ValueBounds.Rectangle.X;

			Log.Debug (
			    "position="
			    + position.ToString ()
			    + ", min="
			    + min.ToString ()
			    + ", max="
			    + max.ToString ()
			);

			float mousePositionX = ((float)(position.Absolute.X)).Clamp (min, max);
			float percent = (mousePositionX - min)/(max-min);

			Value = (int)(MinValue + percent * (MaxValue-MinValue));
		}

		public override void OnLeftClick (Vector2 position, ClickState state, GameTime time)
		{
			//UpdateSlider (position);
		}

		public void OnLeftMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
		{
			UpdateSlider (currentPosition);
		}

		public void OnRightMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
		{
		}

		public void OnMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
		{
		}

		#endregion
	}
}
