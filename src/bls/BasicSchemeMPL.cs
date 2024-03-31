namespace chia.dotnet.bls;

internal static class BasicSchemeMPL
{
    private const string CIPHERSUITE_ID = "BLS_SIG_BLS12381G2_XMD:SHA-256_SSWU_RO_NUL_";
    public static PrivateKey KeyGen(byte[] seed) => CoreMPL.KeyGen(seed);

    public static G2Element Sign(PrivateKey privateKey, byte[] message) => CoreMPL.Sign(privateKey, message, CIPHERSUITE_ID);

    public static G2Element Aggregate(G2Element[] signatures) => CoreMPL.Aggregate(signatures);

    public static bool AggregateVerify(G1Element[] publicKeys, byte[][] messages, G2Element signature)
    {
        if (publicKeys.Length != messages.Length || publicKeys.Length == 0)
        {
            return false;
        }

        return CoreMPL.AggregateVerify(publicKeys, messages, signature, CIPHERSUITE_ID);
    }

    public static PrivateKey DeriveChildSk(PrivateKey privateKey, uint index) => CoreMPL.DeriveChildSk(privateKey, index);

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey privateKey, uint index) => CoreMPL.DeriveChildSkUnhardened(privateKey, index);

    public static G1Element DeriveChildPkUnhardened(G1Element publicKey, uint index) => CoreMPL.DeriveChildPkUnhardened(publicKey, index);
}