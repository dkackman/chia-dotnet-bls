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
            throw new Exception("Must aggregate at least 1 signature.");

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

        var qs = new List<JacobianPoint> { signature };
        var ps = new List<JacobianPoint> { JacobianPoint.GenerateG1().Negate() };
        for (var i = 0; i < pks.Length; i++)
        {
            if (!pks[i].IsValid())
            {
                return false;
            }
            
            qs.Add(OptSwu2MapClass.G2Map(ms[i], dst));
            ps.Add(pks[i]);
        }

        return Fq12.Nil.One(Constants.DefaultEc.Q).Equals(Pairing.AtePairingMulti([.. ps], [.. qs]));
    }
}