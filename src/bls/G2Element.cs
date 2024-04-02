using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a key used in BLS cryptography. Typically used for signing.
/// </summary>
/// <remarks>By convention the <see cref="G2Element"> is used for signing</remarks>
public class G2Element : JacobianPoint
{
    private const int _size = 96;

    private readonly blst.P2 p2;

    internal G2Element(byte[] bytes) => p2 = new blst.P2(bytes);

    internal G2Element(blst.SecretKey sk) => p2 = new blst.P2(sk);

    internal G2Element(blst.P2 p2) => this.p2 = p2;
    internal G2Element() => p2 = new blst.P2(true);

    /// <summary>
    /// Gets the size of the element.
    /// </summary>
    /// <remarks>Size is 96 bytes</remarks>
    public override int Size => _size;

    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public override bool IsValid => p2.is_inf() || p2.in_group();

    /// <summary>
    /// Gets a value indicating whether the element is the point at infinity.
    /// </summary>
    public override bool IsInfinity => p2.is_inf();

    /// <summary>
    /// Gets a value indicating whether the element is in the group.
    /// </summary>
    public override bool IsInGroup => p2.in_group();

    /// <summary>
    /// Gets a value indicating whether the element is on the curve.
    /// </summary>
    public override bool IsOnCurve => p2.on_curve();

    internal blst.P2_Affine ToAffine() => p2.to_affine();

    internal static G2Element FromAffine(blst.P2_Affine affine) => new(affine.to_jacobian());

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public override byte[] ToBytes() => p2.compress();

    /// <summary>
    /// Serializes the element.
    /// </summary>
    /// <returns>The uncompressed serialization of the element</returns>
    public override byte[] Serialize() => p2.serialize();

    /// <summary>
    /// Signs a message with the private key.
    /// </summary>
    /// <param name="sk">The private key</param>
    /// <returns>The signed message</returns>
    public G2Element SignWith(PrivateKey sk)
    {
        var newP2 = p2.dup();
        newP2.sign_with(sk.secretKey);
        return new G2Element(newP2);
    }

    /// <summary>
    /// Hashes a message to a G2Element.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="dst">The Domain Separation Tag</param>
    /// <param name="aug">Extra data to include in the hash</param>
    /// <returns>The hashed message</returns>
    public static G2Element FromMessage(byte[] message, string dst = "", byte[]? aug = null)
    {
        var p2 = new blst.P2();
        p2.hash_to(message, dst, aug!);
        return new G2Element(p2);
    }

    /// <summary>
    /// Converts a byte array to a G2Element.
    /// </summary>
    /// <param name="bytes">The byte array</param>
    /// <returns>The G2Element</returns>
    /// <exception cref="ArgumentException"> if the array isn't 96 bytes long</exception>
    public static new G2Element FromBytes(byte[] bytes)
    {
        if (bytes.Length != _size)
        {
            throw new ArgumentException("G2Element::FromBytes: Invalid size");
        }

        var affine = new blst.P2_Affine(bytes);
        return new G2Element(affine.to_jacobian());
    }

    /// <summary>
    /// Converts a hex string to a <see cref="G2Element"/>.
    /// </summary>
    /// <param name="hex"></param>
    /// <returns><see cref="G2Element"/></returns>
    public static new G2Element FromHex(string hex) => FromBytes(hex.ToHexBytes());

    /// <summary>
    /// Returns the G2Element representing the point at infinity.
    /// </summary>
    /// <returns>G2Element</returns>
    public static G2Element GetInfinity()
    {
        var bytes = new byte[_size];
        bytes[0] = 0xc0;
        return new G2Element(bytes);
    }

    /// <summary>
    /// Adds two G2Elements together.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static G2Element operator +(G2Element a, G2Element b)
    {
        var p1 = a.p2.dup();
        p1.add(b.p2);
        return new G2Element(p1);
    }

    /// <summary>
    /// Multiplies a G2Element by a scalar.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    public static G2Element operator *(G2Element a, blst.Scalar k) => new(a.p2.mult(k));

    /// <summary>
    /// Multiplies a scalar by a G2Element.
    /// </summary>
    /// <param name="k"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static G2Element operator *(blst.Scalar k, G2Element a) => a * k;
}