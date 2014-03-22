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
    /// Ein Menü, das alle Einträge vertikal anordnet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public sealed class Menu : Container, IMouseClickEventListener, IMouseMoveEventListener, IMouseScrollEventListener
    {
        /// <summary>
        /// Die von der Auflösung unabhängige Höhe der Menüeinträge in Prozent.
        /// </summary>
        /// <value>
        /// The height of the relative item.
        /// </value>
        public float RelativeItemHeight { get; set; }

        public Bounds MouseClickBounds { get { return Bounds; } }

        public Bounds MouseScrollBounds { get { return Bounds; } }

        private SpriteBatch spriteBatch;

        private Bounds ScrollBarBounds
        {
            get {
                Bounds bounds = new Bounds (
                    Bounds.Position + Bounds.Size.OnlyX + new ScreenPoint (Screen, 0.005f, 0f),
                    new ScreenPoint (Screen, 0.02f, Bounds.Size.Relative.Y)
                );
                return bounds;
            }
        }

        public Bounds MouseMoveBounds
        {
            get { return new Bounds (Bounds.Position, Bounds.Size + ScrollBarBounds.Size.OnlyX); }
        }

        private Bounds ScrollSliderInBarBounds
        {
            get {
                Bounds moveBounds = ScrollBarBounds;
                float maxValue = maxScrollPosition;
                float pageValue = pageScrollPosition;
                float visiblePercent = (pageValue / maxValue).Clamp (0.05f, 1f);
                float currentValue = (float)currentScrollPosition / (maxValue - pageValue);
                // Log.Debug ("currentValue=", currentValue, ", pos=", moveBounds.FromTop (currentValue).Position);
                Bounds bounds = new Bounds (
                    position: moveBounds.Size.OnlyY * currentValue * (1f - visiblePercent),
                    size: moveBounds.Size.ScaleY (visiblePercent)
                );
                return bounds;
            }
        }

        /// <summary>
        /// Erzeugt eine neue Instanz eines VerticalMenu-Objekts und initialisiert diese mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem ist die Angaben der Zeichenreihenfolge Pflicht.
        /// </summary>
        public Menu (IScreen screen, DisplayLayer drawOrder)
        : base (screen, drawOrder)
        {
            RelativeItemHeight = Design.NavigationItemHeight;
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);

            ItemBackgroundColor = Design.MenuItemBackgroundColorFunc;
            ItemForegroundColor = Design.MenuItemForegroundColorFunc;
            BackgroundColorFunc = Design.WidgetBackgroundColorFunc;
            ForegroundColorFunc = Design.WidgetForegroundColorFunc;
        }

        protected override void assignMenuItemInformation (MenuItem item)
        {
            Bounds itemBounds = ItemBounds (item);
            item.Bounds.Position = itemBounds.Position;
            item.Bounds.Size = itemBounds.Size;
            base.assignMenuItemInformation (item);
        }

        /// <summary>
        /// Die von der Auflösung unabhängigen Ausmaße der Menüeinträge.
        /// </summary>
        public Bounds ItemBounds (MenuItem item)
        {
            return new Bounds (
                       position: new ScreenPoint (Screen, () => verticalRelativeItemPosition (item.ItemOrder)),
                       size: new ScreenPoint (Screen, () => verticalRelativeItemSize (item.ItemOrder))
                   );
        }

        private Vector2 verticalRelativeItemPosition (int itemOrder)
        {
            if (itemOrder < currentScrollPosition) {
                return Vector2.Zero;
            }
            else if (itemOrder > currentScrollPosition + pageScrollPosition - 1) {
                return Vector2.Zero;
            }
            else {
                float itemHeight = RelativeItemHeight + Bounds.Padding.Relative.Y;
                return Bounds.Position.Relative + new Vector2 (0, itemHeight * (itemOrder - currentScrollPosition));
            }
        }

        private Vector2 verticalRelativeItemSize (int itemOrder)
        {
            if (itemOrder < currentScrollPosition) {
                return Vector2.Zero;
            }
            else if (itemOrder > currentScrollPosition + pageScrollPosition - 1) {
                return Vector2.Zero;
            }
            else {
                return new Vector2 (Bounds.Size.Relative.X, RelativeItemHeight);
            }
        }

        /// <summary>
        /// Wird für jeden Frame aufgerufen.
        /// </summary>
        [ExcludeFromCodeCoverageAttribute]
        public override void Update (GameTime time)
        {
            base.Update (time);

            performScroll ();
        }

        /// <summary>
        /// Die Reaktion auf eine Bewegung des Mausrads. Das Menü scrollt die Einträge.
        /// </summary>
        public override void OnScroll (int scrollValue, GameTime time)
        {
            tempScrollValue = scrollValue;
        }

        private void performScroll ()
        {
            if (System.Math.Abs (tempScrollValue) > 0) {
                currentScrollPosition += tempScrollValue;
                tempScrollValue = 0;
            }
        }

        private int tempScrollValue = 0;

        private float currentScrollPosition
        {
            get {
                return _currentScrollPosition;
            }
            set {
                _currentScrollPosition = MathHelper.Clamp (value, 0, maxScrollPosition - pageScrollPosition);
            }
        }

        private float _currentScrollPosition;

        private float maxScrollPosition { get { return items.Count; } }

        private float pageScrollPosition
        {
            get {
                return (int)System.Math.Ceiling (Bounds.Size.Relative.Y / (RelativeItemHeight + Bounds.Padding.Relative.Y));
            }
        }

        private bool HasScrollbar { get { return maxScrollPosition > pageScrollPosition; } }

        /// <summary>
        /// Tut nichts.
        /// </summary>
        public void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        /// <summary>
        /// Tut nichts.
        /// </summary>
        public void OnRightClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        /// <summary>
        /// Tut nichts.
        /// </summary>
        public void SetHovered (bool hovered, GameTime time)
        {
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            if (IsVisible && IsEnabled) {
                spriteBatch.Begin ();
                Texture2D backgroundTexture = ContentLoader.CreateTexture (Screen.GraphicsDevice, Color.White);
                spriteBatch.Draw (backgroundTexture, Bounds, BackgroundColor);

                if (HasScrollbar) {
                    Texture2D rectangleTexture = ContentLoader.CreateTexture (Screen.GraphicsDevice, Color.White);
                    Bounds sliderBounds = ScrollSliderInBarBounds.In (ScrollBarBounds);
                    spriteBatch.Draw (rectangleTexture, sliderBounds.Rectangle.Grow (1), Design.DefaultOutlineColor);
                    spriteBatch.Draw (rectangleTexture, sliderBounds.Rectangle, Design.DefaultLineColor);
                    // Log.Debug ("ScrollSliderBounds=", sliderBounds.Rectangle);
                    // Log.Debug ("ScrollBarBounds=", ScrollBarBounds.Rectangle);
                }
                spriteBatch.End ();
            }
        }

        public void OnLeftMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
            //currentScrollPosition += (int)((move.Y / RelativeItemHeight)
            //  * ((float)minScrollPosition / (maxScrollPosition - pageScrollPosition)));

            if (IsVisible && IsEnabled && HasScrollbar) {
                //Bounds slider = ScrollSliderInBarBounds;
                Bounds bar = ScrollBarBounds;

                float percentOfBar = move.Relative.Y / bar.Size.Relative.Y;
                currentScrollPosition += percentOfBar * maxScrollPosition;

                /*
                float maxValue = maxScrollPosition;
                float pageValue = pageScrollPosition;
                float visiblePercent = (pageValue / maxValue).Clamp (0.05f, 1f);
                float sliderPosition = ScrollSliderInBarBounds.Position.Absolute.Y / ScrollBarBounds.Size.Absolute.Y;
                Log.Debug ("sliderPosition=", sliderPosition, ", ScrollSliderInBarBounds=", ScrollSliderInBarBounds);
                sliderPosition = move.Y / ScrollBarBounds.Size.Absolute.Y;

                Log.Debug ("sliderPosition new=", sliderPosition, ", current.Y=", currentPosition.Y
                    + ", bar.Size.Y=" + ScrollBarBounds.Size.Absolute.Y
                );
                currentScrollPosition = (int)(sliderPosition * (maxValue - pageValue));
                */
            }
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
