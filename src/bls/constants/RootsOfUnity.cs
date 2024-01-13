using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

internal static partial class Constants
{
    public static readonly BigInteger rv1 =
        BigInteger.Parse("06af0e0437ff400b6831e36d6bd17ffe48395dabc2d3435e77f76e17009241c5ee67992f72ec05f4c81084fbede3cc09", NumberStyles.HexNumber);

    public static readonly IEnumerable<Fq2> rootsOfUnity =
    [
        new(Q, new Fq(Q, 1), new Fq(Q, 0)),
        new(Q, new Fq(Q, 0), new Fq(Q, 1)),
        new(Q, new Fq(Q, rv1), new Fq(Q, rv1)),
        new(Q, new Fq(Q, rv1), new Fq(Q, Q - rv1)),
    ];
}