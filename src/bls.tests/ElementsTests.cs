
using System.Numerics;
using chia.dotnet.bls;

namespace bls.tests;

public class ElementsTests
{
    private BigInteger i1 = new byte[] { 1, 2 }.BytesToBigInt(Endian.Big);
    private BigInteger i2 = new byte[] { 3, 1, 4, 1, 5, 9 }.BytesToBigInt(Endian.Big);
    private BigInteger b1;
    private BigInteger b2;
    private JacobianPoint g1 = JacobianPoint.GenerateG1();
    private JacobianPoint g2 = JacobianPoint.GenerateG2();
    private JacobianPoint u1 = JacobianPoint.InfinityG1();
    private JacobianPoint u2 = JacobianPoint.InfinityG2();
    private JacobianPoint x1;
    private JacobianPoint x2;
    private JacobianPoint y1;
    private JacobianPoint y2;

    public ElementsTests()
    {
        b1 = i1;
        b2 = i2;

        x1 = g1.Multiply(b1);
        x2 = g1.Multiply(b2);
        y1 = g2.Multiply(b1);
        y2 = g2.Multiply(b2);
    }

    [Fact]
    public void G1MultiplicationEquality()
    {
        Assert.False(x1.Equals(x2));
        Assert.True(x1.Multiply(b1).Equals(x1.Multiply(b1)));
        Assert.False(x1.Multiply(b1).Equals(x1.Multiply(b2)));
    }

    [Fact]
    public void G1AdditionEquality()
    {
        var left = x1.Add(u1);
        var right = x1;

        Assert.True(left.Equals(right));
        Assert.True(x1.Add(x2).Equals(x2.Add(x1)));
        Assert.True(x1.Add(x1.Negate()).Equals(u1));
        Assert.True(x1.Equals(JacobianPoint.FromBytesG1(x1.ToBytes())));
    }

    [Fact]
    public void G1Copy()
    {
        var copy = x1.Clone();
        var new_x1 = x1.Add(x2);
        Assert.True(x1.Equals(copy));
        Assert.False(new_x1.Equals(copy));
    }

    [Fact]
    public void G2MultiplicationEquality()
    {
        Assert.False(y1.Equals(y2));
        Assert.True(y1.Multiply(b1).Equals(y1.Multiply(b1)));
        Assert.False(y1.Multiply(b1).Equals(y1.Multiply(b2)));
    }

    [Fact]
    public void G2AdditionEquality()
    {
        Assert.True(y1.Add(u2).Equals(y1));
        Assert.True(y1.Add(y2).Equals(y2.Add(y1)));
        Assert.True(y1.Add(y1.Negate()).Equals(u2));
        Assert.True(y1.Equals(JacobianPoint.FromBytesG2(y1.ToBytes())));
    }

    [Fact]
    public void G2Copy()
    {
        var copy2 = y1.Clone();
        var new_y1 = y1.Add(y2);

        Assert.True(y1.Equals(copy2));
        Assert.False(new_y1.Equals(copy2));
    }

    // TODO atePairing tests
}