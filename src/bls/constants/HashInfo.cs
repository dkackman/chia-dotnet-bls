using System.Security.Cryptography;

namespace chia.dotnet.bls
{
    /// <summary>
    /// Represents information about a hash algorithm.
    /// </summary>
    internal class HashInfo
    {
        /// <summary>
        /// Gets or sets the size of the hash output in bytes.
        /// </summary>
        public int ByteSize { get; init; } = 32;

        /// <summary>
        /// Gets or sets the size of the hash block in bytes.
        /// </summary>
        public int BlockSize { get; init; } = 64;

        /// <summary>
        /// Gets or sets the function used to convert input data to a hash.
        /// </summary>
        public Func<byte[], byte[]> Convert { get; init; } = SHA256.HashData;

        /// <summary>
        /// Represents the SHA-256 hash algorithm.
        /// </summary>
        public static readonly HashInfo Sha256 = new();

        /// <summary>
        /// Represents the SHA-512 hash algorithm.
        /// </summary>
        public static readonly HashInfo Sha512 = new()
        {
            ByteSize = 64,
            BlockSize = 128,
            Convert = SHA512.HashData
        };
    }
}


