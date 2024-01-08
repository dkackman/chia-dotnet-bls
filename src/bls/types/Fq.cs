using System.Numerics;

namespace chia.dotnet.bls;

public class Fq(BigInteger q, BigInteger value) : IField<Fq>
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
    public virtual Fq FromFq(BigInteger q, Fq fq) => fq; // ?
    public virtual Fq Clone() => new(Q, Value);
    public virtual byte[] ToBytes() => Value.BigIntToBytes(48, Endian.Big);
    public virtual bool ToBool() => true;
    public virtual string ToHex() => ToBytes().ToHex();
    public override string ToString()
    {
        string hex = Value.ToString("x"); // Lowercase "x" for lowercase hexadecimal
        hex = hex.Length % 2 == 0 && hex.StartsWith('0') ? hex[1..] : hex;
        return hex.Length > 10
            ? $"Fq(0x{hex[..5]}..{hex[^5..]})"
            : $"Fq(0x{hex})";
    }
    public virtual Fq Negate() => new(Q, -Value);

    public virtual Fq Inverse()
    {
        BigInteger x0 = 1,
                   x1 = 0,
                   y0 = 0,
                   y1 = 1;
        BigInteger a = Q;
        BigInteger b = Value;

        while (a != 0)
        {
            BigInteger q = b / a;
            BigInteger tempB = b;
            b = a;
            a = ModMath.Mod(tempB, a);
            BigInteger temp_x0 = x0;
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
            return new Fq(Q, Value);
        }

        Fq result = new(Q, 1);
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

        if (BigInteger.ModPow(Value, (Q - 1) / 2, Q) != 1)
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

        BigInteger S = 0;
        BigInteger q = Q - 1;
        while (ModMath.Mod(q, 2) == 0)
        {
            q /= 2;
            S++;
        }

        BigInteger z = 0;
        for (BigInteger i = 0; i < Q; i++)
        {
            BigInteger euler = BigInteger.ModPow(i, (Q - 1) / 2, Q);
            if (euler == ModMath.Mod(-1, Q))
            {
                z = i;
                break;
            }
        }

        BigInteger M = S;
        BigInteger c = BigInteger.ModPow(z, q, Q);
        BigInteger t = BigInteger.ModPow(Value, q, Q);
        BigInteger R = BigInteger.ModPow(Value, (q + 1) / 2, Q);
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
            BigInteger f = t;
            while (f != 1)
            {
                f = ModMath.Mod(f * f, Q);
                i++;
            }

            BigInteger b = BigInteger.ModPow(c, BigInteger.ModPow(2, M - i - 1, Q), Q);
            M = i;
            c = ModMath.Mod(b * b, Q);
            t = ModMath.Mod(t * c, Q);
            R = ModMath.Mod(R * b, Q);
        }
    }

    public virtual Fq AddTo(BigInteger value) => new(Q, Value + value);
    public virtual Fq AddTo(Fq value)
    {
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 add
        // which works since addition is transitive
        // this has the effect of also ensuring that the return is the wider type
        if (value is Fq2) // this works for Fq2 derived types
        {
            return value.AddTo(this);
        }

        return new(Q, Value + value.Value);
    }

    public virtual Fq MultiplyWith(BigInteger value) => new(Q, Value * value);
    public virtual Fq MultiplyWith(Fq value)
    {
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 multiply
        // which works since multiplication is transitive
        // this has the effect of also ensuring that the return is the wider type
        if (value is Fq2) // this works for Fq2 derived types
        {
            return value.MultiplyWith(this);
        }

        return new(Q, Value * value.Value);
    }

    public virtual bool EqualTo(BigInteger value) => false;  // the typescript returns false if value is a bigint
    public virtual bool EqualTo(Fq value) => Value == value.Value && Q == value.Q;

    public virtual Fq Subtract(BigInteger value) => AddTo(-value);
    public virtual Fq Subtract(Fq value) => AddTo(value.Negate());

    public virtual Fq Divide(BigInteger value) => MultiplyWith(new Fq(Q, value).Inverse());
    public virtual Fq Divide(Fq value) => MultiplyWith(value.Inverse());

    public virtual bool LessThan(Fq value) => Value < value.Value;
    public virtual bool GreaterThan(Fq value) => Value > value.Value;

    public virtual bool LessThanOrEqual(Fq value) => LessThan(value) || EqualTo(value);
    public virtual bool GreaterThanOrEqual(Fq value) => GreaterThan(value) || EqualTo(value);

    public virtual Fq Add(BigInteger value) => AddTo(value);
    public virtual Fq Add(Fq value) => AddTo(value);

    public virtual Fq Multiply(BigInteger value) => MultiplyWith(value);
    public virtual Fq Multiply(Fq value) => MultiplyWith(value);

    public virtual bool Equals(BigInteger value) => EqualTo(value);
    public virtual bool Equals(Fq value) => EqualTo(value);
}