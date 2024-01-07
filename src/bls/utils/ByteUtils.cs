using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace chia.dotnet.bls;

public enum Endian
{
    Little,
    Big
}

public static partial class ByteUtils
{
    public static byte[] ToBytes(this string value) => Encoding.UTF8.GetBytes(value);
    public static long Flip(this string binary) => Convert.ToInt64(new string(binary.Select(c => c == '0' ? '1' : '0').ToArray()), 2);

    public static int IntBitLength(this long value) => Math.Abs(value).ToString("2").Length;

    public static int BigIntBitLength(this BigInteger value) => (value < 0 ? -value : value).ToString("2").Length;

    public static long[] ToBits(this BigInteger i)
    {
        int size = i == 0 ? 1 : (int)Math.Floor(BigInteger.Log(i, 2)) + 1;
        long[] bits = new long[size];
        for (int index = size - 1; index >= 0; index--)
        {
            bits[index] = (long)ModMath.Mod(i, 2);
            i /= 2;
        }
        return bits;
    }

    public static byte[] HexStringToByteArray(this string value) => Enumerable.Range(0, value.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(value.Substring(x, 2), 16))
                             .ToArray();

    public static byte[] IntToBytes(this long value, int size, Endian endian, bool signed = false)
    {
        if (value < 0 && !signed)
            throw new Exception("Cannot convert negative number to unsigned.");
        if (value != Math.Floor((double)value))
            throw new Exception("Cannot convert floating point number.");

        var binary = Convert.ToString(Math.Abs(value), 2).PadLeft(size * 8, '0');
        if (value < 0)
        {
            binary = Convert.ToString(binary.Flip() + 1, 2).PadLeft(size * 8, '0');
        }

        var bytes = MyRegex().Matches(binary).Select(match => Convert.ToByte(match.Value, 2)).ToArray();
        if (endian == Endian.Little)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    public static long ToInt(this byte[] bytes, Endian endian, bool signed = false)
    {
        if (bytes.Length == 0)
        {
            return 0;
        }

        // Ensure the byte array is not longer than 8 bytes, as it won't fit in a long.
        if (bytes.Length > 8)
        {
            throw new ArgumentException("Byte array too long to convert to a long.");
        }

        long result = 0;
        bool isLittleEndian = BitConverter.IsLittleEndian;

        // Adjust the byte order based on endianness.
        if ((isLittleEndian && endian == Endian.Big) || (!isLittleEndian && endian == Endian.Little))
        {
            Array.Reverse(bytes);
        }

        // Combine the bytes into a long value.
        foreach (var byteValue in bytes)
        {
            result = (result << 8) | byteValue;
        }

        // Handle signed conversion.
        if (signed && (bytes[0] & 0x80) != 0)
        {
            // If the number is negative, fill the leftmost bits with 1's for correct two's complement representation.
            for (int i = bytes.Length; i < 8; i++)
            {
                result |= (long)0xFF << (i * 8);
            }
        }

        return result;
    }

    public static byte[] EncodeInt(this long value) => BitConverter.GetBytes(value);
    public static long DecodeInt(this byte[] bytes) => bytes.ToInt(Endian.Big, true);
    public static byte[] BigIntToBytes(this BigInteger value, int size, Endian endian, bool signed = false)
    {
        if (value < 0 && !signed)
            throw new Exception("Cannot convert negative number to unsigned.");

        var binary = value.ToBinaryString(size * 8);
        var bytes = MyRegex().Matches(binary).Select(match => Convert.ToByte(match.Value, 2)).ToArray();
        if (endian == Endian.Little)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }

    public static BigInteger BytesToBigInt(this byte[] bytes, Endian endian, bool signed = false) => new(bytes, !signed, endian == Endian.Big);

    public static byte[] EncodeBigInt(this BigInteger value)
    {
        if (value == 0)
        {
            return ""u8.ToArray();
        }

        var length = (value.BigIntBitLength() + 8) >> 3;
        var bytes = BigIntToBytes(value, length, Endian.Big, true);
        while (bytes.Length > 1 && bytes[0] == ((bytes[1] & 0x80) != 0 ? (byte)0xff : (byte)0))
        {
            bytes = bytes.Skip(1).ToArray();
        }

        return bytes;
    }

    public static BigInteger DecodeBigInt(this byte[] bytes) => BytesToBigInt(bytes, Endian.Big, true);

    public static byte[] ConcatBytes(params byte[][] arrays)
    {
        // Calculate the total size needed
        int totalSize = 0;
        foreach (var array in arrays)
        {
            totalSize += array.Length;
        }

        // Allocate the array
        byte[] bytes = new byte[totalSize];

        // Fill the array
        int offset = 0;
        foreach (var array in arrays)
        {
            Array.Copy(array, 0, bytes, offset, array.Length);
            offset += array.Length;
        }

        return bytes;
    }

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

    [GeneratedRegex("[01]{8}")]
    private static partial Regex MyRegex();
}