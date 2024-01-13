# chia-dotnet-bls

.NET BLS Library

[![.NET](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql)
[![Nuget](https://img.shields.io/nuget/dt/chia-dotnet-bls)](https://www.nuget.org/packages/chia-dotnet-bls/)

## Introduction

Many thanks to [Rigidity](https://github.com/Rigidity) who's code this is a direct port of and and helped immensely along the way.

This is a direct port of the [chia bls typescript library](https://github.com/Chia-Network/node-chia-bls). Coding style and naming have been converted to C# conventions but otherwise it is very similar in API.

## See Also

- [Documentation](https://dkackman.github.io/chia-dotnet-bls/)
- [chia-blockchain](https://chia.net)
- [chia-dotnet](https://www.nuget.org/packages/chia-dotnet/)
- [dotnetstandard-bip39](https://www.nuget.org/packages/dotnetstandard-bip39/)

## Example Usage

```csharp
using chia.dotnet.bls;
using dotnetstandard_bip39; // https://www.nuget.org/packages/dotnetstandard-bip39/

const string MNEMONIC = "abandon abandon abandon";
const string MESSAGE = "hello world";

// create a secret key from a mnemonic
var bip39 = new BIP39();
var seed = bip39.MnemonicToSeedHex(MNEMONIC, "");
var sk = PrivateKey.FromSeed(seed);

// sign the message
var signature = sk.Sign(MESSAGE);

// verify the signature
var pk = sk.GetG1();
var result = pk.Verify(MESSAGE, signature);

Console.WriteLine($"Signature is valid: {result}");
```

___

_chia and its logo are the registered trademark or trademark of Chia Network, Inc. in the United States and worldwide._
