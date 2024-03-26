namespace chia.dotnet.bls;

internal static class HdKeysClass
{
    private const byte keyLength = 48;
    private static readonly byte[] info = [0, keyLength];

    public static PrivateKey KeyGen(byte[] seed)
    {
        Array.Resize(ref seed, seed.Length + 1);
        var okm = Hkdf.ExtractExpand(keyLength, seed, Constants.SignatureKeygenSalt, info);

        return new PrivateKey(ModMath.Mod(ByteUtils.BytesToBigInt(okm, Endian.Big), Constants.DefaultEc.N));
    }

    public static byte[] IkmToLamportSk(byte[] ikm, byte[] salt) => Hkdf.ExtractExpand(32 * 255, ikm, salt, []);

    public static byte[] ParentSkToLamportPk(PrivateKey parentSk, long index)
    {
        var salt = ByteUtils.IntToBytes(index, 4, Endian.Big);
        var ikm = parentSk.ToBytes();

        // Optimized notIkm calculation
        var notIkm = new byte[ikm.Length];
        for (var i = 0; i < ikm.Length; i++)
        {
            notIkm[i] = (byte)(ikm[i] ^ 0xff);
        }

        var lamport0 = IkmToLamportSk(ikm, salt);
        var lamport1 = IkmToLamportSk(notIkm, salt);

        // Preallocate lamportPk array
        var lamportPk = new byte[255 * 32 * 2]; // Assuming Hmac.Hash256 returns 32 bytes

        // Use Span<byte> for slicing
        for (var i = 0; i < 255; i++)
        {
            var hash = Hmac.Hash256(new ArraySegment<byte>(lamport0, i * 32, 32));
            Array.Copy(hash, 0, lamportPk, i * 32, 32);
        }

        for (var i = 0; i < 255; i++)
        {
            var hash = Hmac.Hash256(new ArraySegment<byte>(lamport1, i * 32, 32));
            Array.Copy(hash, 0, lamportPk, (255 + i) * 32, 32);
        }

        return Hmac.Hash256(lamportPk);
    }

    public static PrivateKey DeriveChildSk(PrivateKey parentSk, long index) => KeyGen(ParentSkToLamportPk(parentSk, index));

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey parentSk, long index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentSk.GetG1().ToBytes(), ByteUtils.IntToBytes(index, 4, Endian.Big));
        var hash = Hmac.Hash256(bytes);

        return PrivateKey.Aggregate([PrivateKey.FromBytes(hash), parentSk]);
    }

    public static JacobianPoint DeriveChildG1Unhardened(JacobianPoint parentPk, long index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentPk.ToBytes(), ByteUtils.IntToBytes(index, 4, Endian.Big));
        var hash = Hmac.Hash256(bytes);

        return parentPk.Add(JacobianPoint.GenerateG1().Multiply(PrivateKey.FromBytes(hash).Value));
    }

    public static JacobianPoint DeriveChildG2Unhardened(JacobianPoint parentPk, long index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentPk.ToBytes(), ByteUtils.IntToBytes(index, 4, Endian.Big));
        var hash = Hmac.Hash256(bytes);
        
        return parentPk.Add(JacobianPoint.GenerateG2().Multiply(PrivateKey.FromBytes(hash).Value));
    }
}