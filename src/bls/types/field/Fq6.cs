using System.Numerics;

namespace chia.dotnet.bls;

public class Fq6 : Fq2
{
    public static readonly new Fq6 Nil = new(BigInteger.One, Fq2.Nil, Fq2.Nil, Fq2.Nil);

    public override int Extension { get; } = 6;

    public Fq6(BigInteger q, Fq2 x, Fq2 y, Fq2 z)
        : base(q, [x, y, z]) => Root = new Fq2(q, Fq.Nil.One(q), Fq.Nil.One(q));

    protected Fq6(BigInteger q, Fq[] elements)
        : base(q, elements) => Root = new Fq2(q, Fq.Nil.One(q), Fq.Nil.One(q));

    public override Fq Construct(BigInteger q, Fq[] elements) => new Fq6(q, (Fq2)elements[0], (Fq2)elements[1], (Fq2)elements[2]);

    public override Fq FromFq(BigInteger q, Fq fq)
    {
        var result = base.FromFq(q, fq);
        ((Fq6)result).Root = new Fq2(q, Fq.Nil.One(q), Fq.Nil.One(q));
        return result;
    }

    public override Fq Inverse()
    {
        var a = Elements[0];
        var b = Elements[1];
        var c = (Fq2)Elements[2];

        var g0 = a.Multiply(a).Subtract(b.Multiply(c.MulByNonResidue()));
        var g1 = ((Fq2)c.Multiply(c)).MulByNonResidue().Subtract(a.Multiply(b));
        var g2 = b.Multiply(b).Subtract(a.Multiply(c));

        var factor = g0.Multiply(a)
            .Add(((Fq2)g1.Multiply(c).Add(g2.Multiply(b))).MulByNonResidue())
            .Inverse();

        return new Fq6(
            Q,
            (Fq2)g0.Multiply(factor),
            (Fq2)g1.Multiply(factor),
            (Fq2)g2.Multiply(factor)
        );
    }

    public override Fq MulByNonResidue()
    {
        var a = Elements[0];
        var b = Elements[1];
        var c = Elements[2];
        return new Fq6(Q, (Fq2)c.Multiply(Root), (Fq2)a, (Fq2)b);
    }
}
