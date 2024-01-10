using System.Diagnostics;

namespace PerfMon;

public static class Program
{
    public static void Main(string[] args)
    {
        var stopwatch = Stopwatch.StartNew();
        var iterations = 100;

        //for (var i = 0; i < iterations; i++)
        {
            Tests.Derive();
        }

        stopwatch.Stop();
        Console.WriteLine($"GenerateAndSign {iterations} times in {stopwatch.ElapsedMilliseconds} ms");
    }
}
