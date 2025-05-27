public class DiceSelector : IDiceSelector
{
    private readonly IFairRandomGenerator _random;
    private readonly IProbabilityCalculator _calculator;
    private readonly IOutputPrinter _printer;
    private readonly IProbabilityVisualizer _visualizer;

    public DiceSelector(
        IFairRandomGenerator random,
        IProbabilityCalculator calculator,
        IOutputPrinter printer,
        IProbabilityVisualizer visualizer)
    {
        _random = random;
        _calculator = calculator;
        _printer = printer;
        _visualizer = visualizer;
    }

    public Dice SelectComputerDice(List<Dice> availableDice)
    {
        return availableDice
            .Select(dice => CreateDiceWithProbability(dice, availableDice))
            .OrderByDescending(x => x.AvgProbability)
            .First()
            .Dice;
    }

    public Dice? SelectPlayerDice(List<Dice> availableDice, List<Dice> allDice)
    {
        while (true)
        {
            ShowAvailableDice(availableDice);
            var input = GetPlayerInput();
            
            if (ShouldExit(input)) return null;
            if (ShouldShowProbabilities(input, allDice)) continue;
            if (TryGetSelectedDice(input, availableDice, out var dice)) return dice;
            
            ShowInvalidSelection();
        }
    }

    private dynamic CreateDiceWithProbability(Dice dice, List<Dice> availableDice)
    {
        return new
        {
            Dice = dice,
            AvgProbability = CalculateAverageProbability(dice, availableDice)
        };
    }

    private double CalculateAverageProbability(Dice dice, List<Dice> availableDice)
    {
        return availableDice
            .Where(d => d != dice)
            .Average(d => _calculator.CalculateWinProbability(dice, d));
    }

    private void ShowAvailableDice(List<Dice> availableDice)
    {
        _printer.PrintLines("\nAvailable dice:");
        for (int i = 0; i < availableDice.Count; i++)
            _printer.PrintLines($"{i} - {availableDice[i]}");
    }

    private string GetPlayerInput()
    {
        _printer.ShowMenu(new()
        {
            { "?", "show probabilities" },
            { "X", "exit" }
        }, "Select your dice: ");
        
        return Console.ReadLine()?.Trim().ToUpper() ?? "";
    }

    private bool ShouldExit(string input) => input == "X";

    private bool ShouldShowProbabilities(string input, List<Dice> allDice)
    {
        if (input != "?") return false;
        _visualizer.ShowProbabilities(allDice);
        return true;
    }

    private bool TryGetSelectedDice(string input, List<Dice> availableDice, out Dice? dice)
    {
        dice = null;
        if (!int.TryParse(input, out int index)) return false;
        if (index < 0 || index >= availableDice.Count) return false;
        
        dice = availableDice[index];
        return true;
    }

    private void ShowInvalidSelection()
    {
        _printer.PrintLines("Invalid selection. Try again.");
    }
}