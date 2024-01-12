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

        // go backwards to avoid the need to reverse
        for (var j = bitCount - 1; j >= 0; j--)
        {
            bits[j] = (byte)(i & 1); // Extract the least significant bit
            i >>= 1; // Shift right to get the next bit
        }

        return bits;
    }

    public static byte[] HexStringToByteArray(this string hex)
    {
        var length = hex.Length;
        var bytes = new byte[length / 2];
        for (var i = 0; i < length; i += 2)
        {
            bytes[i / 2] = (byte)((GetHexVal(hex[i]) << 4) + GetHexVal(hex[i + 1]));
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

    private static BigInteger ApplyTwosComplement(BigInteger value, int sizeInBits)
    {
        var maxValue = BigInteger.Pow(2, sizeInBits) - 1;
        return maxValue - value + 1;
    }

    public static BigInteger BytesToBigInt(this byte[] bytes, Endian endian, bool signed = false) => new(bytes, !signed, endian == Endian.Big);

    public static bool BytesEqual(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }

    public static string ToHex(this byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }

    public static byte[] FromHex(this string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Invalid hex string");

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)((GetHexVal(hex[i * 2]) << 4) + GetHexVal(hex[i * 2 + 1]));
        }

        return bytes;
    }

    private static int GetHexVal(int val) => val - (val < 58 ? 48 : (val < 97 ? 55 : 87));

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