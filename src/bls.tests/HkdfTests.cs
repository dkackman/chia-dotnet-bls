namespace bls.tests
{
    public class HkdfTests
    {
        public static TheoryData<string, string, string, string, string, int> TestData =>
            new()
            {
                {
                    "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
                    "000102030405060708090a0b0c",
                    "f0f1f2f3f4f5f6f7f8f9",
                    "077709362c2e32df0ddc3f0dc47bba6390b6c73bb50f9c3122ec844ad7c2b3e5",
                    "3cb25f25faacd57a90434f64d0362f2a2d2d0a90cf1a5a4c5db02d56ecc4c5bf34007208d5b887185865",
                    42
                },
                {
                    "000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f",
                    "606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeaf",
                    "b0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff",
                    "06a6b88c5853361a06104c9ceb35b45cef760014904671014a193f40c15fc244",
                    "b11e398dc80327a1c8e7f78c596a49344f012eda2d4efad8a050cc4c19afa97c59045a99cac7827271cb41c65e590e09da3275600c2f09b8367793a9aca3db71cc30c58179ec3e87c14c01d5c1f3434f1d87",
                    82
                },
                {
                    "0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b",
                    "",
                    "",
                    "19ef24a32c717b167f33a91d6f648bdf96596776afdb6377ac434c1c293ccb04",
                    "8da4e775a563c18f715f802a063c5a31b8a11f5c5ee1879ec3454e5f3c738d2d9d201395faa4b61a96c8",
                    42
                },
                {
                    "8704f9ac024139fe62511375cf9bc534c0507dcf00c41603ac935cd5943ce0b4b88599390de14e743ca2f56a73a04eae13aa3f3b969b39d8701e0d69a6f8d42f",
                    "53d8e19b",
                    "",
                    "eb01c9cd916653df76ffa61b6ab8a74e254ebfd9bfc43e624cc12a72b0373dee",
                    "8faabea85fc0c64e7ca86217cdc6dcdc88551c3244d56719e630a3521063082c46455c2fd5483811f9520a748f0099c1dfcfa52c54e1c22b5cdf70efb0f3c676",
                    64
                },
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestHkdf(string ikm, string salt, string info, string prkExpected, string okmExpected, int length)
        {
            byte[] saltBytes = Convert.FromHexString(salt);
            byte[] ikmBytes = Convert.FromHexString(ikm);
            byte[] infoBytes = Convert.FromHexString(info);
            byte[] prkExpectedBytes = Convert.FromHexString(prkExpected);
            byte[] okmExpectedBytes = Convert.FromHexString(okmExpected);

            byte[] prk = Hkdf.Extract(saltBytes, ikmBytes);
            byte[] okm = Hkdf.Expand(length, prk, infoBytes);

            Assert.Equal(32, prkExpectedBytes.Length);
            Assert.Equal(length, okmExpectedBytes.Length);
            Assert.Equal(prk, prkExpectedBytes);
            Assert.Equal(okm, okmExpectedBytes);
        }
    }
}
