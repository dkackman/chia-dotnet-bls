using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

internal static partial class Constants
{
    public static readonly Fq2 xi_2 = new(Q, new Fq(Q, -2), new Fq(Q, -1));
    public static readonly Fq2 Ell2p_a = new(Q, new Fq(Q, 0), new Fq(Q, 240));
    public static readonly Fq2 Ell2p_b = new(Q, new Fq(Q, 1012), new Fq(Q, 1012));

    public static readonly BigInteger ev1 =
        BigInteger.Parse("0699be3b8c6870965e5bf892ad5d2cc7b0e85a117402dfd83b7f4a947e02d978498255a2aaec0ac627b5afbdf1bf1c90", NumberStyles.HexNumber);
    public static readonly BigInteger ev2 =
        BigInteger.Parse("08157cd83046453f5dd0972b6e3949e4288020b5b8a9cc99ca07e27089a2ce2436d965026adad3ef7baba37f2183e9b5", NumberStyles.HexNumber);
    public static readonly BigInteger ev3 =
        BigInteger.Parse("0ab1c2ffdd6c253ca155231eb3e71ba044fd562f6f72bc5bad5ec46a0b7a3b0247cf08ce6c6317f40edbc653a72dee17", NumberStyles.HexNumber);
    public static readonly BigInteger ev4 =
        BigInteger.Parse("0aa404866706722864480885d68ad0ccac1967c7544b447873cc37e0181271e006df72162a3d3e0287bf597fbf7f8fc1", NumberStyles.HexNumber);

    public static readonly IEnumerable<Fq2> etas =
    [
        new Fq2(Q, new Fq(Q, ev1), new Fq(Q, ev2)),
        new Fq2(Q, new Fq(Q, Q - ev2), new Fq(Q, ev1)),
        new Fq2(Q, new Fq(Q, ev3), new Fq(Q, ev4)),
        new Fq2(Q, new Fq(Q, Q - ev4), new Fq(Q, ev3)),
    ];
}