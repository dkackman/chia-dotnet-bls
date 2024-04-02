using chia.dotnet.bls;

namespace bls.tests;

public class ElementTests
{
    [Fact]
    public void G1Infinity()
    {
        var infinity = G1Element.GetInfinity();
        Assert.True(infinity.IsInfinity);
    }

    [Fact]
    public void G2Infinity()
    {
        var infinity = G2Element.GetInfinity();
        Assert.True(infinity.IsInfinity);
    }
}