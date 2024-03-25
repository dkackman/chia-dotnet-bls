using System.Numerics;

namespace chia.dotnet.bls;

internal static class Pairing
{
    public static Fq DoubleLineEval(AffinePoint R, AffinePoint P, EC ec)
    {
        var R12 = R.Untwist();
        var slope = new Fq(ec.Q, 3)
            .Multiply(R12.X.Pow(2).Add(ec.A))
            .Divide(R12.Y.Multiply(new Fq(ec.Q, 2)));
        var v = R12.Y.Subtract(R12.X.Multiply(slope));

        return P.Y.Subtract(P.X.Multiply(slope)).Subtract(v);
    }

    public static Fq AddLineEval(AffinePoint R, AffinePoint Q, AffinePoint P)
    {
        var R12 = R.Untwist();
        var Q12 = Q.Untwist();
        if (R12.Equals(Q12.Negate()))
        {
            return P.X.Subtract(R12.X);
        }

        var slope = Q12.Y.Subtract(R12.Y).Divide(Q12.X.Subtract(R12.X));
        var v = Q12.Y
            .Multiply(R12.X)
            .Subtract(R12.Y.Multiply(Q12.X))
            .Divide(R12.X.Subtract(Q12.X));

        return P.Y.Subtract(P.X.Multiply(slope)).Subtract(v);
    }

    public static Fq12 MillerLoop(BigInteger T, AffinePoint P, AffinePoint Q, EC ec)
    {
        var T_bits = ByteUtils.BigIntToBits(T);
        var R = Q;
        var f = Fq12.Nil.One(ec.Q);
        var FqQ2 = new Fq(ec.Q, 2);
        for (var i = 1; i < T_bits.Length; i++)
        {
            var lrr = DoubleLineEval(R, P, ec);
            f = f.Multiply(f).Multiply(lrr);
            R = R.Multiply(FqQ2);
            if (T_bits[i] == BigInteger.One)
            {
                f = f.Multiply(AddLineEval(R, Q, P));
                R = R.Add(Q);
            }
        }
        return (Fq12)f;
    }

    public static Fq12 FinalExponentiation(Fq12 element, EC ec)
    {
        if (ec.K == 12)
        {
            var ans = element.Pow((BigInteger.Pow(ec.Q, 4) - BigInteger.Pow(ec.Q, 2) + BigInteger.One) / ec.N);
            ans = ans.QiPower(2).Multiply(ans);
            ans = ans.QiPower(6).Divide(ans);

            return (Fq12)ans;
        }

        return (Fq12)element.Pow((BigInteger.Pow(ec.Q, (int)ec.K) - BigInteger.One) / ec.N);
    }

    public static Fq12 AtePairing(JacobianPoint P, JacobianPoint Q, EC ec)
    {
        var t = Constants.DefaultEc.X + BigInteger.One;
        var T = t - BigInteger.One;
        T = T < BigInteger.Zero ? -T : T;

        return FinalExponentiation(MillerLoop(T, P.ToAffine(), Q.ToAffine(), ec), ec);
    }

    public static Fq12 AtePairingMulti(JacobianPoint[] Ps, JacobianPoint[] Qs, EC ec)
    {
        var t = Constants.DefaultEc.X + BigInteger.One;
        var T = t - BigInteger.One;
        T = T < BigInteger.Zero ? -T : T;
        var prod = Fq12.Nil.One(ec.Q);
        for (var i = 0; i < Qs.Length; i++)
        {
            prod = prod.Multiply(MillerLoop(T, Ps[i].ToAffine(), Qs[i].ToAffine(), ec));
        }

        return FinalExponentiation((Fq12)prod, ec);
    }
}