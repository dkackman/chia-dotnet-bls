using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a key used in BLS cryptography. Typically used as a public key.
/// </summary>
/// <remarks>By convention G1Element is used for public keys</remarks>
public class G1Element : Jacobian
{
    private const int _size = 48;

    private readonly blst.P1 p1;

    internal G1Element(byte[] bytes) => p1 = new blst.P1(bytes);

    internal G1Element(blst.SecretKey sk) => p1 = new blst.P1(sk);

    internal G1Element(blst.P1 p1) => this.p1 = p1;

    internal G1Element() => p1 = new blst.P1(true);

    public override int Size => _size;

    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public override bool IsValid => p1.is_inf() || p1.in_group();

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public override byte[] ToBytes() => p1.compress();
    public override byte[] Serialize() => p1.serialize();

    /// <summary>
    /// Gets the fingerprint of the element.
    /// </summary>
    /// <returns></returns>
    public long GetFingerprint() => Hmac.Hash256(ToBytes()).Take(4).ToArray().ToInt(Endian.Big);

    internal blst.P1_Affine ToAffine() => p1.to_affine();

    /// <summary>
    /// Hashes a message to a G1Element.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="dst"></param>
    /// <param name="pk"></param>
    /// <returns></returns>
    public static G1Element FromMessage(byte[] message, string dst = "", byte[]? pk = null)
    {
        var p1 = new blst.P1();
        p1.hash_to(message, dst, pk!);
        return new G1Element(p1);
    }

    internal static G1Element FromAffine(blst.P1_Affine affine) => new(affine.to_jacobian());
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

    public static G1Element operator +(G1Element a, G1Element b)
    {
        var p1 = a.p1.dup();
        p1.add(b.p1);
        return new G1Element(p1);
    }

    public static G1Element operator *(G1Element a, blst.Scalar k) => new(a.p1.mult(k));

    public static G1Element operator *(blst.Scalar k, G1Element a) => a * k;
}