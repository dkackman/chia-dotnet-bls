using chia.dotnet.bls;

namespace bls.tests;

public class CurrentTests
{
    private readonly byte[] seed =
    [
        0, 50, 6, 244, 24, 199, 1, 25, 52, 88, 192, 19, 18, 12, 89, 6, 220, 18,
        102, 58, 209, 82, 12, 62, 89, 110, 182, 9, 44, 20, 254, 22,
    ];
    private readonly byte[] message = [1, 2, 3, 4, 5];

    [Fact]
    public void IsVerified()
    {
        var sk = AugSchemeMPL.KeyGen(seed);
        var pk = sk.GetG1();
        var signature = AugSchemeMPL.Sign(sk, message);
        Assert.True(AugSchemeMPL.Verify(pk, message, signature));
    }
}