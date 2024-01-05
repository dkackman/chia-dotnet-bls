using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
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
    public static long Flip(this string binary) => Convert.ToInt64(new string(binary.Select(c => c == '0' ? '1' : '0').ToArray()), 2);

    public static int IntBitLength(this long value) => Math.Abs(value).ToString("2").Length;

    public static int BigIntBitLength(this BigInteger value) => (value < 0 ? -value : value).ToString("2").Length;

    public static List<long> BigIntToBits(this BigInteger i)
    {
        var bits = new List<long>();
        while (i != 0)
        {
            bits.Add((long)ModMath.Mod(i, 2));
            i /= 2;
        }
        bits.Reverse();
        return bits;
    }

    public static IEnumerable<long> IntToBits(this long i)
    {
        var bits = new List<long>();
        while (i != 0)
        {
            bits.Add(i % 2);
            i /= 2;
        }
        bits.Reverse();
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

    public static long BytesToInt(this byte[] bytes, Endian endian, bool signed = false)
    {
        if (bytes.Length == 0)
            return 0;
        var sign = Convert.ToString(bytes[endian == Endian.Little ? bytes.Length - 1 : 0], 2).PadLeft(8, '0')[0].ToString();
        var byteList = endian == Endian.Little ? bytes.Reverse().ToArray() : bytes;
        var binary = "";
        foreach (var byteVal in byteList)
        {
            binary += Convert.ToString(byteVal, 2).PadLeft(8, '0');
        }

        if (sign == "1" && signed)
        {
            binary = Convert.ToString(binary.Flip() + 1, 2).PadLeft(bytes.Length * 8, '0');
        }
        var result = Convert.ToInt64(binary, 2);
        return sign == "1" && signed ? -result : result;
    }

    public static byte[] EncodeInt(this long value)
    {
        if (value == 0)
        {
            return ""u8.ToArray();
        }

        var length = (value.IntBitLength() + 8) >> 3;
        var bytes = value.IntToBytes(length, Endian.Big, true);
        while (bytes.Length > 1 && bytes[0] == ((bytes[1] & 0x80) != 0 ? (byte)0xff : (byte)0))
        {
            bytes = bytes.Skip(1).ToArray();
        }
        return bytes;
    }

    public static long DecodeInt(this byte[] bytes) => bytes.BytesToInt(Endian.Big, true);
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
        return bytes.Select(b => b).ToArray();
    }

    public static BigInteger BytesToBigInt(this byte[] bytes, Endian endian, bool signed = false) => new(bytes, !signed, endian == Endian.Big);

    public static byte[] EncodeBigInt(this BigInteger value)
    {
        if (value == 0) return ""u8.ToArray();
        var length = (value.BigIntBitLength() + 8) >> 3;
        var bytes = BigIntToBytes(value, length, Endian.Big, true);
        while (bytes.Length > 1 && bytes[0] == ((bytes[1] & 0x80) != 0 ? (byte)0xff : (byte)0))
        {
            bytes = bytes.Skip(1).ToArray();
        }
        return bytes;
    }

    public static BigInteger DecodeBigInt(this byte[] bytes) => BytesToBigInt(bytes, Endian.Big, true);

    public static byte[] ConcatBytes(params byte[][] lists)
    {
        List<byte> bytes = [];
        foreach (var list in lists)
        {
            bytes.AddRange(list);
        }
        return [.. bytes];
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