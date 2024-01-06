using chia.dotnet.bls;
using System.Numerics;
using System.Globalization;

namespace bls.tests;

public class SWUTests
{
    private readonly byte[] dst_1 = "QUUX-V01-CS02-with-BLS12381G2_XMD:SHA-256_SSWU_RO_".ToBytes();
    private readonly byte[] msg_1 = "abcdef0123456789".ToBytes();
    private readonly AffinePoint res;

    public SWUTests()
    {
        res = OptSwu2MapClass.G2Map(msg_1, dst_1).ToAffine();
    }

    [Fact]
    public void FirstXElementIsCorrect()
    {
        Assert.Equal(BigInteger.Parse("0121982811d2491fde9ba7ed31ef9ca474f0e1501297f68c298e9f4c0028add35aea8bb83d53c08cfc007c1e005723cd0", NumberStyles.HexNumber), ((Fq2)res.X).Elements[0].Value);
    }

    [Fact]
    public void SecondXElementIsCorrect()
    {
        Assert.Equal(BigInteger.Parse("0190d119345b94fbd15497bcba94ecf7db2cbfd1e1fe7da034d26cbba169fb3968288b3fafb265f9ebd380512a71c3f2c", NumberStyles.HexNumber), ((Fq2)res.X).Elements[1].Value);
    }

    [Fact]
    public void FirstYElementIsCorrect()
    {
        Assert.Equal(BigInteger.Parse("05571a0f8d3c08d094576981f4a3b8eda0a8e771fcdcc8ecceaf1356a6acf17574518acb506e435b639353c2e14827c8", NumberStyles.HexNumber), ((Fq2)res.Y).Elements[0].Value);
    }

    [Fact]
    public void SecondYElementIsCorrect()
    {
        Assert.Equal(BigInteger.Parse("0bb5e7572275c567462d91807de765611490205a941a5a6af3b1691bfe596c31225d3aabdf15faff860cb4ef17c7c3be", NumberStyles.HexNumber), ((Fq2)res.Y).Elements[1].Value);
    }
}