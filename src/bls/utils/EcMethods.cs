using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

internal static class EcMethods
{
    public static IFq YForX(IFq x, EC ec)
    {
        var u = x.Multiply(x).Multiply(x).Add(ec.A.Multiply(x)).Add(ec.B);
        var y = u.ModSqrt();
        if (y.Equals(BigInteger.Zero) || !new AffinePoint(x, y, false, ec).IsOnCurve)
        {
            throw new Exception("No y for point x.");
        }

        return y;
    }

    public static JacobianPoint ScalarMultJacobian(BigInteger value, JacobianPoint point, EC ec)
    {
        var pointXOne = point.X.One(ec.Q);
        var result = new JacobianPoint(pointXOne, pointXOne, point.X.Zero(ec.Q), true, ec);
        if (point.IsInfinity || ModMath.Mod(value, ec.Q) == BigInteger.Zero)
        {
            return result;
        }

        var addend = point;
        while (value > BigInteger.Zero)
        {
            if ((value & BigInteger.One) == BigInteger.One)
            {
                result = result.Add(addend);
            }

            addend = addend.Add(addend);
            value >>= 1;
        }

        return result;
    }

    public static JacobianPoint EvalIso(JacobianPoint P, Fq2[][] mapCoeffs, EC ec)
    {
        var x = P.X;
        var y = P.Y;
        var z = P.Z;
        var mapValues = new Fq2[4];
        var maxOrd = mapCoeffs[0].Length;
        foreach (var coeffs in mapCoeffs.Skip(1))
        {
            maxOrd = Math.Max(maxOrd, coeffs.Length);
        }

        var zPows = new Fq2[maxOrd];
        zPows[0] = (Fq2)z.Pow(BigInteger.Zero);
        zPows[1] = (Fq2)z.Pow(2);
        for (var i = 2; i < zPows.Length; i++)
        {
            Debug.Assert(zPows[i - 1] != null);
            Debug.Assert(zPows[1] != null);
            zPows[i] = (Fq2)zPows[i - 1].Multiply(zPows[1]);
        }

        for (var i = 0; i < mapCoeffs.Length; i++)
        {
            var length = mapCoeffs[i].Length;
            var coeffsZ = new Fq2[length];
            for (var j = 0; j < length; j++)
            {
                coeffsZ[j] = (Fq2)mapCoeffs[i][length - j - 1].Multiply(zPows[j]);
            }

            var temp = coeffsZ[0];
            for (var j = 1; j < coeffsZ.Length; j++)
            {
                temp = (Fq2)temp.Multiply(x);
                temp = (Fq2)temp.Add(coeffsZ[j]);
            }
            mapValues[i] = temp;
        }

        Debug.Assert(mapCoeffs[1].Length + 1 == mapCoeffs[0].Length);
        Debug.Assert(zPows[1] != null);
        Debug.Assert(mapValues[1] != null);
        mapValues[1] = (Fq2)mapValues[1].Multiply(zPows[1]);
        Debug.Assert(mapValues[2] != null);
        Debug.Assert(mapValues[3] != null);
        mapValues[2] = (Fq2)mapValues[2].Multiply(y);
        mapValues[3] = (Fq2)mapValues[3].Multiply(z.Pow(3));
        var Z = mapValues[1].Multiply(mapValues[3]);
        var X = mapValues[0].Multiply(mapValues[3]).Multiply(Z);
        var Y = mapValues[2].Multiply(mapValues[1]).Multiply(Z).Multiply(Z);

        return new JacobianPoint(X, Y, Z, P.IsInfinity, ec);
    }

    public static bool SignFq(IFq element, EC ec) => element.GreaterThan(new Fq(ec.Q, (ec.Q - BigInteger.One) / 2));

    public static bool SignFq2(Fq2 element, EC ec)
    {
        if (element.Elements[1].Equals(new Fq(ec.Q, BigInteger.Zero)))
        {
            return SignFq(element.Elements[0], ec);
        }

        return element.Elements[1].GreaterThan(new Fq(ec.Q, (ec.Q - BigInteger.One) / 2));
    }
}