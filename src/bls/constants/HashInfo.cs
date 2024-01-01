using System.Security.Cryptography;

namespace chia.dotnet.bls;

public class HashInfo
{
    public int ByteSize { get; init; } = 32;
    public int BlockSize { get; init; } = 64;
    public Func<byte[], byte[]> Convert { get; init; } = SHA256.HashData;

    public static readonly HashInfo Sha256 = new();

    public static readonly HashInfo Sha512 = new()
    {
        ByteSize = 64,
        BlockSize = 128,
        Convert = SHA512.HashData
    };
}


