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

	public class SliderItem : MenuItem
	{
		public virtual int Value
		{
			get;
			set;
		}

		public virtual int MinValue
		{
			get;
			set;
		}

		public virtual int MaxValue
		{
			get;
			set;
		}

		public virtual int Step
		{
			get;
			set;
		}

        public SliderItem(GameScreen screen, DisplayLayer drawOrder, int max, int min, int step, int value)
		{
			throw new System.NotImplementedException();
		}

	}
}

