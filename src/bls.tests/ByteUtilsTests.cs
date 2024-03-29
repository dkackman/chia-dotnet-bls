using chia.dotnet.bls;
using System.Numerics;

namespace bls.tests;

public class ByteUtilsTests
{
    [Theory]
    [InlineData(new byte[] { 0x00, 0xC8 }, Endian.Big, false, 200)] // Positive number in Big Endian
    //[InlineData(new byte[] { 0xC8, 0x00 }, Endian.Little, false, 51200)] // Positive number in Little Endian
    [InlineData(new byte[] { 0xFE }, Endian.Big, true, -2)] // Negative number, 1 byte, signed
    [InlineData(new byte[] { 0x80 }, Endian.Big, true, -128)] // Negative number, edge case, signed
    [InlineData(new byte[] { 0x7F }, Endian.Big, true, 127)] // Positive number, max value for 1 byte, signed
    [InlineData(new byte[] { 0xFF, 0xFF }, Endian.Big, true, -1)] // -1 in Big Endian, 2 bytes, signed
    [InlineData(new byte[] { 0x00, 0x01 }, Endian.Big, false, 1)] // Small positive number in Big Endian
    //[InlineData(new byte[] { 0x01, 0x00 }, Endian.Little, false, 256)] // Small positive number in Little Endian
    [InlineData(new byte[] { 0xFF }, Endian.Big, false, 255)] // Positive number, edge case, unsigned
    public void BytesToInt_ShouldCorrectlyConvert(byte[] input, Endian endian, bool signed, long expected)
    {
        var result = input.ToInt(endian, signed);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void EncodeInt()
    {
        var bytes100 = ByteUtils.Encode(100);
        Assert.Equal([100], bytes100);

        var bytes200 = ByteUtils.Encode(200);
        Assert.Equal([0, 200], bytes200);

        var bytes300 = ByteUtils.Encode(300);
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

    [Fact]
    public void TwoFiftyFour()
    {
        var result = ByteUtils.DecodeInt([254]);
        Assert.Equal(-2, result);
    }

    [Fact]
    public void IntBitLength_Zero_ReturnsZero()
    {
        int result = ByteUtils.BitLength(0);
        Assert.Equal(0, result);
    }

    [Fact]
    public void IntBitLength_PositiveValue_ReturnsCorrectBitLength()
    {
        int result = ByteUtils.BitLength(10);
        Assert.Equal(4, result);
    }

    [Fact]
    public void BigIntBitLength_Zero_ReturnsZero()
    {
        BigInteger value = BigInteger.Zero;
        var result = ByteUtils.BitLength(value);
        Assert.Equal(0, result);
    }


    [Fact]
    public void BigIntBitLength_PositiveValue_ReturnsCorrectBitLength()
    {
        BigInteger value = new BigInteger(123456789);
        var result = ByteUtils.BitLength(value);
        Assert.Equal(27, result);
    }

    [Fact]
    public void ToBytes_ConvertsStringToByteArray()
    {
        string value = "Hello, World!";
        byte[] result = value.ToBytes();
        byte[] expected = new byte[] { 72, 101, 108, 108, 111, 44, 32, 87, 111, 114, 108, 100, 33 };
        Assert.Equal(expected, result);
    }

}