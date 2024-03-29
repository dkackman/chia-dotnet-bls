using System.Numerics;

namespace chia.dotnet.bls;

/// <summary>
/// Represents an extension field Fq2, which is an extension of the base field IFq.
/// </summary>
internal class Fq2 : IFq
{
    public static readonly Fq2 Nil = new(BigInteger.One, [Fq.Nil, Fq.Nil]);

    public IFq Root { get; protected set; }
    public IFq[] Elements { get; }
    public IFq Basefield { get; }
    public virtual int Extension { get; } = 2;
    public BigInteger Value { get; }
    public BigInteger Q { get; }
    // used by derived classes that have more than 2 elements
    protected Fq2(BigInteger q, IFq[] elements)
    {
        Q = q;
        Value = ModMath.Mod(BigInteger.Zero, q);
        Elements = elements;
        Basefield = Elements.Length > 0 ? Elements[0] : throw new InvalidOperationException("Elements must not be empty.");
        Root = new Fq(q, BigInteger.MinusOne);
    }

    public Fq2(BigInteger q, IFq x, IFq y)
        : this(q, [x, y])
    {
    }

    public virtual IFq Construct(BigInteger q, IFq[] elements) => new Fq2(q, elements);
    public IFq ConstructWithRoot(BigInteger q, IFq[] elements) => ((Fq2)Construct(q, elements)).WithRoot(Root);

    public IFq WithRoot(IFq root)
    {
        Root = root;
        return this;
    }

    public IFq FromBytes(BigInteger q, byte[] bytes)
    {
        var length = Extension * 48;
        if (bytes.Length != length)
        {
            throw new ArgumentOutOfRangeException($"Expected {length} bytes.");
        }

        var embeddedSize = 48 * (Extension / Elements.Length);
        var constructedElements = new IFq[Elements.Length];

        for (var i = 0; i < Elements.Length; i++)
        {
            // Directly extract and convert each element, avoiding extra array allocations
            var elementBytes = new byte[embeddedSize];
            Array.Copy(bytes, i * embeddedSize, elementBytes, 0, embeddedSize);
            constructedElements[Elements.Length - 1 - i] = Basefield.FromBytes(q, elementBytes);
        }

        return Construct(q, constructedElements);
    }

    public virtual IFq Inverse()
    {
        var a = Elements[0];
        var b = Elements[1];
        var factor = a.Multiply(a).Add(b.Multiply(b)).Inverse();

        return new Fq2(Q, [a.Multiply(factor), b.Negate().Multiply(factor)]);
    }

    public IFq ModSqrt()
    {
        var a0 = Elements[0];
        var a1 = Elements[1];

        if (a1.Equals(Basefield.One(Q)))
        {
            return FromFq(Q, a0.ModSqrt());
        }

        var alpha = a0.Pow(2).Add(a1.Pow(2));
        var qMinusOneDivideTwo = (Q - BigInteger.One) / 2;
        var gamma = alpha.Pow(qMinusOneDivideTwo);
        var FqMinus1 = new Fq(Q, BigInteger.MinusOne);
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

    public IFq FromHex(BigInteger q, string hex) => FromBytes(q, hex.ToHexBytes());

    public virtual IFq FromFq(BigInteger q, IFq fq)
    {
        var y = Basefield.FromFq(q, fq);
        var z = Basefield.Zero(q);

        var elements = new IFq[Elements.Length];

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

    public IFq Zero(BigInteger q) => FromFq(q, new Fq(q, BigInteger.Zero));
    public IFq One(BigInteger q) => FromFq(q, new Fq(q, BigInteger.One));

    public IFq Clone()
    {
        var clonedElements = new IFq[Elements.Length];

        for (var i = 0; i < Elements.Length; i++)
        {
            clonedElements[i] = Elements[i].Clone();
        }

        return ConstructWithRoot(Q, clonedElements);
    }

    public bool ToBool() => Elements.All(element => element.ToBool());

    public byte[] ToBytes()
    {
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

    public string ToHex() => ToBytes().ToHex();

    public override string ToString() => ToHex();

    public IFq Negate()
    {
        var negatedElements = new IFq[Elements.Length];

        for (var i = 0; i < Elements.Length; i++)
        {
            negatedElements[i] = Elements[i].Negate();
        }

        return ConstructWithRoot(Q, negatedElements);
    }

    public IFq QiPower(int i)
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

        var newElements = new IFq[Elements.Length];
        for (var index = 0; index < Elements.Length; index++)
        {
            if (index == 0)
            {
                newElements[index] = Elements[index].QiPower(i);
            }
            else
            {
                var frobCoeff = FrobCoefficients.GetCoefficient(Extension, i, index) ?? throw new InvalidOperationException("Frobenius coefficient not found.");
                newElements[index] = Elements[index].QiPower(i).Multiply(frobCoeff);
            }
        }

        return ConstructWithRoot(Q, newElements);
    }

    public IFq Pow(BigInteger exponent)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(exponent);

        var result = ((Fq2)One(Q)).WithRoot(Root);
        var baseField = this;
        while (exponent != BigInteger.Zero)
        {
            if ((exponent & 1).IsOne)
            {
                result = result.Multiply(baseField);
            }

            baseField = (Fq2)baseField.Multiply((IFq)baseField);
            exponent >>= 1;
        }

        return result;
    }

    public IFq Add(IFq value)
    {
        // Use the wider type to do the math
        if (value.Extension > Extension)
        {
            return value.Add(this);
        }

        var newElements = new IFq[Elements.Length];

        if (value is Fq2 ext)
        {
            var baseFieldZero = Basefield.Zero(Q);
            for (var i = 0; i < Elements.Length; i++)
            {
                newElements[i] = Elements[i].Add(i < ext.Elements.Length ? ext.Elements[i] : baseFieldZero);
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

    public IFq Add(BigInteger value)
    {
        // Assuming Elements array is not empty and its length is known.
        var newElements = new IFq[Elements.Length];

        // Directly add value to the first element
        newElements[0] = Elements[0].Add(value);

        // Copy the remaining elements as they are
        for (var i = 1; i < Elements.Length; i++)
        {
            newElements[i] = Elements[i];
        }

        return ConstructWithRoot(Q, newElements);
    }

    public IFq Multiply(IFq value)
    {
        // use the wider type to do the math
        if (value.Extension > Extension)
        {
            return value.Multiply(this);
        }

        var elements = new IFq[Elements.Length];
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
                if (value is Fq2 ext && value.Extension == Extension)
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

    public virtual IFq MulByNonResidue()
    {
        var a = Elements[0];
        var b = Elements[1];

        return new Fq2(Q, a.Subtract(b), a.Add(b));
    }

    public IFq Multiply(BigInteger value)
    {
        var newElements = new IFq[Elements.Length];
        for (int i = 0; i < Elements.Length; i++)
        {
            newElements[i] = Elements[i].Multiply(value);
        }

        return ConstructWithRoot(Q, newElements);
    }

    public IFq Subtract(BigInteger value) => Add(-value);
    public IFq Subtract(IFq value) => Add(value.Negate());
    public IFq Divide(BigInteger value) => Multiply(~value);
    public IFq Divide(IFq value) => Multiply(value.Inverse());

    public bool Equals(IFq value)
    {
        if (value is Fq2 fieldExtValue)
        {
            if (value.GetType() == this.GetType())
            {
                if (Q != value.Q)
                {
                    return false;
                }

                for (var i = 0; i < Elements.Length; i++)
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
                for (var i = 1; i < Elements.Length; i++)
                {
                    if (!Elements[i].Equals(zeroQ))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        return value.Equals(this);
    }

    public bool Equals(BigInteger value)
    {
        // Check if the first element is equal to the value
        if (!Elements[0].Equals(value))
        {
            return false;
        }

        var zeroQ = Root.Zero(Q);
        // Check if all other elements are zero
        for (var i = 1; i < Elements.Length; i++)
        {
            if (!Elements[i].Equals(zeroQ))
            {
                return false;
            }
        }

        return true;
    }

    public bool LessThan(IFq value)
    {
        var valueElements = ((Fq2)value).Elements;
        for (var i = Elements.Length - 1; i >= 0; i--)
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

    public bool GreaterThan(IFq value)
    {
        var valueElements = ((Fq2)value).Elements;
        for (var i = Elements.Length - 1; i >= 0; i--)
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

    public bool LessThanOrEqual(IFq value) => LessThan(value) || Equals(value);
    public bool GreaterThanOrEqual(IFq value) => GreaterThan(value) || Equals(value);
}