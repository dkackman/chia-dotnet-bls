using System.Numerics;
using System.Globalization;

namespace chia.dotnet.bls;

public static class KeyDerivation
{
    public static readonly byte[] DEFAULT_HIDDEN_PUZZLE_HASH = "711d6c4e32c92e53179b199484cf8c897542bc57f2b22582799f9d657eec4699".FromHex();

    private static readonly BigInteger groupOrder = BigInteger.Parse("073eda753299d7d483339d80809a1d80553bda402fffe5bfeffffffff00000001", NumberStyles.AllowHexSpecifier);

    public static JacobianPoint CalculateSyntheticPublicKey(this JacobianPoint publicKey, byte[] hiddenPuzzleHash)
    {
        var syntheticOffset = CalculateSyntheticOffset(publicKey, hiddenPuzzleHash);
        var privateKeyToAdd = PrivateKey.FromBytes(syntheticOffset.BigIntToBytes(32, Endian.Big));
        return publicKey.Add(privateKeyToAdd.GetG1());
    }

    public static PrivateKey CalculateSyntheticPrivateKey(this PrivateKey privateKey, byte[] hiddenPuzzleHash)
    {
        var privateExponent = privateKey.ToBytes().BytesToBigInt(Endian.Big);
        var publicKey = privateKey.GetG1();
        var syntheticOffset = CalculateSyntheticOffset(publicKey, hiddenPuzzleHash);
        var syntheticPrivateExponent = ModMath.Mod(privateExponent + syntheticOffset, groupOrder);
        var blob = syntheticPrivateExponent.BigIntToBytes(32, Endian.Big);

        return PrivateKey.FromBytes(blob);
    }

    public static BigInteger CalculateSyntheticOffset(this JacobianPoint publicKey, byte[] hiddenPuzzleHash)
    {
        var blob = Hmac.Hash256(ByteUtils.ConcatenateArrays(publicKey.ToBytes(), hiddenPuzzleHash));
        return ModMath.Mod(blob.BytesToInt(Endian.Big, true), groupOrder);
    }

    public static PrivateKey DerivePrivateKeyPath(this PrivateKey privateKey, int[] path, bool hardened)
    {
        foreach (var index in path)
        {
            privateKey = hardened ? AugSchemeMPL.DeriveChildSk(privateKey, index) : AugSchemeMPL.DeriveChildSkUnhardened(privateKey, index);
        }

        return privateKey;
    }

    public static JacobianPoint DerivePublicKeyPath(this JacobianPoint publicKey, int[] path)
    {
        foreach (var index in path)
        {
            publicKey = AugSchemeMPL.DeriveChildPkUnhardened(publicKey, index);
        }

        return publicKey;
    }

    public static PrivateKey DerivePrivateKey(this PrivateKey masterPrivateKey, int index, bool hardened) => DerivePrivateKeyPath(masterPrivateKey, [12381, 8444, 2, index], hardened);

    public static JacobianPoint DerivePublicKey(this JacobianPoint masterPublicKey, int index) => DerivePublicKeyPath(masterPublicKey, [12381, 8444, 2, index]);
}