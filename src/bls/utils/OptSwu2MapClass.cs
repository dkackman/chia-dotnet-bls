using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

internal static class OptSwu2MapClass
{
    private static readonly BigInteger sqrtCandidateExponent = (BigInteger.Pow(Constants.Q, 2) - 9) / 16;
    public static BigInteger Sgn0(Fq2 x)
    {
        var sign0 = ModMath.Mod(x.Elements[0].Value, 2).IsOne;
        var zero0 = x.Elements[0].Value.IsZero;
        var sign1 = ModMath.Mod(x.Elements[1].Value, 2).IsOne;

        return sign0 || (zero0 && sign1) ? BigInteger.One : BigInteger.Zero;
    }

    public static JacobianPoint Osswu2Help(Fq2 t)
    {
        var t_pow_2 = t.Pow(2);
        var xi_2_mult_t_pow_2 = OpSquG2.xi_2.Multiply(t_pow_2);
        var numDenCommon = OpSquG2.xi_2.Pow(2).Multiply(t_pow_2.Multiply(t_pow_2)).Add(xi_2_mult_t_pow_2);
        var x0_num = OpSquG2.Ell2p_b.Multiply(numDenCommon.Add(Constants.FqOne));
        var x0_den = OpSquG2.Ell2p_a.Negate().Multiply(numDenCommon);
        x0_den = x0_den.Equals(BigInteger.Zero) ? OpSquG2.Ell2p_a.Multiply(OpSquG2.xi_2) : x0_den;
        var gx0_den = x0_den.Pow(3);
        var gx0_num = OpSquG2.Ell2p_b.Multiply(gx0_den)
            .Add(OpSquG2.Ell2p_a.Multiply(x0_num).Multiply(x0_den.Pow(2)))
            .Add(x0_num.Pow(3));
        var temp1 = gx0_den.Pow(7);
        var temp2 = gx0_num.Multiply(temp1);
        temp1 = temp1.Multiply(temp2).Multiply(gx0_den);
        var sqrtCandidate = temp2.Multiply(temp1.Pow(sqrtCandidateExponent));

        foreach (var root in UnityRoots.RootsOfUnity)
        {
            var y0 = (Fq2)sqrtCandidate.Multiply(root);
            if (y0.Pow(2).Multiply(gx0_den).Equals(gx0_num))
            {
                if (Sgn0(y0) != Sgn0(t))
                {
                    y0 = (Fq2)y0.Negate();
                }

                Debug.Assert(Sgn0(y0) == Sgn0(t));
                return new JacobianPoint(
                    x0_num.Multiply(x0_den),
                    y0.Multiply(x0_den.Pow(3)),
                    x0_den,
                    false,
                    Constants.DefaultEcTwist
                );
            }
        }

        var x1_num = xi_2_mult_t_pow_2.Multiply(x0_num);
        var x1_den = x0_den;
        var gx1_num = OpSquG2.xi_2.Pow(3).Multiply(t.Pow(6)).Multiply(gx0_num);
        var gx1_den = gx0_den;
        sqrtCandidate = sqrtCandidate.Multiply(t.Pow(3));
        foreach (var eta in OpSquG2.etas)
        {
            var y1 = (Fq2)eta.Multiply(sqrtCandidate);
            if (y1.Pow(2).Multiply(gx1_den).Equals(gx1_num))
            {
                if (Sgn0(y1) != Sgn0(t))
                {
                    y1 = (Fq2)y1.Negate();
                }

                Debug.Assert(Sgn0(y1) == Sgn0(t));
                return new JacobianPoint(
                    x1_num.Multiply(x1_den),
                    y1.Multiply(x1_den.Pow(3)),
                    x1_den,
                    false,
                    Constants.DefaultEcTwist
                );
            }
        }

        throw new Exception("Bad osswu2Help.");
    }

    public static JacobianPoint Iso3(JacobianPoint P) => EcMethods.EvalIso(P, [Iso.Xnum, Iso.Xden, Iso.Ynum, Iso.Yden], Constants.DefaultEcTwist);

    public static JacobianPoint OptSwu2Map(Fq2 t, Fq2? t2 = null)
    {
        var Pp = Iso3(Osswu2Help(t));
        if (t2 != null)
        {
            var Pp2 = Iso3(Osswu2Help(t2));
            Pp = Pp.Add(Pp2);
        }

        return Pp.Multiply(Constants.HEff);
    }

    public static JacobianPoint G2Map(byte[] alpha, byte[] dst)
    {
        var hashResult = HashToFieldClass.Hp2(alpha, 2, dst);
        var elements = new Fq2[hashResult.Length];

        var index = 0;
        foreach (var hh in hashResult)
        {
            var items = new Fq[hh.Length];
            for (var i = 0; i < hh.Length; i++)
            {
                items[i] = new Fq(Constants.Q, hh[i]);
            }
            elements[index++] = new Fq2(Constants.Q, items[0], items[1]);
        }

        return OptSwu2Map(elements[0], elements[1]);
    }
}