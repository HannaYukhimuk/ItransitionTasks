using System.Security.Cryptography;


public class FairRandomGenerator : IFairRandomGenerator
{
    public int Next(int minValue, int maxValue)
    {
        return RandomNumberGenerator.GetInt32(minValue, maxValue);
    }
}