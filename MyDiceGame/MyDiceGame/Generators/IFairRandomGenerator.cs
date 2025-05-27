public interface IFairRandomGenerator : IDisposable
{
    int Next(int minValue, int maxValue);
}