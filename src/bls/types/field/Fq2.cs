using System.Numerics;

namespace chia.dotnet.bls;

public class Fq2 : Fq, IFieldExt<Fq>
{
    public static readonly new Fq2 Nil = new(BigInteger.One, [Fq.Nil, Fq.Nil]);

    public Fq Root { get; protected set; }
    public Fq[] Elements { get; }
    public Fq Basefield { get; }

    public override int Extension { get; } = 2;

    // used by derived classes that have more than 2 elements
    protected Fq2(BigInteger q, Fq[] elements)
        : base(q, 0)
    {
        Elements = elements;
        Basefield = Elements.Length > 0 ? Elements[0] : throw new InvalidOperationException("Elements must not be empty.");
        Root = new Fq(q, -1);
    }

    public Fq2(BigInteger q, Fq x, Fq y)
        : this(q, [x, y])
    {
    }

    public virtual Fq Construct(BigInteger q, Fq[] elements) => new Fq2(q, [elements[0], elements[1]]);
    public Fq ConstructWithRoot(BigInteger q, Fq[] elements) => ((IFieldExt<Fq>)Construct(q, elements)).WithRoot(Root);

    public Fq WithRoot(Fq root)
    {
        Root = root;
        return this;
    }

    public override Fq FromBytes(BigInteger q, byte[] bytes)
    {
        var length = Extension * 48;
        if (bytes.Length != length)
        {
            throw new ArgumentOutOfRangeException($"Expected {length} bytes.");
        }

        var embeddedSize = 48 * (Extension / Elements.Length);
        List<byte[]> elements = [];

        for (var i = 0; i < Elements.Length; i++)
        {
            var elementBytes = new byte[embeddedSize];
            Array.Copy(bytes, i * embeddedSize, elementBytes, 0, embeddedSize);
            elements.Add(elementBytes);
        }

        elements.Reverse();
        var constructedElements = elements.Select(elementBytes => Basefield.FromBytes(q, elementBytes)).ToList();

        return Construct(q, [.. constructedElements]);
    }

    public override Fq Inverse()
    {
        var a = Elements[0];
        var b = Elements[1];
        var factor = a.Multiply(a).Add(b.Multiply(b)).Inverse();
        return new Fq2(Q, [a.Multiply(factor), b.Negate().Multiply(factor)]);
    }

    public override Fq ModSqrt()
    {
        var a0 = Elements[0];
        var a1 = Elements[1];

        if (a1.Equals(Basefield.One(Q)))
            return FromFq(Q, a0.ModSqrt());

        var alpha = a0.Pow(2).Add(a1.Pow(2));
        var gamma = alpha.Pow((Q - 1) / 2);

        if (new Fq(Q, -1).Equals(gamma))
            throw new Exception("No sqrt exists.");

        alpha = alpha.ModSqrt();
        var delta = a0.Add(alpha).Multiply(new Fq(Q, 2).Inverse());

        gamma = delta.Pow((Q - 1) / 2);

        if (gamma.Equals(new Fq(Q, -1)))
            delta = a0.Subtract(alpha).Multiply(new Fq(Q, 2).Inverse());

        var x0 = delta.ModSqrt();
        var x1 = a1.Multiply(new Fq(Q, 2).Multiply(x0).Inverse());

        return new Fq2(Q, [x0, x1]);
    }

    public override Fq FromHex(BigInteger q, string hex) => FromBytes(q, hex.FromHex());

    public override Fq FromFq(BigInteger q, Fq fq)
    {
        var y = Basefield.FromFq(q, fq);
        var z = Basefield.Zero(q);
        var elements = new List<Fq>();

        for (int i = 0; i < Elements.Length; i++)
            elements.Add(i == 0 ? y : z);

        var result = Construct(q, [.. elements]);
        ((Fq2)result).Root = new Fq(q, BigInteger.MinusOne);

        return result;
    }

    public override Fq Zero(BigInteger q) => FromFq(q, new Fq(q, 0));
    public override Fq One(BigInteger q) => FromFq(q, new Fq(q, 1));

    public override Fq Clone() => ConstructWithRoot(Q, Elements.Select(element => element.Clone()).ToArray());

    public override byte[] ToBytes()
    {
        var bytes = new List<byte>();
        for (int i = Elements.Length - 1; i >= 0; i--)
        {
            bytes.AddRange(Elements[i].ToBytes());
        }
        return [.. bytes];
    }

    public override bool ToBool() => Elements.All(element => element.ToBool());

    public override string ToHex() => ToBytes().ToHex();

    public override string ToString() => $"Fq{Extension}({string.Join(", ", Elements.ToList())})";

    public override Fq Negate() => ConstructWithRoot(Q, Elements.Select(element => element.Negate()).ToArray());

    public override Fq QiPower(int i)
    {
        if (Q != Constants.Q)
            throw new InvalidDataException("Invalid Q in QiPower.");

        i %= Extension;

        if (i == 0)
            return this;

        return ConstructWithRoot(
            Q,
            Elements.Select((element, index) =>
            {
                if (index == 0)
                {
                    return element.QiPower(i);
                }

                var frobCoeff = Constants.GetFrobCoeff(Extension, i, index) ?? throw new InvalidOperationException("Frobenius coefficient not found.");
                return element.QiPower(i).Multiply(frobCoeff);
            }).ToArray()
        );
    }

    public override Fq Pow(BigInteger exponent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(exponent);

        var result = ((Fq2)One(Q)).WithRoot(Root);
        var baseField = this;
        while (exponent != 0)
        {
            if ((exponent & 1) == 1)
                result = result.Multiply(baseField);
            baseField = (Fq2)baseField.Multiply(baseField);
            exponent >>= 1;
        }
        return result;
    }

    public override Fq AddTo(Fq value)
    {
        // use the wider type to do the math
        if (value.Extension > Extension)
        {
            return value.AddTo(this);
        }

        IField<Fq>[] elements = [];

        if (value is IFieldExt<Fq> ext)
        {
            elements = ext.Elements;
        }
        else
        {
            elements = Elements.Select(_ => Basefield.Zero(Q)).ToArray();
            elements[0] = elements[0].Add(value);
        }

        return ConstructWithRoot(
            Q,
            Elements.Select((element, i) => element.Add((Fq)elements[i])).ToArray()
        );
    }

    public override Fq AddTo(BigInteger value)
    {
        var elements = Elements.Select(_ => Basefield.Zero(Q)).ToArray();
        elements[0] = elements[0].Add(value);

        return ConstructWithRoot(
            Q,
            Elements.Select((element, i) => element.Add(elements[i])).ToArray()
        );
    }

    public override Fq MultiplyWith(Fq value)
    {
        // use the wider type to do the math
        if (value.Extension > Extension)
        {
            return value.MultiplyWith(this);
        }

        var elements = Elements.Select(_ => Basefield.Zero(Q)).ToArray();
        for (int i = 0; i < Elements.Length; i++)
        {
            var x = Elements[i];
            if (x.ToBool())
            {
                if (value is IFieldExt<Fq> ext && value.Extension == Extension)
                {
                    for (int j = 0; j < ext.Elements.Length; j++)
                    {
                        var y = ext.Elements[j];
                        if (y.ToBool())
                        {
                            var index = (i + j) % Elements.Length;

                            if (i + j >= Elements.Length)
                            {
                                elements[index] = elements[index].Add(x.Multiply(y).Multiply(Root));
                            }
                            else
                            {
                                elements[index] = elements[index].Add(x.Multiply(y));
                            }
                        }
                    }
                }
                else
                {
                    elements[i] = x.Multiply(value);
                }
            }
        }

        return ConstructWithRoot(Q, elements);
    }

    public virtual Fq MulByNonResidue()
    {
        var a = Elements[0];
        var b = Elements[1];
        return new Fq2(Q, a.Subtract(b), a.Add(b));
    }

    public override Fq MultiplyWith(BigInteger value) => ConstructWithRoot(Q, Elements.Select(element => element.Multiply(value)).ToArray());
    public override Fq Subtract(BigInteger value) => AddTo(-value);
    public override Fq Subtract(Fq value) => AddTo(value.Negate());
    public override Fq Divide(BigInteger value) => MultiplyWith(~value);
    public override Fq Divide(Fq value) => MultiplyWith(value.Inverse());

    public override bool EqualTo(Fq value)
    {
        if (value is IFieldExt<Fq> fieldExtValue && value.GetType() == GetType())
        {
            return Elements.Zip(fieldExtValue.Elements, (element, otherElement) => element.Equals(otherElement)).All(x => x) && Q == value.Q;
        }

        if (value is IFieldExt<Fq> && Extension > value.Extension)
        {
            return Elements.Skip(1).All(element => element.Equals(Root.Zero(Q))) && Elements[0].Equals(value);
        }

        return value.EqualTo(this);
    }

    public override bool EqualTo(BigInteger value) => Elements.Skip(1).All(element => element.Equals(Root.Zero(Q))) && Elements[0].Equals(value);

    public override bool LessThan(Fq value)
    {
        var valueElements = ((IFieldExt<Fq>)value).Elements;
        for (int i = Elements.Length - 1; i >= 0; i--)
        {
            var a = Elements[i];
            var b = valueElements[i];
            if (a.LessThan(b))
                return true;
            else if (a.GreaterThan(b))
                return false;
        }
        return false;
    }

    public override bool GreaterThan(Fq value)
    {
        var valueElements = ((IFieldExt<Fq>)value).Elements;
        for (int i = Elements.Length - 1; i >= 0; i--)
        {
            var a = Elements[i];
            var b = valueElements[i];
            if (a.GreaterThan(b))
                return true;
            else if (a.LessThan(b))
                return false;
        }
        return false;
    }

    public override bool LessThanOrEqual(Fq value) => LessThan(value) || Equals(value);
    public override bool GreaterThanOrEqual(Fq value) => GreaterThan(value) || Equals(value);
    public override Fq Add(BigInteger value) => AddTo(value);
    public override Fq Add(Fq value) => AddTo(value);
    public override Fq Multiply(BigInteger value) => MultiplyWith(value);
    public override Fq Multiply(Fq value) => MultiplyWith(value);
    public override bool Equals(BigInteger value) => EqualTo(value);
    public override bool Equals(Fq value) => EqualTo(value);
}