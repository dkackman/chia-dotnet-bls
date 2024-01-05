using System.Diagnostics;

namespace chia.dotnet.bls;

public static class Signing
{
    public static JacobianPoint CoreSignMpl(PrivateKey sk, byte[] message, byte[] dst)
    {
        return OptSwu2MapClass.G2Map(message, dst).Multiply(sk.Value);
    }

    public static bool CoreVerifyMpl(JacobianPoint pk, byte[] message, JacobianPoint signature, byte[] dst)
    {
        if (!signature.IsValid() || !pk.IsValid())
            return false;
        var q = OptSwu2MapClass.G2Map(message, dst);
        var one = Fq12.Nil.One(Constants.DefaultEc.Q);
        var pairingResult = Pairing.AtePairingMulti(
            [pk, JacobianPoint.GenerateG1().Negate()],
            [q, signature]
        );
        return pairingResult.Equals(one);
    }

    public static JacobianPoint CoreAggregateMpl(List<JacobianPoint> signatures)
    {
        if (!signatures.Any())
            throw new Exception("Must aggregate at least 1 signature.");
        var aggregate = signatures[0];
        Debug.Assert(aggregate.IsValid());
        foreach (var signature in signatures.Skip(1))
        {
            Debug.Assert(signature.IsValid());
            aggregate = aggregate.Add(signature);
        }
        return aggregate;
    }

    public static bool CoreAggregateVerify(List<JacobianPoint> pks, List<byte[]> ms, JacobianPoint signature, byte[] dst)
    {
        if (pks.Count != ms.Count || !pks.Any())
            return false;
        if (!signature.IsValid())
            return false;
        var qs = new List<JacobianPoint> { signature };
        var ps = new List<JacobianPoint> { JacobianPoint.GenerateG1().Negate() };
        for (var i = 0; i < pks.Count; i++)
        {
            if (!pks[i].IsValid())
            {
                return false;
            }
            qs.Add(OptSwu2MapClass.G2Map(ms[i], dst));
            ps.Add(pks[i]);
        }
        return Fq12.Nil.One(Constants.DefaultEc.Q).Equals(Pairing.AtePairingMulti(ps.ToArray(), qs.ToArray()));
    }
}