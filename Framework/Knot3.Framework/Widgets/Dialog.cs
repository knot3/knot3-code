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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Knot3.Framework.Core;
using Knot3.Framework.Input;
using Knot3.Framework.Math;
using Knot3.Framework.Platform;
using Knot3.Framework.Storage;
using Knot3.Framework.Utilities;

namespace Knot3.Framework.Widgets
{
    /// <summary>
    /// Ein Dialog ist ein im Vordergrund erscheinendes Fenster, das auf Nutzerinteraktionen wartet.
    /// </summary>
    [ExcludeFromCodeCoverageAttribute]
    public abstract class Dialog : Widget, IKeyEventListener, IMouseClickEventListener, IMouseMoveEventListener
    {
        /// <summary>
        /// Der Fenstertitel.
        /// </summary>
        public string Title { get; set; }

        protected SpriteBatch spriteBatch;

        /// <summary>
        /// Wird aufgerufen, wenn der Dialog geschlossen wird.
        /// </summary>
        public Action<GameTime> Close;

        /// <summary>
        /// Die Hintergrundfarbe der Titelleiste.
        /// </summary>
        protected Func<Color> TitleBackgroundColor { get; set; }

        private Border titleBorder;
        private Border dialogBorder;

        public Bounds MouseClickBounds { get { return Bounds; } }

        public Bounds MouseMoveBounds { get { return TitleBounds; } }

        protected Func<Color>  TitleForegroundColor {get; set;}

        /// <summary>
        /// Erzeugt ein neues Dialog-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
        /// Zudem sind Angaben zur Zeichenreihenfolge, einer Zeichenkette für den Titel und für den eingeblendeten Text Pflicht.
        /// [base=screen, drawOrder]
        /// </summary>
        public Dialog (IScreen screen, DisplayLayer drawOrder, string title)
        : base (screen, drawOrder)
        {
            // Setzte Titel und Text
            Title = title;

            // erstelle einen Spritebatch zum Zeichnen der Titelleiste
            spriteBatch = new SpriteBatch (screen.GraphicsDevice);

            // Dialoge sind nach dem erstellen sichtbar, und das Delegate zum schließen macht sie unsichtbar
            IsVisible = true;
            IsModal = true;
            Close = (time) => {
                IsVisible = false;
                Screen.RemoveGameComponents (time, this);
            };

            // Die Standardposition ist in der Mitte des Bildschirms
            Bounds.Position = ScreenPoint.Centered (screen, Bounds);
            // Die Standardgröße
            Bounds.Size = new ScreenPoint (screen, 0.500f, 0.500f);
            // Der Standardabstand
            Bounds.Padding = new ScreenPoint (screen, 0.010f, 0.010f);
            // Die Standardfarben
            BackgroundColorFunc = (s) => Design.DialogBackground;
            ForegroundColorFunc = (s) => Design.DialogForeground;
            TitleBackgroundColor = () => Design.DefaultLineColor * 0.75f;
            TitleForegroundColor = () => Design.TitleForegroundColor;

            // Einen Rahmen um den Titel des Dialogs
            titleBorder = new Border (
                screen: screen,
                drawOrder: Index,
                bounds: TitleBounds,
                lineWidth: 2,
                padding: 1,
                lineColor: TitleBackgroundColor (),
                outlineColor: Design.DefaultOutlineColor * 0.75f
            );

            // Einen Rahmen um den Dialog
            dialogBorder = new Border (
                screen: screen,
                drawOrder: Index,
                widget: this,
                lineWidth: 2,
                padding: 1,
                lineColor: TitleBackgroundColor (),
                outlineColor: Design.DefaultOutlineColor * 0.75f
            );

            // Tasten, auf die wir reagieren
            ValidKeys = new List<Keys> ();
        }

        [ExcludeFromCodeCoverageAttribute]
        public override void Draw (GameTime time)
        {
            base.Draw (time);

            spriteBatch.Begin ();

            // zeichne den Hintergrund
            spriteBatch.DrawColoredRectangle (BackgroundColor, Bounds);

            // lade die Schrift
            SpriteFont font = Design.MenuFont (Screen);

            // zeichne den Titel des Dialogs
            spriteBatch.DrawColoredRectangle (TitleBackgroundColor (), TitleBounds);
            spriteBatch.DrawStringInRectangle (font, Title.Localize (), TitleForegroundColor (), TitleBounds, AlignX, AlignY);

            spriteBatch.End ();
        }

        public override IEnumerable<IScreenComponent> SubComponents (GameTime time)
        {
            foreach (DrawableScreenComponent component in base.SubComponents (time)) {
                yield return component;
            }
            yield return titleBorder;
            yield return dialogBorder;
        }

        protected Bounds TitleBounds
        {
            get {
                ScreenPoint pos = Bounds.Position;
                ScreenPoint size = new ScreenPoint (Screen, () => Bounds.Size.Relative.X, () => 0.050f);
                return new Bounds (pos, size);
            }
        }

        protected Bounds ContentBounds
        {
            get {
                ScreenPoint pos = Bounds.Position + TitleBounds.Size.OnlyY + Bounds.Padding;
                ScreenPoint size = Bounds.Size - TitleBounds.Size.OnlyY - Bounds.Padding * 2;
                return new Bounds (pos, size);
            }
        }

        /// <summary>
        /// Durch Drücken der Entertaste wird die ausgewählte Aktion ausgeführt. Durch Drücken der Escape-Taste wird der Dialog abgebrochen.
        /// Mit Hilfe der Pfeiltasten kann zwischen den Aktionen gewechselt werden.
        /// </summary>
        public virtual void OnKeyEvent (List<Keys> key, KeyEvent keyEvent, GameTime time)
        {
        }

        public virtual void SetHovered (bool hovered, GameTime time)
        {
        }

        /// <summary>
        /// Bei einem Linksklick geschieht nichts.
        /// </summary>
        public virtual void OnLeftClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        /// <summary>
        /// Bei einem Rechtsklick geschieht nichts.
        /// </summary>
        public virtual void OnRightClick (Vector2 position, ClickState state, GameTime time)
        {
        }

        public void OnLeftMove (ScreenPoint previousPosition, ScreenPoint currentPosition, ScreenPoint move, GameTime time)
        {
            Log.Debug (
                "OnLeftMove ("
                + previousPosition.ToString ()
                + ","
                + currentPosition.ToString ()
                + ","
                + move.ToString ()
                + ")"
            );

            if (new Bounds (ScreenPoint.Zero (Screen), MouseMoveBounds.Size).Contains (previousPosition)) {
                Log.Debug (
                    "TitleBounds ="
                    + Vector2.Zero.CreateRectangle (TitleBounds.Size).ToString ()
                    + "; previousPosition="
                    + previousPosition.ToString ()
                );
                Bounds.Position = (Bounds.Position + move).Const;
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
