using supranational;

namespace chia.dotnet.bls;

internal static class CoreMPL
{
    public static PrivateKey KeyGen(byte[] seed) => PrivateKey.FromSeed(seed);

    public static G2Element Sign(PrivateKey privateKey, string message, string dst = "") => privateKey.SignG2(message.ToBytes(), dst);
    public static G2Element Sign(PrivateKey privateKey, byte[] message, string dst = "") => privateKey.SignG2(message, dst);

    public static bool Verify(G1Element publicKey, byte[] message, G2Element signature, string dst = "")
    {
        var publicKeyAffine = publicKey.ToAffine();
        var sigAffine = signature.ToAffine();

        return sigAffine.core_verify(publicKeyAffine, true, message, dst) == blst.ERROR.SUCCESS;
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

    public static G2Element Aggregate(G2Element[] signatures)
    {
        var ret = new G2Element();
        foreach (var signature in signatures)
        {
            ret += signature;
        }

        return ret;
    }

    public static bool AggregateVerify(this G1Element[] publicKeys, byte[][] messages, G2Element signature, string dst = "")
    {
        if (publicKeys.Length != messages.Length || publicKeys.Length == 0)
        {
            return false;
        }

        var sig_affine = signature.ToAffine();
        var pairing = new blst.Pairing(true, dst);
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

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, uint index) => HdKeys.DeriveChildSk(privateKey, index);
    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, uint index) => HdKeys.DeriveChildSkUnhardened(privateKey, index);
    public static G1Element DeriveChildPkUnhardened(G1Element publicKey, uint index) => HdKeys.DeriveChildG1Unhardened(publicKey, index);
}