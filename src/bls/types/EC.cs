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
    /// Gets or sets the coefficient 'A' of the curve equation.
    /// </summary>
    public Fq A { get; init; } = Fq.Nil;

    /// <summary>
    /// Gets or sets the coefficient 'B' of the curve equation.
    /// </summary>
    public Fq B { get; init; } = Fq.Nil;

    /// <summary>
    /// Gets or sets the x-coordinate of the generator point on the curve.
    /// </summary>
    public Fq Gx { get; init; } = Fq.Nil;

    /// <summary>
    /// Gets or sets the y-coordinate of the generator point on the curve.
    /// </summary>
    public Fq Gy { get; init; } = Fq.Nil;

    /// <summary>
    /// Gets or sets the x-coordinate of the generator point on the twisted curve.
    /// </summary>
    public Fq2 G2x { get; init; } = Fq2.Nil;

    /// <summary>
    /// Gets or sets the y-coordinate of the generator point on the twisted curve.
    /// </summary>
    public Fq2 G2y { get; init; } = Fq2.Nil;

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

    //
    // These are all precomputed in the constants file.
    //
    public Fq Two { get; init; } = Fq.Nil;
    public Fq Three { get; init; } = Fq.Nil;

    public Fq12 NilOne { get; init; } = Fq12.Nil;
    public Fq6 NilZero { get; init; } = Fq6.Nil;
    public Fq12 Wsq { get; init; } = Fq12.Nil;
}
