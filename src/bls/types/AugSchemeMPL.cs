namespace chia.dotnet.bls;

public static class AugSchemeMPL
{
    public static PrivateKey KeyGen(byte[] seed) => HdKeysClass.KeyGen(seed);

    public static JacobianPoint Sign(PrivateKey privateKey, byte[] message)
    {
        var publicKey = privateKey.GetG1();
        return Signing.CoreSignMpl(privateKey, ByteUtils.ConcatenateArrays(publicKey.ToBytes(), message), Constants.AugSchemeDst);
    }

    public static bool Verify(JacobianPoint publicKey, byte[] message, JacobianPoint signature)
        => Signing.CoreVerifyMpl(publicKey, ByteUtils.ConcatenateArrays(publicKey.ToBytes(), message), signature, Constants.AugSchemeDst);

    public static JacobianPoint Aggregate(JacobianPoint[] signatures) => Signing.CoreAggregateMpl(signatures);

    public static bool AggregateVerify(JacobianPoint[] publicKeys, byte[][] messages, JacobianPoint signature)
    {
        int length = publicKeys.Length;
        if (length != messages.Length || length == 0)
        {
            return false;
        }

        var mPrimes = new byte[length][];
        for (int i = 0; i < length; i++)
        {
            mPrimes[i] = ByteUtils.ConcatenateArrays(publicKeys[i].ToBytes(), messages[i]);
        }

        return Signing.CoreAggregateVerify(publicKeys, mPrimes, signature, Constants.AugSchemeDst);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSk(privateKey, index);

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, long index) => HdKeysClass.DeriveChildSkUnhardened(privateKey, index);

    public static JacobianPoint DeriveChildPkUnhardened(JacobianPoint publicKey, long index) => HdKeysClass.DeriveChildG1Unhardened(publicKey, index);
}