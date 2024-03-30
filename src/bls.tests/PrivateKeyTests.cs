using chia.dotnet.bls;
using dotnetstandard_bip39;
using supranational;

namespace bls.tests;

public class PrivateKeyTests
{
    [Fact]
    public void CheckPrivateKeyAgainstChiaBlsTs()
    {
        const string MNEMONIC = "abandon abandon abandon";
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var byteArray = seed.HexStringToByteArray();
        var privateKey = PrivateKey.FromSeed(byteArray);
        var privateKeyHex = privateKey.ToHex();

        Assert.Equal("01c0f8ba369c670f348b31d6fd86102953e6099d9404bafe510133b889b16d63", privateKeyHex);
    }

    [Fact]
    public void CompareToBlstSecretKey()
    {
        const string MNEMONIC = "abandon abandon abandon";
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var byteArray = seed.HexStringToByteArray();

        var privateKey = new blst.SecretKey();
        privateKey.keygen_v3(byteArray);
        var be = privateKey.to_bendian().ToHex();

        Assert.Equal("01c0f8ba369c670f348b31d6fd86102953e6099d9404bafe510133b889b16d63", be);
    }
}
