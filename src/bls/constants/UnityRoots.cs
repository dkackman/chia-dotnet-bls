using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

internal static class UnityRoots
{
    public static readonly BigInteger rv1 =
        BigInteger.Parse("06af0e0437ff400b6831e36d6bd17ffe48395dabc2d3435e77f76e17009241c5ee67992f72ec05f4c81084fbede3cc09", NumberStyles.HexNumber);

    private static readonly Fq fqRv1 = new(Constants.Q, rv1);

    public static readonly IEnumerable<Fq2> RootsOfUnity =
    [
        new(Constants.Q, Constants.FqOne, Constants.FqZero),
        new(Constants.Q, Constants.FqZero, Constants.FqOne),
        new(Constants.Q, fqRv1, fqRv1),
        new(Constants.Q, fqRv1, new Fq(Constants.Q, Constants.Q - rv1)),
    ];
}