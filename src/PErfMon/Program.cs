﻿using System.Diagnostics;

namespace PerfMon;

public static class Program
{
    public static void Main(string[] args)
    {
        var iterations = 1000;

        var stopwatch = Stopwatch.StartNew();

        Tests.Derive(iterations);

        stopwatch.Stop();
        Console.WriteLine($"Derive {iterations} times in {stopwatch.ElapsedMilliseconds} ms");
    }
}
