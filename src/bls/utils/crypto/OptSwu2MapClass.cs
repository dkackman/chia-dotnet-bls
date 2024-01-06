
using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

public static class OptSwu2MapClass
{
    public static BigInteger Sgn0(Fq2 x)
    {
        var sign0 = ModMath.Mod(x.Elements[0].Value, 2) == 1;
        var zero0 = x.Elements[0].Value == 0;
        var sign1 = ModMath.Mod(x.Elements[1].Value, 2) == 1;
        return sign0 || (zero0 && sign1) ? 1 : 0;
    }

    public static JacobianPoint Osswu2Help(Fq2 t)
    {
        var numDenCommon = Constants.xi_2.Pow(2).Multiply(t.Pow(4)).Add(Constants.xi_2.Multiply(t.Pow(2)));
        var x0_num = Constants.Ell2p_b.Multiply(numDenCommon.Add(new Fq(Constants.Q, 1)));
        var x0_den = Constants.Ell2p_a.Negate().Multiply(numDenCommon);
        x0_den = x0_den.Equals(0) ? Constants.Ell2p_a.Multiply(Constants.xi_2) : x0_den;
        var gx0_den = x0_den.Pow(3);
        var gx0_num = Constants.Ell2p_b.Multiply(gx0_den)
            .Add(Constants.Ell2p_a.Multiply(x0_num).Multiply(x0_den.Pow(2)))
            .Add(x0_num.Pow(3));
        var temp1 = gx0_den.Pow(7);
        var temp2 = gx0_num.Multiply(temp1);
        temp1 = temp1.Multiply(temp2).Multiply(gx0_den);
        var sqrtCandidate = temp2.Multiply(temp1.Pow((BigInteger.Pow(Constants.Q, 2) - 9) / 16));
        foreach (var root in Constants.rootsOfUnity)
        {
            var y0 = (Fq2)sqrtCandidate.Multiply(root);
            if (y0.Pow(2).Multiply(gx0_den).Equals(gx0_num))
            {
                if (Sgn0(y0) != Sgn0(t))
                    y0 = (Fq2)y0.Negate();

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
        var x1_num = Constants.xi_2.Multiply(t.Pow(2)).Multiply(x0_num);
        var x1_den = x0_den;
        var gx1_num = Constants.xi_2.Pow(3).Multiply(t.Pow(6)).Multiply(gx0_num);
        var gx1_den = gx0_den;
        sqrtCandidate = sqrtCandidate.Multiply(t.Pow(3));
        foreach (var eta in Constants.etas)
        {
            var y1 = (Fq2)eta.Multiply(sqrtCandidate);
            if (y1.Pow(2).Multiply(gx1_den).Equals(gx1_num))
            {
                if (Sgn0(y1) != Sgn0(t))
                    y1 = (Fq2)y1.Negate();

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

    public static JacobianPoint Iso3(JacobianPoint P) => EcMethods.EvalIso(P, [Constants.Xnum, Constants.Xden, Constants.Ynum, Constants.Yden], Constants.DefaultEcTwist);

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
        var elements = HashToFieldClass.Hp2(alpha, 2, dst).Select(hh =>
        {
            var items = hh.Select(value => new Fq(Constants.Q, value)).ToList();
            return new Fq2(Constants.Q, items[0], items[1]);
        }).ToList();
        return OptSwu2Map(elements[0], elements[1]);
    }
}