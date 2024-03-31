using System.Diagnostics;
using System.Numerics;
using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a private key used in BLS cryptography.
/// </summary>
public readonly struct PrivateKey
{
    /// <summary>
    /// The length of the private key in bytes.
    /// </summary>
    public const byte Length = 48;

    /// <summary>
    /// The size of the private key in bytes.
    /// </summary>
    public const int Size = 32;

    /// <summary>
    /// The value of the private key as a BigInteger.
    /// </summary>
    public readonly BigInteger Value;

    internal readonly blst.SecretKey secretKey = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivateKey"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value of the private key.</param>
    public PrivateKey(BigInteger value)
    {
        Debug.Assert(value < Constants.DefaultEc.N);
        Value = value;
        secretKey.key = value.ToBytes(Size, Endian.Little);
    }

    private PrivateKey(byte[] key, BigInteger value)
    {
        secretKey.key = key;
        Value = value;
    }

    private PrivateKey(byte[] seed)
    {
        secretKey.keygen_v3(seed);
        Value = secretKey.key!.ToBigInt(Endian.Little);
    }

    /// <summary>
    /// Creates a <see cref="PrivateKey"/> instance from the specified byte array.
    /// </summary>
    /// <param name="bytes">The byte array representing the private key.</param>
    /// <returns>A new <see cref="PrivateKey"/> instance.</returns>
    public static PrivateKey FromBytes(byte[] bytes)
    {
        var scalar = new blst.Scalar(bytes);
        return new PrivateKey(scalar.val.ToBigInt(Endian.Little));
    }

    /// <summary>
    /// Creates a <see cref="PrivateKey"/> instance from the specified hexadecimal string.
    /// </summary>
    /// <param name="hex">The hexadecimal string representing the private key.</param>
    /// <returns>A new <see cref="PrivateKey"/> instance.</returns>
    public static PrivateKey FromHex(string hex) => FromBytes(hex.ToHexBytes());

    /// <summary>
    /// Creates a <see cref="PrivateKey"/> instance from the specified seed.
    /// </summary>
    /// <param name="seed">The seed used to generate the private key.</param>
    /// <returns>A new <see cref="PrivateKey"/> instance.</returns>
    public static PrivateKey FromSeed(string seed) => FromSeed(seed.HexStringToByteArray());

    /// <summary>
    /// Creates a <see cref="PrivateKey"/> instance from the specified seed.
    /// </summary>
    /// <param name="seed">The seed used to generate the private key.</param>
    /// <returns>A new <see cref="PrivateKey"/> instance.</returns>
    /// 
    public static PrivateKey FromSeed(byte[] seed) => new(seed);

    /// <summary>
    /// Creates a <see cref="PrivateKey"/> instance from the specified BigInteger value.
    /// </summary>
    /// <param name="value">The BigInteger value representing the private key.</param>
    /// <returns>A new <see cref="PrivateKey"/> instance.</returns>
    public static PrivateKey FromBigInt(BigInteger value) => new(ModMath.Mod(value, Constants.DefaultEc.N));

    /// <summary>
    /// Aggregates an array of private keys into a single private key.
    /// </summary>
    /// <param name="privateKeys">The array of private keys to aggregate.</param>
    /// <returns>The aggregated private key.</returns>
    public static PrivateKey Aggregate(PrivateKey[] privateKeys)
    {
        if (privateKeys.Length == 0)
        {
            throw new ArgumentException("The array of private keys must not be empty.", nameof(privateKeys));
        }

        var scalar = new blst.Scalar([0]);
        foreach (var privateKey in privateKeys)
        {
            scalar.add(privateKey.secretKey);
        }

        return new PrivateKey(scalar.val, scalar.val.ToBigInt(Endian.Little));
    }

    /// <summary>
    /// Gets the corresponding G1 point on the elliptic curve.
    /// The G1 point is the PublicKey.
    /// </summary>
    /// <returns>The G1 point.</returns>
    public JacobianPoint GetG1() => JacobianPoint.GenerateG1().Multiply(Value);

    public G1Element GetG1Element() => new(secretKey);
    public G2Element GetG2Element() => new(secretKey);

    public G2Element SignG2(byte[] message)
    {
        var p2 = new blst.P2();
        p2.hash_to(message);
        p2.sign_with(secretKey);

        return new G2Element(p2);
    }

    /// <summary>
    /// Converts the private key to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the private key.</returns>
    public byte[] ToBytes(Endian endianness = Endian.Big)
    {
        if (secretKey.key is null)
        {
            return [];
        }

        return endianness == Endian.Little ? secretKey.to_lendian() : secretKey.to_bendian();
    }

    /// <summary>
    /// Converts the private key to a hexadecimal string.
    /// </summary>
    /// <returns>The hexadecimal string representation of the private key.</returns>
    public string ToHex(Endian endianness = Endian.Big) => ToBytes(endianness).ToHex();

    /// <summary>
    /// Returns a string that represents the current private key.
    /// </summary>
    /// <returns>A string representation of the private key.</returns>
    public override string ToString() => ToHex();

    /// <summary>
    /// Determines whether the specified <see cref="PrivateKey"/> object is equal to the current <see cref="PrivateKey"/>.
    /// </summary>
    /// <param name="value">The <see cref="PrivateKey"/> object to compare with the current <see cref="PrivateKey"/>.</param>
    /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
    public bool Equals(PrivateKey value) => ByteUtils.BytesEqual(secretKey.key, value.secretKey.key);

    public override bool Equals(object? obj) => obj is PrivateKey value && Equals(value);

    public override int GetHashCode() => secretKey.key?.GetHashCode() ?? 0;

    public static bool operator ==(PrivateKey left, PrivateKey right) => left.Equals(right);

    public static bool operator !=(PrivateKey left, PrivateKey right) => !left.Equals(right);
}