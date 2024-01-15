using System.Globalization;
using System.Numerics;

namespace chia.dotnet.bls;

internal static class FrobCoefficients
{
    private static readonly IReadOnlyDictionary<string, Fq> FrobCoeffs = new Dictionary<string, Fq>
    {
        ["2,1,1"] = Constants.FqNegativeOne,
        ["6,1,1"] = new Fq2(
                Constants.Q,
                Constants.FqZero,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01a0111ea397fe699ec02408663d4de85aa0d857d89759ad4897d29650fb85f9b409427eb4f49fffd8bfd00000000aaac", NumberStyles.AllowHexSpecifier)
                )
            ),
        ["6,1,2"] = new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01a0111ea397fe699ec02408663d4de85aa0d857d89759ad4897d29650fb85f9b409427eb4f49fffd8bfd00000000aaad", NumberStyles.AllowHexSpecifier)
                ),
                Constants.FqZero
            ),
        ["6,2,1"] = new Fq2(
            Constants.Q,
            new Fq(
                Constants.Q,
                BigInteger.Parse("05f19672fdf76ce51ba69c6076a0f77eaddb3a93be6f89688de17d813620a00022e01fffffffefffe", NumberStyles.AllowHexSpecifier)
            ),
            Constants.FqZero
        ),
        ["6,2,2"] = new Fq2(
            Constants.Q,
            new Fq(
                Constants.Q,
                BigInteger.Parse("01a0111ea397fe699ec02408663d4de85aa0d857d89759ad4897d29650fb85f9b409427eb4f49fffd8bfd00000000aaac", NumberStyles.AllowHexSpecifier)
            ),
            Constants.FqZero
        ),
        ["6,3,1"] = new Fq2(Constants.Q, Constants.FqZero, Constants.FqOne),
        ["6,3,2"] = new Fq2(
            Constants.Q,
            new Fq(
                Constants.Q,
                BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaaaa", NumberStyles.AllowHexSpecifier)
            ),
            Constants.FqZero
        ),
        ["6,4,1"] = new Fq2(
            Constants.Q,
            new Fq(
                Constants.Q,
                BigInteger.Parse("01a0111ea397fe699ec02408663d4de85aa0d857d89759ad4897d29650fb85f9b409427eb4f49fffd8bfd00000000aaac", NumberStyles.AllowHexSpecifier)
            ),
            Constants.FqZero
        ),
        ["6,4,2"] = new Fq2(
            Constants.Q,
            new Fq(
                Constants.Q,
                BigInteger.Parse("05f19672fdf76ce51ba69c6076a0f77eaddb3a93be6f89688de17d813620a00022e01fffffffefffe", NumberStyles.AllowHexSpecifier)
            ),
            Constants.FqZero
        ),
        ["6,5,1"] = new Fq2(
            Constants.Q,
            Constants.FqZero,
            new Fq(
                Constants.Q,
                BigInteger.Parse("05f19672fdf76ce51ba69c6076a0f77eaddb3a93be6f89688de17d813620a00022e01fffffffefffe", NumberStyles.AllowHexSpecifier)
            )
        ),
        ["6,5,2"] = new Fq2(
            Constants.Q,
            new Fq(
                Constants.Q,
                BigInteger.Parse("05f19672fdf76ce51ba69c6076a0f77eaddb3a93be6f89688de17d813620a00022e01fffffffeffff", NumberStyles.AllowHexSpecifier)
            ),
            Constants.FqZero
        ),
        ["12,1,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01904d3bf02bb0667c231beb4202c0d1f0fd603fd3cbd5f4f7b2443d784bab9c4f67ea53d63e7813d8d0775ed92235fb8", NumberStyles.AllowHexSpecifier)
                ),
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("0fc3e2b36c4e03288e9e902231f9fb854a14787b6c7b36fec0c8ec971f63c5f282d5ac14d6c7ec22cf78a126ddc4af3", NumberStyles.AllowHexSpecifier)
                )
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,2,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("05f19672fdf76ce51ba69c6076a0f77eaddb3a93be6f89688de17d813620a00022e01fffffffeffff", NumberStyles.AllowHexSpecifier)
                ),
                Constants.FqZero
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,3,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("0135203e60180a68ee2e9c448d77a2cd91c3dedd930b1cf60ef396489f61eb45e304466cf3e67fa0af1ee7b04121bdea2", NumberStyles.AllowHexSpecifier)
                ),
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("06af0e0437ff400b6831e36d6bd17ffe48395dabc2d3435e77f76e17009241c5ee67992f72ec05f4c81084fbede3cc09", NumberStyles.AllowHexSpecifier)
                )
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,4,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("05f19672fdf76ce51ba69c6076a0f77eaddb3a93be6f89688de17d813620a00022e01fffffffefffe", NumberStyles.AllowHexSpecifier)
                ),
                Constants.FqZero
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,5,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("0144e4211384586c16bd3ad4afa99cc9170df3560e77982d0db45f3536814f0bd5871c1908bd478cd1ee605167ff82995", NumberStyles.AllowHexSpecifier)
                ),
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("05b2cfd9013a5fd8df47fa6b48b1e045f39816240c0b8fee8beadf4d8e9c0566c63a3e6e257f87329b18fae980078116", NumberStyles.AllowHexSpecifier)
                )
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,6,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01a0111ea397fe69a4b1ba7b6434bacd764774b84f38512bf6730d2a0f6b0f6241eabfffeb153ffffb9feffffffffaaaa", NumberStyles.AllowHexSpecifier)
                ),
                Constants.FqZero
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,7,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("0fc3e2b36c4e03288e9e902231f9fb854a14787b6c7b36fec0c8ec971f63c5f282d5ac14d6c7ec22cf78a126ddc4af3", NumberStyles.AllowHexSpecifier)
                ),
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01904d3bf02bb0667c231beb4202c0d1f0fd603fd3cbd5f4f7b2443d784bab9c4f67ea53d63e7813d8d0775ed92235fb8", NumberStyles.AllowHexSpecifier)
                )
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,8,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01a0111ea397fe699ec02408663d4de85aa0d857d89759ad4897d29650fb85f9b409427eb4f49fffd8bfd00000000aaac", NumberStyles.AllowHexSpecifier)
                ),
                Constants.FqZero
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,9,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("06af0e0437ff400b6831e36d6bd17ffe48395dabc2d3435e77f76e17009241c5ee67992f72ec05f4c81084fbede3cc09", NumberStyles.AllowHexSpecifier)
                ),
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("0135203e60180a68ee2e9c448d77a2cd91c3dedd930b1cf60ef396489f61eb45e304466cf3e67fa0af1ee7b04121bdea2", NumberStyles.AllowHexSpecifier)
                )
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,10,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("01a0111ea397fe699ec02408663d4de85aa0d857d89759ad4897d29650fb85f9b409427eb4f49fffd8bfd00000000aaad", NumberStyles.AllowHexSpecifier)
                ),
                Constants.FqZero
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        ),
        ["12,11,1"] = new Fq6(
            Constants.Q,
            new Fq2(
                Constants.Q,
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("05b2cfd9013a5fd8df47fa6b48b1e045f39816240c0b8fee8beadf4d8e9c0566c63a3e6e257f87329b18fae980078116", NumberStyles.AllowHexSpecifier)
                ),
                new Fq(
                    Constants.Q,
                    BigInteger.Parse("0144e4211384586c16bd3ad4afa99cc9170df3560e77982d0db45f3536814f0bd5871c1908bd478cd1ee605167ff82995", NumberStyles.AllowHexSpecifier)
                )
            ),
            Constants.Fq2Zero,
            Constants.Fq2Zero
        )
    };

    public static Fq? GetCoefficient(int extension, int i, int index)
    {
        if (FrobCoeffs.TryGetValue($"{extension},{i},{index}", out Fq? value))
        {
            return value;
        }
        return null;
    }
}
