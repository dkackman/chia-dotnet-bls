using System.Numerics;

namespace chia.dotnet.bls;

internal interface IFq
{
    int Extension { get; }
    BigInteger Q { get; }
    BigInteger Value { get; }

    IFq Add(BigInteger value);
    IFq Add(IFq value);
    IFq Clone();
    IFq Divide(BigInteger value);
    IFq Divide(IFq value);
    bool Equals(BigInteger value);
    bool Equals(IFq value);
    IFq FromBytes(BigInteger q, byte[] bytes);
    IFq FromFq(BigInteger q, IFq fq);
    IFq FromHex(BigInteger q, string hex);
    bool GreaterThan(IFq value);
    bool GreaterThanOrEqual(IFq value);
    IFq Inverse();
    bool LessThan(IFq value);
    bool LessThanOrEqual(IFq value);
    IFq ModSqrt();
    IFq Multiply(BigInteger value);
    IFq Multiply(IFq value);
    IFq Negate();
    IFq One(BigInteger q);
    IFq Pow(BigInteger exponent);
    IFq QiPower(int _i);
    IFq Subtract(BigInteger value);
    IFq Subtract(IFq value);
    bool ToBool();
    byte[] ToBytes();
    string ToHex();
    string ToString();
    IFq Zero(BigInteger q);
}