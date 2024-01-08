using System.Numerics;

namespace chia.dotnet.bls;

internal static class ModMath
{
    public static BigInteger Mod(BigInteger value, BigInteger modulus) => ((value % modulus) + modulus) % modulus;
}