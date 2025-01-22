using System.Security.Cryptography;
using System.Text;
using T3.Interfaces;

namespace T3;

public class TrustHMAC(ISecureRNG rng) : ITrustHMAC
{
    public (string, string) Compute(string input)
    {
        var key = rng.GenerateKey();
        
        var hash = GenerateHmac(key, input);
        
        return (key, hash);
    }
    
    public bool Verify(string key, string input, string signature)
    {
        return signature == GenerateHmac(key, input);
    }
    
    private static string GenerateHmac(string key, string input)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        
        return BitConverter.ToString(computeHash).Replace("-", string.Empty);
    }
}