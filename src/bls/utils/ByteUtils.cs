using System.Globalization;
using System.Numerics;
using System.Text;

namespace chia.dotnet.bls;

public enum Endian
{
    Little,
    Big
}

public static partial class ByteUtils
{
    public static byte[] ToBytes(this string value) => Encoding.UTF8.GetBytes(value);

    public static byte[] BigIntToBits(this BigInteger i)
    {
        if (i.Sign < 0)
        {
            throw new ArgumentException("Input must be a non-negative BigInteger.");
        }

        var bitCount = (int)BigInteger.Log2(i) + 1; // Calculate the number of bits needed

        var bits = new byte[bitCount];

        for (var j = 0; j < bitCount; j++)
        {
            bits[j] = (byte)(i & 1); // Extract the least significant bit
            i >>= 1; // Shift right to get the next bit
        }

        Array.Reverse(bits); // Reverse the array to get the bits in the correct order

        return bits;
    }

    public static byte[] HexStringToByteArray(this string hex)
    {
        var length = hex.Length;
        var bytes = new byte[length / 2];
        for (var i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }

    public static byte[] IntToBytes(this long value, int size, Endian endian, bool signed = false)
    {
        if (value < 0 && !signed)
            throw new Exception("Cannot convert negative number to unsigned.");
        if (value != Math.Floor((double)value))
            throw new Exception("Cannot convert floating point number.");

        var bytes = new byte[size];

        for (var i = size - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(value & 0xFF);
            value >>= 8;
        }

        if (endian == Endian.Little)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    public static long BytesToInt(this byte[] bytes, Endian endian, bool signed = false)
    {
        if (bytes.Length == 0)
        {
            return 0;
        }

        if (endian == Endian.Little)
        {
            Array.Reverse(bytes);
        }

        long result = 0;

        for (var i = 0; i < bytes.Length; i++)
        {
            result |= (long)bytes[i] << (i * 8);
        }

        if (signed && (bytes[^1] & 0x80) != 0)
        {
            result -= 1L << (bytes.Length * 8);
        }

        return result;
    }

    public static byte[] BigIntToBytes(this BigInteger value, int size, Endian endian, bool signed = false)
    {
        if (value < 0)
        {
            if (!signed)
                throw new Exception("Cannot convert negative number to unsigned.");

            value = ApplyTwosComplement(value, size * 8);
        }

        var bytes = new byte[size];

        for (var i = size - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(value & 0xFF);
            value >>= 8;
        }

        if (endian == Endian.Little)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    static BigInteger ApplyTwosComplement(BigInteger value, int sizeInBits)
    {
        var maxValue = BigInteger.Pow(2, sizeInBits) - 1;
        return maxValue - value + 1;
    }

    public static BigInteger BytesToBigInt(this byte[] bytes, Endian endian, bool signed = false) => new(bytes, !signed, endian == Endian.Big);

    public static bool BytesEqual(byte[] a, byte[] b) => a.Length == b.Length && !a.Where((t, i) => b[i] != t).Any();

    public static string ToHex(this byte[] bytes) => BitConverter.ToString(bytes).Replace("-", "").ToLower();

    public static byte[] FromHex(this string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Invalid hex string");

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var byteValue = hex.Substring(i * 2, 2);
            bytes[i] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return bytes;
    }
    public static byte[] ConcatenateArrays(params byte[][] arrays)
    {
        // Preallocate a buffer for the concatenated data
        var concatBuffer = new byte[arrays.Sum(a => a.Length)];

        // Copy each array into concatBuffer
        var offset = 0;
        foreach (var array in arrays)
        {
            Array.Copy(array, 0, concatBuffer, offset, array.Length);
            offset += array.Length;
        }

        return concatBuffer;
    }
}