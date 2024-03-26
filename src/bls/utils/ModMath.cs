using System.Numerics;

namespace chia.dotnet.bls;

internal static class ModMath
{
    public static BigInteger Mod(BigInteger value, BigInteger modulus)
    {
        // these optimizations have a pretty good impact on performance
        // by limiting the number of modulus operations that are performed
        if (value.Sign < 0)
        {
            if (modulus.IsPowerOfTwo)
            {
                return value & (modulus - 1);
            }

            return ((value % modulus) + modulus) % modulus;
        }

        return value % modulus;
    }
}