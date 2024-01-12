using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

public class JacobianPoint
{
    public Fq X { get; }
    public Fq Y { get; }
    public Fq Z { get; }
    public bool IsInfinity { get; }
    public EC Ec { get; }

    // these are precomputed for performance
    private readonly Fq zero;
    private readonly Fq one;
    private readonly Fq two;

    private static readonly int[] sourceArray = [0x20, 0x60, 0xe0];

    /// <summary>
    /// Represents a point in Jacobian coordinates on an elliptic curve.
    /// </summary>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <param name="z">The z-coordinate of the point.</param>
    /// <param name="isInfinity">A flag indicating whether the point is at infinity.</param>
    /// <param name="ec">The elliptic curve associated with the point (optional).</param>
    public JacobianPoint(Fq x, Fq y, Fq z, bool isInfinity, EC? ec = null)
    {
        ec ??= Constants.DefaultEc;

        Debug.Assert(x.GetType() == y.GetType());
        Debug.Assert(y.GetType() == z.GetType());
        X = x;
        Y = y;
        Z = z;
        IsInfinity = isInfinity;
        Ec = ec;

        zero = X.Zero(Ec.Q);
        one = X.One(Ec.Q);
        two = new Fq(Ec.Q, 2);
    }

    public static JacobianPoint FromBytes(byte[] bytes, bool isExtension, EC? ec = null)
    {
        ec ??= Constants.DefaultEc;

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

            return new AffinePoint(
                nil.Zero(ec.Q),
                nil.Zero(ec.Q),
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

    public static JacobianPoint FromHex(string hex, bool isExtension, EC? ec = null)
    {
        ec ??= Constants.DefaultEc;

        return FromBytes(hex.FromHex(), isExtension, ec);
    }

    public static JacobianPoint GenerateG1() => new AffinePoint(Constants.DefaultEc.Gx, Constants.DefaultEc.Gy, false, Constants.DefaultEc).ToJacobian();

    public static JacobianPoint GenerateG2() => new AffinePoint(Constants.DefaultEcTwist.G2x, Constants.DefaultEcTwist.G2y, false, Constants.DefaultEcTwist).ToJacobian();

    public static JacobianPoint InfinityG1(bool isExtension = false)
    {
        var nil = isExtension ? Fq2.Nil : Fq.Nil;

        Fq x = nil.Zero(Constants.DefaultEc.Q);
        return new JacobianPoint(
            x,
            x,
            x,
            true,
            Constants.DefaultEc
        );
    }

    public static JacobianPoint InfinityG2(bool isExtension = true)
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

    public static JacobianPoint FromBytesG1(byte[] bytes, bool isExtension = false) => FromBytes(bytes, isExtension, Constants.DefaultEc);

    public static JacobianPoint FromBytesG2(byte[] bytes, bool isExtension = true) => FromBytes(bytes, isExtension, Constants.DefaultEcTwist);

    public static JacobianPoint FromHexG1(string hex, bool isExtension = false) => FromBytesG1(hex.FromHex(), isExtension);

    public static JacobianPoint FromHexG2(string hex, bool isExtension = true) => FromBytesG2(hex.FromHex(), isExtension);

    public bool IsOnCurve() => IsInfinity || ToAffine().IsOnCurve;

    public bool IsValid() => IsOnCurve() && Multiply(Ec.N).Equals(X is Fq ? InfinityG1() : InfinityG2());

    public long GetFingerprint() => Hmac.Hash256(ToBytes()).Take(4).ToArray().BytesToInt(Endian.Big);

    public AffinePoint ToAffine()
    {
        return IsInfinity
            ? new AffinePoint(
                zero,
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
    }

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

    public string ToHex() => ToBytes().ToHex();

    public override string ToString() => ToHex();

    public JacobianPoint Double()
    {
        if (IsInfinity || Y.Equals(zero))
        {
            return new JacobianPoint(
                      one,
                      one,
                      zero,
                      true,
                      Ec
                  );
        }
        var three = new Fq(Ec.Q, 3);
        var four = new Fq(Ec.Q, 4);
        var eight = new Fq(Ec.Q, 8);

        var S = X.Multiply(Y).Multiply(Y).Multiply(four);
        var Z_sq = Z.Multiply(Z);
        var Z_4th = Z_sq.Multiply(Z_sq);
        var Y_sq = Y.Multiply(Y);
        var Y_4th = Y_sq.Multiply(Y_sq);
        var threeXsq = X.Multiply(X).Multiply(three);
        var M = threeXsq.Add(Ec.A.Multiply(Z_4th));
        var twoS = S.Multiply(two);
        var X_p = M.Multiply(M).Subtract(twoS);
        var Y_p = M.Multiply(S.Subtract(X_p)).Subtract(Y_4th.Multiply(eight));
        var Z_p = Y.Multiply(Z).Multiply(two);

        return new JacobianPoint(X_p, Y_p, Z_p, false, Ec);
    }

    public JacobianPoint Negate() => ToAffine().Negate().ToJacobian();

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
                    one,
                    one,
                    zero,
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
        var X3 = R.Multiply(R).Subtract(H_cu).Subtract(U1H_sq.Multiply(two));
        var Y3 = R.Multiply(U1H_sq.Subtract(X3)).Subtract(S1.Multiply(H_cu));
        var Z3 = H.Multiply(Z).Multiply(value.Z);

        return new JacobianPoint(X3, Y3, Z3, false, Ec);
    }


    public JacobianPoint Multiply(Fq value) => EcMethods.ScalarMultJacobian(value.Value, this, Ec);
    public JacobianPoint Multiply(BigInteger value) => EcMethods.ScalarMultJacobian(value, this, Ec);
    public bool Equals(JacobianPoint value) => ToAffine().Equals(value.ToAffine());

    public JacobianPoint Clone()
    {
        return new JacobianPoint(
            X.Clone(),
            Y.Clone(),
            Z.Clone(),
            IsInfinity,
            Ec
        );
    }
}