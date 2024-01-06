using System.Security.Cryptography;

namespace chia.dotnet.bls;

internal static class Hkdf
{
    public const int BlockSize = 32;

    public static byte[] Extract(byte[] salt, byte[] ikm)
    {
        using var hmac = new HMACSHA256(salt);
        return hmac.ComputeHash(ikm);
    }

    public static byte[] Expand(int length, byte[] prk, byte[] info)
    {
        var blocks = (int)Math.Ceiling((double)length / BlockSize);
        var bytesWritten = 0;
        var okm = new byte[length];
        var temp = Array.Empty<byte>();
        for (var i = 1; i <= blocks; i++)
        {
            temp = Extract(prk, i == 1 ? [.. info, .. new byte[] { 1 }] : [.. temp, .. info, .. new byte[] { (byte)i }]);
            var toWrite = length - bytesWritten;
            if (toWrite > BlockSize) toWrite = BlockSize;
            Array.Copy(temp, 0, okm, bytesWritten, toWrite);
            bytesWritten += toWrite;
        }
        if (bytesWritten != length) 
            throw new Exception("Bytes written does not match length");
        return okm;
    }

    public static byte[] ExtractExpand(int length, byte[] key, byte[] salt, byte[] info) => Expand(length, Extract(salt, key), info);
}