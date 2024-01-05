using System.Numerics;
using chia.dotnet.bls;

namespace bls.tests;

public class EllipticCurveTests
{
    private readonly JacobianPoint g = JacobianPoint.GenerateG1();
    private readonly JacobianPoint g2 = JacobianPoint.GenerateG2();
    private readonly BigInteger q = Constants.Q;
    private readonly JacobianPoint s;

    public EllipticCurveTests()
    {
        s = g2.Add(g2);
    }

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
    public void UntwistIdentity()
    {
        Assert.True(s.ToAffine().Twist().Untwist().Equals(s.ToAffine()));
    }

    [Fact]
    public void MultiplicationWithoutTwistIdentity()
    {
        Assert.True(
            s.ToAffine().Twist().Multiply(5).Untwist().Equals(s.Multiply(5).ToAffine())
        );
    }

    [Fact]
    public void MultiplicationWithTwistIdentity()
    {
        Assert.True(
            s.ToAffine().Twist().Multiply(5).Equals(s.Multiply(5).ToAffine().Twist())
        );
    }

    [Fact]
    public void DoubleOnCurve()
    {
        Assert.True(s.IsOnCurve());
    }

    [Fact]
    public void G2IsOnCurve()
    {
        Assert.True(g2.IsOnCurve());
    }

    [Fact]
    public void G2DoubleSameAsAddition()
    {
        Assert.True(g2.Add(g2).Equals(g2.Multiply(new BigInteger(2))));
    }

    [Fact]
    public void G2FiveMultiplicationAndAddition()
    {
        Assert.True(
            g2.Multiply(new BigInteger(5))
            .Equals(g2.Multiply(new BigInteger(2)).Add(g2.Multiply(new BigInteger(2))).Add(g2))
        );
    }
}