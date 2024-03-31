using supranational;

namespace chia.dotnet.bls;

internal static class PopSchemeMPL
{
    private const string CIPHERSUITE_ID = "BLS_SIG_BLS12381G2_XMD:SHA-256_SSWU_RO_POP_";
    private const string POP_CIPHERSUITE_ID = "BLS_POP_BLS12381G2_XMD:SHA-256_SSWU_RO_POP_";

    public static PrivateKey KeyGen(byte[] seed) => CoreMPL.KeyGen(seed);

    public static G2Element Sign(PrivateKey privateKey, byte[] message) => CoreMPL.Sign(privateKey, message, CIPHERSUITE_ID);

    public static bool Verify(G1Element publicKey, byte[] message, G2Element signature) => CoreMPL.Verify(publicKey, message, signature, CIPHERSUITE_ID);

    public static G2Element Aggregate(G2Element[] signatures) => CoreMPL.Aggregate(signatures);

    public static G2Element PopProve(PrivateKey privateKey) => privateKey.SignG2(privateKey.GetG1Element().ToBytes(), POP_CIPHERSUITE_ID);

    public static bool PopVerify(G1Element publicKey, G2Element proof)
    {
        if (!proof.IsValid || !publicKey.IsValid)
        {
            return false;
        }

        var pubkeyAffine = publicKey.ToAffine();
        var sigAffine = proof.ToAffine();
        var err = sigAffine.core_verify(pubkeyAffine, true, publicKey.ToBytes(), POP_CIPHERSUITE_ID);

        return err == blst.ERROR.SUCCESS;
    }
    public static bool FastAggregateVerify(G1Element[] publicKeys, byte[] message, G2Element signature)
    {
        if (publicKeys.Length == 0)
        {
            return false;
        }

        return CoreMPL.Verify(CoreMPL.Aggregate(publicKeys), message, signature, CIPHERSUITE_ID);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, uint index) => CoreMPL.DeriveChildSk(privateKey, index);

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, uint index) => CoreMPL.DeriveChildSkUnhardened(privateKey, index);

    public static G1Element DeriveChildPkUnhardened(G1Element publicKey, uint index) => CoreMPL.DeriveChildPkUnhardened(publicKey, index);
}