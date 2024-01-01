using System;
using System.Numerics;

namespace chia.dotnet.bls;

// incase we need to pass these around without knowing T
public interface IField
{
    BigInteger Q { get; }
    int Extension { get; }
    byte[] ToBytes();
    bool ToBool();
    string ToHex();
    bool EqualTo(BigInteger value);
    bool Equals(BigInteger value);
}

public interface IField<T> : IField where T : IField<T>
{
    T Zero(BigInteger Q);
    T One(BigInteger Q);
    T FromBytes(BigInteger Q, byte[] bytes);
    T FromHex(BigInteger Q, string hex);
    T FromFq(BigInteger Q, Fq fq);
    T Clone();
    T Negate();
    T Inverse();
    T QiPower(int i);
    T Pow(BigInteger exponent);
    T AddTo(T value);
    T AddTo(BigInteger value);
    T MultiplyWith(T value);
    T MultiplyWith(BigInteger value);
    T Add(T value);
    T Add(BigInteger value);
    T Subtract(T value);
    T Subtract(BigInteger value);
    T Multiply(T value);
    T Multiply(BigInteger value);
    T Divide(T value);
    T Divide(BigInteger value);
    bool EqualTo(T value);
    bool Equals(T value);
    bool LessThan(T value);
    bool GreaterThan(T value);
    bool LessThanOrEqual(T value);
    bool GreaterThanOrEqual(T value);
}

public interface IFieldExt<T> : IField<T> where T : IField<T>
{
    T Root { get; set; }
    T[] Elements { get; }
    T Construct(BigInteger Q, T[] elements);
    T ModSqrt();
    T WithRoot(T root);
    T MulByNonResidue();
}