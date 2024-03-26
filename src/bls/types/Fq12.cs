using System.Numerics;

namespace chia.dotnet.bls;

internal class Fq12 : Fq6
{
    public static readonly new Fq12 Nil = new(BigInteger.One, Fq6.Nil, Fq6.Nil);

    public override int Extension { get; } = 12;

    public Fq12(BigInteger q, Fq6 x, Fq6 y)
        : this(q, [x, y])
    {
    }

    protected Fq12(BigInteger q, IFq[] elements)
        : base(q, elements) => Root = new Fq6(q, (Fq2)Fq2.Nil.Zero(q), (Fq2)Fq2.Nil.One(q), (Fq2)Fq2.Nil.Zero(q));

    public override IFq Construct(BigInteger q, IFq[] elements) => new Fq12(q, elements);

    public override IFq FromFq(BigInteger q, IFq fq)
    {
        var result = base.FromFq(q, fq);
        ((Fq12)result).Root = new Fq6(q, (Fq2)Fq2.Nil.Zero(q), (Fq2)Fq2.Nil.One(q), (Fq2)Fq2.Nil.Zero(q));

        return result;
    }

    public override IFq Inverse()
    {
        var a = Elements[0];
        var b = Elements[1];
        var factor = a
            .Multiply(a)
            .Subtract(((Fq6)b.Multiply(b)).MulByNonResidue())
            .Inverse();

        return new Fq12(Q, (Fq6)a.Multiply(factor), (Fq6)b.Negate().Multiply(factor));
    }
}