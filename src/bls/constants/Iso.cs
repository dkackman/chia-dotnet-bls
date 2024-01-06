using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

public static partial class Constants
{
    public static readonly Fq2[] Xnum =
    [
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("05c759507e8e333ebb5b7a9a47d7ed8532c52d39fd3a042a88b58423c50ae15d5c2638e343d9c71c6238aaaaaaaa97d6", NumberStyles.HexNumber)
            ),
            new Fq(
                Q,
                BigInteger.Parse("05c759507e8e333ebb5b7a9a47d7ed8532c52d39fd3a042a88b58423c50ae15d5c2638e343d9c71c6238aaaaaaaa97d6", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(Q, BigInteger.Zero),
            new Fq(
                Q,
                BigInteger.Parse("011560bf17baa99bc32126fced787c88f984f87adf7ae0c7f9a208c6b4f20a4181472aaa9cb8d555526a9ffffffffc71a", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("011560bf17baa99bc32126fced787c88f984f87adf7ae0c7f9a208c6b4f20a4181472aaa9cb8d555526a9ffffffffc71e", NumberStyles.HexNumber)
            ),
            new Fq(
                Q,
                BigInteger.Parse("08ab05f8bdd54cde190937e76bc3e447cc27c3d6fbd7063fcd104635a790520c0a395554e5c6aaaa9354ffffffffe38d", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("0171d6541fa38ccfaed6dea691f5fb614cb14b4e7f4e810aa22d6108f142b85757098e38d0f671c7188e2aaaaaaaa5ed1", NumberStyles.HexNumber)
            ),
            new Fq(Q, BigInteger.Zero)
        ),
    ];

    public static readonly Fq2[] Xden =
    [
        new Fq2(
            Q,
            new Fq(Q, BigInteger.Zero),
            new Fq(
                Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaa63", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(Q, BigInteger.Parse("0c", NumberStyles.HexNumber)),
            new Fq(
                Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaa9f", NumberStyles.HexNumber)
            )
        ),
        new Fq2(Q, new Fq(Q, BigInteger.One), new Fq(Q, BigInteger.Zero)),
    ];
    public static readonly Fq2[] Ynum =
    [
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("01530477c7ab4113b59a4c18b076d11930f7da5d4a07f649bf54439d87d27e500fc8c25ebf8c92f6812cfc71c71c6d706", NumberStyles.HexNumber)
            ),
            new Fq(
                Q,
                BigInteger.Parse("01530477c7ab4113b59a4c18b076d11930f7da5d4a07f649bf54439d87d27e500fc8c25ebf8c92f6812cfc71c71c6d706", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(Q, BigInteger.Zero),
            new Fq(
                Q,
                BigInteger.Parse("05c759507e8e333ebb5b7a9a47d7ed8532c52d39fd3a042a88b58423c50ae15d5c2638e343d9c71c6238aaaaaaaa97be", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("011560bf17baa99bc32126fced787c88f984f87adf7ae0c7f9a208c6b4f20a4181472aaa9cb8d555526a9ffffffffc71c", NumberStyles.HexNumber)
            ),
            new Fq(
                Q,
                BigInteger.Parse("08ab05f8bdd54cde190937e76bc3e447cc27c3d6fbd7063fcd104635a790520c0a395554e5c6aaaa9354ffffffffe38f", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("0124c9ad43b6cf79bfbf7043de3811ad0761b0f37a1e26286b0e977c69aa274524e79097a56dc4bd9e1b371c71c718b10", NumberStyles.HexNumber)
            ),
            new Fq(Q, BigInteger.Zero)
        ),
    ];

    public static readonly Fq2[] Yden =
    [
        new Fq2(
            Q,
            new Fq(
                Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffa8fb", NumberStyles.HexNumber)
            ),
            new Fq(
                Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffa8fb", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(Q, BigInteger.Zero),
            new Fq(
                Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffa9d3", NumberStyles.HexNumber)
            )
        ),
        new Fq2(
            Q,
            new Fq(Q, 18),
            new Fq(
                Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaa99", NumberStyles.HexNumber)
            )
        ),
        new Fq2(Q, new Fq(Q, BigInteger.One), new Fq(Q, BigInteger.Zero)),
    ];
}