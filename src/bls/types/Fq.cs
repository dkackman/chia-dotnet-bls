using System.Numerics;

namespace chia.dotnet.bls;

internal class Fq(BigInteger q, BigInteger value)
{
    public static readonly Fq Nil = new(1, 0);

    public virtual int Extension { get; } = 1;
    public BigInteger Value { get; } = ModMath.Mod(value, q);
    public BigInteger Q { get; } = q;

    public virtual Fq Zero(BigInteger q) => new(q, 0);
    public virtual Fq One(BigInteger q) => new(q, 1);
    public virtual Fq FromBytes(BigInteger q, byte[] bytes)
    {
        if (bytes.Length != 48)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        return new Fq(q, bytes.BytesToBigInt(Endian.Big));
    }
    public virtual Fq FromHex(BigInteger q, string hex) => Nil.FromBytes(q, hex.FromHex());
    public virtual Fq FromFq(BigInteger q, Fq fq) => fq;
    public virtual Fq Clone() => new(Q, Value);

    private byte[]? bytes;
    public virtual byte[] ToBytes() => bytes ??= Value.BigIntToBytes(48, Endian.Big);
    private string? hex;
    public virtual string ToHex() => hex ??= ToBytes().ToHex();
    public override string ToString() => ToHex();
    public virtual bool ToBool() => true;

    public virtual Fq Negate() => new(Q, -Value);

    public virtual Fq Inverse()
    {
        BigInteger x0 = 1,
                   x1 = 0,
                   y0 = 0,
                   y1 = 1;
        var a = Q;
        var b = Value;

        while (a != 0)
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

    public virtual Fq QiPower(int _i) => this;

    public virtual Fq Pow(BigInteger exponent)
    {
        if (exponent == 0)
        {
            return new Fq(Q, 1);
        }

        if (exponent == 1)
        {
            return this;
        }

        Fq result = new Fq(Q, 1);
        Fq baseValue = this;

        while (exponent > 0)
        {
            if (ModMath.Mod(exponent, 2) == 1)
            {
                result = result.Multiply(baseValue);
            }

            baseValue = new Fq(Q, baseValue.Value * baseValue.Value);
            exponent /= 2;
        }

        return result;
    }

    public virtual Fq ModSqrt()
    {
        if (Value == 0)
        {
            return new Fq(Q, 0);
        }

        var qMinusOneDivideTwo = (Q - 1) / 2;
        if (BigInteger.ModPow(Value, qMinusOneDivideTwo, Q) != 1)
        {
            throw new Exception("No sqrt exists.");
        }

        if (ModMath.Mod(Q, 4) == 3)
        {
            return new Fq(
                Q,
                BigInteger.ModPow(Value, (Q + 1) / 4, Q)
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
        var q = Q - 1;
        while (ModMath.Mod(q, 2) == 0)
        {
            q /= 2;
            S++;
        }

        BigInteger z = 0;
        var minusOneModQ = ModMath.Mod(-1, Q);
        for (BigInteger i = 0; i < Q; i++)
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
        var R = BigInteger.ModPow(Value, (q + 1) / 2, Q);
        while (true)
        {
            if (t == 0)
            {
                return new Fq(Q, 0);
            }

            if (t == 1)
            {
                return new Fq(Q, R);
            }

            BigInteger i = 0;
            var f = t;
            while (f != 1)
            {
                f = ModMath.Mod(f * f, Q);
                i++;
            }

            var b = BigInteger.ModPow(c, BigInteger.ModPow(2, M - i - 1, Q), Q);
            M = i;
            c = ModMath.Mod(b * b, Q);
            t = ModMath.Mod(t * c, Q);
            R = ModMath.Mod(R * b, Q);
        }
    }

    public virtual Fq Add(BigInteger value) => new(Q, Value + value);
    public virtual Fq Add(Fq value)
    {
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 add
        // which works since addition is transitive
        // this has the effect of also ensuring that the return is the wider type
        if (value is Fq2) // this works for Fq2 derived types
        {
            return value.Add(this);
        }

        return new(Q, Value + value.Value);
    }

    public virtual Fq Multiply(BigInteger value) => new(Q, Value * value);
    public virtual Fq Multiply(Fq value)
    {
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 multiply
        // which works since multiplication is transitive
        // this has the effect of also ensuring that the return is the wider type
        if (value is Fq2) // this works for Fq2 derived types
        {
            return value.Multiply(this);
        }

        return new(Q, Value * value.Value);
    }

    public virtual bool Equals(BigInteger value) => false;  // the typescript returns false if value is a bigint
    public virtual bool Equals(Fq value) => Value == value.Value && Q == value.Q;

    public virtual Fq Subtract(BigInteger value) => Add(-value);
    public virtual Fq Subtract(Fq value) => Add(value.Negate());

    public virtual Fq Divide(BigInteger value) => Multiply(new Fq(Q, value).Inverse());
    public virtual Fq Divide(Fq value) => Multiply(value.Inverse());

    public virtual bool LessThan(Fq value) => Value < value.Value;
    public virtual bool GreaterThan(Fq value) => Value > value.Value;

    public virtual bool LessThanOrEqual(Fq value) => LessThan(value) || Equals(value);
    public virtual bool GreaterThanOrEqual(Fq value) => GreaterThan(value) || Equals(value);
}