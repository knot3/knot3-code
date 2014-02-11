using System;
using System.Collections;
using System.Collections.Generic;
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

using Knot3.Core;
using Knot3.GameObjects;
using Knot3.Screens;
using Knot3.RenderEffects;
using Knot3.KnotData;
using Knot3.Utilities;
using Knot3.Development;

namespace Knot3.Widgets
{
	/// <summary>
	/// Ein Dialog ist ein im Vordergrund erscheinendes Fenster, das auf Nutzerinteraktionen wartet.
	/// </summary>
	public abstract class Dialog : Widget, IKeyEventListener, IMouseClickEventListener, IMouseMoveEventListener
	{
		#region Properties

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

		#endregion

		#region Constructors

		/// <summary>
		/// Erzeugt ein neues Dialog-Objekt und initialisiert dieses mit dem zugehörigen IGameScreen-Objekt.
		/// Zudem sind Angaben zur Zeichenreihenfolge, einer Zeichenkette für den Titel und für den eingeblendeten Text Pflicht.
		/// [base=screen, drawOrder]
		/// </summary>
		public Dialog (IGameScreen screen, DisplayLayer drawOrder, string title, string text)
		: base(screen, drawOrder)
		{
			// Setzte Titel und Text
			Title = title;

			// erstelle einen Spritebatch zum Zeichnen der Titelleiste
			spriteBatch = new SpriteBatch (screen.Device);

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
			BackgroundColorFunc = (s) => screen.BackgroundColor.Mix (Design.WidgetForeground, 0.05f);
			ForegroundColorFunc = (s) => Design.WidgetBackground;
			TitleBackgroundColor = () => Design.DefaultLineColor * 0.75f;

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

		#endregion

		#region Methods

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
			spriteBatch.DrawStringInRectangle (font, Title, ForegroundColor, TitleBounds, AlignX, AlignY);

			spriteBatch.End ();
		}

		public override IEnumerable<IGameScreenComponent> SubComponents (GameTime time)
		{
			foreach (DrawableGameScreenComponent component in base.SubComponents(time)) {
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
			    "OnLeftMove("
			    + previousPosition.ToString ()
			    + ","
			    + currentPosition.ToString ()
			    + ","
			    + move.ToString ()
			    + ")"
			);

			if (new Bounds(ScreenPoint.Zero(Screen), MouseMoveBounds.Size).Contains (previousPosition)) {
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

		#endregion
	}
}