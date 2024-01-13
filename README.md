# chia-dotnet-bls

.NET BLS Library

[![.NET](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql)
[![Nuget](https://img.shields.io/nuget/dt/chia-dotnet-bls)](https://www.nuget.org/packages/chia-dotnet-bls/)

## Introduction

This is a direct port of the [chia bls typescript library](https://github.com/Chia-Network/node-chia-bls). Coding style and naming have been converted to C# conventions but otherwise it is very similar in API.

All of the unit tests from the original project have been ported and pass.

## Example Usage

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

// sign the message
var signature = sk.Sign(MESSAGE);

// verify the signature
var pk = sk.GetG1();
var result = pk.Verify(MESSAGE, signature);

Console.WriteLine($"Signature is valid: {result}");
```

___

_chia and its logo are the registered trademark or trademark of Chia Network, Inc. in the United States and worldwide._
