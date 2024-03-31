using System.Numerics;

namespace chia.dotnet.bls;

/// <summary>
/// Represents an elliptic curve.
/// </summary>
internal readonly struct EC
{
    public EC()
    {
    }

    /// <summary>
    /// Gets or sets the prime order of the curve.
    /// </summary>
    public BigInteger Q { get; init; }

    /// <summary>
    /// Gets or sets the order of the generator point on the curve.
    /// </summary>
    public BigInteger N { get; init; }

    /// <summary>
    /// Gets or sets the cofactor of the curve.
    /// </summary>
    public BigInteger H { get; init; }

    /// <summary>
    /// Gets or sets the x-coordinate of the base point for the pairing.
    /// </summary>
    public BigInteger X { get; init; }

    /// <summary>
    /// Gets or sets the scalar used for the pairing.
    /// </summary>
    public BigInteger K { get; init; }

    /// <summary>
    /// Gets or sets the square root of N^3.
    /// </summary>
    public BigInteger SqrtN3 { get; init; }

    /// <summary>
    /// Gets or sets the square root of (N^3 - 1) / 2.
    /// </summary>
    public BigInteger SqrtN3m1o2 { get; init; }
}
