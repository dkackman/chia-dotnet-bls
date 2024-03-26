using System.Diagnostics;

namespace chia.dotnet.bls;

internal static class Signing
{
    private static readonly IFq one = Fq12.Nil.One(Constants.DefaultEc.Q);
    public static JacobianPoint CoreSignMpl(PrivateKey sk, byte[] message, byte[] dst) => OptSwu2MapClass.G2Map(message, dst).Multiply(sk.Value);

    public static bool CoreVerifyMpl(JacobianPoint pk, byte[] message, JacobianPoint signature, byte[] dst)
    {
        if (!signature.IsValid() || !pk.IsValid())
        {
            return false;
        }

        var q = OptSwu2MapClass.G2Map(message, dst);
        var pairingResult = Pairing.AtePairingMulti(
            [pk, JacobianPoint.GenerateG1().Negate()],
            [q, signature],
            Constants.DefaultEc
        );

        return pairingResult.Equals(one);
    }

    public static JacobianPoint CoreAggregateMpl(JacobianPoint[] signatures)
    {
        if (signatures.Length == 0)
        {
            throw new Exception("Must aggregate at least 1 signature.");
        }

        var aggregate = signatures[0];
        Debug.Assert(aggregate.IsValid());

        for (int i = 1; i < signatures.Length; i++)
        {
            var signature = signatures[i];
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

        var qs = new JacobianPoint[pks.Length + 1];
        var ps = new JacobianPoint[pks.Length + 1];
        qs[0] = signature;
        ps[0] = JacobianPoint.GenerateG1().Negate();

        for (var i = 0; i < pks.Length; i++)
        {
            if (!pks[i].IsValid())
            {
                return false;
            }

            qs[i + 1] = OptSwu2MapClass.G2Map(ms[i], dst);
            ps[i + 1] = pks[i];
        }

        return one.Equals(Pairing.AtePairingMulti(ps, qs, Constants.DefaultEc));
    }
}