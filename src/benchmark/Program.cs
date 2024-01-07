using System.Diagnostics;
using chia.dotnet.bls;

var msgBytes = "assertion".ToBytes();
var DST = "MY-DST";
var keyBytes = new string('*', 32).ToBytes();


var stopwatch = new Stopwatch();
stopwatch.Start();

for (var i = 0; i < 100; i++)
{
    var sk = PrivateKey.FromSeed(keyBytes);
    var g1 = sk.GetG1();
    var pk = AugSchemeMPL.DeriveChildPkUnhardened(g1, 0);
}

stopwatch.Stop();
Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
