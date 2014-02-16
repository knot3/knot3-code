using System;
using Knot3.Widgets;
using Knot3.Core;

namespace Knot3.MockObjects
{
	public class FakeWidget : Widget
	{
		public FakeWidget (IGameScreen screen, DisplayLayer drawOrder)
			: base(screen, drawOrder)
		{
		}
	}
}

