public interface IHmacGenerator : IDisposable
{
    byte[] GenerateSecret();
    string GenerateHmac(int number, byte[] key);
}