using System.Numerics;

namespace chia.dotnet.bls;

internal class Fq(BigInteger q, BigInteger value) : IFq
{
    public static readonly IFq Nil = new Fq(BigInteger.One, BigInteger.Zero);

    public int Extension { get; } = 1;
    public BigInteger Value { get; } = ModMath.Mod(value, q);
    public BigInteger Q { get; } = q;

    public IFq Zero(BigInteger q) => new Fq(q, BigInteger.Zero);
    public IFq One(BigInteger q) => new Fq(q, BigInteger.One);
    public IFq FromBytes(BigInteger q, byte[] bytes)
    {
        if (bytes.Length != 48)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        return new Fq(q, bytes.BytesToBigInt(Endian.Big));
    }
    public IFq FromHex(BigInteger q, string hex) => Nil.FromBytes(q, hex.FromHex());
    public IFq FromFq(BigInteger q, IFq fq) => fq;
    public IFq Clone() => new Fq(Q, Value);
    public byte[] ToBytes() => Value.BigIntToBytes(48, Endian.Big);
    public string ToHex() => ToBytes().ToHex();
    public override string ToString() => ToHex();
    public bool ToBool() => true;

    public IFq Negate() => new Fq(Q, -Value);

    public IFq Inverse()
    {
        BigInteger x0 = BigInteger.One,
                   x1 = BigInteger.Zero,
                   y0 = BigInteger.Zero,
                   y1 = BigInteger.One;
        var a = Q;
        var b = Value;

        while (!a.IsZero)
        {
            var q = b / a;
            var tempB = b;
            b = a;
            a = ModMath.Mod(tempB, a);
            var temp_x0 = x0;
            x0 = x1;
            x1 = temp_x0 - q * x1;
            BigInteger temp_y0 = y0;
            y0 = y1;
            y1 = temp_y0 - q * y1;
        }

        return new Fq(Q, x0);
    }

    public IFq QiPower(int _i) => this;

    public IFq Pow(BigInteger exponent)
    {
        if (exponent.IsZero)
        {
            return new Fq(Q, BigInteger.One);
        }

        if (exponent.IsOne)
        {
            return this;
        }

        IFq result = new Fq(Q, BigInteger.One);
        var baseValue = this;

        while (exponent > BigInteger.Zero)
        {
            // don't need the ModMath.Mod here since the exponent is positive(??)
            if ((exponent % 2).IsOne)
            {
                result = result.Multiply(baseValue);
            }

            baseValue = new Fq(Q, baseValue.Value * baseValue.Value);
            exponent /= 2;
        }

        return result;
    }

    public IFq ModSqrt()
    {
        if (Value.IsZero)
        {
            return new Fq(Q, BigInteger.Zero);
        }

        var qMinusOneDivideTwo = (Q - BigInteger.One) / 2;
        if (!BigInteger.ModPow(Value, qMinusOneDivideTwo, Q).IsOne)
        {
            throw new Exception("No sqrt exists.");
        }

        if (ModMath.Mod(Q, 4) == 3)
        {
            return new Fq(
                Q,
                BigInteger.ModPow(Value, (Q + BigInteger.One) / 4, Q)
            );
        }

        if (ModMath.Mod(Q, 8) == 5)
        {
            return new Fq(
                Q,
                BigInteger.ModPow(Value, (Q + 3) / 8, Q)
            );
        }

        var S = BigInteger.Zero;
        var q = Q - BigInteger.One;
        while (ModMath.Mod(q, 2).IsZero)
        {
            q /= 2;
            S++;
        }

        var z = BigInteger.Zero;
        var minusOneModQ = ModMath.Mod(BigInteger.MinusOne, Q);
        for (var i = BigInteger.Zero; i < Q; i++)
        {
            var euler = BigInteger.ModPow(i, qMinusOneDivideTwo, Q);
            if (euler == minusOneModQ)
            {
                z = i;
                break;
            }
        }

        var M = S;
        var c = BigInteger.ModPow(z, q, Q);
        var t = BigInteger.ModPow(Value, q, Q);
        var R = BigInteger.ModPow(Value, (q + BigInteger.One) / 2, Q);
        while (true)
        {
            if (t.IsZero)
            {
                return new Fq(Q, BigInteger.Zero);
            }

            if (t.IsOne)
            {
                return new Fq(Q, R);
            }

            var i = BigInteger.Zero;
            var f = t;
            while (!f.IsOne)
            {
                f = ModMath.Mod(f * f, Q);
                i++;
            }

            var b = BigInteger.ModPow(c, BigInteger.ModPow(2, M - i - BigInteger.One, Q), Q);
            M = i;
            c = ModMath.Mod(b * b, Q);
            t = ModMath.Mod(t * c, Q);
            R = ModMath.Mod(R * b, Q);
        }
    }

    public IFq Add(BigInteger value) => new Fq(Q, Value + value);
    public IFq Add(IFq value)
    {
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 add
        // which works since addition is transitive
        // this has the effect of also ensuring that the return is the wider type
        if (value.Extension > Extension)
        {
            return value.Add(this);
        }

        return new Fq(Q, Value + value.Value);
    }

    public IFq Multiply(BigInteger value) => new Fq(Q, Value * value);
    public IFq Multiply(IFq value)
    {
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 multiply
        // which works since multiplication is transitive
        // this has the effect of also ensuring that the return is the wider type
        if (value.Extension > Extension)
        {
            return value.Multiply(this);
        }

        return new Fq(Q, Value * value.Value);
    }

    public bool Equals(BigInteger value) => false;  // the typescript returns false if value is a bigint
    public bool Equals(IFq value) => Value == value.Value && Q == value.Q;

    public IFq Subtract(BigInteger value) => Add(-value);
    public IFq Subtract(IFq value) => Add(value.Negate());

    public IFq Divide(BigInteger value) => Multiply(new Fq(Q, value).Inverse());
    public IFq Divide(IFq value) => Multiply(value.Inverse());

    public bool LessThan(IFq value) => Value < value.Value;
    public bool GreaterThan(IFq value) => Value > value.Value;

    public bool LessThanOrEqual(IFq value) => LessThan(value) || Equals(value);
    public bool GreaterThanOrEqual(IFq value) => GreaterThan(value) || Equals(value);
}