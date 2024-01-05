using System.Numerics;
using chia.dotnet.bls;

namespace bls.tests;

public class EdgeCaseSignFq2Tests
{
    private readonly BigInteger q = Constants.Q;
    [Fact]
    public void FirstNotSecond()
    {
        var a = new Fq(q, 62323);
        var testCase1 = new Fq2(q, a, new Fq(q, BigInteger.Zero));
        var testCase2 = new Fq2(q, a.Negate(), new Fq(q, BigInteger.Zero));
        Assert.False(EcMethods.SignFq2(testCase1).Equals(EcMethods.SignFq2(testCase2)));
    }

    [Fact]
    public void ThirdNotFourth()
    {
        var a = new Fq(q, 62323);
        var testCase3 = new Fq2(q, new Fq(q, BigInteger.Zero), a);
        var testCase4 = new Fq2(q, new Fq(q, BigInteger.Zero), a.Negate());
        Assert.False(EcMethods.SignFq2(testCase3).Equals(EcMethods.SignFq2(testCase4)));        
    }
}