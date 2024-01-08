using System.Numerics;

namespace chia.dotnet.bls;

public record EC
{
    public BigInteger Q { get; init; }
    public Fq A { get; init; } = Fq.Nil;
    public Fq B { get; init; } = Fq.Nil;
    public Fq Gx { get; init; } = Fq.Nil;
    public Fq Gy { get; init; } = Fq.Nil;
    public Fq2 G2x { get; init; } = Fq2.Nil;
    public Fq2 G2y { get; init; } = Fq2.Nil;
    public BigInteger N { get; init; }
    public BigInteger H { get; init; }
    public BigInteger X { get; init; }
    public BigInteger K { get; init; }
    public BigInteger SqrtN3 { get; init; }
    public BigInteger SqrtN3m1o2 { get; init; }
}
