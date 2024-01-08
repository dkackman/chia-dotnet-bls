using System.Security.Cryptography;

namespace chia.dotnet.bls;

internal static class Hmac
{
    public const int HmacBlockSize = 64;

    public static byte[] Hash256(byte[] message) => SHA256.HashData(message);

    public static byte[] Hash512(byte[] message)
    {
        var messageWithZero = new byte[message.Length + 1];
        Array.Copy(message, messageWithZero, message.Length);
        messageWithZero[^1] = 0;

        var messageWithOne = new byte[message.Length + 1];
        Array.Copy(message, messageWithOne, message.Length);
        messageWithOne[^1] = 1;

        return [.. Hash256(messageWithZero), .. Hash256(messageWithOne)];
    }

    public static byte[] Hmac256(byte[] message, byte[] k)
    {
        if (k.Length > HmacBlockSize)
        {
            k = Hash256(k);
        }
        while (k.Length < HmacBlockSize)
        {
            Array.Resize(ref k, k.Length + 1);
        }

        var kopad = new byte[HmacBlockSize];
        for (int i = 0; i < HmacBlockSize; i++)
        {
            kopad[i] = (byte)(k[i] ^ 0x5c);
        }

        var kipad = new byte[HmacBlockSize];
        for (int i = 0; i < HmacBlockSize; i++)
        {
            kipad[i] = (byte)(k[i] ^ 0x36);
        }

        var kipadAndMessage = new byte[kipad.Length + message.Length];
        Array.Copy(kipad, 0, kipadAndMessage, 0, kipad.Length);
        Array.Copy(message, 0, kipadAndMessage, kipad.Length, message.Length);

        return Hash256([.. kopad, .. Hash256(kipadAndMessage)]);
    }
}