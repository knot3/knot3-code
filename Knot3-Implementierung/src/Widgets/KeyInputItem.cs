﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Wenn der Code neu generiert wird, gehen alle Änderungen an dieser Datei verloren
// </auto-generated>
//------------------------------------------------------------------------------
namespace Widgets
{
	using Core;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class KeyInputItem : InputItem
	{
		private OptionInfo option
		{
			get;
			set;
		}

		public virtual void OnKeyEvent()
		{
			throw new System.NotImplementedException();
		}

		public KeyInputItem(GameScreen screen, DisplayLayer drawOrder, OptionInfo option)
		{
		}

	}
}

