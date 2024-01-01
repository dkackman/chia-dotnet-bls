using System.Numerics;
using System.Text;

namespace chia.dotnet.bls;

public static class BigIntegerExtensions
{
    public static string ToBinaryString(this BigInteger bigInt, int size)
    {
        // Check if the number is negative
        var isNegative = bigInt.Sign < 0;

        // Convert BigInteger to its absolute byte array representation
        var byteArray = BigInteger.Abs(bigInt).ToByteArray();
        var binaryStringBuilder = new StringBuilder(size);

        // Convert to binary and append to StringBuilder
        foreach (var b in byteArray)
        {
            binaryStringBuilder.Insert(0, Convert.ToString(b, 2).PadLeft(8, '0'));
        }

        var binaryString = binaryStringBuilder.ToString();

        // If negative, apply two's complement
        if (isNegative)
        {
            binaryString = ApplyTwosComplement(binaryString);
        }

        // Adjust the string length based on the specified size
        if (binaryString.Length > size)
        {
            binaryString = binaryString.Substring(binaryString.Length - size);
        }
        else if (binaryString.Length < size)
        {
            var padChar = isNegative ? '1' : '0';
            binaryString = binaryString.PadLeft(size, padChar);
        }

        return binaryString;
    }

    static string ApplyTwosComplement(string binaryString)
    {
        // Invert the bits
        var invertedArray = binaryString.Select(c => c == '0' ? '1' : '0').ToArray();

        // Convert to BigInteger and add 1 to get the two's complement
        var invertedBigInt = new BigInteger(ConvertToByteArray(new string(invertedArray))) + 1;

        // Convert back to binary string
        return invertedBigInt.ToString("D");
    }

    static byte[] ConvertToByteArray(string str)
    {
        var byteArray = new byte[(str.Length + 7) / 8];
        var byteIndex = 0;
        var bitIndex = 0;

        for (var i = str.Length - 1; i >= 0; i--)
        {
            if (str[i] == '1')
            {
                byteArray[byteIndex] |= (byte)(1 << bitIndex);
            }

            bitIndex++;
            if (bitIndex == 8)
            {
                bitIndex = 0;
                byteIndex++;
            }
        }

        return byteArray;
    }
}
