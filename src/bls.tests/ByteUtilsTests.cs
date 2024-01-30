using chia.dotnet.bls;
using System.Numerics;

namespace bls.tests;

public class ByteUtilsTests
{
    [Fact]
    public void EncodeInt()
    {
        var bytes100 = ByteUtils.EncodeInt(100);
        Assert.Equal([100], bytes100);

        var bytes200 = ByteUtils.EncodeInt(200);
        Assert.Equal([0, 200], bytes200);

        var bytes300 = ByteUtils.EncodeInt(300);
        Assert.Equal([1, 44], bytes300);
    }

    [Fact]
    public void DecodeInt()
    {
        var i100 = ByteUtils.DecodeInt([100]);
        Assert.Equal(100, i100);

        var i200 = ByteUtils.DecodeInt([0, 200]);
        Assert.Equal(200, i200);

        var i300 = ByteUtils.DecodeInt([1, 44]);
        Assert.Equal(300, i300);
    }
}