using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

/// <summary>
/// Represents an affine point on an elliptic curve.
/// </summary>
public readonly struct AffinePoint
{
    /// <summary>
    /// Gets the X-coordinate of the affine point.
    /// </summary>
    internal Fq X { get; }

    /// <summary>
    /// Gets the Y-coordinate of the affine point.
    /// </summary>
    internal Fq Y { get; }

    /// <summary>
    /// Gets a value indicating whether the affine point is at infinity.
    /// </summary>
    public bool IsInfinity { get; }

    /// <summary>
    /// Gets the elliptic curve associated with the affine point.
    /// </summary>
    internal EC Ec { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AffinePoint"/> class.
    /// </summary>
    /// <param name="x">The X-coordinate of the affine point.</param>
    /// <param name="y">The Y-coordinate of the affine point.</param>
    /// <param name="isInfinity">A value indicating whether the affine point is at infinity.</param>
    /// <param name="ec">The elliptic curve associated with the affine point.</param>
    internal AffinePoint(Fq x, Fq y, bool isInfinity, EC ec)
    {
        Ec = ec;

        Debug.Assert(x.GetType() == y.GetType());
        X = x;
        Y = y;
        IsInfinity = isInfinity;
    }

    /// <summary>
    /// Gets a value indicating whether the affine point is on the curve.
    /// </summary>
    public bool IsOnCurve => IsInfinity || Y.Multiply(Y).Equals(X.Multiply(X).Multiply(X).Add(Ec.A.Multiply(X)).Add(Ec.B));

    /// <summary>
    /// Converts the affine point to its Jacobian representation.
    /// </summary>
    /// <returns>The Jacobian representation of the affine point.</returns>
    public JacobianPoint ToJacobian() => new(X, Y, X.One(Ec.Q), IsInfinity, Ec);

    /// <summary>
    /// Twists the affine point.
    /// </summary>
    /// <returns>The twisted affine point.</returns>
    public AffinePoint Twist()
    {
        var one = (Fq12)Fq12.Nil.One(Ec.Q);
        var zero = (Fq6)Fq6.Nil.Zero(Ec.Q);
        var wsq = new Fq12(Ec.Q, (Fq6)one.Root, zero);
        var wcu = new Fq12(Ec.Q, zero, (Fq6)one.Root);

        return new AffinePoint(X.Multiply(wsq), Y.Multiply(wcu), false, Ec);
    }

    /// <summary>
    /// Untwists the affine point.
    /// </summary>
    /// <returns>The untwisted affine point.</returns>
    public AffinePoint Untwist()
    {
        var one = (Fq12)Fq12.Nil.One(Ec.Q);
        var zero = (Fq6)Fq6.Nil.Zero(Ec.Q);
        var wsq = new Fq12(Ec.Q, (Fq6)one.Root, zero);
        var wcu = new Fq12(Ec.Q, zero, (Fq6)one.Root);

        return new AffinePoint(X.Divide(wsq), Y.Divide(wcu), false, Ec);
    }


    /// <summary>
    /// Doubles the affine point.
    /// </summary>
    /// <returns>The doubled affine point.</returns>
    public AffinePoint Double()
    {
        var left = X.Multiply(X).Multiply(new Fq(Ec.Q, 3)).Add(Ec.A);
        var s = left.Divide(Y.Multiply(new Fq(Ec.Q, 2)));
        var newX = s.Multiply(s).Subtract(X).Subtract(X);
        var newY = s.Multiply(X.Subtract(newX)).Subtract(Y);

        return new AffinePoint(newX, newY, false, Ec);
    }

    /// <summary>
    /// Adds another affine point to the current affine point.
    /// </summary>
    /// <param name="value">The affine point to add.</param>
    /// <returns>The sum of the two affine points.</returns>
    public AffinePoint Add(AffinePoint value)
    {
        Debug.Assert(IsOnCurve);
        Debug.Assert(value.IsOnCurve);

        if (IsInfinity)
        {
            return value;
        }

        if (value.IsInfinity)
        {
            return this;
        }

        if (Equals(value))
        {
            return Double();
        }

        var s = value.Y.Subtract(Y).Divide(value.X.Subtract(X));
        var newX = s.Multiply(s).Subtract(X).Subtract(value.X);
        var newY = s.Multiply(X.Subtract(newX)).Subtract(Y);

        return new(newX, newY, false, Ec);
    }

    /// <summary>
    /// Subtracts another affine point from the current affine point.
    /// </summary>
    /// <param name="value">The affine point to subtract.</param>
    /// <returns>The difference between the two affine points.</returns>
    public AffinePoint Subtract(AffinePoint value) => Add(value.Negate());

    /// <summary>
    /// Multiplies the affine point by a scalar value.
    /// </summary>
    /// <param name="value">The scalar value.</param>
    /// <returns>The result of the scalar multiplication.</returns>
    public AffinePoint Multiply(BigInteger value) => EcMethods.ScalarMultJacobian(value, ToJacobian(), Ec).ToAffine();

    /// <summary>
    /// Multiplies the affine point by a field element.
    /// </summary>
    /// <param name="value">The field element.</param>
    /// <returns>The result of the scalar multiplication.</returns>
    internal AffinePoint Multiply(Fq value) => EcMethods.ScalarMultJacobian(value.Value, ToJacobian(), Ec).ToAffine();

    /// <summary>
    /// Negates the affine point.
    /// </summary>
    /// <returns>The negated affine point.</returns>
    public AffinePoint Negate() => new(X, Y.Negate(), IsInfinity, Ec);

    /// <summary>
    /// Determines whether the specified object is equal to the current affine point.
    /// </summary>
    /// <param name="value">The object to compare with the current affine point.</param>
    /// <returns><c>true</c> if the specified object is equal to the current affine point; otherwise, <c>false</c>.</returns>
    public bool Equals(AffinePoint value) => X.Equals(value.X) && Y.Equals(value.Y) && IsInfinity == value.IsInfinity;

    /// <summary>
    /// Creates a new instance that is a copy of the current affine point.
    /// </summary>
    /// <returns>A new instance that is a copy of the current affine point.</returns>
    public AffinePoint Clone() => new(X.Clone(), Y.Clone(), IsInfinity, Ec);

    /// <summary>
    /// Returns a string that represents the current affine point.
    /// </summary>
    /// <returns>A string that represents the current affine point.</returns>
    public override string ToString() => $"AffinePoint(x={X}, y={Y}, i={IsInfinity})";
}