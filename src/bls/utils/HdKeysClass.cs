using supranational;

namespace chia.dotnet.bls;

internal static class HdKeysClass
{
    private const byte keyLength = 48;
    private static readonly byte[] info = [0, keyLength];

    public static PrivateKey KeyGen(byte[] seed)
    {
        Array.Resize(ref seed, seed.Length + 1);
        var okm = Hkdf.ExtractExpand(keyLength, seed, Constants.SignatureKeygenSalt, info);

        return new PrivateKey(ModMath.Mod(okm.ToBigInt(Endian.Big), Constants.DefaultEc.N));
    }

    public static byte[] IkmToLamportSk(byte[] ikm, byte[] salt) => Hkdf.ExtractExpand(32 * 255, ikm, salt, []);

    public static byte[] ParentSkToLamportPk(PrivateKey parentSk, uint index)
    {
        var salt = index.ToFourBytes();
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

    public static PrivateKey DeriveChildSk(PrivateKey parentSk, uint index) => KeyGen(ParentSkToLamportPk(parentSk, index));

    public static PrivateKey DeriveChildSkUnhardened(PrivateKey parentSk, uint index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentSk.GetG1().ToBytes(), index.ToFourBytes());
        var hash = Hmac.Hash256(bytes);

        return PrivateKey.Aggregate([PrivateKey.FromBytes(hash), parentSk]);
    }

    public static JacobianPoint DeriveChildG1Unhardened(JacobianPoint parentPk, uint index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentPk.ToBytes(), index.ToFourBytes());
        var hash = Hmac.Hash256(bytes);

        return parentPk.Add(JacobianPoint.GenerateG1().Multiply(PrivateKey.FromBytes(hash).Value));
    }

    public static G1Element DeriveChildG1Unhardened(G1Element parentPk, uint index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentPk.ToBytes(), index.ToFourBytes());
        var hash = Hmac.Hash256(bytes);

        var nonce = new blst.SecretKey();
        nonce.from_lendian(hash);

        var gen = G1Element.FromAffine(blst.P1_Affine.generator());
        return parentPk + (gen * new blst.Scalar(nonce.key!));
    }

    public static G2Element DeriveChildG2Unhardened(G2Element parentPk, uint index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentPk.ToBytes(), index.ToFourBytes());
        var hash = Hmac.Hash256(bytes);

        var nonce = new blst.SecretKey();
        nonce.from_lendian(hash);

        var gen = G2Element.FromAffine(blst.P2_Affine.generator());
        return parentPk + (gen * new blst.Scalar(nonce.key!));
    }

    public static JacobianPoint DeriveChildG2Unhardened(JacobianPoint parentPk, uint index)
    {
        var bytes = ByteUtils.ConcatenateArrays(parentPk.ToBytes(), index.ToFourBytes());
        var hash = Hmac.Hash256(bytes);

        return parentPk.Add(JacobianPoint.GenerateG2().Multiply(PrivateKey.FromBytes(hash).Value));
    }
}