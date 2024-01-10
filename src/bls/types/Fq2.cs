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

    public virtual Fq Construct(BigInteger q, Fq[] elements) => new Fq2(q, elements);
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
        var constructedElements = new Fq[Elements.Length];

        for (var i = 0; i < Elements.Length; i++)
        {
            // Directly extract and convert each element, avoiding extra array allocations
            var elementBytes = new byte[embeddedSize];
            Array.Copy(bytes, i * embeddedSize, elementBytes, 0, embeddedSize);
            constructedElements[Elements.Length - 1 - i] = Basefield.FromBytes(q, elementBytes);
        }

        return Construct(q, constructedElements);
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
        {
            return FromFq(Q, a0.ModSqrt());
        }

        var alpha = a0.Pow(2).Add(a1.Pow(2));
        var qMinusOneDivideTwo = (Q - 1) / 2;
        var gamma = alpha.Pow(qMinusOneDivideTwo);
        var FqMinus1 = new Fq(Q, -1);
        if (FqMinus1.Equals(gamma))
        {
            throw new Exception("No sqrt exists.");
        }

        var two = new Fq(Q, 2);
        var inverseTwo = two.Inverse();
        alpha = alpha.ModSqrt();
        var delta = a0.Add(alpha).Multiply(inverseTwo);
        gamma = delta.Pow(qMinusOneDivideTwo);

        if (gamma.Equals(FqMinus1))
        {
            delta = a0.Subtract(alpha).Multiply(inverseTwo);
        }

        var x0 = delta.ModSqrt();
        var x1 = a1.Multiply(two.Multiply(x0).Inverse());

        return new Fq2(Q, x0, x1);
    }

    public override Fq FromHex(BigInteger q, string hex) => FromBytes(q, hex.FromHex());

    public override Fq FromFq(BigInteger q, Fq fq)
    {
        var y = Basefield.FromFq(q, fq);
        var z = Basefield.Zero(q);

        var elements = new Fq[Elements.Length];

        // Directly assign the first element
        elements[0] = y;

        // Fill the rest of the elements with zero
        for (var i = 1; i < Elements.Length; i++)
        {
            elements[i] = z;
        }

        var result = Construct(q, elements);
        ((Fq2)result).Root = new Fq(q, BigInteger.MinusOne);

        return result;
    }

    public override Fq Zero(BigInteger q) => FromFq(q, new Fq(q, 0));
    public override Fq One(BigInteger q) => FromFq(q, new Fq(q, 1));

    public override Fq Clone()
    {
        var clonedElements = new Fq[Elements.Length];

        for (var i = 0; i < Elements.Length; i++)
        {
            clonedElements[i] = Elements[i].Clone();
        }

        return ConstructWithRoot(Q, clonedElements);
    }

    public override byte[] ToBytes()
    {
        // Calculate the total size needed
        var totalSize = Elements.Sum(element => element.ToBytes().Length);
        var bytes = new byte[totalSize];

        // Copy each element's bytes into the array
        var offset = 0;
        for (var i = Elements.Length - 1; i >= 0; i--)
        {
            var elementBytes = Elements[i].ToBytes();
            Array.Copy(elementBytes, 0, bytes, offset, elementBytes.Length);
            offset += elementBytes.Length;
        }

        return bytes;
    }


    public override bool ToBool() => Elements.All(element => element.ToBool());

    public override string ToHex() => ToBytes().ToHex();

    public override string ToString() => $"Fq{Extension}({string.Join(", ", Elements.ToList())})";

    public override Fq Negate()
    {
        var negatedElements = new Fq[Elements.Length];

        for (var i = 0; i < Elements.Length; i++)
        {
            negatedElements[i] = Elements[i].Negate();
        }

        return ConstructWithRoot(Q, negatedElements);
    }

    public override Fq QiPower(int i)
    {
        if (Q != Constants.Q)
        {
            throw new InvalidDataException("Invalid Q in QiPower.");
        }

        i %= Extension;

        if (i == 0)
        {
            return this;
        }

        var newElements = new Fq[Elements.Length];
        for (var index = 0; index < Elements.Length; index++)
        {
            if (index == 0)
            {
                newElements[index] = Elements[index].QiPower(i);
            }
            else
            {
                var frobCoeff = Constants.GetFrobCoeff(Extension, i, index) ?? throw new InvalidOperationException("Frobenius coefficient not found.");
                newElements[index] = Elements[index].QiPower(i).Multiply(frobCoeff);
            }
        }

        return ConstructWithRoot(Q, newElements);
    }

    public override Fq Pow(BigInteger exponent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(exponent);

        var result = ((Fq2)One(Q)).WithRoot(Root);
        var baseField = this;
        while (exponent != 0)
        {
            if ((exponent & 1) == 1)
            {
                result = result.Multiply(baseField);
            }

            baseField = (Fq2)baseField.Multiply(baseField);
            exponent >>= 1;
        }
        return result;
    }

    public override Fq AddTo(Fq value)
    {
        // Use the wider type to do the math
        if (value.Extension > Extension)
        {
            return value.AddTo(this);
        }

        var newElements = new Fq[Elements.Length];

        if (value is IFieldExt<Fq> ext)
        {
            var basefieldZero = Basefield.Zero(Q);
            for (var i = 0; i < Elements.Length; i++)
            {
                newElements[i] = Elements[i].Add(i < ext.Elements.Length ? ext.Elements[i] : basefieldZero);
            }
        }
        else
        {
            // Add value to the first element and copy the rest as-is
            newElements[0] = Elements[0].Add(value);
            for (var i = 1; i < Elements.Length; i++)
            {
                newElements[i] = Elements[i];
            }
        }

        return ConstructWithRoot(Q, newElements);
    }


    public override Fq AddTo(BigInteger value)
    {
        // Assuming Elements array is not empty and its length is known.
        var newElements = new Fq[Elements.Length];

        // Directly add value to the first element
        newElements[0] = Elements[0].Add(value);

        // Copy the remaining elements as they are
        for (var i = 1; i < Elements.Length; i++)
        {
            newElements[i] = Elements[i];
        }

        return ConstructWithRoot(Q, newElements);
    }

    public override Fq MultiplyWith(Fq value)
    {
        // use the wider type to do the math
        if (value.Extension > Extension)
        {
            return value.MultiplyWith(this);
        }

        var elements = new Fq[Elements.Length];
        var zfq = Basefield.Zero(Q);
        for (var i = 0; i < Elements.Length; i++)
        {
            elements[i] = zfq;
        }

        for (var i = 0; i < Elements.Length; i++)
        {
            var x = Elements[i];
            if (x.ToBool())
            {
                if (value is IFieldExt<Fq> ext && value.Extension == Extension)
                {
                    for (var j = 0; j < ext.Elements.Length; j++)
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

    public override Fq MultiplyWith(BigInteger value)
    {
        var newElements = new Fq[Elements.Length];
        for (int i = 0; i < Elements.Length; i++)
        {
            newElements[i] = Elements[i].Multiply(value);
        }

        return ConstructWithRoot(Q, newElements);
    }

    public override Fq Subtract(BigInteger value) => AddTo(-value);
    public override Fq Subtract(Fq value) => AddTo(value.Negate());
    public override Fq Divide(BigInteger value) => MultiplyWith(~value);
    public override Fq Divide(Fq value) => MultiplyWith(value.Inverse());

    public override bool EqualTo(Fq value)
    {
        if (value is IFieldExt<Fq> fieldExtValue)
        {
            if (value.GetType() == GetType())
            {
                if (Q != value.Q)
                {
                    return false;
                }

                for (int i = 0; i < Elements.Length; i++)
                {
                    if (!Elements[i].Equals(fieldExtValue.Elements[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (Extension > value.Extension)
            {
                if (!Elements[0].Equals(value))
                {
                    return false;
                }

                var zeroQ = Root.Zero(Q);
                for (int i = 1; i < Elements.Length; i++)
                {
                    if (!Elements[i].Equals(zeroQ))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return value.EqualTo(this);
    }


    public override bool EqualTo(BigInteger value)
    {
        // Check if the first element is equal to the value
        if (!Elements[0].Equals(value))
        {
            return false;
        }

        var zeroQ = Root.Zero(Q);
        // Check if all other elements are zero
        for (int i = 1; i < Elements.Length; i++)
        {
            if (!Elements[i].Equals(zeroQ))
            {
                return false;
            }
        }

        return true;
    }
    public override bool LessThan(Fq value)
    {
        var valueElements = ((IFieldExt<Fq>)value).Elements;
        for (int i = Elements.Length - 1; i >= 0; i--)
        {
            var a = Elements[i];
            var b = valueElements[i];
            if (a.LessThan(b))
            {
                return true;
            }

            if (a.GreaterThan(b))
            {
                return false;
            }
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
            {
                return true;
            }

            if (a.LessThan(b))
            {
                return false;
            }
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