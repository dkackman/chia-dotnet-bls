namespace chia.dotnet.bls;

/// <summary>
/// Provides extension methods for the AugSchemeMPL signature scheme. This is the scheme Chia uses for its signatures.
/// </summary>
public static class AugSchemeMPL
{
    private const string CIPHERSUITE_ID = "BLS_SIG_BLS12381G2_XMD:SHA-256_SSWU_RO_AUG_";

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
    public static G2Element Sign(this PrivateKey privateKey, byte[] message) =>
        CoreMPL.Sign(privateKey, ByteUtils.ConcatenateArrays(privateKey.GetG1Element().ToBytes(), message), CIPHERSUITE_ID);

    /// <summary>
    /// Signs a message using the specified private key.
    /// </summary>
    /// <param name="privateKey">The private key used for signing.</param>
    /// <param name="message">The message to be signed.</param>
    /// <returns>The signature as a JacobianPoint.</returns>
    public static G2Element Sign(this PrivateKey privateKey, string message) => 
        CoreMPL.Sign(privateKey, ByteUtils.ConcatenateArrays(privateKey.GetG1Element().ToBytes(), message.ToBytes()), CIPHERSUITE_ID);

    /// <summary>
    /// Verifies the signature of a message using the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key used for verification.</param>
    /// <param name="message">The message to be verified.</param>
    /// <param name="signature">The signature to be verified.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    public static bool Verify(this G1Element publicKey, byte[] message, G2Element signature) =>
        CoreMPL.Verify(publicKey, ByteUtils.ConcatenateArrays(publicKey.ToBytes(), message), signature, CIPHERSUITE_ID);

    /// <summary>
    /// Verifies the signature of a message using the specified public key.
    /// </summary>
    /// <param name="publicKey">The public key used for verification.</param>
    /// <param name="message">The message to be verified.</param>
    /// <param name="signature">The signature to be verified.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    public static bool Verify(this G1Element publicKey, string message, G2Element signature) =>
        CoreMPL.Verify(publicKey, ByteUtils.ConcatenateArrays(publicKey.ToBytes(), message.ToBytes()), signature, CIPHERSUITE_ID);

    /// <summary>
    /// Aggregates multiple signatures into a single signature.
    /// </summary>
    /// <param name="signatures">The array of signatures to be aggregated.</param>
    /// <returns>The aggregated signature as a JacobianPoint.</returns>
    public static G2Element Aggregate(G2Element[] signatures) => CoreMPL.Aggregate(signatures);

    /// <summary>
    /// Verifies an aggregated signature against multiple public keys and messages.
    /// </summary>
    /// <param name="publicKeys">The array of public keys used for verification.</param>
    /// <param name="messages">The array of messages to be verified.</param>
    /// <param name="signature">The aggregated signature to be verified.</param>
    /// <returns>True if the aggregated signature is valid, false otherwise.</returns>
    public static bool AggregateVerify(this G1Element[] publicKeys, byte[][] messages, G2Element signature)
    {
        int length = publicKeys.Length;
        if (length != messages.Length || length == 0)
        {
            return false;
        }

        var augMessages = new byte[length][];
        for (int i = 0; i < length; i++)
        {
            var pubkey = publicKeys[i].ToBytes();
            augMessages[i] = new byte[pubkey.Length + messages[i].Length];
            pubkey.CopyTo(augMessages[i], 0);
            messages[i].CopyTo(augMessages[i], pubkey.Length);
        }

        return CoreMPL.AggregateVerify(publicKeys, augMessages, signature, CIPHERSUITE_ID);

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
    
    /// <summary>
    /// Derives a child unhardened public key from the specified public key and index.
    /// </summary>
    /// <param name="publicKey">The parent public key.</param>
    /// <param name="index">The index of the child public key.</param>
    /// <returns>The derived child unhardened public key.</returns>
    public static G1Element DeriveChildPkUnhardened(G1Element publicKey, uint index) => CoreMPL.DeriveChildPkUnhardened(publicKey, index);

}