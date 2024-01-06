using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

public static partial class Constants
{
    public static readonly BigInteger X = -1 * BigInteger.Parse("0d201000000010000", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger Q = BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaaab", NumberStyles.AllowHexSpecifier);
    public static readonly Fq A = new(Q, 0);
    public static readonly Fq B = new(Q, 4);
    public static readonly Fq2 ATwist = new(Q, new Fq(Q, 0), new Fq(Q, 0));
    public static readonly Fq2 BTwist = new(Q, new Fq(Q, 4), new Fq(Q, 4));

    public static readonly Fq Gx = new(Q, BigInteger.Parse("017f1d3a73197d7942695638c4fa9ac0fc3688c4f9774b905a14e3a3f171bac586c55e83ff97a1aeffb3af00adb22c6bb", NumberStyles.AllowHexSpecifier));
    public static readonly Fq Gy = new(Q, BigInteger.Parse("08b3f481e3aaa0f1a09e30ed741d8ae4fcf5e095d5d00af600db18cb2c04b3edd03cc744a2888ae40caa232946c5e7e1", NumberStyles.AllowHexSpecifier));

    public static readonly Fq2 G2x = new(Q,
        new Fq(Q, BigInteger.Parse("352701069587466618187139116011060144890029952792775240219908644239793785735715026873347600343865175952761926303160")),
        new Fq(Q, BigInteger.Parse("3059144344244213709971259814753781636986470325476647558659373206291635324768958432433509563104347017837885763365758")));
    public static readonly Fq2 G2y = new(Q,
        new Fq(Q, BigInteger.Parse("1985150602287291935568054521177171638300868978215655730859378665066344726373823718423869104263333984641494340347905")),
        new Fq(Q, BigInteger.Parse("927553665492332455747201965776037880757740193453592970025027978793976877002675564980949289727957565575433344219582")));

    public static readonly BigInteger N = BigInteger.Parse("073eda753299d7d483339d80809a1d80553bda402fffe5bfeffffffff00000001", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger H = BigInteger.Parse("0396c8c005555e1568c00aaab0000aaab", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger HEff = BigInteger.Parse("0bc69f08f2ee75b3584c6a0ea91b352888e2a8e9145ad7689986ff031508ffe1329c2f178731db956d82bf015d1212b02ec0ec69d7477c1ae954cbc06689f6a359894c0adebbf6b4e8020005aaa95551", NumberStyles.AllowHexSpecifier);
    public static readonly BigInteger K = 12;
    public static readonly BigInteger SqrtN3 = BigInteger.Parse("1586958781458431025242759403266842894121773480562120986020912974854563298150952611241517463240701");
    public static readonly BigInteger SqrtN3m1o2 = BigInteger.Parse("793479390729215512621379701633421447060886740281060493010456487427281649075476305620758731620350");

    public static readonly EC DefaultEc = new()
    {
        Q = Q,
        A = A,
        B = B,
        Gx = Gx,
        Gy = Gy,
        G2x = G2x,
        G2y = G2y,
        N = N,
        H = H,
        X = X,
        K = K,
        SqrtN3 = SqrtN3,
        SqrtN3m1o2 = SqrtN3m1o2,
    };

    public static readonly EC DefaultEcTwist = new()
    {
        Q = Q,
        A = ATwist,
        B = BTwist,
        Gx = Gx,
        Gy = Gy,
        G2x = G2x,
        G2y = G2y,
        N = N,
        H = HEff,
        X = X,
        K = K,
        SqrtN3 = SqrtN3,
        SqrtN3m1o2 = SqrtN3m1o2,
    };
}
