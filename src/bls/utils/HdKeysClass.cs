namespace chia.dotnet.bls;

internal static class HdKeysClass
{
    private static readonly byte[] _saltBytes = "BLS-SIG-KEYGEN-SALT-".ToBytes();
    private const byte keyLength = 48;
    private static readonly byte[] info = [0, keyLength];

    public static PrivateKey KeyGen(byte[] seed)
    {
        Array.Resize(ref seed, seed.Length + 1);
        var okm = Hkdf.ExtractExpand(keyLength, seed, _saltBytes, info);

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
        List<byte> lamportPk = [];

        for (int i = 0; i < 255; i++)
        {
            lamportPk.AddRange(Hmac.Hash256(lamport0.Skip(i * 32).Take(32).ToArray()));
        }

        for (int i = 0; i < 255; i++)
        {
            lamportPk.AddRange(Hmac.Hash256(lamport1.Skip(i * 32).Take(32).ToArray()));
        }

        return Hmac.Hash256([.. lamportPk]);
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