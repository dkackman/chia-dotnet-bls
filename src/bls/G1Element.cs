using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a key used in BLS cryptography. Typically used as a public key.
/// </summary>
/// <remarks>By convention the <see cref="G1Element"> is used for public keys</remarks>
public class G1Element : JacobianPoint
{
    private const int _size = 48;

    private readonly blst.P1 p1;

    internal G1Element(byte[] bytes) => p1 = new blst.P1(bytes);

    internal G1Element(blst.SecretKey sk) => p1 = new blst.P1(sk);

    internal G1Element(blst.P1 p1) => this.p1 = p1;

    internal G1Element() => p1 = new blst.P1(true);

    /// <summary>
    /// Gets the size of the element.
    /// </summary>
    /// <remarks>Size is 48 bytes</remarks>
    public override int Size => _size;

    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public override bool IsValid => p1.is_inf() || p1.in_group();

    /// <summary>
    /// Gets a value indicating whether the element is the point at infinity.
    /// </summary>
    public override bool IsInfinity => p1.is_inf();

    /// <summary>
    /// Gets a value indicating whether the element is in the group.
    /// </summary>
    public override bool IsInGroup => p1.in_group();

    /// <summary>
    /// Gets a value indicating whether the element is on the curve.
    /// </summary>
    public override bool IsOnCurve => p1.on_curve();

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public override byte[] ToBytes() => p1.compress();

    /// <summary>
    /// Serializes the element.
    /// </summary>
    /// <returns>The uncompressed serialization of the element</returns>
    public override byte[] Serialize() => p1.serialize();

    /// <summary>
    /// Gets the fingerprint of the element.
    /// </summary>
    /// <returns></returns>
    public uint GetFingerprint() => Hmac.Hash256(ToBytes()).Take(4).ToArray().ToUint();

    internal blst.P1_Affine ToAffine() => p1.to_affine();

    internal static G1Element FromAffine(blst.P1_Affine affine) => new(affine.to_jacobian());

    /// <summary>
    /// Hashes a message to a G1Element.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="dst">The Domain Separation Tag</param>
    /// <param name="aug">Extra data to include in the hash</param>
    /// <returns>The hashed message</returns>
    public static G1Element FromMessage(byte[] message, string dst = "", byte[]? aug = null)
    {
        var p1 = new blst.P1();
        p1.hash_to(message, dst, aug!);
        return new G1Element(p1);
    }

    /// <summary>
    /// Converts a byte array to a G1Element.
    /// </summary>
    /// <param name="bytes">The byte array</param>
    /// <returns>The G1Element</returns>
    /// <exception cref="ArgumentException"> if the array isn't 48 bytes long</exception>
    public static new G1Element FromBytes(byte[] bytes)
    {
        if (bytes.Length != _size)
        {
            throw new ArgumentException("G1Element::FromBytes: Invalid size");
        }

        // check if the element is canonical
        bool fZerosOnly = new ArraySegment<byte>(bytes, 1, bytes.Length - 1).All(b => b == 0);
        if ((bytes[0] & 0xc0) == 0xc0)  // representing infinity
        {
            // enforce that infinity must be 0xc0000..00
            if (bytes[0] != 0xc0 || !fZerosOnly)
            {
                throw new ArgumentException("Given G1 infinity element must be canonical");
            }
            return new G1Element(new byte[_size]);  // return infinity element (point all zero)
        }
        else
        {
            if ((bytes[0] & 0xc0) != 0x80)
            {
                throw new ArgumentException("Given G1 non-infinity element must start with 0b10");
            }

            if (fZerosOnly)
            {
                throw new ArgumentException("G1 non-infinity element can't have only zeros");
            }
        }

        var affine = new blst.P1_Affine(bytes);
        return new G1Element(affine.to_jacobian());
    }

    /// <summary>
    /// Converts a hex string to a <see cref="G1Element"/>.
    /// </summary>
    /// <param name="hex"></param>
    /// <returns><see cref="G1Element"/></returns>
    public static new G1Element FromHex(string hex) => FromBytes(hex.ToHexBytes());

    /// <summary>
    /// Adds two G1Elements.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>The result</returns>
    public static G1Element operator +(G1Element a, G1Element b)
    {
        var p1 = a.p1.dup();
        p1.add(b.p1);
        return new G1Element(p1);
    }

    /// <summary>
    /// Multiplies a G1Element by a scalar.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="k"></param>
    /// <returns>The result</returns>
    public static G1Element operator *(G1Element a, blst.Scalar k) => new(a.p1.mult(k));

    /// <summary>
    /// Multiplies a scalar by a G1Element.
    /// </summary>
    /// <param name="k"></param>
    /// <param name="a"></param>
    /// <returns>The result</returns>
    public static G1Element operator *(blst.Scalar k, G1Element a) => a * k;

    /// <summary>
    /// Returns the G1Element representing the point at infinity.
    /// </summary>
    /// <returns>G1Element</returns>
    public static G1Element GetInfinity()
    {
        var bytes = new byte[_size];
        bytes[0] = 0xc0;
        return new G1Element(bytes);
    }
}