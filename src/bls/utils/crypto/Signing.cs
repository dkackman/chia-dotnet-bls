using System.Diagnostics;

namespace chia.dotnet.bls;

internal static class Signing
{
    public static JacobianPoint CoreSignMpl(PrivateKey sk, byte[] message, byte[] dst) => OptSwu2MapClass.G2Map(message, dst).Multiply(sk.Value);

    public static bool CoreVerifyMpl(JacobianPoint pk, byte[] message, JacobianPoint signature, byte[] dst)
    {
        if (!signature.IsValid() || !pk.IsValid())
        {
            return false;
        }

        var q = OptSwu2MapClass.G2Map(message, dst);
        var one = Fq12.Nil.One(Constants.DefaultEc.Q);
        var pairingResult = Pairing.AtePairingMulti(
            [pk, JacobianPoint.GenerateG1().Negate()],
            [q, signature]
        );

        return pairingResult.Equals(one);
    }

    public static JacobianPoint CoreAggregateMpl(IEnumerable<JacobianPoint> signatures)
    {
        if (!signatures.Any())
        {
            throw new Exception("Must aggregate at least 1 signature.");
        }

        var aggregate = signatures.First();
        Debug.Assert(aggregate.IsValid());
        foreach (var signature in signatures.Skip(1))
        {
            Debug.Assert(signature.IsValid());
            aggregate = aggregate.Add(signature);
        }

        return aggregate;
    }

    public static bool CoreAggregateVerify(JacobianPoint[] pks, byte[][] ms, JacobianPoint signature, byte[] dst)
    {
        if (pks.Length != ms.Length || pks.Length == 0)
        {
            return false;
        }

        if (!signature.IsValid())
        {
            return false;
        }

        JacobianPoint[] qs = new JacobianPoint[pks.Length + 1];
        JacobianPoint[] ps = new JacobianPoint[pks.Length + 1];
        qs[0] = signature;
        ps[0] = JacobianPoint.GenerateG1().Negate();
        for (var i = 1; i < pks.Length + 1; i++)
        {
            if (!pks[i - 1].IsValid())
            {
                return false;
            }

            qs[i] = OptSwu2MapClass.G2Map(ms[i - 1], dst);
            ps[i] = pks[i - 1];
        }

        return Fq12.Nil.One(Constants.DefaultEc.Q).Equals(Pairing.AtePairingMulti(ps, qs));
    }
}