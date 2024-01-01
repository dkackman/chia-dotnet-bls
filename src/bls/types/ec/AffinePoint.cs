using System.Numerics;
using System.Diagnostics;

namespace chia.dotnet.bls;

public class AffinePoint
{
    public Fq X { get; set; }
    public Fq Y { get; set; }
    public bool IsInfinity { get; set; }
    public EC Ec { get; set; }

    public AffinePoint(Fq x, Fq y, bool isInfinity, EC? ec = null)
    {
        Ec = ec ?? Constants.DefaultEc;
        Debug.Assert(x.GetType() == y.GetType());
        X = x;
        Y = y;
        IsInfinity = isInfinity;
    }

    public bool IsOnCurve()
    {
        return IsInfinity || Y.Multiply(Y).Equals(X.Multiply(X).Multiply(X).Add(Ec.A.Multiply(X)).Add(Ec.B));
    }

    public JacobianPoint ToJacobian() => new(X, Y, X.One(Ec.Q), IsInfinity, Ec);

    public AffinePoint Twist()
    {
        var f = (Fq12)Fq12.Nil.One(Ec.Q);
        var wsq = new Fq12(Ec.Q, (Fq6)f.Root, (Fq6)Fq6.Nil.Zero(Ec.Q));
        var wcu = new Fq12(Ec.Q, (Fq6)Fq6.Nil.Zero(Ec.Q), (Fq6)f.Root);
        return new AffinePoint(X.Multiply(wsq), Y.Multiply(wcu), false, Ec);
    }

    public AffinePoint Untwist()
    {
        var f = (Fq12)Fq12.Nil.One(Ec.Q);
        var wsq = new Fq12(Ec.Q, (Fq6)f.Root, (Fq6)Fq6.Nil.Zero(Ec.Q));
        var wcu = new Fq12(Ec.Q, (Fq6)Fq6.Nil.Zero(Ec.Q), (Fq6)f.Root);
        return new AffinePoint(X.Divide(wsq), Y.Divide(wcu), false, Ec);
    }

    public AffinePoint Double()
    {
        var left = X.Multiply(X).Multiply(new Fq(Ec.Q, 3)).Add(Ec.A);
        var s = left.Divide(Y.Multiply(new Fq(Ec.Q, 2)));
        var newX = s.Multiply(s).Subtract(X).Subtract(X);
        var newY = s.Multiply(X.Subtract(newX)).Subtract(Y);
        return new AffinePoint(newX, newY, false, Ec);
    }

    public AffinePoint Add(AffinePoint value)
    {
        Debug.Assert(IsOnCurve());
        Debug.Assert(value.IsOnCurve());
        if (IsInfinity) return value;
        else if (value.IsInfinity) return this;
        else if (Equals(value)) return Double();
        var s = value.Y.Subtract(Y).Divide(value.X.Subtract(X));
        var newX = s.Multiply(s).Subtract(X).Subtract(value.X);
        var newY = s.Multiply(X.Subtract(newX)).Subtract(Y);
        return new(newX, newY, false, Ec);
    }

    public AffinePoint Subtract(AffinePoint value) => Add(value.Negate());

    public AffinePoint Multiply(BigInteger value) => EcMethods.ScalarMultJacobian(value, ToJacobian(), Ec).ToAffine();
    public AffinePoint Multiply(Fq value) => EcMethods.ScalarMultJacobian(value.Value, ToJacobian(), Ec).ToAffine();

    public AffinePoint Negate() => new(X, Y.Negate(), IsInfinity, Ec);

    public bool Equals(AffinePoint value) => X.Equals(value.X) && Y.Equals(value.Y) && IsInfinity == value.IsInfinity;

    public AffinePoint Clone() => new(X.Clone(), Y.Clone(), IsInfinity, Ec);

    public override string ToString()
    {
        return $"AffinePoint(x={X}, y={Y}, i={IsInfinity})";
    }
}