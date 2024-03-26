using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a point in Jacobian coordinates on an elliptic curve.
/// It can represent both a PublicKey and a Signature.
/// </summary>
public readonly struct JacobianPoint
{
    private readonly static IFq four = new Fq(Constants.Q, 4);
    private readonly static IFq eight = new Fq(Constants.Q, 8);

    internal IFq X { get; }
    internal IFq Y { get; }
    internal IFq Z { get; }

    /// <summary>
    /// A flag indicating whether the point is at infinity.
    /// </summary>
    public bool IsInfinity { get; }
    internal EC Ec { get; }

    private readonly IFq x_zero;
    private readonly IFq x_one;
    private static readonly int[] sourceArray = [0x20, 0x60, 0xe0];

    /// <summary>
    /// Represents a point in Jacobian coordinates on an elliptic curve.
    /// </summary>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <param name="z">The z-coordinate of the point.</param>
    /// <param name="isInfinity">A flag indicating whether the point is at infinity.</param>
    /// <param name="ec">The elliptic curve associated with the point (optional).</param>
    internal JacobianPoint(IFq x, IFq y, IFq z, bool isInfinity, EC ec)
    {
        Debug.Assert(x.GetType() == y.GetType());
        Debug.Assert(y.GetType() == z.GetType());
        X = x;
        Y = y;
        Z = z;
        IsInfinity = isInfinity;
        Ec = ec;
        x_zero = X.Zero(Ec.Q);
        x_one = X.One(Ec.Q);
    }

    /// <summary>
    /// Creates a JacobianPoint from a byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="isExtension"></param>
    /// <returns><see cref="JacobianPoint"/></returns>
    public static JacobianPoint FromBytes(byte[] bytes, bool isExtension) => FromBytes(bytes, isExtension, Constants.DefaultEc);

    internal static JacobianPoint FromBytes(byte[] bytes, bool isExtension, EC ec)
    {
        if (isExtension)
        {
            if (bytes.Length != 96)
                throw new Exception("Expected 96 bytes.");
        }
        else
        {
            if (bytes.Length != 48)
                throw new Exception("Expected 48 bytes.");
        }

        var mByte = bytes[0] & 0xe0;
        if (sourceArray.Contains(mByte))
            throw new Exception("Invalid first three bits.");

        var compressed = (mByte & 0x80) != 0;
        var infinity = (mByte & 0x40) != 0;
        var signed = (mByte & 0x20) != 0;
        if (!compressed)
            throw new Exception("Compression bit must be 1.");

        bytes[0] &= 0x1f;

        var nil = isExtension ? Fq2.Nil : Fq.Nil;

        if (infinity)
        {
            if (bytes.Any(b => b != 0))
                throw new Exception("Point at infinity, but found non-zero byte.");

            var nilZero = nil.Zero(ec.Q);
            return new AffinePoint(
                nilZero,
                nilZero,
                true,
                ec
            ).ToJacobian();
        }

        var x = nil.FromBytes(ec.Q, bytes);
        var yValue = EcMethods.YForX(x, ec);
        var sign = isExtension
            ? EcMethods.SignFq2((Fq2)yValue, ec)
            : EcMethods.SignFq(yValue, ec);
        var y = sign == signed ? yValue : yValue.Negate();

        return new AffinePoint(x, y, false, ec).ToJacobian();
    }

    /// <summary>
    /// Creates a JacobianPoint from a hexadecimal string.
    /// </summary>
    /// <param name="hex"></param>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint FromHex(string hex, bool isExtension) => FromBytes(hex.FromHex(), isExtension, Constants.DefaultEc);

    /// <summary>
    /// Creates a JacobianPoint 
    /// </summary>
    /// <returns></returns>
    public static JacobianPoint GenerateG1() => new AffinePoint(Constants.DefaultEc.Gx, Constants.DefaultEc.Gy, false, Constants.DefaultEc).ToJacobian();

    /// <summary>
    /// Creates a JacobianPoint 
    /// </summary>
    /// <returns></returns>
    public static JacobianPoint GenerateG2() => new AffinePoint(Constants.DefaultEcTwist.G2x, Constants.DefaultEcTwist.G2y, false, Constants.DefaultEcTwist).ToJacobian();

    /// <summary>
    /// Creates a JacobianPoint at the G1 Infinity point.
    /// </summary>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint InfinityG1(bool isExtension)
    {
        var nil = isExtension ? Fq2.Nil : Fq.Nil;

        var x = nil.Zero(Constants.DefaultEc.Q);
        return new JacobianPoint(
            x,
            x,
            x,
            true,
            Constants.DefaultEc
        );
    }

    /// <summary>
    /// Creates a JacobianPoint at the G1 Infinity point.
    /// </summary>
    /// <returns></returns>
    public static JacobianPoint InfinityG1()
    {
        var x = Fq.Nil.Zero(Constants.DefaultEc.Q);
        return new JacobianPoint(
            x,
            x,
            x,
            true,
            Constants.DefaultEc
        );
    }
    /// <summary>
    /// Creates a JacobianPoint at the G2 Infinity point.
    /// </summary>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint InfinityG2(bool isExtension)
    {
        var nil = isExtension ? Fq2.Nil : Fq.Nil;
        var x = nil.Zero(Constants.DefaultEcTwist.Q);

        return new JacobianPoint(
            x,
            x,
            x,
            true,
            Constants.DefaultEcTwist
        );
    }

    /// <summary>
    /// Creates a JacobianPoint at the G2 Infinity point.
    /// </summary>
    /// <returns></returns>
    public static JacobianPoint InfinityG2()
    {
        var x = Fq2.Nil.Zero(Constants.DefaultEcTwist.Q);

        return new JacobianPoint(
            x,
            x,
            x,
            true,
            Constants.DefaultEcTwist
        );
    }

    /// <summary>
    /// Creates a JacobianPoint from a byte array. 
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint FromBytesG1(byte[] bytes, bool isExtension = false) => FromBytes(bytes, isExtension, Constants.DefaultEc);

    /// <summary>
    /// Creates a JacobianPoint from a byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint FromBytesG2(byte[] bytes, bool isExtension = true) => FromBytes(bytes, isExtension, Constants.DefaultEcTwist);

    /// <summary>
    /// Creates a JacobianPoint from a hexadecimal string.
    /// </summary>
    /// <param name="hex"></param>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint FromHexG1(string hex, bool isExtension = false) => FromBytesG1(hex.FromHex(), isExtension);

    /// <summary>
    /// Creates a JacobianPoint from a hexadecimal string.
    /// </summary>
    /// <param name="hex"></param>
    /// <param name="isExtension"></param>
    /// <returns></returns>
    public static JacobianPoint FromHexG2(string hex, bool isExtension = true) => FromBytesG2(hex.FromHex(), isExtension);

    /// <summary>
    /// Checks if the point is on the curve.
    /// </summary>
    /// <returns></returns>
    public bool IsOnCurve() => IsInfinity || ToAffine().IsOnCurve;

    /// <summary>
    /// Checks if the point is valid.`
    /// </summary>
    /// <returns></returns>
    public bool IsValid() => IsOnCurve() && Multiply(Ec.N).Equals(X is Fq ? InfinityG1() : InfinityG2());

    /// <summary>
    /// Gets the fingerprint of the point.
    /// </summary>
    /// <returns></returns>
    public long GetFingerprint() => Hmac.Hash256(ToBytes()).Take(4).ToArray().BytesToInt(Endian.Big);

    /// <summary>
    /// Converts the point to an AffinePoint.
    /// </summary>
    /// <returns><see cref="AffinePoint"/> </returns>
    ///     
    public AffinePoint ToAffine() => IsInfinity
            ? new AffinePoint(
                x_zero,
                Y.Zero(Ec.Q),
                true,
                Ec
            )
            : new AffinePoint(
                X.Divide(Z.Pow(2)),
                Y.Divide(Z.Pow(3)),
                false,
                Ec
            );



    /// <summary>
    /// Converts the point to a byte array.
    /// </summary>
    /// <returns>The byte array</returns>
    public byte[] ToBytes()
    {
        var point = ToAffine();
        var output = point.X.ToBytes();
        if (point.IsInfinity)
        {
            var bytes = new byte[output.Length];
            bytes[0] = 0xc0;
            return bytes;
        }

        var sign = point.Y is Fq2 fq ? EcMethods.SignFq2(fq, Ec) : EcMethods.SignFq(point.Y, Ec);
        output[0] |= (byte)(sign ? 0xa0 : 0x80);

        return output;
    }

    /// <summary>
    /// Converts the point to a hexadecimal string.
    /// </summary>
    /// <returns>Hex string</returns>
    public string ToHex() => ToBytes().ToHex();

    /// <summary>
    /// Converts the point to a string. This is an alias for <see cref="ToHex"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => ToHex();

    /// <summary>
    /// Doubles the point.
    /// </summary>
    /// <returns><see cref="JacobianPoint"/> </returns>
    public JacobianPoint Double()
    {
        if (IsInfinity || Y.Equals(x_zero))
        {
            return new JacobianPoint(
                      x_one,
                      x_one,
                      x_zero,
                      true,
                      Ec
                  );
        }

        var S = X.Multiply(Y).Multiply(Y).Multiply(four);
        var Z_sq = Z.Multiply(Z);
        var Z_4th = Z_sq.Multiply(Z_sq);
        var Y_sq = Y.Multiply(Y);
        var Y_4th = Y_sq.Multiply(Y_sq);
        var threeXsq = X.Multiply(X).Multiply(Ec.Three);
        var M = threeXsq.Add(Ec.A.Multiply(Z_4th));
        var twoS = S.Multiply(Ec.Two);
        var X_p = M.Multiply(M).Subtract(twoS);
        var Y_p = M.Multiply(S.Subtract(X_p)).Subtract(Y_4th.Multiply(eight));
        var Z_p = Y.Multiply(Z).Multiply(Ec.Two);

        return new JacobianPoint(X_p, Y_p, Z_p, false, Ec);
    }

    /// <summary>
    /// Negates the point.
    /// </summary>
    /// <returns><see cref="JacobianPoint"/></returns>
    public JacobianPoint Negate() => ToAffine().Negate().ToJacobian();

    /// <summary>
    /// Adds the point to another point.
    /// </summary>
    /// <param name="value">The other point</param>
    /// <returns>The resulting point</returns>
    public JacobianPoint Add(JacobianPoint value)
    {
        if (IsInfinity)
        {
            return value;
        }

        if (value.IsInfinity)
        {
            return this;
        }

        var Z1_sq = Z.Pow(2);
        var Z2_sq = value.Z.Pow(2);

        var U1 = X.Multiply(Z2_sq);
        var U2 = value.X.Multiply(Z1_sq);
        var S1 = Y.Multiply(value.Z).Multiply(Z2_sq);
        var S2 = value.Y.Multiply(Z).Multiply(Z1_sq);

        if (U1.Equals(U2))
        {
            if (!S1.Equals(S2))
            {
                return new JacobianPoint(
                    x_one,
                    x_one,
                    x_zero,
                    true,
                    Ec
                );
            }

            return Double();
        }

        var H = U2.Subtract(U1);
        var R = S2.Subtract(S1);
        var H_sq = H.Multiply(H);
        var U1H_sq = U1.Multiply(H_sq);
        var H_cu = H.Multiply(H_sq);

        var X3 = R.Multiply(R).Subtract(H_cu).Subtract(U1H_sq.Multiply(Ec.Two));
        var Y3 = R.Multiply(U1H_sq.Subtract(X3)).Subtract(S1.Multiply(H_cu));
        var Z3 = H.Multiply(Z).Multiply(value.Z);

        return new JacobianPoint(X3, Y3, Z3, false, Ec);
    }

    internal JacobianPoint Multiply(Fq value) => EcMethods.ScalarMultJacobian(value.Value, this, Ec);

    /// <summary>
    /// Multiplies the point by a scalar.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public JacobianPoint Multiply(BigInteger value) => EcMethods.ScalarMultJacobian(value, this, Ec);

    /// <summary>
    /// Compares the point to another point.
    /// </summary>
    /// <param name="value">The other point</param>
    /// <returns>True if they are equal</returns>
    public bool Equals(JacobianPoint value) => ToAffine().Equals(value.ToAffine());

    /// <summary>
    /// Clones the point.
    /// </summary>
    /// <returns>The cloned point</returns>
    public JacobianPoint Clone() => new(
            X.Clone(),
            Y.Clone(),
            Z.Clone(),
            IsInfinity,
            Ec
        );
}