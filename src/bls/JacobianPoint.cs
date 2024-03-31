namespace chia.dotnet.bls;

/// <summary>
/// Represents a point in Jacobian coordinates on an elliptic curve.
/// </summary>
/// <remarks>By convention the <see cref="G1Element"> is used for public keys and <see cref="G2Element"> is used for signing</remarks
public abstract class JacobianPoint
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
    /// <returns>The compressed byte array representation of the element.</returns>
    public abstract byte[] ToBytes();

    /// <summary>
    /// Serializes the element.
    /// </summary>
    /// <returns></returns>
    public abstract byte[] Serialize();

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

    /// <summary>
    /// Determines whether the specified <see cref="JacobianPoint"/> object is equal to the current <see cref="JacobianPoint"/>.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is JacobianPoint value && Equals(value);

    /// <summary>
    /// Determines whether the specified <see cref="JacobianPoint"/> object is equal to the current <see cref="JacobianPoint"/>.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Equals(JacobianPoint? obj) => this == obj;

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => ToHex().GetHashCode();

    /// <summary>
    /// Converts a byte array to a <see cref="JacobianPoint"/>.
    /// </summary>
    /// <param name="bytes"></param>
    /// <remarks>Depending on the length of the byte array, the method will return either a <see cref="G1Element"/> or a <see cref="G2Element"/>.</remarks>
    /// <returns>Either a <see cref="G1Element"/> or a <see cref="G2Element"/> </returns>
    public static JacobianPoint FromBytes(byte[] bytes) => bytes.Length == 48 ? G1Element.FromBytes(bytes) : G2Element.FromBytes(bytes);

    /// <summary>
    /// Determines whether two specified instances of <see cref="JacobianPoint"/> are equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(JacobianPoint? left, JacobianPoint? right)
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

    /// <summary>
    /// Determines whether two specified instances of <see cref="JacobianPoint"/> are not equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(JacobianPoint? left, JacobianPoint? right) => !(left == right);
}