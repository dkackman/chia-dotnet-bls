using chia.dotnet.bls;

namespace bls.tests;

public class ChiaVectorsTests
{
    private readonly byte[] seed1 = new byte[32];
    private readonly byte[] seed2 = Enumerable.Repeat<byte>(1, 32).ToArray();
    private readonly byte[] msg1 = [7, 8, 9];
    private readonly byte[] msg2 = [10, 11, 12];
    private readonly PrivateKey sk1;
    private readonly PrivateKey sk2;

    public ChiaVectorsTests()
    {
        sk1 = BasicSchemeMPL.KeyGen(seed1);
        sk2 = BasicSchemeMPL.KeyGen(seed2);
    }

    [Fact]
    public void PrivateKeyIsCorrect()
    {
        Assert.Equal("4a353be3dac091a0a7e640620372f5e1e2e4401717c1e79cac6ffba8f6905604", sk1.ToHex());
    }

    [Fact]
    public void PublicKeyIsCorrect()
    {
        Assert.Equal("85695fcbc06cc4c4c9451f4dce21cbf8de3e5a13bf48f44cdbb18e2038ba7b8bb1632d7911ef1e2e08749bddbf165352", sk1.GetG1Element().ToHex());
    }

    [Fact]
    public void FirstSignatureIsCorrect()
    {
        var sig1 = BasicSchemeMPL.Sign(sk1, msg1);
        Assert.Equal("b8faa6d6a3881c9fdbad803b170d70ca5cbf1e6ba5a586262df368c75acd1d1ffa3ab6ee21c71f844494659878f5eb230c958dd576b08b8564aad2ee0992e85a1e565f299cd53a285de729937f70dc176a1f01432129bb2b94d3d5031f8065a1", sig1.ToHex());
    }

    [Fact]
    public void SecondSignatureIsCorrect()
    {
        var sig2 = BasicSchemeMPL.Sign(sk2, msg2);
        Assert.Equal("a9c4d3e689b82c7ec7e838dac2380cb014f9a08f6cd6ba044c263746e39a8f7a60ffee4afb78f146c2e421360784d58f0029491e3bd8ab84f0011d258471ba4e87059de295d9aba845c044ee83f6cf2411efd379ef38bf4cf41d5f3c0ae1205d", sig2.ToHex());
    }

    [Fact]
    public void FirstAggregateSignatureIsCorrect()
    {
        var sig1 = BasicSchemeMPL.Sign(sk1, msg1);
        var sig2 = BasicSchemeMPL.Sign(sk2, msg2);
        var aggSig1 = BasicSchemeMPL.Aggregate([sig1, sig2]);
        Assert.Equal("aee003c8cdaf3531b6b0ca354031b0819f7586b5846796615aee8108fec75ef838d181f9d244a94d195d7b0231d4afcf06f27f0cc4d3c72162545c240de7d5034a7ef3a2a03c0159de982fbc2e7790aeb455e27beae91d64e077c70b5506dea3", aggSig1.ToHex());
        Assert.True(BasicSchemeMPL.AggregateVerify([sk1.GetG1Element(), sk2.GetG1Element()], [msg1, msg2], aggSig1));
    }

    [Fact]
    public void SecondAggregateSignatureIsCorrect()
    {
        var msg3 = new byte[] { 1, 2, 3 };
        var msg4 = new byte[] { 1, 2, 3, 4 };
        var msg5 = new byte[] { 1, 2 };
        var sig3 = BasicSchemeMPL.Sign(sk1, msg3);
        var sig4 = BasicSchemeMPL.Sign(sk1, msg4);
        var sig5 = BasicSchemeMPL.Sign(sk2, msg5);
        var aggSig2 = BasicSchemeMPL.Aggregate([sig3, sig4, sig5]);
        Assert.Equal("a0b1378d518bea4d1100adbc7bdbc4ff64f2c219ed6395cd36fe5d2aa44a4b8e710b607afd965e505a5ac3283291b75413d09478ab4b5cfbafbeea366de2d0c0bcf61deddaa521f6020460fd547ab37659ae207968b545727beba0a3c5572b9c", aggSig2.ToHex());
        Assert.True(BasicSchemeMPL.AggregateVerify([sk1.GetG1Element(), sk1.GetG1Element(), sk2.GetG1Element()], [msg3, msg4, msg5], aggSig2));
    }

    [Fact]
    public void ChiaVectors3ProofIsCorrect()
    {
        var seed1 = Enumerable.Repeat((byte)4, 32).ToArray();
        var sk1 = PopSchemeMPL.KeyGen(seed1);
        var proof = PopSchemeMPL.PopProve(sk1);
        Assert.Equal("84f709159435f0dc73b3e8bf6c78d85282d19231555a8ee3b6e2573aaf66872d9203fefa1ef700e34e7c3f3fb28210100558c6871c53f1ef6055b9f06b0d1abe22ad584ad3b957f3018a8f58227c6c716b1e15791459850f2289168fa0cf9115", proof.ToHex());
    }
}