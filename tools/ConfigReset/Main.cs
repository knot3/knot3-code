using System;
using System.IO;

using Knot3;
using Knot3.Core;
using Knot3.Utilities;

namespace ConfigReset
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Settings directory: " + FileUtility.SettingsDirectory);
			Console.WriteLine ();
			Console.WriteLine ("Press <Enter> to delete it.");
			Console.ReadLine();

			try {
				Directory.Delete (FileUtility.SettingsDirectory, true);
				Console.WriteLine ("Settings deleted.");
			}
			catch (Exception ex) {
				Console.WriteLine ("Error: Could not delete settings!");
				Console.WriteLine (ex.ToString ());
			}
			Console.WriteLine ();

			Console.WriteLine("Press <Enter> to exit.");
			Console.ReadLine();
		}
	}
}
