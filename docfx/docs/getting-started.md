# Getting Started

## Sign and Verify a Message

```csharp
using chia.dotnet.bls;

// https://www.nuget.org/packages/dotnetstandard-bip39/
using dotnetstandard_bip39;

const string MNEMONIC = "abandon abandon abandon";
const string MESSAGE = "hello world";

// create a secret key from a mnemonic
var bip39 = new BIP39();
var seed = bip39.MnemonicToSeedHex(MNEMONIC, "").HexStringToByteArray();
var sk = PrivateKey.FromSeed(seed);

// sign a message
var messageBytes = MESSAGE.ToBytes();
var signature = sk.Sign(messageBytes);

// verify the signature
var pk = sk.GetG1();
var result = pk.Verify(messageBytes, signature);

Console.WriteLine($"Signature is valid: {result}");
```
