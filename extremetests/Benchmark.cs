#region Copyright

/*
 * Copyright (c) 2013-2014 Tobias Schulz, Maximilian Reuter, Pascal Knodel,
 *                         Gerd Augsburg, Christina Erler, Daniel Warzel
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Knot3.ExtremeTests
{
	public struct TimeStatistics {
		public string name;
		public long outlier;
		public long slowest;
		public long fastest;
		public long average;
	}

	public class Benchmark
	{
		public static TimeStatistics StopTime (Action action, int numberOfPasses, TimeStatistics timeStatistics)
		{
			Stopwatch stopwatch = new Stopwatch ();
			long nsPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;

			timeStatistics.slowest = long.MinValue;
			timeStatistics.fastest = long.MaxValue;
			timeStatistics.average = 0;
			// todo: timeStatistics.name = ...

			numberOfPasses++;

			for (int countedPass = 0; countedPass < numberOfPasses; countedPass++) {
				stopwatch.Reset ();
				stopwatch.Start ();
				action ();
				stopwatch.Stop ();

				if (countedPass == 0) {
					timeStatistics.outlier = stopwatch.ElapsedTicks * nsPerTick;
					numberOfPasses--;
					continue;
				}

				if (stopwatch.ElapsedTicks < 0) {
					numberOfPasses++;
					continue;
				}

				if (stopwatch.ElapsedTicks > timeStatistics.slowest) {
					timeStatistics.slowest = stopwatch.ElapsedTicks;
				}
				else if (stopwatch.ElapsedTicks < timeStatistics.fastest) {
					timeStatistics.fastest = stopwatch.ElapsedTicks;
				}

				// no overflow testing ...

				timeStatistics.average = timeStatistics.average + stopwatch.ElapsedTicks;

				// no cache pollution ...
			}

			timeStatistics.average = timeStatistics.average / numberOfPasses;

			// todo
			timeStatistics.slowest = timeStatistics.slowest * nsPerTick;
			timeStatistics.fastest = timeStatistics.fastest * nsPerTick;
			timeStatistics.average = timeStatistics.average * nsPerTick;

			return timeStatistics;
		}

		public static void PrintTimerProperties ()
		{
			Console.Write (
			    "\n"
			    + "--- Eigenschaften des Zeitgebers ---"
			    + "\n\n"

			    // Auflösung:
			    + "\t Ist hochaufgelöst? - "
			    + ((Stopwatch.IsHighResolution) ? "Ja" : "Nein")
			    + "."
			    + "\n\n"

			    // Frequenz:
			    + "\t Frequenz: "
			    + Stopwatch.Frequency
			    + " Ticks pro Sekunde.\n\n"

			    // Ticks in NS:
			    + "\t Ein Tick dauert "
			    + ((1000L * 1000L * 1000L) / Stopwatch.Frequency)
			    + " NS.\n\n"
			);
		}

		// todo noch in MS und S ...
		public static void PrintTimeStatistics (TimeStatistics timeStatistics)
		{
			Console.Write (
			    // "Name: "
			    timeStatistics.name
			    + "\n\nmax = "
			    + timeStatistics.slowest // Langsamste.
			    + " NS\nmin = "
			    + timeStatistics.fastest // Schnellste.
			    + " NS\navg = "
			    + timeStatistics.average // Durchschnittliche.
			    + " NS\nout = "
			    + timeStatistics.outlier // Ausreißende.
			    + " NS\n\n"
			);
		}

		static void Main (string[] args)
		{
			Action test = null;
			TimeStatistics timeStatistics = new TimeStatistics ();

			PrintTimerProperties ();

			// Tests ...

			timeStatistics.name = "Knoten-Laden: Knoten mit 100 Kanten, 100 WH:"; // todo
			test = () => ExtremeKnots.LoadSquareKnot ("Square-Knot_100");
			timeStatistics = StopTime (test, 100, timeStatistics);
			PrintTimeStatistics (timeStatistics);

			// ...
			// test = ...
			// timeStatistics = ...
		}
	}
}