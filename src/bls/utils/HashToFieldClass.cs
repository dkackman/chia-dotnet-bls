using System.Numerics;

namespace chia.dotnet.bls;

internal static class HashToFieldClass
{
    public static byte[] I2OSP(BigInteger value, int length)
    {
        if (value < 0 || value >= BigInteger.One << (8 * length))
        {
            throw new Exception($"Bad I2OSP call: value={value}, length={length}.");
        }

        var bytes = new byte[length];
        for (int i = length - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(value & 0xff);
            value >>= 8;
        }
        
        return bytes;
    }

    public static BigInteger OS2IP(IEnumerable<byte> octets)
    {
        BigInteger result = 0;
        foreach (var octet in octets)
        {
            result <<= 8;
            result += octet;
        }

        return result;
    }

    public static byte[] BytesXor(byte[] a, byte[] b) => a.Zip(b, (x, y) => (byte)(x ^ y)).ToArray();

    public static byte[] ExpandMessageXmd(byte[] message, byte[] dst, int length, HashInfo hash)
    {
        var ell = (length + hash.ByteSize - 1) / hash.ByteSize;
        if (ell > 255)
        {
            throw new Exception($"Bad expandMessageXmd call: ell={ell} out of range.");
        }

        byte[] dst_prime = [.. dst, .. I2OSP(dst.Length, 1)];
        var Z_pad = I2OSP(0, hash.BlockSize);
        var lib_str = I2OSP(length, 2);
        var b_0 = hash.Convert([.. Z_pad, .. message, .. lib_str, .. I2OSP(0, 1), .. dst_prime]);

        List<byte[]> bValues = [hash.Convert([.. b_0, .. I2OSP(1, 1), .. dst_prime])];
        for (int i = 1; i <= ell; i++)
        {
            bValues.Add(hash.Convert([.. BytesXor(b_0, bValues[i - 1]), .. I2OSP(i + 1, 1), .. dst_prime]));
        }

        List<byte> pseudoRandomBytes = [];
        foreach (var item in bValues)
        {
            pseudoRandomBytes.AddRange(item);
        }

        return pseudoRandomBytes.Take(length).ToArray();
    }

    public static byte[] ExpandMessageXof(byte[] message, byte[] dst, int length, HashInfo hash)
    {
        byte[] dst_prime = [.. dst, .. I2OSP(dst.Length, 1)];
        byte[] message_prime = [.. message, .. I2OSP(length, 2), .. dst_prime];

        return hash.Convert(message_prime).Take(length).ToArray();
    }

    public static BigInteger[][] HashToField(byte[] message, int count, byte[] dst, BigInteger modulus, int degree, int byteLength, Func<byte[], byte[], int, HashInfo, byte[]> expand, HashInfo hash)
    {
        var lengthInBytes = count * degree * byteLength;
        var pseudoRandomBytes = expand(message, dst, lengthInBytes, hash);
        BigInteger[][] uValues = new BigInteger[count][];
        for (var i = 0; i < count; i++)
        {
            BigInteger[] eValues = new BigInteger[degree];
            for (var j = 0; j < degree; j++)
            {
                var elmOffset = byteLength * (j + i * degree);
                var tv = new ArraySegment<byte>(pseudoRandomBytes, elmOffset, byteLength);
                eValues[j] = OS2IP(tv) % modulus;
            }
            uValues[i] = eValues;
        }

        return uValues;
    }

    public static BigInteger[][] Hp(byte[] message, int count, byte[] dst) => HashToField(message, count, dst, Constants.Q, 1, 64, ExpandMessageXmd, HashInfo.Sha256);

    public static BigInteger[][] Hp2(byte[] message, int count, byte[] dst) => HashToField(message, count, dst, Constants.Q, 2, 64, ExpandMessageXmd, HashInfo.Sha256);
}