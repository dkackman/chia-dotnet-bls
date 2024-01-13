using chia.dotnet.bls;
using dotnetstandard_bip39;

namespace bls.tests;

public class Examples
{
    [Fact]
    public void SignAndVerifyAMessage()
    {
        const string MNEMONIC = "abandon abandon abandon";
        const string MESSAGE = "hello world";

        // create a secret key from a mnemonic
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var byteArray = seed.HexStringToByteArray();
        var sk = PrivateKey.FromSeed(byteArray);

        // sign a message
        var signature = sk.Sign(MESSAGE);

        // verify the signature
        var pk = sk.GetG1();
        var result = pk.Verify(MESSAGE, signature);
        Assert.True(result);
    }
}