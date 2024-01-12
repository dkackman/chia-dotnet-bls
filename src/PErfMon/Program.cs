using System.Diagnostics;

namespace PerfMon;

public static class Program
{
    public static void Main(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();
        var iterations = 100;

        Tests.Derive(iterations);

        stopwatch.Stop();
        Console.WriteLine($"Derive {iterations} times in {stopwatch.ElapsedMilliseconds} ms");
    }
}
