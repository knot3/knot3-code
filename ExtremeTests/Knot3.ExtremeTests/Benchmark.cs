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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Knot3.Framework.Platform;

using Knot3.Game.Data;

using Knot3.MockObjects;

namespace Knot3.ExtremeTests
{
    public struct TimeStatistics {
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
        public static void PrintTimeStatistics (TimeStatistics timeStatistics, string description)
        {
            long nanosToMilis = 1000L * 1000L;
            long milisToSecs = 1000L;

            // Langsamste Zeit ...
            long maxNanos = timeStatistics.slowest;
            long maxMilis = maxNanos / nanosToMilis;
            long maxSecs = maxMilis / milisToSecs;

            // Schnellste Zeit ...
            long minNanos = timeStatistics.fastest;
            long minMilis = minNanos / nanosToMilis;
            long minSecs = minMilis / milisToSecs;

            // Mittlere Zeit ...
            long avgNanos = timeStatistics.average;
            long avgMilis = avgNanos / nanosToMilis;
            long avgSecs = avgMilis / milisToSecs;

            // Ausreißer ...
            long outNanos = timeStatistics.outlier;
            long outMilis = outNanos / nanosToMilis;
            long outSecs = outMilis / milisToSecs;

            Console.Write (
                // "Name: "
                description + "\n"
                + "max = " + maxNanos + " NS >= " + maxMilis + " MS >= " + maxSecs + " S\n"
                + "min = " + minNanos + " NS >= " + minMilis + " MS >= " + minSecs + " S\n"
                + "avg = " + avgNanos + " NS >= " + avgMilis + " MS >= " + avgSecs + " S\n"
                + "out = " + outNanos + " NS >= " + outMilis + " MS >= " + outSecs + " S\n"
            );
        }

        public static void setUp ()
        {
            ExtremeKnots.generateTestKnots ();
        }

        static void Main (string[] args)
        {
            Action test = null;
            string description = null;
            TimeStatistics timeStatistics = new TimeStatistics ();

            // setUp ();
            PrintTimerProperties ();

            //foreach (int length in ExtremeKnots.SquareKnot_TestLengths) {
            /*
            description = "Knoten-Erzeugen: Knoten mit 100 Kanten, 100 WH:";
            test = () => ExtremeKnots.CreateSquareKnot (100, "");
            timeStatistics = StopTime (test, 100, timeStatistics);
            PrintTimeStatistics (timeStatistics, description);
            */
            // }
            /*
            int my_length = 100;

            description = "Knoten-Erzeugen: Knoten mit " + my_length + " Kanten, " + my_length + " WH:";
            test = () => ExtremeKnots.CreateSquareKnot (my_length, "");
            timeStatistics = StopTime (test, my_length, timeStatistics);
            PrintTimeStatistics (timeStatistics, description);
            */

            description = "Knoten-Laden: Knoten mit 10000 Kanten, 100 WH:";
            test = () => ExtremeKnots.LoadSquareKnot (SystemInfo.SavegameDirectory + "\\SquareKnot10000.knot");
            timeStatistics = StopTime (test, 100, timeStatistics);
            PrintTimeStatistics (timeStatistics, description);

            /*
            Knot knot = KnotGenerator.generateSquareKnot (10000 / 4, "SquareKnot10000");
            KnotFileIO knotFileIO = new KnotFileIO ();
            knotFileIO.Save (knot);
            */

            System.Console.Write (ExtremeKnots.knot);
        }
    }
}
