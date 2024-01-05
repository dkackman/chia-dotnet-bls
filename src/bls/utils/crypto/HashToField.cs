using System.Numerics;

namespace chia.dotnet.bls;

public static class HashToFieldClass
{
    public static byte[] I2OSP(BigInteger value, int length)
    {
        if (value < 0 || value >= BigInteger.One << (8 * length))
            throw new Exception($"Bad I2OSP call: value={value}, length={length}.");
        var bytes = new byte[length];
        for (int i = length - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(value & 0xff);
            value >>= 8;
        }
        return bytes;
    }

    public static BigInteger OS2IP(byte[] octets)
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
        int ell = (length + hash.ByteSize - 1) / hash.ByteSize;
        if (ell > 255)
            throw new Exception($"Bad expandMessageXmd call: ell={ell} out of range.");
        byte[] dst_prime = dst.Concat(I2OSP(dst.Length, 1)).ToArray();
        byte[] Z_pad = I2OSP(0, hash.BlockSize);
        byte[] lib_str = I2OSP(length, 2);
        byte[] b_0 = hash.Convert(Z_pad.Concat(message).Concat(lib_str).Concat(I2OSP(0, 1)).Concat(dst_prime).ToArray());
        List<byte[]> bValues = [hash.Convert([.. b_0, .. I2OSP(1, 1), .. dst_prime])];
        for (int i = 1; i <= ell; i++)
        {
            bValues.Add(hash.Convert(BytesXor(b_0, bValues[i - 1]).Concat(I2OSP(i + 1, 1)).Concat(dst_prime).ToArray()));
        }
        List<byte> pseudoRandomBytes = [];
        foreach (var item in bValues)
            pseudoRandomBytes.AddRange(item);

        return pseudoRandomBytes.Take(length).ToArray();
    }

    public static byte[] ExpandMessageXof(byte[] message, byte[] dst, int length, HashInfo hash)
    {
        byte[] dst_prime = dst.Concat(I2OSP(dst.Length, 1)).ToArray();
        byte[] message_prime = message.Concat(I2OSP(length, 2)).Concat(dst_prime).ToArray();
        return hash.Convert(message_prime).Take(length).ToArray();
    }

    public static List<List<BigInteger>> HashToField(byte[] message, int count, byte[] dst, BigInteger modulus, int degree, int byteLength, Func<byte[], byte[], int, HashInfo, byte[]> expand, HashInfo hash)
    {
        int lengthInBytes = count * degree * byteLength;
        byte[] pseudoRandomBytes = expand(message, dst, lengthInBytes, hash);
        List<List<BigInteger>> uValues = new List<List<BigInteger>>();
        for (int i = 0; i < count; i++)
        {
            List<BigInteger> eValues = new List<BigInteger>();
            for (int j = 0; j < degree; j++)
            {
                int elmOffset = byteLength * (j + i * degree);
                byte[] tv = pseudoRandomBytes.Skip(elmOffset).Take(byteLength).ToArray();
                eValues.Add(OS2IP(tv) % modulus);
            }
            uValues.Add(eValues);
        }
        return uValues;
    }
    public static List<List<BigInteger>> Hp(byte[] message, int count, byte[] dst) => HashToField(message, count, dst, Constants.Q, 1, 64, ExpandMessageXmd, HashInfo.Sha256);

    public static List<List<BigInteger>> Hp2(byte[] message, int count, byte[] dst) => HashToField(message, count, dst, Constants.Q, 2, 64, ExpandMessageXmd, HashInfo.Sha256);
}