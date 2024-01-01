using System.Numerics;

namespace chia.dotnet.bls;

public class Fq(BigInteger Q, BigInteger value) : IField<Fq>
{
    public static readonly Fq Nil = new(1, 0);

    public virtual int Extension { get; } = 1;
    public BigInteger Value { get; private set; } = ModMath.Mod(value, Q); // wrap around is not need in c#
    public virtual BigInteger Q { get; protected set; } = Q;

    public virtual Fq Zero(BigInteger Q) => new(Q, 0);
    public virtual Fq One(BigInteger Q) => new(Q, 1);
    public virtual Fq FromBytes(BigInteger Q, byte[] bytes)
    {
        if (bytes.Length != 48) throw new ArgumentOutOfRangeException(nameof(bytes));
        return new Fq(Q, bytes.BytesToBigInt(Endian.Big));
    }
    public virtual Fq FromHex(BigInteger Q, string hex) => Nil.FromBytes(Q, hex.FromHex());
    public virtual Fq FromFq(BigInteger Q, Fq fq) => fq; // ?
    public virtual Fq Clone() => new(Q, Value);
    public virtual byte[] ToBytes() => Value.BigIntToBytes(48, Endian.Big);
    public virtual bool ToBool() => true;
    public virtual string ToHex() => ToBytes().ToHex();
    public override string ToString()
    {
        string hex = Value.ToString("x"); // Lowercase "x" for lowercase hexadecimal
        hex = hex.Length % 2 == 0 && hex.StartsWith("0") ? hex.Substring(1) : hex;
        return hex.Length > 10
            ? $"Fq(0x{hex.Substring(0, 5)}..{hex.Substring(hex.Length - 5)})"
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
        return exponent == 0
            ? new Fq(Q, 1)
            : exponent == 1
            ? new Fq(Q, Value)
            : ModMath.Mod(exponent, 2) == 0
            ? new Fq(Q, Value * Value).Pow(exponent / 2)
            : new Fq(Q, Value * Value).Pow(exponent / 2).Multiply(this);
    }

    public virtual Fq ModSqrt()
    {
        if (Value == 0)
        {
            return new Fq(Q, 0);
        }

        if (ModMath.ModPow(Value, (Q - 1) / 2, Q) != 1)
        {
            throw new Exception("No sqrt exists.");
        }

        if (ModMath.Mod(Q, 4) == 3)
        {
            return new Fq(
                Q,
                ModMath.ModPow(Value, (Q + 1) / 4, Q)
            );
        }

        if (ModMath.Mod(Q, 8) == 5)
        {
            return new Fq(
                Q,
                ModMath.ModPow(Value, (Q + 3) / 8, Q)
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
            BigInteger euler = ModMath.ModPow(i, (Q - 1) / 2, Q);
            if (euler == ModMath.Mod(-1, Q))
            {
                z = i;
                break;
            }
        }
        BigInteger M = S;
        BigInteger c = ModMath.ModPow(z, q, Q);
        BigInteger t = ModMath.ModPow(Value, q, Q);
        BigInteger R = ModMath.ModPow(Value, (q + 1) / 2, Q);
        while (true)
        {
            if (t == 0)
                return new Fq(Q, 0);

            if (t == 1)
                return new Fq(Q, R);

            BigInteger i = 0;
            BigInteger f = t;
            while (f != 1)
            {
                f = ModMath.Mod(f * f, Q);
                i++;
            }
            BigInteger b = ModMath.ModPow(c, ModMath.ModPow(2, M - i - 1, Q), Q);
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
    public virtual bool EqualTo(Fq value)
    {
        // this is in the typescript code
        // if value is Fq2 or derived from Fq2, then we need to use the Fq2 equal
        // which works if both l & r are of equal sizes
        // if one is bigger than i think this is more of a set operation
        // and not strict equality
        // if (value is Fq2) // this works for Fq2 derived types
        // {
        //     return value.EqualTo(this);
        // }

        return Value == value.Value && Q == value.Q;
    }

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