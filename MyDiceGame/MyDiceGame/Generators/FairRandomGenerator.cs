using System.Security.Cryptography;

public class FairRandomGenerator : IFairRandomGenerator
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public int Next(int minValue, int maxValue)
    {
        ValidateRange(minValue, maxValue);
        uint range = CalculateRange(minValue, maxValue);
        uint randomValue = GenerateRandomValue(range);
        return (int)(minValue + (randomValue % range));
    }

    public void Dispose() => _rng?.Dispose();

    private void ValidateRange(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("Max value must be greater than min value");
    }

    private uint CalculateRange(int minValue, int maxValue)
    {
        return (uint)(maxValue - minValue);
    }

    private uint GenerateRandomValue(uint range)
    {
        uint max = uint.MaxValue - uint.MaxValue % range;
        uint value;
        
        do
        {
            value = GetNextRandomUInt();
        } while (value > max);

        return value;
    }

    private uint GetNextRandomUInt()
    {
        byte[] bytes = new byte[4];
        _rng.GetBytes(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
}