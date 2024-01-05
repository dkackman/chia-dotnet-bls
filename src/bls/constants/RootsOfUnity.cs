using System.Numerics;
using System.Globalization;

namespace chia.dotnet.bls;

public static partial class Constants
{
    public static readonly BigInteger rv1 =
        BigInteger.Parse("06af0e0437ff400b6831e36d6bd17ffe48395dabc2d3435e77f76e17009241c5ee67992f72ec05f4c81084fbede3cc09", NumberStyles.HexNumber);

    public static readonly List<Fq2> rootsOfUnity =
    [
        new(Constants.Q, new Fq(Constants.Q, 1), new Fq(Constants.Q, 0)),
        new(Constants.Q, new Fq(Constants.Q, 0), new Fq(Constants.Q, 1)),
        new(Constants.Q, new Fq(Constants.Q, rv1), new Fq(Constants.Q, rv1)),
        new(Constants.Q, new Fq(Constants.Q, rv1), new Fq(Constants.Q, Constants.Q - rv1)),
    ];
}