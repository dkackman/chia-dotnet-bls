using System.Diagnostics;
using System.Numerics;

namespace chia.dotnet.bls;

public class PrivateKey
{
    public const byte Length = 48;

    public const int Size = 32;

    public readonly BigInteger Value;

    public PrivateKey(BigInteger value)
    {
        Debug.Assert(value < Constants.DefaultEc.N);
        Value = value;
    }
    public static PrivateKey FromBytes(byte[] bytes) => new(ModMath.Mod(bytes.BytesToBigInt(Endian.Big), Constants.DefaultEc.N));
    public static PrivateKey FromHex(string hex) => FromBytes(hex.FromHex());
    public static PrivateKey FromSeed(byte[] seed)
    {
        var okm = Hkdf.ExtractExpand(Length, ByteUtils.ConcatenateArrays(seed, [0]), "BLS-SIG-KEYGEN-SALT-".ToBytes(), [0, Length]);
        return new PrivateKey(ModMath.Mod(okm.BytesToBigInt(Endian.Big), Constants.DefaultEc.N));
    }
    public static PrivateKey FromBigInt(BigInteger value) => new(ModMath.Mod(value, Constants.DefaultEc.N));
    public static PrivateKey Aggregate(PrivateKey[] privateKeys)
    {
        var aggregate = privateKeys.Aggregate(BigInteger.Zero, (acc, privateKey) => acc + privateKey.Value);
        return new PrivateKey(ModMath.Mod(aggregate, Constants.DefaultEc.N));
    }
    public JacobianPoint GetG1() => JacobianPoint.GenerateG1().Multiply(Value);
    public byte[] ToBytes() => Value.BigIntToBytes(Size, Endian.Big);
    public string ToHex() => ToBytes().ToHex();
    public override string ToString() => ToHex();
    public bool Equals(PrivateKey value) => Value == value.Value;
}