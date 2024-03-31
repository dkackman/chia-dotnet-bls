namespace chia.dotnet.bls;

/// <summary>
/// Represents a point in Jacobian coordinates on an elliptic curve.
/// </summary>
public abstract class Jacobian
{
    /// <summary>
    /// Gets the size of the element in bytes.
    /// </summary>
    public abstract int Size { get; }
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

    public override bool Equals(object? obj) => obj is Jacobian value && Equals(value);

    public bool Equals(Jacobian? obj) => this == obj;

    public override int GetHashCode() => ToHex().GetHashCode();

    public static Jacobian FromBytes(byte[] bytes) => bytes.Length == 48 ? G1Element.FromBytes(bytes) : G2Element.FromBytes(bytes);

    public static bool operator ==(Jacobian? left, Jacobian? right)
    {
        if (ReferenceEquals(left, right))
            return true;
        if (left is null)
            return false;
        if (right is null)
            return false;
        if (left.GetType() != right.GetType())
            return false;

        return left.ToBytes().BytesEqual(right.ToBytes());
    }

    public static bool operator !=(Jacobian? left, Jacobian? right) => !(left == right);
}