using System.Numerics;

namespace chia.dotnet.bls;

internal static class ModMath
{
    public static BigInteger ModPow(BigInteger @base, BigInteger exponent, BigInteger modulo)
    {
        if (exponent < 1)
        {
            return 1;
        }

        if (@base < 0 || @base > modulo)
        {
            @base = Mod(@base, modulo);
        }

        BigInteger result = 1;
        while (exponent > 0)
        {
            if ((exponent & 1) > 0)
            {
                result = Mod(result * @base, modulo);
            }
            exponent >>= 1;
            @base = Mod(@base * @base, modulo);
        }

        return result;
    }

    public static BigInteger Mod(BigInteger value, BigInteger modulus) => ((value % modulus) + modulus) % modulus;
}