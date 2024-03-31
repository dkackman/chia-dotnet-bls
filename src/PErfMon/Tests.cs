using chia.dotnet.bls;
using dotnetstandard_bip39;
using System.Security.Cryptography.X509Certificates;


namespace PerfMon;

internal class Tests
{
    public static void CalculateSyntheticPublicKey(int count)
    {
        const string MNEMONIC = "flip advice pumpkin price wreck simple lucky bicycle fun lesson warm couple hover legend pass bachelor curve primary hurt wrist pigeon menu order injury";
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var byteArray = seed.HexStringToByteArray();
        var privateKey = PrivateKey.FromSeed(byteArray);
        var publicKey = privateKey.GetG1();
        byte[] hiddenPuzzleHash = "711d6c4e32c92e53179b199484cf8c897542bc57f2b22582799f9d657eec4699".ToHexBytes();

        for (var i = 0; i < count; i++)
        {
            publicKey.CalculateSyntheticPublicKey(hiddenPuzzleHash);

        }
    }


    public static void Derive(int count)
    {
        const string MNEMONIC = "flip advice pumpkin price wreck simple lucky bicycle fun lesson warm couple hover legend pass bachelor curve primary hurt wrist pigeon menu order injury";
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var byteArray = seed.HexStringToByteArray();
        var privateKey = PrivateKey.FromSeed(byteArray);

        var intermediateKey = AugSchemeMPL.DeriveChildSkUnhardened(privateKey, 8444);
        intermediateKey = AugSchemeMPL.DeriveChildSkUnhardened(privateKey, 12381);
        intermediateKey = AugSchemeMPL.DeriveChildSkUnhardened(privateKey, 2);

        for (uint i = 0; i < count; i++)
        {
            var sk = AugSchemeMPL.DeriveChildSkUnhardened(intermediateKey, i);
        }
    }

    public static void GenerateKeyPairs(int count)
    {
        const string MNEMONIC = "flip advice pumpkin price wreck simple lucky bicycle fun lesson warm couple hover legend pass bachelor curve primary hurt wrist pigeon menu order injury";
        var bip39 = new BIP39();
        var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
        var byteArray = seed.HexStringToByteArray();
        var privateKey = PrivateKey.FromSeed(byteArray);

        for (var i = 0; i < count; i++)
        {
            GenerateKeyPair(privateKey, 1);
        }
    }

    public static void GenerateAndSign(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var sk = AugSchemeMPL.KeyGen(seed);
        }
        //var signature = AugSchemeMPL.Sign(sk, message);
    }

    private static void GenerateKeyPair(PrivateKey rootPrivateKey, int index)
    {

        PrivateKey privateKey = KeyDerivation.DerivePrivateKey(rootPrivateKey, index, false);
        JacobianPoint publicKey = privateKey.GetG1();
    }

    private static readonly byte[] message = [1, 2, 3, 4, 5];
    private static readonly byte[] seed =
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


}
