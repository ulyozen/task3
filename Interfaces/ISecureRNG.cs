namespace T3.Interfaces;

public interface ISecureRNG
{
    string GenerateKey();

    string GenerateNumber(int numbers);
}