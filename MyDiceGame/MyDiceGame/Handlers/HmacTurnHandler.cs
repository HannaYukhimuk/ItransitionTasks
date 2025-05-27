public class HmacTurnHandler : IHmacTurnHandler
{
    private readonly IHmacGenerator _hmacGenerator;
    private readonly IFairRandomGenerator _random;
    private readonly IOutputPrinter _printer;

    public HmacTurnHandler(
        IHmacGenerator hmacGenerator,
        IFairRandomGenerator random,
        IOutputPrinter printer)
    {
        _hmacGenerator = hmacGenerator;
        _random = random;
        _printer = printer;
    }

    public (int Result, byte[] Secret, int ComputerNumber)? HandleTurn(
        int range,
        Dictionary<string, string> menu,
        Func<string, bool> isSpecialCommand,
        Action<string> handleSpecialCommand,
        Func<int, int, int> computeResult,
        Func<int, string>? resultDescription = null)
    {
        var (secret, computerNumber, hmac) = PrepareTurnData(range, menu);
        return ProcessUserInput(range, isSpecialCommand, 
            handleSpecialCommand, computeResult, resultDescription, 
            secret, computerNumber);
    }

    private (byte[] secret, int computerNumber, string hmac) PrepareTurnData(int range, Dictionary<string, string> menu)
    {
        var secret = _hmacGenerator.GenerateSecret();
        int computerNumber = _random.Next(0, range);
        string hmac = _hmacGenerator.GenerateHmac(computerNumber, secret);
        _printer.PrintLines($"HMAC: {hmac}");
        _printer.ShowMenu(menu);
        return (secret, computerNumber, hmac);
    }

    private (int Result, byte[] Secret, int ComputerNumber)? ProcessUserInput(
        int range,
        Func<string, bool> isSpecialCommand,
        Action<string> handleSpecialCommand,
        Func<int, int, int> computeResult,
        Func<int, string>? resultDescription,
        byte[] secret,
        int computerNumber)
    {
        while (true)
        {
            string input = GetUserInput();
            
            if (ShouldExit(input)) return null;
            if (HandleSpecialCommand(input, isSpecialCommand, handleSpecialCommand)) continue;
            
            var result = TryProcessNumberInput(input, range, computerNumber, 
                computeResult, resultDescription, secret);
            if (result.HasValue) return result.Value;
            
            ShowInvalidInput(range);
        }
    }

    private string GetUserInput()
    {
        return Console.ReadLine()?.Trim().ToUpper() ?? "";
    }

    private bool ShouldExit(string input) => input == "X";

    private bool HandleSpecialCommand(
        string input,
        Func<string, bool> isSpecialCommand,
        Action<string> handleSpecialCommand)
    {
        if (!isSpecialCommand(input)) return false;
        handleSpecialCommand(input);
        return true;
    }

    private (int Result, byte[] Secret, int ComputerNumber)? TryProcessNumberInput(
        string input,
        int range,
        int computerNumber,
        Func<int, int, int> computeResult,
        Func<int, string>? resultDescription,
        byte[] secret)
    {
        if (!int.TryParse(input, out int userNumber)) return null;
        if (userNumber < 0 || userNumber >= range) return null;
        
        return ProcessValidNumber(computerNumber, userNumber, 
            range, computeResult, resultDescription, secret);
    }

    private (int Result, byte[] Secret, int ComputerNumber) ProcessValidNumber(
        int computerNumber,
        int userNumber,
        int range,
        Func<int, int, int> computeResult,
        Func<int, string>? resultDescription,
        byte[] secret)
    {
        int result = computeResult(computerNumber, userNumber);
        PrintTurnResults(computerNumber, userNumber, range, result, resultDescription, secret);
        return (result, secret, computerNumber);
    }

    private void PrintTurnResults(
        int computerNumber,
        int userNumber,
        int range,
        int result,
        Func<int, string>? resultDescription,
        byte[] secret)
    {
        var lines = new List<string>
        {
            $"Computer's number: {computerNumber}",
            $"Secret key: {BitConverter.ToString(secret).Replace("-", "")}",
            $"Result: ({computerNumber} + {userNumber}) mod {range} = {result}"
        };

        if (resultDescription != null)
            lines.Add(resultDescription(result));

        _printer.PrintLines(lines.ToArray());
    }

    private void ShowInvalidInput(int range)
    {
        _printer.PrintLines($"Invalid input. Entооer 0–{range - 1}.");
    }
}