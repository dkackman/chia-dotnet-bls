using System.Security.Cryptography;

namespace chia.dotnet.bls;

/// <summary>
/// Provides methods for HMAC (Hash-based Message Authentication Code) operations.
/// </summary>
public static class Hmac
{
    /// <summary>
    /// The block size used in HMAC operations.
    /// </summary>
    public const int HmacBlockSize = 64;

    /// <summary>
    /// Computes the SHA-256 hash of the specified message.
    /// </summary>
    /// <param name="message">The message to hash.</param>
    /// <returns>The SHA-256 hash of the message.</returns>
    public static byte[] Hash256(this byte[] message) => SHA256.HashData(message);

    /// <summary>
    /// Computes the SHA-256 hash of the specified message segment.
    /// </summary>
    /// <param name="messageSegment">The segment of the message to hash.</param>
    /// <returns>The SHA-256 hash of the message segment.</returns>
    public static byte[] Hash256(ArraySegment<byte> messageSegment)
    {
        // Convert the ArraySegment to a ReadOnlySpan
        ReadOnlySpan<byte> messageSpan = messageSegment.AsSpan();

        // Hash the span
        return SHA256.HashData(messageSpan);
    }

    /// <summary>
    /// Computes the SHA-512 hash of the specified message.
    /// </summary>
    /// <param name="message">The message to hash.</param>
    /// <returns>The SHA-512 hash of the message.</returns>
    public static byte[] Hash512(this byte[] message)
    {
        var messageWithZero = new byte[message.Length + 1];
        Array.Copy(message, messageWithZero, message.Length);
        messageWithZero[^1] = 0;

        var messageWithOne = new byte[message.Length + 1];
        Array.Copy(message, messageWithOne, message.Length);
        messageWithOne[^1] = 1;

        return ByteUtils.ConcatenateArrays(Hash256(messageWithZero), Hash256(messageWithOne));
    }

    /// <summary>
    /// Computes the HMAC-SHA-256 of the specified message using the specified key.
    /// </summary>
    /// <param name="message">The message to compute the HMAC for.</param>
    /// <param name="k">The key to use for HMAC computation.</param>
    /// <returns>The HMAC-SHA-256 of the message.</returns>
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

        return Hash256(ByteUtils.ConcatenateArrays(kopad, Hash256(kipadAndMessage)));
    }
}