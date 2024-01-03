using System.Numerics;
using chia.dotnet.bls;

namespace bls.tests;

public class EllipticCurveTests
{
    private readonly JacobianPoint g = JacobianPoint.GenerateG1();

    [Fact]
    public void IsOnCurve()
    {
        Assert.True(g.IsOnCurve());
    }

    [Fact]
    public void DoubleSameAsAddition()
    {
        Assert.True(g.Multiply(2).Equals(g.Add(g)));
    }

    [Fact]
    public void TripleOnCurve()
    {
        Assert.True(g.Multiply(3).IsOnCurve());
    }

    [Fact]
    public void TripleSameAsAddition()
    {
        Assert.True(g.Multiply(3).Equals(g.Add(g).Add(g)));
    }
}