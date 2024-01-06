using chia.dotnet.bls;
using System.Security.Cryptography;

namespace bls.tests;

public class XMDTests
{
    private readonly byte[] msg = RandomNumberGenerator.GetBytes(48);
    private readonly byte[] dst = RandomNumberGenerator.GetBytes(16);
    private Dictionary<byte[], int> ress = new(new ByteArrayComparer());

    [Fact]
    public void LengthsAreCorrect()
    {
        for (int length = 16; length < 8192; length++)
        {
            var result = HashToFieldClass.ExpandMessageXmd(msg, dst, length, HashInfo.Sha512);
            Assert.Equal(length, result.Length);

            var key = result.Take(16).ToArray();
            key = ress.Keys.FirstOrDefault(buffer => ByteUtils.BytesEqual(key, buffer)) ?? key;
            if (ress.TryGetValue(key, out int value))
            {
                ress[key] = ++value;
            }
            else
            {
                ress.Add(key, 1);
            }
        }
    }

    [Fact]
    public void AllOnes()
    {
        foreach (var item in ress.Values)
        {
            Assert.Equal(1, item);
        }
    }
}

public class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is null || y is null)
            return false;

        if (x.Length != y.Length)
            return false;

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
            {
                return false;
            }
        }
        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        return obj.Sum(b => b);
    }
}