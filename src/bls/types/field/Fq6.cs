using System.Numerics;

namespace chia.dotnet.bls;

public class Fq6 : Fq2
{
    public static readonly new Fq6 Nil = new(BigInteger.One, Fq2.Nil, Fq2.Nil, Fq2.Nil);

    public override int Extension { get; } = 6;

    public Fq6(BigInteger Q, Fq2 x, Fq2 y, Fq2 z)
        : base(Q, [x, y, z]) => Root = new Fq2(Q, Fq.Nil.One(Q), Fq.Nil.One(Q));

    protected Fq6(BigInteger Q, Fq[] elements)
        : base(Q, elements) => Root = new Fq2(Q, Fq.Nil.One(Q), Fq.Nil.One(Q));

    public override Fq Construct(BigInteger Q, Fq[] elements) => new Fq6(Q, (Fq2)elements[0], (Fq2)elements[1], (Fq2)elements[2]);

    public override Fq FromFq(BigInteger Q, Fq fq)
    {
        var result = base.FromFq(Q, fq);
        ((Fq6)result).Root = new Fq2(Q, Fq.Nil.One(Q), Fq.Nil.One(Q));
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
