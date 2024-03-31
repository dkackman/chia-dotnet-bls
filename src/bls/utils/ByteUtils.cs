using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace chia.dotnet.bls;

/// <summary>
/// Enum representing the endianness of byte arrays.
/// </summary>
public enum Endian
{
    /// <summary>
    /// Little endian byte order.
    /// </summary>
    Little,

    /// <summary>
    /// Big endian byte order.
    /// </summary>
    Big
}

/// <summary>
/// Extension methods for working with byte arrays and conversions.
/// </summary>
public static class ByteUtils
{
    /// <summary>
    /// Calculates the number of bits required to represent a BigInteger.
    /// </summary>
    /// <param name="value">The BigInteger value.</param>
    /// <returns>The number of bits required to represent the BigInteger.</returns>
    public static int BitLength(this long value)
    {
        if (value == 0)
        {
            return 0;
        }
        value = Math.Abs(value);
        int bits = 0;
        while (value > 0)
        {
            bits++;
            value >>= 1;
        }
        return bits;
    }

    /// <summary>
    /// Calculates the number of bits required to represent a BigInteger.
    /// </summary>
    /// <param name="value">The BigInteger value.</param>
    /// <returns>The number of bits required to represent the BigInteger.</returns>
    public static long BitLength(this BigInteger value) => value.IsZero ? 0 : value.GetBitLength();

    /// <summary>
    /// Converts a string to a byte array using UTF-8 encoding.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The byte array representation of the string.</returns>
    public static byte[] ToBytes(this string value) => Encoding.UTF8.GetBytes(value);

    /// <summary>
    /// Converts a BigInteger to a byte array representing its binary representation.
    /// </summary>
    /// <param name="i">The BigInteger to convert.</param>
    /// <returns>The byte array representing the binary representation of the BigInteger.</returns>
    public static byte[] ToBits(this BigInteger i)
    {
        Debug.Assert(i.Sign >= 0);

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

    /// <summary>
    /// Converts a hexadecimal string to a byte array.
    /// </summary>
    /// <param name="hex">The hexadecimal string to convert.</param>
    /// <returns>The byte array representation of the hexadecimal string.</returns>
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

    /// <summary>
    /// Converts a long integer to a byte array.
    /// </summary>
    /// <param name="value">The long integer to convert.</param>
    /// <param name="size">The size of the resulting byte array.</param>
    /// <param name="endian">The endianness of the byte array.</param>
    /// <param name="signed">Indicates whether the value is signed or unsigned.</param>
    /// <returns>The byte array representation of the long integer.</returns>
    public static byte[] ToBytes(this long value, int size, Endian endian = Endian.Big, bool signed = false)
    {
        Debug.Assert(!(value < 0 && !signed));
        Debug.Assert(value == Math.Floor((double)value));
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

    /// <summary>
    /// Converts a long integer to a byte array, as signed and big endian,
    /// with a special case for zero returning an empty array.
    /// </summary>
    /// <param name="value">The long integer to convert.</param>
    /// <returns>The byte array representation of the long integer.</returns>
    public static byte[] Encode(this long value)
    {
        if (value == 0)
        {
            return [];
        }

        int length = (BitLength(value) + 8) >> 3;
        byte[] bytes = value.ToBytes(length, Endian.Big, true);
        int start = 0;
        while (
            bytes.Length - start > 1 &&
            bytes[start] == ((bytes[start + 1] & 0x80) != 0 ? (byte)0xff : (byte)0)
        )
        {
            start++;
        }

        return bytes.AsSpan(start).ToArray();
    }

    /// <summary>
    /// Converts a <see cref="BigInteger"/> to a byte array, as signed and big endian,
    /// with a special case for zero returning an empty array.
    /// </summary>
    /// <param name="value">The <see cref="BigInteger"/> to convert.</param>
    /// <returns>The byte array representation of the <see cref="BigInteger"/>.</returns>
    public static byte[] Encode(this BigInteger value)
    {
        if (value.IsZero)
        {
            return [];
        }

        int length = (int)(BitLength(value) + 8) >> 3;
        byte[] bytes = value.ToBytes(length, Endian.Big, true);
        int start = 0;
        while (
            bytes.Length - start > 1 &&
            bytes[start] == ((bytes[start + 1] & 0x80) != 0 ? (byte)0xff : (byte)0)
        )
        {
            start++;
        }

        return bytes.AsSpan(start).ToArray();
    }

    /// <summary>
    /// Converts a byte array to a long integer, assuming signed and big endian
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The long integer representation of the byte array.</returns>
    public static long DecodeInt(this byte[] bytes) => bytes.ToInt(Endian.Big, true);

    /// <summary>
    /// Converts a byte array to a big integer, assuming signed and big endian
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The big integer representation of the byte array.</returns>
    public static BigInteger DecodeBigInt(this byte[] bytes) => bytes.ToBigInt(Endian.Big, true);

    /// <summary>
    /// Converts a uint to a four byte array.
    /// </summary>
    /// <param name="input">The uint</param>
    /// <returns>The byte array</returns>
    public static byte[] ToBytes(this uint input)
    {
        byte[] result = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            result[3 - i] = (byte)(input >> (i * 8));
        }
        return result;
    }

    /// <summary>
    /// Converts a uint to a four byte array.
    /// </summary>
    /// <param name="input">The uint</param>
    /// <returns>The byte array</returns>
    public static uint ToUint(this byte[] bytes) => (uint)bytes.ToInt(Endian.Big, false);

    /// <summary>
    /// Converts a byte array to a long integer.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="endian">The endianness of the byte array.</param>
    /// <param name="signed">Indicates whether the value is signed or unsigned.</param>
    /// <returns>The long integer representation of the byte array.</returns>
    public static long ToInt(this byte[] bytes, Endian endian = Endian.Big, bool signed = false)
    {
        if (bytes.Length == 0)
        {
            return 0;
        }

        long result = 0;
        int shift = 0;

        if (endian == Endian.Little)
        {
            for (int i = 0; i < bytes.Length; i++, shift += 8)
            {
                result |= (long)bytes[i] << shift;
            }
        }
        else
        {
            for (int i = bytes.Length - 1; i >= 0; i--, shift += 8)
            {
                result |= (long)bytes[i] << shift;
            }
        }

        // Check if the number should be treated as signed and adjust accordingly
        if (signed && (bytes[0] & 0x80) != 0)
        {
            // Apply sign extension
            result |= -1L << (bytes.Length * 8);
        }

        return result;
    }

    /// <summary>
    /// Converts a BigInteger to a byte array.
    /// </summary>
    /// <param name="value">The BigInteger to convert.</param>
    /// <param name="size">The size of the resulting byte array.</param>
    /// <param name="endian">The endianness of the byte array.</param>
    /// <param name="signed">Indicates whether the value is signed or unsigned.</param>
    /// <returns>The byte array representation of the BigInteger.</returns>
    public static byte[] ToBytes(this BigInteger value, int size, Endian endian = Endian.Big, bool signed = false)
    {
        Debug.Assert(!(value < 0 && !signed));

        byte[] fullBytes = value.ToByteArray();

        Debug.Assert(fullBytes.Length <= size);

        byte[] result = new byte[size];

        if (endian == Endian.Little)
        {
            Array.Copy(fullBytes, result, fullBytes.Length);
        }
        else
        {
            // Copy bytes in reverse order for Big Endian
            for (int i = 0; i < fullBytes.Length; i++)
            {
                result[size - 1 - i] = fullBytes[i];
            }
        }

        return result;
    }

    /// <summary>
    /// Converts a byte array to a BigInteger.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="endian">The endianness of the byte array.</param>
    /// <param name="signed">Indicates whether the value is signed or unsigned.</param>
    /// <returns>The BigInteger representation of the byte array.</returns>
    public static BigInteger ToBigInt(this byte[] bytes, Endian endian = Endian.Big, bool signed = false) => new(bytes, !signed, endian == Endian.Big);

    /// <summary>
    /// Checks if two byte arrays are equal.
    /// </summary>
    /// <param name="a">The first byte array.</param>
    /// <param name="b">The second byte array.</param>
    /// <returns>True if the byte arrays are equal, false otherwise.</returns>
    public static bool BytesEqual(this byte[] a, byte[] b) => a.AsSpan().SequenceEqual(b.AsSpan());

    /// <summary>
    /// Checks if a byte array is all zeros.
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public static bool IsAllZeros(this byte[] a) => a.All(b => b == 0);

    /// <summary>
    /// Converts a byte array to a hexadecimal string.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <returns>The hexadecimal string representation of the byte array.</returns>
    public static string ToHex(this byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }

    /// <summary>
    /// Converts a hexadecimal string to a byte array.
    /// </summary>
    /// <param name="hex">The hexadecimal string to convert.</param>
    /// <returns>The byte array representation of the hexadecimal string.</returns>
    public static byte[] ToHexBytes(this string hex)
    {
        Debug.Assert(hex.Length % 2 == 0);

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)((GetHexVal(hex[i * 2]) << 4) + GetHexVal(hex[i * 2 + 1]));
        }

        return bytes;
    }

    private static int GetHexVal(int val) => val - (val < 58 ? 48 : (val < 97 ? 55 : 87));

    /// <summary>
    /// Concatenates multiple byte arrays into a single byte array.
    /// </summary>
    /// <param name="arrays">The byte arrays to concatenate.</param>
    /// <returns>The concatenated byte array.</returns>
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