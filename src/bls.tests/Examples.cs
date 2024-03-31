using chia.dotnet.bls;
using dotnetstandard_bip39;

namespace bls.tests;

public class Examples
{
    const string MNEMONIC = "abandon abandon abandon";
    const string MESSAGE = "hello world";
    [Fact]
    public void SignAndVerifyAMessage()
    {


        // create a secret key from a mnemonic
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var sk = PrivateKey.FromSeed(seed);

        // sign a message
        var signature = sk.Sign(MESSAGE);

        // verify the signature
        var pk = sk.GetG1();
        var result = pk.Verify(MESSAGE, signature);
        Assert.True(result);
    }

    [Fact]
    public void CoreMpl()
    {
        // create a secret key from a mnemonic
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var sk = PrivateKey.FromSeed(seed);

        var signature = CoreMPL.Sign(sk, MESSAGE.ToBytes());
        var verified = CoreMPL.Verify(sk.GetG1Element(), MESSAGE.ToBytes(), signature);

        Assert.True(verified);
    }
}