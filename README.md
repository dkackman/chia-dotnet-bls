# chia-dotnet-bls

.NET BLS Library

[![.NET](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql)

## Introduction

Very much a work in progress but the goal is to provide a .NET library for chia BLS signatures. This is a direct port of the [chia bls typescript library](https://github.com/Chia-Network/node-chia-bls).

Check out the unit tests to see where progress is at.

Code and passing unit tests for:
- [x] Field types (`Fq, Fq2, fq6, Fq12` and associated types and constants)
- [x] AffinePoint
- [x] JacobianPoint
- [x] PrivateKey
- [x] AugSchemeMPL
- [x] BasicSchemeMPL
- [x] PopSchemeMPL
- [ ] Readme Unit tests

___

_chia and its logo are the registered trademark or trademark of Chia Network, Inc. in the United States and worldwide._
