public class FirstPlayerDecider : IFirstPlayerDecider
{
    private readonly IHmacTurnHandler _turnHandler;
    private readonly IProbabilityVisualizer _probabilityVisualizer;
    private readonly List<Dice> _diceList;
    private readonly IOutputPrinter _printer;

    public FirstPlayerDecider(
        IHmacTurnHandler turnHandler,
        IProbabilityVisualizer probabilityVisualizer,
        List<Dice> diceList,
        IOutputPrinter printer)
    {
        _turnHandler = turnHandler;
        _probabilityVisualizer = probabilityVisualizer;
        _diceList = diceList;
        _printer = printer;
    }

    public bool? DecideFirstPlayer()
{
    PrintHeader();
    var turnResult = ProcessPlayerTurn();

    if (turnResult == null)
    {
        return null;
    }

    return DetermineFirstPlayer(turnResult);
}


    private void PrintHeader()
    {
        _printer.PrintLines("\n=== Determining First Player ===");
    }

    private (int Result, byte[] Secret, int ComputerNumber)? ProcessPlayerTurn()
    {
        return _turnHandler.HandleTurn(
            2,
            CreateTurnOptions(),
            IsHelpRequested,
            ShowProbabilities,
            CalculateTurnResult,
            GetResultMessage
        );
    }

    private Dictionary<string, string> CreateTurnOptions()
    {
        return new Dictionary<string, string>
        {
            { "0", "choose 0" },
            { "1", "choose 1" },
            { "?", "show probabilities" },
            { "X", "exit" }
        };
    }

    private bool IsHelpRequested(string input) => input == "?";

    private void ShowProbabilities(string _) => 
        _probabilityVisualizer.ShowProbabilities(_diceList);

    private int CalculateTurnResult(int computer, int user) => 
        (computer + user) % 2;

    private string GetResultMessage(int result) => 
        result == 1 ? "You go first" : "Computer goes first";

    private bool? DetermineFirstPlayer((int Result, byte[] Secret, int ComputerNumber)? turnResult)
    {
        return turnResult?.Result == 1;
    }
}