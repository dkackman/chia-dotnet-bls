namespace chia.dotnet.bls;

public static class BasicSchemeMPL
{
    public static PrivateKey KeyGen(byte[] seed) => HdKeysClass.KeyGen(seed);

    public static JacobianPoint Sign(PrivateKey privateKey, byte[] message) => Signing.CoreSignMpl(privateKey, message, Constants.BasicSchemeDst);

    public static bool Verify(JacobianPoint publicKey, byte[] message, JacobianPoint signature) => Signing.CoreVerifyMpl(publicKey, message, signature, Constants.BasicSchemeDst);

    public static JacobianPoint Aggregate(List<JacobianPoint> signatures) => Signing.CoreAggregateMpl(signatures);

    public static bool AggregateVerify(List<JacobianPoint> publicKeys, List<byte[]> messages, JacobianPoint signature)
    {
        if (publicKeys.Count != messages.Count || publicKeys.Count == 0)
            return false;
        foreach (var message in messages)
        {
            foreach (var match in messages)
            {
                if (!message.SequenceEqual(match) && ByteUtils.BytesEqual(message, match))
                    return false;
            }
        }
        return Signing.CoreAggregateVerify(publicKeys, messages, signature, Constants.BasicSchemeDst);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSk(privateKey, index);
    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSkUnhardened(privateKey, index);
    public static JacobianPoint DeriveChildPkUnhardened(JacobianPoint publicKey, long index) => HdKeysClass.DeriveChildG1Unhardened(publicKey, index);
}