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

namespace Knot3.Widgets
{
	/// <summary>
	/// Eine abstrakte Klasse, von der alle Element der grafischen Benutzeroberfläche erben.
	/// </summary>
	public abstract class Widget : DrawableGameScreenComponent
	{
		#region Properties

		/// <summary>
		/// Die Ausmaße des Widgets.
		/// </summary>
		public Bounds Bounds { get; set; }

		/// <summary>
		/// Gibt an, ob das grafische Element sichtbar ist.
		/// </summary>
		public virtual bool IsVisible
		{
			get { return _isVisible && Bounds.Size.Absolute.Length () > 0; }
			set { _isVisible = value; }
		}

		private bool _isVisible;

		/// <summary>
		/// Die Hintergrundfarbe.
		/// </summary>
		public Func<State, Color> BackgroundColorFunc { private get; set; }

		/// <summary>
		/// Die Vordergrundfarbe.
		/// </summary>
		public Func<State, Color> ForegroundColorFunc { private get; set; }

		public Color BackgroundColor { get { return BackgroundColorFunc(State); } }

		public Color ForegroundColor { get { return ForegroundColorFunc(State); } }

		/// <summary>
		/// Die horizontale Ausrichtung.
		/// </summary>
		public HorizontalAlignment AlignX { get; set; }

		/// <summary>
		/// Die vertikale Ausrichtung.
		/// </summary>
		public VerticalAlignment AlignY { get; set; }

		public List<Keys> ValidKeys { get; protected set; }

		public virtual bool IsKeyEventEnabled
		{
			get { return IsVisible && ValidKeys.Count > 0; }
			set { }
		}

		public virtual bool IsMouseClickEventEnabled
		{
			get { return IsVisible; }
			set { }
		}

		public virtual bool IsMouseMoveEventEnabled
		{
			get { return IsVisible; }
			set { }
		}

		public virtual bool IsMouseScrollEventEnabled
		{
			get { return IsVisible; }
			set { }
		}

		public virtual bool IsEnabled
		{
			get { return _isEnabled; }
			set { _isEnabled = value; }
		}

		private bool _isEnabled;

		public virtual State State { get; set; }

		public virtual Color SelectedColorBackground { get; set; }

		public virtual Color SelectedColorForeground { get; set; }

		public bool IsModal { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Erstellt ein neues grafisches Benutzerschnittstellenelement in dem angegebenen Spielzustand
		/// mit der angegebenen Zeichenreihenfolge.
		/// </summary>
		public Widget (IGameScreen screen, DisplayLayer drawOrder)
		: base (screen, drawOrder)
		{
			Bounds = Bounds.Zero (screen);
			AlignX = HorizontalAlignment.Left;
			AlignY = VerticalAlignment.Center;
			ForegroundColorFunc = Design.WidgetForegroundColorFunc;
			BackgroundColorFunc = Design.WidgetBackgroundColorFunc;
			ValidKeys = new List<Keys> ();
			IsVisible = true;
			_isEnabled = true;
			IsModal = false;
			State = State.None;
			SelectedColorBackground = Design.WidgetForeground;
			SelectedColorForeground = Design.WidgetBackground;
		}

		#endregion
	}
}