namespace chia.dotnet.bls;

/// <summary>
/// Provides extension methods for the AugSchemeMPL signature scheme. This is the scheme Chia uses for its signatures.
/// </summary>
public static class AugSchemeMPL
{
    /// <summary>
    /// Generates a private key from the given seed.
    /// </summary>
    /// <param name="seed">The seed used to generate the private key.</param>
    /// <returns>The generated private key.</returns>
    public static PrivateKey KeyGen(this byte[] seed) => CoreMPL.KeyGen(seed);

    /// <summary>
    /// Signs a message using the specified private key.
    /// </summary>
    /// <param name="privateKey">The private key used for signing.</param>
    /// <param name="message">The message to be signed.</param>
    /// <returns>The signature as a JacobianPoint.</returns>
    public static JacobianPoint Sign(this PrivateKey privateKey, byte[] message) =>
        Signing.CoreSignMpl(privateKey, ByteUtils.ConcatenateArrays(privateKey.GetG1Element().ToBytes(), message), Schemes.AugSchemeDst);

    /// <summary>
    /// Signs a message using the specified private key.
    /// </summary>
    /// <param name="privateKey">The private key used for signing.</param>
    /// <param name="message">The message to be signed.</param>
    /// <returns>The signature as a JacobianPoint.</returns>
    public static JacobianPoint Sign(this PrivateKey privateKey, string message) => Sign(privateKey, message.ToBytes());

    /// <summary>
    /// Verifies the signature of a message using the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key used for verification.</param>
    /// <param name="message">The message to be verified.</param>
    /// <param name="signature">The signature to be verified.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    public static bool Verify(this JacobianPoint publicKey, byte[] message, JacobianPoint signature) =>
        Signing.CoreVerifyMpl(publicKey, ByteUtils.ConcatenateArrays(publicKey.ToBytes(), message), signature, Schemes.AugSchemeDst);

    /// <summary>
    /// Verifies the signature of a message using the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key used for verification.</param>
    /// <param name="message">The message to be verified.</param>
    /// <param name="signature">The signature to be verified.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    public static bool Verify(this JacobianPoint publicKey, string message, JacobianPoint signature) => Verify(publicKey, message.ToBytes(), signature);

    /// <summary>
    /// Aggregates multiple signatures into a single signature.
    /// </summary>
    /// <param name="signatures">The array of signatures to be aggregated.</param>
    /// <returns>The aggregated signature as a JacobianPoint.</returns>
    public static JacobianPoint Aggregate(JacobianPoint[] signatures) => Signing.CoreAggregateMpl(signatures);

    /// <summary>
    /// Verifies an aggregated signature against multiple public keys and messages.
    /// </summary>
    /// <param name="publicKeys">The array of public keys used for verification.</param>
    /// <param name="messages">The array of messages to be verified.</param>
    /// <param name="signature">The aggregated signature to be verified.</param>
    /// <returns>True if the aggregated signature is valid, false otherwise.</returns>
    public static bool AggregateVerify(this JacobianPoint[] publicKeys, byte[][] messages, JacobianPoint signature)
    {
        int length = publicKeys.Length;
        if (length != messages.Length || length == 0)
        {
            return false;
        }

        var mPrimes = new byte[length][];
        for (int i = 0; i < length; i++)
        {
            mPrimes[i] = ByteUtils.ConcatenateArrays(publicKeys[i].ToBytes(), messages[i]);
        }

        return Signing.CoreAggregateVerify(publicKeys, mPrimes, signature, Schemes.AugSchemeDst);
    }

    /// <summary>
    /// Derives a child private key from the specified private key and index.
    /// </summary>
    /// <param name="privateKey">The parent private key.</param>
    /// <param name="index">The index of the child private key.</param>
    /// <returns>The derived child private key.</returns>
    public static PrivateKey DeriveChildSk(this PrivateKey privateKey, uint index) => CoreMPL.DeriveChildSk(privateKey, index);

    /// <summary>
    /// Derives a child unhardened private key from the specified private key and index.
    /// </summary>
    /// <param name="privateKey">The parent private key.</param>
    /// <param name="index">The index of the child private key.</param>
    /// <returns>The derived child unhardened private key.</returns>
    public static PrivateKey DeriveChildSkUnhardened(this PrivateKey privateKey, uint index) => CoreMPL.DeriveChildSkUnhardened(privateKey, index);

    /// <summary>
    /// Derives a child unhardened public key from the specified public key and index.
    /// </summary>
    /// <param name="publicKey">The parent public key.</param>
    /// <param name="index">The index of the child public key.</param>
    /// <returns>The derived child unhardened public key.</returns>
    public static JacobianPoint DeriveChildPkUnhardened(this JacobianPoint publicKey, uint index) => HdKeysClass.DeriveChildG1Unhardened(publicKey, index);

    public static G1Element DeriveChildPkUnhardened(G1Element publicKey, uint index) => CoreMPL.DeriveChildPkUnhardened(publicKey, index);

}