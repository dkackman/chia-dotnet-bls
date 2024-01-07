namespace chia.dotnet.bls;

internal static class HdKeysClass
{
    public static PrivateKey KeyGen(byte[] seed)
    {
        int length = 48;
        byte[] okm = Hkdf.ExtractExpand(length, [.. seed, .. new byte[] { 0 }], "BLS-SIG-KEYGEN-SALT-".ToBytes(), [0, (byte)length]);
        
        return new PrivateKey(ModMath.Mod(ByteUtils.BytesToBigInt(okm, Endian.Big), Constants.DefaultEc.N));
    }

    public static byte[] IkmToLamportSk(byte[] ikm, byte[] salt) => Hkdf.ExtractExpand(32 * 255, ikm, salt, []);

    public static byte[] ParentSkToLamportPk(PrivateKey parentSk, long index)
    {
        byte[] salt = ByteUtils.IntToBytes(index, 4, Endian.Big);
        byte[] ikm = parentSk.ToBytes();
        byte[] notIkm = ikm.Select(e => (byte)(e ^ 0xff)).ToArray();
        byte[] lamport0 = IkmToLamportSk(ikm, salt);
        byte[] lamport1 = IkmToLamportSk(notIkm, salt);

        // Assuming Hmac.Hash256 returns a 32-byte array
        int hashSize = 32;
        int totalLamportSize = 255 * hashSize * 2; // 255 hashes for lamport0 and 255 for lamport1
        byte[] lamportPk = new byte[totalLamportSize];

        int offset = 0;
        for (int i = 0; i < 255; i++)
        {
            byte[] hash = Hmac.Hash256(lamport0.Skip(i * 32).Take(32).ToArray());
            Array.Copy(hash, 0, lamportPk, offset, hashSize);
            offset += hashSize;
        }

        for (int i = 0; i < 255; i++)
        {
            byte[] hash = Hmac.Hash256(lamport1.Skip(i * 32).Take(32).ToArray());
            Array.Copy(hash, 0, lamportPk, offset, hashSize);
            offset += hashSize;
        }

        return Hmac.Hash256(lamportPk);
    }

    public static PrivateKey DeriveChildSk(PrivateKey parentSk, long index) => KeyGen(ParentSkToLamportPk(parentSk, index));

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey parentSk, long index)
    {
        byte[] hash = Hmac.Hash256([.. parentSk.GetG1().ToBytes(), .. ByteUtils.IntToBytes(index, 4, Endian.Big)]);
        return PrivateKey.Aggregate([PrivateKey.FromBytes(hash), parentSk]);
    }

    public static JacobianPoint DeriveChildG1Unhardened(JacobianPoint parentPk, long index)
    {
        byte[] hash = Hmac.Hash256([.. parentPk.ToBytes(), .. ByteUtils.IntToBytes(index, 4, Endian.Big)]);
        return parentPk.Add(JacobianPoint.GenerateG1().Multiply(PrivateKey.FromBytes(hash).Value));
    }

    public static JacobianPoint DeriveChildG2Unhardened(JacobianPoint parentPk, long index)
    {
        byte[] hash = Hmac.Hash256([.. parentPk.ToBytes(), .. ByteUtils.IntToBytes(index, 4, Endian.Big)]);
        return parentPk.Add(JacobianPoint.GenerateG2().Multiply(PrivateKey.FromBytes(hash).Value));
    }
}