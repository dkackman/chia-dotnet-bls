namespace chia.dotnet.bls;

/// <summary>
/// Represents a point in Jacobian coordinates on an elliptic curve.
/// </summary>
public abstract class Jacobian
{
    /// <summary>
    /// Gets a value indicating whether the element is valid.
    /// </summary>
    public abstract bool IsValid { get; }

    /// <summary>
    /// Converts the element to a byte array.
    /// </summary>
    /// <returns>The byte array representation of the element.</returns>
    public abstract byte[] ToBytes();

    /// <summary>
    /// Converts the element to a hexadecimal string.
    /// </summary>
    /// <returns>The hexadecimal string representation of the element.</returns>
    public string ToHex() => ToBytes().ToHex();

    /// <summary>
    /// Returns a string that represents the current element.
    /// </summary>
    /// <returns>A string representation of the element.</returns>
    public override string ToString() => ToHex();
}