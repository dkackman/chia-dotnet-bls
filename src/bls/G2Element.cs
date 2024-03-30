using supranational;

namespace chia.dotnet.bls;

/// <summary>
/// Represents a key used in BLS cryptography. Typically used for signing.
/// </summary>
public class G2Element : Jacobian
{
    private readonly blst.P2 p2;

    internal G2Element(blst.SecretKey sk) => p2 = new blst.P2(sk);

    internal G2Element(blst.P2 p2) => this.p2 = p2;
    
    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public override bool IsValid => p2.is_inf() || p2.in_group();

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public override byte[] ToBytes() => p2.serialize();

    public G2Element SignWith(PrivateKey sk)
    {
        var newP2 = p2.dup();
        newP2.sign_with(sk.secretKey);
        return new G2Element(newP2);
    }

    public static G2Element FromMessage(byte[] message, string dst, byte[]? pk = null)
    {
        var p2 = new blst.P2();
        p2.hash_to(message, dst, pk!);
        return new G2Element(p2);
    }
}