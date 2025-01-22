namespace T3.Interfaces;

public interface ITrustHMAC
{
    (string, string) Compute(string input);
    
    bool Verify(string key, string input, string signature);
}