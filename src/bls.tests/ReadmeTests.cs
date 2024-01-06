using chia.dotnet.bls;

namespace bls.tests;

public class ReadmeTests
{
    private readonly byte[] seed =
    [
        0,
        50,
        6,
        244,
        24,
        199,
        1,
        25,
        52,
        88,
        192,
        19,
        18,
        12,
        89,
        6,
        220,
        18,
        102,
        58,
        209,
        82,
        12,
        62,
        89,
        110,
        182,
        9,
        44,
        20,
        254,
        22,
    ];
    private readonly byte[] message = [1, 2, 3, 4, 5];
    private readonly PrivateKey sk;
    private readonly JacobianPoint pk;
    private readonly JacobianPoint signature;
    private readonly byte[] skBytes;
    private readonly byte[] pkBytes;
    private readonly byte[] signatureBytes;
    private readonly PrivateKey skFromBytes;
    private readonly JacobianPoint pkFromBytes;
    private readonly byte[] seed1;
    private readonly PrivateKey sk1;
    private readonly byte[] seed2;
    private readonly PrivateKey sk2;
    private readonly byte[] message2;
    private readonly JacobianPoint pk1;
    private readonly JacobianPoint sig1;
    private readonly JacobianPoint pk2;
    private readonly JacobianPoint sig2;
    private readonly JacobianPoint aggSig;
    private readonly byte[] seed3;
    private readonly PrivateKey sk3;
    private readonly JacobianPoint pk3;
    private readonly byte[] message3;
    private readonly JacobianPoint sig3;
    private readonly JacobianPoint popSig1;
    private readonly JacobianPoint popSig2;
    private readonly JacobianPoint popSig3;
    private readonly JacobianPoint pop1;
    private readonly JacobianPoint pop2;
    private readonly JacobianPoint pop3;
    private readonly JacobianPoint popSigAgg;

    public ReadmeTests()
    {
        sk = AugSchemeMPL.KeyGen(seed);
        pk = sk.GetG1();
        signature = AugSchemeMPL.Sign(sk, message);

        skBytes = sk.ToBytes();
        pkBytes = pk.ToBytes();
        signatureBytes = signature.ToBytes();

        skFromBytes = PrivateKey.FromBytes(skBytes);
        pkFromBytes = JacobianPoint.FromBytesG1(pkBytes);
        seed1 = [1, .. seed.Skip(1)];
        sk1 = AugSchemeMPL.KeyGen(seed1);
        seed2 = [2, .. seed.Skip(1)];
        sk2 = AugSchemeMPL.KeyGen(seed2);
        message2 = [1, 2, 3, 4, 5, 6, 7];
        pk1 = sk1.GetG1();
        sig1 = AugSchemeMPL.Sign(sk1, message);
        pk2 = sk2.GetG1();
        sig2 = AugSchemeMPL.Sign(sk2, message2);
        aggSig = AugSchemeMPL.Aggregate([sig1, sig2]);
        seed3 = [3, .. seed.Skip(1)];
        sk3 = AugSchemeMPL.KeyGen(seed3);
        pk3 = sk3.GetG1();
        message3 = [100, 2, 254, 88, 90, 45, 23];
        sig3 = AugSchemeMPL.Sign(sk3, message3);
        popSig1 = PopSchemeMPL.Sign(sk1, message);
        popSig2 = PopSchemeMPL.Sign(sk2, message);
        popSig3 = PopSchemeMPL.Sign(sk3, message);
        pop1 = PopSchemeMPL.PopProve(sk1);
        pop2 = PopSchemeMPL.PopProve(sk2);
        pop3 = PopSchemeMPL.PopProve(sk3);
        popSigAgg = PopSchemeMPL.Aggregate([popSig1, popSig2, popSig3]);
    }

    [Fact]
    public void AugSchemeMPLVerify()
    {
        var sk = AugSchemeMPL.KeyGen(seed);
        var pk = sk.GetG1();
        var signature = AugSchemeMPL.Sign(sk, message);
        Assert.True(AugSchemeMPL.Verify(pk, message, signature));
    }

    [Fact]
    public void FromBytes()
    {
        var signatureFromBytes = JacobianPoint.FromBytesG2(signatureBytes);

        Assert.True(sk.Equals(skFromBytes));
        Assert.True(pk.Equals(pkFromBytes));
        Assert.True(signature.Equals(signatureFromBytes));
    }

    [Fact]
    public void FirstAugAggregateVerify()
    {
        Assert.True(AugSchemeMPL.AggregateVerify([pk1, pk2], [message, message2], aggSig));
    }

    [Fact]
    public void SecondAugAggregateVerify()
    {
        var aggSigFinal = AugSchemeMPL.Aggregate([aggSig, sig3]);
        Assert.True(AugSchemeMPL.AggregateVerify([pk1, pk2, pk3], [message, message2, message3], aggSigFinal));
    }

    [Fact]
    public void FirstPopVerifyTest()
    {
        Assert.True(PopSchemeMPL.PopVerify(pk1, pop1));
    }

    [Fact]
    public void SecondPopVerifyTest()
    {
        Assert.True(PopSchemeMPL.PopVerify(pk2, pop2));
    }

    [Fact]
    public void ThirdPopVerifyTest()
    {
        Assert.True(PopSchemeMPL.PopVerify(pk3, pop3));
    }

    [Fact]
    public void PopSchemeMPLFastAggregateVerifyTest()
    {
        JacobianPoint[] pkList = [pk1, pk2, pk3];
        Assert.True(PopSchemeMPL.FastAggregateVerify(pkList, message, popSigAgg));
    }

    [Fact]
    public void PopSchemeMPLVerifyTest()
    {
        var popAggPk = pk1.Add(pk2).Add(pk3);
        Assert.True(PopSchemeMPL.Verify(popAggPk, message, popSigAgg));
    }

    [Fact]
    public void PopSchemeMPLAggregateSignTest()
    {
        var popAggSk = PrivateKey.Aggregate([sk1, sk2, sk3]);
        Assert.True(PopSchemeMPL.Sign(popAggSk, message).Equals(popSigAgg));
    }

    [Fact]
    public void AugSchemeMPLChildKeysTest()
    {
        var masterSk = AugSchemeMPL.KeyGen(seed);
        var child = AugSchemeMPL.DeriveChildSk(masterSk, 152);
        AugSchemeMPL.DeriveChildSk(child, 952);
        var masterPk = masterSk.GetG1();
        var childU = AugSchemeMPL.DeriveChildSkUnhardened(masterSk, 22);
        var grandchildU = AugSchemeMPL.DeriveChildSkUnhardened(childU, 0);
        var childUPk = AugSchemeMPL.DeriveChildPkUnhardened(masterPk, 22);
        var grandchildUPk = AugSchemeMPL.DeriveChildPkUnhardened(childUPk, 0);

        Assert.True(grandchildUPk.Equals(grandchildU.GetG1()));
    }
}