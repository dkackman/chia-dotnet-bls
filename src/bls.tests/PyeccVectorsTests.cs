using chia.dotnet.bls;

namespace bls.tests;

public class PyeccVectorsTests
{
    private readonly byte[] ref_sig1Basic =
    [
        0x96,
        0xba,
        0x34,
        0xfa,
        0xc3,
        0x3c,
        0x7f,
        0x12,
        0x9d,
        0x60,
        0x2a,
        0x0b,
        0xc8,
        0xa3,
        0xd4,
        0x3f,
        0x9a,
        0xbc,
        0x01,
        0x4e,
        0xce,
        0xaa,
        0xb7,
        0x35,
        0x91,
        0x46,
        0xb4,
        0xb1,
        0x50,
        0xe5,
        0x7b,
        0x80,
        0x86,
        0x45,
        0x73,
        0x8f,
        0x35,
        0x67,
        0x1e,
        0x9e,
        0x10,
        0xe0,
        0xd8,
        0x62,
        0xa3,
        0x0c,
        0xab,
        0x70,
        0x07,
        0x4e,
        0xb5,
        0x83,
        0x1d,
        0x13,
        0xe6,
        0xa5,
        0xb1,
        0x62,
        0xd0,
        0x1e,
        0xeb,
        0xe6,
        0x87,
        0xd0,
        0x16,
        0x4a,
        0xdb,
        0xd0,
        0xa8,
        0x64,
        0x37,
        0x0a,
        0x7c,
        0x22,
        0x2a,
        0x27,
        0x68,
        0xd7,
        0x70,
        0x4d,
        0xa2,
        0x54,
        0xf1,
        0xbf,
        0x18,
        0x23,
        0x66,
        0x5b,
        0xc2,
        0x36,
        0x1f,
        0x9d,
        0xd8,
        0xc0,
        0x0e,
        0x99
    ];
    private readonly byte[] ref_sig2Basic =
    [
        0xa4,
        0x02,
        0x79,
        0x09,
        0x32,
        0x13,
        0x0f,
        0x76,
        0x6a,
        0xf1,
        0x1b,
        0xa7,
        0x16,
        0x53,
        0x66,
        0x83,
        0xd8,
        0xc4,
        0xcf,
        0xa5,
        0x19,
        0x47,
        0xe4,
        0xf9,
        0x08,
        0x1f,
        0xed,
        0xd6,
        0x92,
        0xd6,
        0xdc,
        0x0c,
        0xac,
        0x5b,
        0x90,
        0x4b,
        0xee,
        0x5e,
        0xa6,
        0xe2,
        0x55,
        0x69,
        0xe3,
        0x6d,
        0x7b,
        0xe4,
        0xca,
        0x59,
        0x06,
        0x9a,
        0x96,
        0xe3,
        0x4b,
        0x7f,
        0x70,
        0x07,
        0x58,
        0xb7,
        0x16,
        0xf9,
        0x49,
        0x4a,
        0xaa,
        0x59,
        0xa9,
        0x6e,
        0x74,
        0xd1,
        0x4a,
        0x3b,
        0x55,
        0x2a,
        0x9a,
        0x6b,
        0xc1,
        0x29,
        0xe7,
        0x17,
        0x19,
        0x5b,
        0x9d,
        0x60,
        0x06,
        0xfd,
        0x6d,
        0x5c,
        0xef,
        0x47,
        0x68,
        0xc0,
        0x22,
        0xe0,
        0xf7,
        0x31,
        0x6a,
        0xbf
    ];
    private readonly byte[] ref_sigABasic =
    [
        0x98,
        0x7c,
        0xfd,
        0x3b,
        0xcd,
        0x62,
        0x28,
        0x02,
        0x87,
        0x02,
        0x74,
        0x83,
        0xf2,
        0x9c,
        0x55,
        0x24,
        0x5e,
        0xd8,
        0x31,
        0xf5,
        0x1d,
        0xd6,
        0xbd,
        0x99,
        0x9a,
        0x6f,
        0xf1,
        0xa1,
        0xf1,
        0xf1,
        0xf0,
        0xb6,
        0x47,
        0x77,
        0x8b,
        0x01,
        0x67,
        0x35,
        0x9c,
        0x71,
        0x50,
        0x55,
        0x58,
        0xa7,
        0x6e,
        0x15,
        0x8e,
        0x66,
        0x18,
        0x1e,
        0xe5,
        0x12,
        0x59,
        0x05,
        0xa6,
        0x42,
        0x24,
        0x6b,
        0x01,
        0xe7,
        0xfa,
        0x5e,
        0xe5,
        0x3d,
        0x68,
        0xa4,
        0xfe,
        0x9b,
        0xfb,
        0x29,
        0xa8,
        0xe2,
        0x66,
        0x01,
        0xf0,
        0xb9,
        0xad,
        0x57,
        0x7d,
        0xdd,
        0x18,
        0x87,
        0x6a,
        0x73,
        0x31,
        0x7c,
        0x21,
        0x6e,
        0xa6,
        0x1f,
        0x43,
        0x04,
        0x14,
        0xec,
        0x51,
        0xc5
    ];
    private readonly byte[] ref_sig1Aug =
    [
        0x81,
        0x80,
        0xf0,
        0x2c,
        0xcb,
        0x72,
        0xe9,
        0x22,
        0xb1,
        0x52,
        0xfc,
        0xed,
        0xbe,
        0x0e,
        0x1d,
        0x19,
        0x52,
        0x10,
        0x35,
        0x4f,
        0x70,
        0x70,
        0x36,
        0x58,
        0xe8,
        0xe0,
        0x8c,
        0xbe,
        0xbf,
        0x11,
        0xd4,
        0x97,
        0x0e,
        0xab,
        0x6a,
        0xc3,
        0xcc,
        0xf7,
        0x15,
        0xf3,
        0xfb,
        0x87,
        0x6d,
        0xf9,
        0xa9,
        0x79,
        0x7a,
        0xbd,
        0x0c,
        0x1a,
        0xf6,
        0x1a,
        0xae,
        0xad,
        0xc9,
        0x2c,
        0x2c,
        0xfe,
        0x5c,
        0x0a,
        0x56,
        0xc1,
        0x46,
        0xcc,
        0x8c,
        0x3f,
        0x71,
        0x51,
        0xa0,
        0x73,
        0xcf,
        0x5f,
        0x16,
        0xdf,
        0x38,
        0x24,
        0x67,
        0x24,
        0xc4,
        0xae,
        0xd7,
        0x3f,
        0xf3,
        0x0e,
        0xf5,
        0xda,
        0xa6,
        0xaa,
        0xca,
        0xed,
        0x1a,
        0x26,
        0xec,
        0xaa,
        0x33,
        0x6b
    ];
    private readonly byte[] ref_sig2Aug =
    [
        0x99,
        0x11,
        0x1e,
        0xea,
        0xfb,
        0x41,
        0x2d,
        0xa6,
        0x1e,
        0x4c,
        0x37,
        0xd3,
        0xe8,
        0x06,
        0xc6,
        0xfd,
        0x6a,
        0xc9,
        0xf3,
        0x87,
        0x0e,
        0x54,
        0xda,
        0x92,
        0x22,
        0xba,
        0x4e,
        0x49,
        0x48,
        0x22,
        0xc5,
        0xb7,
        0x65,
        0x67,
        0x31,
        0xfa,
        0x7a,
        0x64,
        0x59,
        0x34,
        0xd0,
        0x4b,
        0x55,
        0x9e,
        0x92,
        0x61,
        0xb8,
        0x62,
        0x01,
        0xbb,
        0xee,
        0x57,
        0x05,
        0x52,
        0x50,
        0xa4,
        0x59,
        0xa2,
        0xda,
        0x10,
        0xe5,
        0x1f,
        0x9c,
        0x1a,
        0x69,
        0x41,
        0x29,
        0x7f,
        0xfc,
        0x5d,
        0x97,
        0x0a,
        0x55,
        0x72,
        0x36,
        0xd0,
        0xbd,
        0xeb,
        0x7c,
        0xf8,
        0xff,
        0x18,
        0x80,
        0x0b,
        0x08,
        0x63,
        0x38,
        0x71,
        0xa0,
        0xf0,
        0xa7,
        0xea,
        0x42,
        0xf4,
        0x74,
        0x80
    ];
    private readonly byte[] ref_sigAAug =
    [
        0x8c,
        0x5d,
        0x03,
        0xf9,
        0xda,
        0xe7,
        0x7e,
        0x19,
        0xa5,
        0x94,
        0x5a,
        0x06,
        0xa2,
        0x14,
        0x83,
        0x6e,
        0xdb,
        0x8e,
        0x03,
        0xb8,
        0x51,
        0x52,
        0x5d,
        0x84,
        0xb9,
        0xde,
        0x64,
        0x40,
        0xe6,
        0x8f,
        0xc0,
        0xca,
        0x73,
        0x03,
        0xee,
        0xed,
        0x39,
        0x0d,
        0x86,
        0x3c,
        0x9b,
        0x55,
        0xa8,
        0xcf,
        0x6d,
        0x59,
        0x14,
        0x0a,
        0x01,
        0xb5,
        0x88,
        0x47,
        0x88,
        0x1e,
        0xb5,
        0xaf,
        0x67,
        0x73,
        0x4d,
        0x44,
        0xb2,
        0x55,
        0x56,
        0x46,
        0xc6,
        0x61,
        0x6c,
        0x39,
        0xab,
        0x88,
        0xd2,
        0x53,
        0x29,
        0x9a,
        0xcc,
        0x1e,
        0xb1,
        0xb1,
        0x9d,
        0xdb,
        0x9b,
        0xfc,
        0xbe,
        0x76,
        0xe2,
        0x8a,
        0xdd,
        0xf6,
        0x71,
        0xd1,
        0x16,
        0xc0,
        0x52,
        0xbb,
        0x18,
        0x47
    ];
    private readonly byte[] ref_sig1Pop =
    [
        0x95,
        0x50,
        0xfb,
        0x4e,
        0x7f,
        0x7e,
        0x8c,
        0xc4,
        0xa9,
        0x0b,
        0xe8,
        0x56,
        0x0a,
        0xb5,
        0xa7,
        0x98,
        0xb0,
        0xb2,
        0x30,
        0x00,
        0xb6,
        0xa5,
        0x4a,
        0x21,
        0x17,
        0x52,
        0x02,
        0x10,
        0xf9,
        0x86,
        0xf3,
        0xf2,
        0x81,
        0xb3,
        0x76,
        0xf2,
        0x59,
        0xc0,
        0xb7,
        0x80,
        0x62,
        0xd1,
        0xeb,
        0x31,
        0x92,
        0xb3,
        0xd9,
        0xbb,
        0x04,
        0x9f,
        0x59,
        0xec,
        0xc1,
        0xb0,
        0x3a,
        0x70,
        0x49,
        0xeb,
        0x66,
        0x5e,
        0x0d,
        0xf3,
        0x64,
        0x94,
        0xae,
        0x4c,
        0xb5,
        0xf1,
        0x13,
        0x6c,
        0xca,
        0xee,
        0xfc,
        0x99,
        0x58,
        0xcb,
        0x30,
        0xc3,
        0x33,
        0x3d,
        0x3d,
        0x43,
        0xf0,
        0x71,
        0x48,
        0xc3,
        0x86,
        0x29,
        0x9a,
        0x7b,
        0x1b,
        0xfc,
        0x0d,
        0xc5,
        0xcf,
        0x7c
    ];
    private readonly byte[] ref_sig2Pop =
    [
        0xa6,
        0x90,
        0x36,
        0xbc,
        0x11,
        0xae,
        0x5e,
        0xfc,
        0xbf,
        0x61,
        0x80,
        0xaf,
        0xe3,
        0x9a,
        0xdd,
        0xde,
        0x7e,
        0x27,
        0x73,
        0x1e,
        0xc4,
        0x02,
        0x57,
        0xbf,
        0xdc,
        0x3c,
        0x37,
        0xf1,
        0x7b,
        0x8d,
        0xf6,
        0x83,
        0x06,
        0xa3,
        0x4e,
        0xbd,
        0x10,
        0xe9,
        0xe3,
        0x2a,
        0x35,
        0x25,
        0x37,
        0x50,
        0xdf,
        0x5c,
        0x87,
        0xc2,
        0x14,
        0x2f,
        0x82,
        0x07,
        0xe8,
        0xd5,
        0x65,
        0x47,
        0x12,
        0xb4,
        0xe5,
        0x54,
        0xf5,
        0x85,
        0xfb,
        0x68,
        0x46,
        0xff,
        0x38,
        0x04,
        0xe4,
        0x29,
        0xa9,
        0xf8,
        0xa1,
        0xb4,
        0xc5,
        0x6b,
        0x75,
        0xd0,
        0x86,
        0x9e,
        0xd6,
        0x75,
        0x80,
        0xd7,
        0x89,
        0x87,
        0x0b,
        0xab,
        0xe2,
        0xc7,
        0xc8,
        0xa9,
        0xd5,
        0x1e,
        0x7b,
        0x2a
    ];
    private readonly byte[] ref_sigAPop =
    [
        0xa4,
        0xea,
        0x74,
        0x2b,
        0xcd,
        0xc1,
        0x55,
        0x3e,
        0x9c,
        0xa4,
        0xe5,
        0x60,
        0xbe,
        0x7e,
        0x5e,
        0x6c,
        0x6e,
        0xfa,
        0x6a,
        0x64,
        0xdd,
        0xdf,
        0x9c,
        0xa3,
        0xbb,
        0x28,
        0x54,
        0x23,
        0x3d,
        0x85,
        0xa6,
        0xaa,
        0xc1,
        0xb7,
        0x6e,
        0xc7,
        0xd1,
        0x03,
        0xdb,
        0x4e,
        0x33,
        0x14,
        0x8b,
        0x82,
        0xaf,
        0x99,
        0x23,
        0xdb,
        0x05,
        0x93,
        0x4a,
        0x6e,
        0xce,
        0x9a,
        0x71,
        0x01,
        0xcd,
        0x8a,
        0x9d,
        0x47,
        0xce,
        0x27,
        0x97,
        0x80,
        0x56,
        0xb0,
        0xf5,
        0x90,
        0x00,
        0x21,
        0x81,
        0x8c,
        0x45,
        0x69,
        0x8a,
        0xfd,
        0xd6,
        0xcf,
        0x8a,
        0x6b,
        0x6f,
        0x7f,
        0xee,
        0x1f,
        0x0b,
        0x43,
        0x71,
        0x6f,
        0x55,
        0xe4,
        0x13,
        0xd4,
        0xb8,
        0x7a,
        0x60,
        0x39
    ];

    private readonly byte[] secret1 = Enumerable.Repeat((byte)1, 32).ToArray();
    private readonly byte[] secret2 = Enumerable.Range(0, 32).Select(i => (byte)(i * 314159 % 256)).ToArray();
    private readonly PrivateKey sk1;
    private readonly PrivateKey sk2;
    private readonly byte[] msg = [3, 1, 4, 1, 5, 9];
    private readonly JacobianPoint sig1Basic;
    private readonly JacobianPoint sig2Basic;
    private readonly JacobianPoint sigABasic;
    private readonly JacobianPoint sig1Aug;
    private readonly JacobianPoint sig2Aug;
    private readonly JacobianPoint sigAAug;
    private readonly JacobianPoint sig1Pop;
    private readonly JacobianPoint sig2Pop;
    private readonly JacobianPoint sigAPop;


    public PyeccVectorsTests()
    {
        sk1 = PrivateKey.FromBytes(secret1);
        sk2 = PrivateKey.FromBytes(secret2);
        sig1Basic = BasicSchemeMPL.Sign(sk1, msg);
        sig2Basic = BasicSchemeMPL.Sign(sk2, msg);
        sigABasic = BasicSchemeMPL.Aggregate([sig1Basic, sig2Basic]);
        sig1Aug = AugSchemeMPL.Sign(sk1, msg);
        sig2Aug = AugSchemeMPL.Sign(sk2, msg);
        sigAAug = AugSchemeMPL.Aggregate([sig1Aug, sig2Aug]);
        sig1Pop = PopSchemeMPL.Sign(sk1, msg);
        sig2Pop = PopSchemeMPL.Sign(sk2, msg);
        sigAPop = PopSchemeMPL.Aggregate([sig1Pop, sig2Pop]);
    }

    [Fact]
    public void FirstBasicSignature()
    {
        Assert.Equal(ref_sig1Basic, sig1Basic.ToBytes());
    }

    [Fact]
    public void SecondBasicSignature()
    {
        Assert.Equal(ref_sig2Basic, sig2Basic.ToBytes());
    }

    [Fact]
    public void AggregateBasicSignature()
    {
        Assert.Equal(ref_sigABasic, sigABasic.ToBytes());
    }

    [Fact]
    public void FirstAugSignature()
    {
        Assert.Equal(ref_sig1Aug, sig1Aug.ToBytes());
    }

    [Fact]
    public void SecondAugSignature()
    {
        Assert.Equal(ref_sig2Aug, sig2Aug.ToBytes());
    }

    [Fact]
    public void AggregateAugSignature()
    {
        Assert.Equal(ref_sigAAug, sigAAug.ToBytes());
    }

    [Fact]
    public void FirstPopSignature()
    {
        Assert.Equal(ref_sig1Pop, sig1Pop.ToBytes());
    }

    [Fact]
    public void SecondPopSignature()
    {
        Assert.Equal(ref_sig2Pop, sig2Pop.ToBytes());
    }

    [Fact]
    public void AggregatePopSignature()
    {
        Assert.Equal(ref_sigAPop, sigAPop.ToBytes());
    }
}
