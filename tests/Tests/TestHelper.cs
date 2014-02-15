using System;
using Knot3.Platform;

namespace Knot3.UnitTests
{
	public static class TestHelper
	{
		public static string TestResourcesDirectory
		{
			get {
				return SystemInfo.BaseDirectory + SystemInfo.PathSeparator + "Resources" + SystemInfo.PathSeparator;
			}
		}
	}
}

