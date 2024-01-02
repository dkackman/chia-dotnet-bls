# chia-dotnet-bls

.NET BLS Library

[![.NET](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/dkackman/chia-dotnet-bls/actions/workflows/github-code-scanning/codeql)

## Introduction

Very much a work in progress but the goal is to provide a .NET library for chia BLS signatures. This is a direct port of the [chia bls typescript library](https://github.com/Chia-Network/node-chia-bls).

Check out the unit tests to see where progress is at.

At the moment it has passing unit tests for the core `Field` objects and coefficients. The next step is to write more tests for `PrivateKey` and `JacobianPoint`, and then finish off the BLS computations.
___

_chia and its logo are the registered trademark or trademark of Chia Network, Inc. in the United States and worldwide._
