using System.Numerics;
using System.Text;

namespace chia.dotnet.bls;

internal static class BigIntegerExtensions
{
    public static string ToBinaryString(this BigInteger bigInt, int size)
    {
        var isNegative = bigInt.Sign < 0;

        // Convert BigInteger to its absolute byte array representation
        var byteArray = BigInteger.Abs(bigInt).ToByteArray();

        // Reverse the byte array for big-endian representation
        Array.Reverse(byteArray);

        var binaryStringBuilder = new StringBuilder();

        // Convert each byte to its binary representation and append
        foreach (var b in byteArray)
        {
            binaryStringBuilder.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }

        var binaryString = binaryStringBuilder.ToString().TrimStart('0');

        // Adjusting for two's complement if the number is negative
        if (isNegative)
        {
            binaryString = InvertBitsAndAddOne(binaryString, size);
        }
        else
        {
            binaryString = binaryString.PadLeft(size, '0');
        }

        // Ensuring the binary string is of the specified size
        if (binaryString.Length > size)
        {
            binaryString = binaryString[^size..];
        }

        return binaryString;
    }

    private static string InvertBitsAndAddOne(string binaryString, int size)
    {
        var invertedString = new StringBuilder(size);

        // Inverting the bits
        foreach (var c in binaryString)
        {
            invertedString.Append(c == '0' ? '1' : '0');
        }

        // Padding to the left if necessary
        invertedString.Insert(0, new string('1', size - invertedString.Length));

        // Adding one to the inverted binary string
        var carry = true;
        for (int i = size - 1; i >= 0 && carry; i--)
        {
            if (invertedString[i] == '1')
            {
                invertedString[i] = '0';
            }
            else
            {
                invertedString[i] = '1';
                carry = false;
            }
        }

        return invertedString.ToString();
    }
}
