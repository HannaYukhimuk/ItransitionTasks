using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

public class HmacGenerator : IHmacGenerator, IDisposable
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public byte[] GenerateSecret()
    {
        byte[] secret = new byte[32];
        _rng.GetBytes(secret);
        return secret;
    }

    public string GenerateHmac(int number, byte[] key)
    {
        byte[] message = BitConverter.GetBytes(number);
        HMac hmac = new HMac(new Sha3Digest(256));
        hmac.Init(new KeyParameter(key));
        hmac.BlockUpdate(message, 0, message.Length);
        byte[] result = new byte[hmac.GetMacSize()];
        hmac.DoFinal(result, 0);
        return BitConverter.ToString(result).Replace("-", "").ToLowerInvariant();
    }

    public void Dispose() => _rng?.Dispose();
}
