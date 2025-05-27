public interface IHmacTurnHandler
{
    (int Result, byte[] Secret, int ComputerNumber)? HandleTurn(
        int range,
        Dictionary<string, string> menu,
        Func<string, bool> isSpecialCommand,
        Action<string> handleSpecialCommand,
        Func<int, int, int> computeResult,
        Func<int, string>? resultDescription = null);
}