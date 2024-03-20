using System.Numerics;
using System.Globalization;

namespace chia.dotnet.bls;

/// <summary>
/// Provides methods for key derivation in the BLS cryptography library.
/// </summary>
public static class KeyDerivation
{
    /// <summary>
    /// Default hidden puzzle hash used in key derivation.
    /// </summary>
    public static readonly byte[] DEFAULT_HIDDEN_PUZZLE_HASH = "711d6c4e32c92e53179b199484cf8c897542bc57f2b22582799f9d657eec4699".FromHex();

    private static readonly BigInteger groupOrder = BigInteger.Parse("073eda753299d7d483339d80809a1d80553bda402fffe5bfeffffffff00000001", NumberStyles.AllowHexSpecifier);

    /// <summary>
    /// Calculates the synthetic public key for the given public key and hidden puzzle hash.
    /// </summary>
    /// <param name="publicKey">The original public key.</param>
    /// <param name="hiddenPuzzleHash">The hidden puzzle hash.</param>
    /// <returns>The synthetic public key.</returns>
    public static JacobianPoint CalculateSyntheticPublicKey(this JacobianPoint publicKey, byte[] hiddenPuzzleHash)
    {
        var syntheticOffset = CalculateSyntheticOffset(publicKey, hiddenPuzzleHash);
        var privateKeyToAdd = PrivateKey.FromBytes(syntheticOffset.BigIntToBytes(32, Endian.Big));
        return publicKey.Add(privateKeyToAdd.GetG1());
    }

    /// <summary>
    /// Calculates the synthetic private key for the given private key and hidden puzzle hash.
    /// </summary>
    /// <param name="privateKey">The original private key.</param>
    /// <param name="hiddenPuzzleHash">The hidden puzzle hash.</param>
    /// <returns>The synthetic private key.</returns>
    public static PrivateKey CalculateSyntheticPrivateKey(this PrivateKey privateKey, byte[] hiddenPuzzleHash)
    {
        var syntheticOffset = CalculateSyntheticOffset(privateKey.GetG1(), hiddenPuzzleHash);
        var syntheticPrivateExponent = ModMath.Mod(privateKey.Value + syntheticOffset, groupOrder);
        var blob = syntheticPrivateExponent.BigIntToBytes(32, Endian.Big);

        return PrivateKey.FromBytes(blob);
    }

    /// <summary>
    /// Calculates the synthetic offset for the given public key and hidden puzzle hash.
    /// </summary>
    /// <param name="publicKey">The public key.</param>
    /// <param name="hiddenPuzzleHash">The hidden puzzle hash.</param>
    /// <returns>The synthetic offset.</returns>
    public static BigInteger CalculateSyntheticOffset(this JacobianPoint publicKey, byte[] hiddenPuzzleHash)
    {
        var blob = Hmac.Hash256(ByteUtils.ConcatenateArrays(publicKey.ToBytes(), hiddenPuzzleHash));
        return ModMath.Mod(blob.BytesToBigInt(Endian.Big, true), groupOrder);
    }

    /// <summary>
    /// Derives a private key path from the given master private key.
    /// </summary>
    /// <param name="privateKey">The master private key.</param>
    /// <param name="path">The path to derive.</param>
    /// <param name="hardened">Indicates if the derivation should be hardened.</param>
    /// <returns>The derived private key.</returns>
    public static PrivateKey DerivePrivateKeyPath(this PrivateKey privateKey, int[] path, bool hardened)
    {
        foreach (var index in path)
        {
            privateKey = hardened ? AugSchemeMPL.DeriveChildSk(privateKey, index) : AugSchemeMPL.DeriveChildSkUnhardened(privateKey, index);
        }

        return privateKey;
    }

    /// <summary>
    /// Derives a public key path from the given master public key.
    /// </summary>
    /// <param name="publicKey">The master public key.</param>
    /// <param name="path">The path to derive.</param>
    /// <returns>The derived public key.</returns>
    public static JacobianPoint DerivePublicKeyPath(this JacobianPoint publicKey, int[] path)
    {
        foreach (var index in path)
        {
            publicKey = AugSchemeMPL.DeriveChildPkUnhardened(publicKey, index);
        }

        return publicKey;
    }

    /// <summary>
    /// Derives a private key from the given master private key.
    /// </summary>
    /// <param name="masterPrivateKey">The master private key.</param>
    /// <param name="index">The index of the derived key.</param>
    /// <param name="hardened">Indicates if the derivation should be hardened.</param>
    /// <returns>The derived private key.</returns>
    public static PrivateKey DerivePrivateKey(this PrivateKey masterPrivateKey, int index, bool hardened) => DerivePrivateKeyPath(masterPrivateKey, [12381, 8444, 2, index], hardened);

    /// <summary>
    /// Derives a public key from the given master public key.
    /// </summary>
    /// <param name="masterPublicKey">The master public key.</param>
    /// <param name="index">The index of the derived key.</param>
    /// <returns>The derived public key.</returns>
    public static JacobianPoint DerivePublicKeyWallet(this JacobianPoint masterPublicKey, int index) => DerivePublicKeyPath(masterPublicKey, [12381, 8444, 2, index]);
}