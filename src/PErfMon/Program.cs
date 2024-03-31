using System.Diagnostics;

namespace PerfMon;

public static class Program
{
    public static void Main(string[] args)
    {
        var iterations = 10000;

        var stopwatch = Stopwatch.StartNew();

        //Tests.Derive(iterations);
        Tests.GenerateKeyPairs(iterations);

        stopwatch.Stop();
        Console.WriteLine($"GenerateKeyPairs {iterations} times in {stopwatch.ElapsedMilliseconds} ms");
    }
}
