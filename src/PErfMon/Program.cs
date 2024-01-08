using chia.dotnet.bls;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

public static class Program

{
    public static void Main(string[] args)
    {
        var sk = AugSchemeMPL.KeyGen(seed);
        var pk = sk.GetG1();
        var signature = AugSchemeMPL.Sign(sk, message);

        Console.WriteLine(signature);
    }

    private static readonly byte[] message = [1, 2, 3, 4, 5];
    private static readonly byte[] seed =
    [
        0,
        50,
        6,
        244,
        24,
        199,
        1,
        25,
        52,
        88,
        192,
        19,
        18,
        12,
        89,
        6,
        220,
        18,
        102,
        58,
        209,
        82,
        12,
        62,
        89,
        110,
        182,
        9,
        44,
        20,
        254,
        22,
    ];
}
