using System.Numerics;
using chia.dotnet.bls;

namespace bls.tests;

public class EllipticCurveTests
{
    private readonly JacobianPoint g = JacobianPoint.GenerateG1();
    private readonly JacobianPoint g2 = JacobianPoint.GenerateG2();
    private readonly BigInteger q = Constants.Q;

    [Fact]
    public void G1IsOnCurve()
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

    [Fact]
    public void IndividualMultiplication()
    {
        Assert.True(
            g2.X.Multiply(new Fq(q, 2).Multiply(g2.Y)).Equals(new Fq(q, 2).Multiply(g2.X.Multiply(g2.Y)))
        );
    }

    [Fact]
    public void G2IsOnCurve()
    {
        Assert.True(g2.IsOnCurve());
    }
}