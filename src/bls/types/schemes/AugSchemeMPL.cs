namespace chia.dotnet.bls;

public static class AugSchemeMPL
{
    public static PrivateKey KeyGen(byte[] seed) => HdKeysClass.KeyGen(seed);

    public static JacobianPoint Sign(PrivateKey privateKey, byte[] message)
    {
        var publicKey = privateKey.GetG1();
        return Signing.CoreSignMpl(privateKey, [.. publicKey.ToBytes(), .. message], Constants.AugSchemeDst);
    }

    public static bool Verify(JacobianPoint publicKey, byte[] message, JacobianPoint signature)
        => Signing.CoreVerifyMpl(publicKey, [.. publicKey.ToBytes(), .. message], signature, Constants.AugSchemeDst);

    public static JacobianPoint Aggregate(List<JacobianPoint> signatures) => Signing.CoreAggregateMpl(signatures);

    public static bool AggregateVerify(JacobianPoint[] publicKeys, byte[][] messages, JacobianPoint signature)
    {
        if (publicKeys.Length != messages.Length || publicKeys.Length == 0)
        {
            return false;
        }

        List<byte[]> mPrimes = [];
        for (int i = 0; i < publicKeys.Length; i++)
        {
            mPrimes.Add([.. publicKeys[i].ToBytes(), .. messages[i]]);
        }

        return Signing.CoreAggregateVerify(publicKeys, [.. mPrimes], signature, Constants.AugSchemeDst);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSk(privateKey, index);

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSkUnhardened(privateKey, index);

    public static JacobianPoint DeriveChildPkUnhardened(JacobianPoint publicKey, long index) => HdKeysClass.DeriveChildG1Unhardened(publicKey, index);
}