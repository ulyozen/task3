using System.Security.Cryptography;
using T3.Interfaces;

namespace T3;

public class SecureRNG : ISecureRNG
{
    private readonly int _keySize = 32;
    
    public string GenerateKey()
    {
        var key = new byte[_keySize];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        
        return BitConverter.ToString(key).Replace("-", string.Empty);
    }

    public string GenerateNumber(int numbers)
    {
        return new Random().Next(numbers).ToString();
    }
}