using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a key used in BLS cryptography. Typically used as a public key.
/// </summary>
public class G1Element : Jacobian
{
    private readonly blst.P1 p1;

    internal G1Element(blst.SecretKey sk) => p1 = new blst.P1(sk);

    internal G1Element(blst.P1 p1) => this.p1 = p1;

    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public override bool IsValid => p1.is_inf() || p1.in_group();

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public override byte[] ToBytes() => p1.serialize();

    /// <summary>
    /// Gets the fingerprint of the element.
    /// </summary>
    /// <returns></returns>
    public long GetFingerprint() =>  Hmac.Hash256(ToBytes()).Take(4).ToArray().ToInt(Endian.Big);

    /// <summary>
    /// Hashes a message to a G1Element.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="dst"></param>
    /// <param name="pk"></param>
    /// <returns></returns>
    public static G1Element FromMessage(byte[] message, string dst, byte[]? pk = null)
    {
        var p1 = new blst.P1();
        p1.hash_to(message, dst, pk!);
        return new G1Element(p1);
    }
}