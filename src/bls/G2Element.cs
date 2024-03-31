using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a key used in BLS cryptography. Typically used for signing.
/// </summary>
/// <remarks>By convention G2Element is used for signing</remarks>
public class G2Element : Jacobian
{
    private const int _size = 96;

    private readonly blst.P2 p2;

    internal G2Element(byte[] bytes) => p2 = new blst.P2(bytes);

    internal G2Element(blst.SecretKey sk) => p2 = new blst.P2(sk);

    internal G2Element(blst.P2 p2) => this.p2 = p2;
    internal G2Element() => p2 = new blst.P2(true);

    public override int Size => _size;

    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public override bool IsValid => p2.is_inf() || p2.in_group();

    internal blst.P2_Affine ToAffine() => p2.to_affine();

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public override byte[] ToBytes() => p2.compress();
    public override byte[] Serialize() => p2.serialize();

    public G2Element SignWith(PrivateKey sk)
    {
        var newP2 = p2.dup();
        newP2.sign_with(sk.secretKey);
        return new G2Element(newP2);
    }

    public static G2Element FromMessage(byte[] message, string dst = "", byte[]? pk = null)
    {
        var p2 = new blst.P2();
        p2.hash_to(message, dst, pk!);
        return new G2Element(p2);
    }

    internal static G2Element FromAffine(blst.P2_Affine affine) => new(affine.to_jacobian());

    public static new G2Element FromBytes(byte[] bytes)
    {
        if (bytes.Length != _size)
        {
            throw new ArgumentException("G2Element::FromBytes: Invalid size");
        }

        var affine = new blst.P2_Affine(bytes);
        return new G2Element(affine.to_jacobian());
    }

    public static G2Element operator +(G2Element a, G2Element b)
    {
        var p1 = a.p2.dup();
        p1.add(b.p2);
        return new G2Element(p1);
    }

    public static G2Element operator *(G2Element a, blst.Scalar k) => new(a.p2.mult(k));

    public static G2Element operator *(blst.Scalar k, G2Element a) => a * k;
}