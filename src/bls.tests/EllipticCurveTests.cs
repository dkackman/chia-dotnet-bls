using chia.dotnet.bls;
using System.Numerics;

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
        Assert.True(g2.Add(g2).Equals(g2.Multiply(2)));
    }

    [Fact]
    public void G2FiveMultiplicationAndAddition()
    {
        Assert.True(
            g2.Multiply(new BigInteger(5))
            .Equals(g2.Multiply(2).Add(g2.Multiply(2)).Add(g2))
        );
    }

    [Fact]
    public void YForX()
    {
        var y = EcMethods.YForX(g2.X, Constants.DefaultEcTwist);
        Assert.True(y.Equals(g2.Y) || y.Negate().Equals(g2.Y));
    }

    [Fact]
    public void BackAndForth()
    {
        var g_j = JacobianPoint.GenerateG1();
        Assert.True(g_j.ToAffine().ToJacobian().Equals(g_j));
    }

    [Fact]
    public void MultiplicationOrder()
    {
        var g_j = JacobianPoint.GenerateG1();
        Assert.True(g_j.Multiply(2).ToAffine().Equals(g_j.ToAffine().Multiply(2)));
    }

    [Fact]
    public void AdditionOrderAndIdentity()
    {
        var g2_j = JacobianPoint.GenerateG2();
        var g2_j2 = JacobianPoint.GenerateG2().Multiply(2);
        Assert.True(g2_j.Add(g2_j2).ToAffine().Equals(g2_j.ToAffine().Multiply(3)));
    }
}