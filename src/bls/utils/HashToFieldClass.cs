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
        for (var i = length - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(value & 0xff);
            value >>= 8;
        }

        return bytes;
    }

    private static BigInteger OS2IP(IEnumerable<byte> octets)
    {
        BigInteger result = 0;
        foreach (var octet in octets)
        {
            result <<= 8;
            result += octet;
        }

        return result;
    }

    private static void BytesXor(Span<byte> a, Span<byte> b, Span<byte> result)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException("Input spans must have the same length.");
        }

        for (var i = 0; i < a.Length; i++)
        {
            result[i] = (byte)(a[i] ^ b[i]);
        }
    }

    // this method is a performance hotspot so it has optimizations
    public static byte[] ExpandMessageXmd(byte[] message, byte[] dst, int length, HashInfo hash)
    {
        var ell = (length + hash.ByteSize - 1) / hash.ByteSize;
        if (ell > 255)
        {
            throw new Exception($"Bad expandMessageXmd call: ell={ell} out of range.");
        }

        // Preallocate buffer for dst_prime
        byte[] dst_prime = new byte[dst.Length + 1];
        dst.CopyTo(dst_prime, 0);
        dst_prime[dst.Length] = (byte)dst.Length; // Assuming I2OSP returns a single byte for length

        // Preallocate buffer for Z_pad, lib_str, and single byte values
        byte[] Z_pad = I2OSP(0, hash.BlockSize);
        byte[] lib_str = I2OSP(length, 2);
        byte[] zeroByte = I2OSP(0, 1);

        // Calculate the total size needed for the concatenated data
        int totalSize = Z_pad.Length + message.Length + lib_str.Length + zeroByte.Length + dst_prime.Length;
        byte[] concatenated = new byte[totalSize];

        // Copy the data into the preallocated buffer
        int offset = 0;
        Array.Copy(Z_pad, 0, concatenated, offset, Z_pad.Length);
        offset += Z_pad.Length;
        Array.Copy(message, 0, concatenated, offset, message.Length);
        offset += message.Length;
        Array.Copy(lib_str, 0, concatenated, offset, lib_str.Length);
        offset += lib_str.Length;
        Array.Copy(zeroByte, 0, concatenated, offset, zeroByte.Length);
        offset += zeroByte.Length;
        Array.Copy(dst_prime, 0, concatenated, offset, dst_prime.Length);

        // Now use the concatenated array for hash conversion
        byte[] b_0 = hash.Convert(concatenated);

        byte[] bValues = new byte[ell * hash.ByteSize];

        byte[] concatBuffer = ByteUtils.ConcatenateArrays(b_0, I2OSP(1, 1), dst_prime);

        // Use the concatenated array for hash conversion
        var firstHash = hash.Convert(concatBuffer);
        firstHash.CopyTo(bValues, 0);

        Span<byte> xorResult = stackalloc byte[hash.ByteSize]; // Assuming hash.ByteSize is small enough for stack allocation
        for (var i = 1; i < ell; i++)
        {
            var previousSegment = new Span<byte>(bValues, (i - 1) * hash.ByteSize, hash.ByteSize);
            BytesXor(b_0, previousSegment, xorResult);

            // Use ConcatenateArrays to concatenate xorResult, I2OSP(i + 1, 1), and dst_prime
            byte[] currentSegmentInput = ByteUtils.ConcatenateArrays(xorResult.ToArray(), I2OSP(i + 1, 1), dst_prime);
            var currentSegment = hash.Convert(currentSegmentInput);

            Array.Copy(currentSegment, 0, bValues, i * hash.ByteSize, hash.ByteSize);
        }

        if (bValues.Length > length)
        {
            var result = new byte[length];
            Array.Copy(bValues, result, length);
            return result;
        }

        return bValues;
    }

    public static BigInteger[][] HashToField(byte[] message, int count, byte[] dst, BigInteger modulus, int degree, int byteLength, Func<byte[], byte[], int, HashInfo, byte[]> expand, HashInfo hash)
    {
        var lengthInBytes = count * degree * byteLength;
        var pseudoRandomBytes = expand(message, dst, lengthInBytes, hash);
        BigInteger[][] uValues = new BigInteger[count][];
        for (var i = 0; i < count; i++)
        {
            var eValues = new BigInteger[degree];
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

    public static BigInteger[][] Hp2(byte[] message, int count, byte[] dst) => HashToField(message, count, dst, Constants.Q, 2, 64, ExpandMessageXmd, HashInfo.Sha256);
}