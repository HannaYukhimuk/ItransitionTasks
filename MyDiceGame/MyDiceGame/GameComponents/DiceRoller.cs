public class DiceRoller : IDiceRoller
{
    private readonly IHmacTurnHandler _turnHandler;
    private readonly IOutputPrinter _printer;
    private readonly IProbabilityVisualizer _visualizer;
    private readonly List<Dice> _diceList;
    private bool _shouldExit = false;

    public DiceRoller(
        IHmacTurnHandler turnHandler,
        IOutputPrinter printer,
        IProbabilityVisualizer visualizer,
        List<Dice> diceList)
    {
        _turnHandler = turnHandler;
        _printer = printer;
        _visualizer = visualizer;
        _diceList = diceList;
    }

    public int? RollDice(string label, Dice dice)
    {
        if (_shouldExit) return null;

        PrintRollMessage(label, dice);
        var result = ProcessDiceRoll(dice);
        return HandleRollResult(dice, result);
    }

    private void PrintRollMessage(string label, Dice dice)
    {
        _printer.PrintLines($"\n{label}'s turn to roll {dice}");
    }

    private (int Result, byte[] Secret, int ComputerNumber)? ProcessDiceRoll(Dice dice)
    {
        var choices = CreateChoiceDictionary(dice);
        return _turnHandler.HandleTurn(
            dice.Faces.Count,
            choices,
            IsHelpRequested,
            ShowProbabilities,
            (computer, user) => (computer + user) % dice.Faces.Count,
            result => $"Dice face: {dice.Faces[result]}"
        );
    }

    private bool IsHelpRequested(string input) => input == "?";

    private void ShowProbabilities(string _)
    {
        _visualizer.ShowProbabilities(_diceList);
    }

    private Dictionary<string, string> CreateChoiceDictionary(Dice dice)
    {
        var choices = Enumerable.Range(0, dice.Faces.Count)
            .ToDictionary(i => i.ToString(), i => i.ToString());
        choices.Add("x", "exit");
        choices.Add("?", "help");
        return choices;
    }

    private int? HandleRollResult(Dice dice, (int Result, byte[] Secret, int ComputerNumber)? result)
    {
        if (result == null)
        {
            _shouldExit = true;
            return null;
        }
        return dice.Faces[result.Value.Result];
    }
}