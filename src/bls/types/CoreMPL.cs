using supranational;

namespace chia.dotnet.bls;

internal static class CoreMPL
{
    public static PrivateKey KeyGen(byte[] seed) => PrivateKey.FromSeed(seed);

    public static G2Element Sign(PrivateKey privateKey, string message) => privateKey.SignG2(message.ToBytes());
    public static G2Element Sign(PrivateKey privateKey, byte[] message) => privateKey.SignG2(message);

    public static bool Verify(G1Element publicKey, byte[] message, G2Element signature)
    {
        var pubkeyAffine = publicKey.ToAffine();
        var sigAffine = signature.ToAffine();

        return sigAffine.core_verify(pubkeyAffine, true, message) == blst.ERROR.SUCCESS;
    }

    public static G1Element Aggregate(G1Element[] publicKeys)
    {
        var ret = new G1Element();
        foreach (var publicKey in publicKeys)
        {
            ret += publicKey;
        }

        return ret;
    }

    public static bool AggregateVerify(this G1Element[] publicKeys, byte[][] messages, G2Element signature)
    {
        if (publicKeys.Length != messages.Length || publicKeys.Length == 0)
        {
            return false;
        }

        var sig_affine = signature.ToAffine();
        var pairing = new blst.Pairing(true);
        var pt = new blst.PT(sig_affine);

        for (var i = 0; i < publicKeys.Length; i++)
        {
            var pkAffine = publicKeys[i].ToAffine();
            var err = pairing.aggregate(pkAffine, null, messages[i]);

            if (err != blst.ERROR.SUCCESS)
            {
                return false;
            }
        }

        pairing.commit();
        return pairing.finalverify(pt);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, uint index) => HdKeysClass.DeriveChildSk(privateKey, index);
    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, uint index) => HdKeysClass.DeriveChildSkUnhardened(privateKey, index);
    public static G1Element DeriveChildPkUnhardened(G1Element publicKey, uint index) => HdKeysClass.DeriveChildG1Unhardened(publicKey, index);
}