using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

internal static class Constants
{
    public static readonly BigInteger Q = BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaaab", NumberStyles.AllowHexSpecifier);
    public static readonly byte[] SignatureKeygenSalt = "BLS-SIG-KEYGEN-SALT-".ToBytes();
    public static readonly BigInteger N = BigInteger.Parse("073eda753299d7d483339d80809a1d80553bda402fffe5bfeffffffff00000001", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger H = BigInteger.Parse("0396c8c005555e1568c00aaab0000aaab", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger HEff = BigInteger.Parse("0bc69f08f2ee75b3584c6a0ea91b352888e2a8e9145ad7689986ff031508ffe1329c2f178731db956d82bf015d1212b02ec0ec69d7477c1ae954cbc06689f6a359894c0adebbf6b4e8020005aaa95551", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger K = 12;
    public static readonly BigInteger SqrtN3 = BigInteger.Parse("1586958781458431025242759403266842894121773480562120986020912974854563298150952611241517463240701");
    public static readonly BigInteger SqrtN3m1o2 = BigInteger.Parse("793479390729215512621379701633421447060886740281060493010456487427281649075476305620758731620350");

    public static readonly EC DefaultEc = new()
    {
        Q = Q,
        N = N,
        H = H,
        K = K,

        // precomputed values
        SqrtN3 = SqrtN3,
        SqrtN3m1o2 = SqrtN3m1o2,
    };

    public static readonly EC DefaultEcTwist = new()
    {
        Q = Q,
        N = N,
        H = HEff,
        K = K,
        SqrtN3 = SqrtN3,
        SqrtN3m1o2 = SqrtN3m1o2,
    };
}
