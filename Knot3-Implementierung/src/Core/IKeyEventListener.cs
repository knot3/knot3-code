﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Wenn der Code neu generiert wird, gehen alle Änderungen an dieser Datei verloren
// </auto-generated>
//------------------------------------------------------------------------------
namespace Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public interface IKeyEventListener 
	{
		DisplayLayer Index { get;set; }

		bool IsKeyEventEnabled { get;set; }

		List<Keys> ValidKeys { get;set; }

		DisplayLayer DisplayLayer { get;set; }

		void OnKeyEvent();

	}
}

