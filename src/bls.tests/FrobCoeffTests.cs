using System.Numerics;
using chia.dotnet.bls;

namespace bls.tests;

public class FrobCoeffsTests
{
    private readonly BigInteger q = Constants.Q;
    private readonly Fq one;
    private readonly Fq two;
    private readonly Fq2 a3;
    private readonly Fq6 b3;
    private readonly Fq12 c3;

    public FrobCoeffsTests()
    {
        one = new Fq(q, BigInteger.One);
        two = one.Add(one);
        a3 = new Fq2(q, two, two);
        b3 = new Fq6(q, a3, a3, a3);
        c3 = new Fq12(q, b3, b3);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(6, 1)]
    [InlineData(6, 2)]
    [InlineData(6, 3)]
    [InlineData(6, 4)]
    [InlineData(6, 5)]
    [InlineData(12, 1)]
    [InlineData(12, 2)]
    [InlineData(12, 3)]
    [InlineData(12, 4)]
    [InlineData(12, 5)]
    [InlineData(12, 6)]
    [InlineData(12, 7)]
    [InlineData(12, 8)]
    [InlineData(12, 9)]
    [InlineData(12, 10)]
    [InlineData(12, 11)]
    public void ExtensionExponentEquality(int extension, int expo)
    {
        var baseValue = extension switch
        {
            2 => a3,
            6 => b3,
            12 => c3,
            _ => throw new ArgumentException("Invalid extension", nameof(extension))
        };
        var qi = baseValue.QiPower(expo);
        var pow = baseValue.Pow(BigInteger.Pow(q, expo));
        Assert.True(qi.Equals(pow));
    }
}