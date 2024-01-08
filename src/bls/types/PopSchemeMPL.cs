namespace chia.dotnet.bls;

public static class PopSchemeMPL
{
    public static PrivateKey KeyGen(byte[] seed) => HdKeysClass.KeyGen(seed);

    public static JacobianPoint Sign(PrivateKey privateKey, byte[] message) => Signing.CoreSignMpl(privateKey, message, Constants.PopSchemeDst);

    public static bool Verify(JacobianPoint publicKey, byte[] message, JacobianPoint signature) => Signing.CoreVerifyMpl(publicKey, message, signature, Constants.PopSchemeDst);

    public static JacobianPoint Aggregate(JacobianPoint[] signatures) => Signing.CoreAggregateMpl(signatures);

    public static bool AggregateVerify(JacobianPoint[] publicKeys, byte[][] messages, JacobianPoint signature)
    {
        if (publicKeys.Length != messages.Length || publicKeys.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < messages.Length; i++)
        {
            for (int j = 0; j < messages.Length; j++)
            {
                if (i != j && messages[i].SequenceEqual(messages[j]))
                    return false;
            }
        }

        return Signing.CoreAggregateVerify(publicKeys, messages, signature, Constants.PopSchemeDst);
    }

    public static JacobianPoint PopProve(PrivateKey privateKey)
    {
        var publicKey = privateKey.GetG1();
        return OptSwu2MapClass.G2Map(publicKey.ToBytes(), Constants.PopSchemePopDst).Multiply(privateKey.Value);
    }

    public static bool PopVerify(JacobianPoint publicKey, JacobianPoint proof)
    {
        if (!proof.IsValid())
        {
            return false;
        }

        if (!publicKey.IsValid())
        {
            return false;
        }

        var q = OptSwu2MapClass.G2Map(publicKey.ToBytes(), Constants.PopSchemePopDst);
        var one = Fq12.Nil.One(Constants.DefaultEc.Q);
        var pairingResult = Pairing.AtePairingMulti(
            [publicKey, JacobianPoint.GenerateG1().Negate()],
            [q, proof]
        );

        return pairingResult.Equals(one);
    }

    public static bool FastAggregateVerify(JacobianPoint[] publicKeys, byte[] message, JacobianPoint signature)
    {
        if (publicKeys.Length == 0)
        {
            return false;
        }

        var aggregate = publicKeys[0];
        foreach (var publicKey in publicKeys.Skip(1))
        {
            aggregate = aggregate.Add(publicKey);
        }

        return Signing.CoreVerifyMpl(aggregate, message, signature, Constants.PopSchemeDst);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSk(privateKey, index);

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSkUnhardened(privateKey, index);

    public static JacobianPoint DeriveChildPkUnhardened(JacobianPoint publicKey, long index) => HdKeysClass.DeriveChildG1Unhardened(publicKey, index);

}