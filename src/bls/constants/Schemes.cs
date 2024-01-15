namespace chia.dotnet.bls;

internal static class Schemes
{
    public static readonly byte[] BasicSchemeDst = "BLS_SIG_BLS12381G2_XMD:SHA-256_SSWU_RO_NUL_".ToBytes();
    public static readonly byte[] AugSchemeDst = "BLS_SIG_BLS12381G2_XMD:SHA-256_SSWU_RO_AUG_".ToBytes();
    public static readonly byte[] PopSchemeDst = "BLS_SIG_BLS12381G2_XMD:SHA-256_SSWU_RO_POP_".ToBytes();
    public static readonly byte[] PopSchemePopDst = "BLS_POP_BLS12381G2_XMD:SHA-256_SSWU_RO_POP_".ToBytes();
}