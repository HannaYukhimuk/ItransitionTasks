public class GameEngine : IGameEngine
{
    private readonly List<Dice> _diceList;
    private readonly IFirstPlayerDecider _firstPlayerDecider;
    private readonly IDiceSelector _diceSelector;
    private readonly IDiceRoller _diceRoller;
    private readonly IOutputPrinter _printer;
    private bool _shouldExit = false;

    public GameEngine(
        List<Dice> diceList,
        IFirstPlayerDecider firstPlayerDecider,
        IDiceSelector diceSelector,
        IDiceRoller diceRoller,
        IOutputPrinter printer)
    {
        ValidateDiceCount(diceList);
        _diceList = new List<Dice>(diceList);
        _firstPlayerDecider = firstPlayerDecider;
        _diceSelector = diceSelector;
        _diceRoller = diceRoller;
        _printer = printer;
    }

    public void Start()
    {
        PrintGameStart();

        bool? playerFirst = _firstPlayerDecider.DecideFirstPlayer();
        if (!playerFirst.HasValue)
        {
            ExitGame("You chose to exit the game.");
            return;
        }

        bool success = SelectDice(playerFirst.Value);
        if (!success || _shouldExit) return;

        PlayRound();
    }

    private void ValidateDiceCount(List<Dice> diceList)
    {
        if (diceList.Count < 3)
            throw new ArgumentException("At least 3 dice are required");
    }

    private void PrintGameStart()
    {
        _printer.PrintLines("=== Non-Transitive Dice Game ===",
            $"Loaded {_diceList.Count} dice");
    }

    private bool SelectDice(bool playerFirst)
    {
        _printer.PrintLines("\n=== Selecting Dice ===");
        var availableDice = new List<Dice>(_diceList);

        if (playerFirst)
            return SelectPlayerFirst(availableDice);

        return SelectComputerFirst(availableDice);
    }

    private bool SelectPlayerFirst(List<Dice> availableDice)
    {
        Dice? playerDice = GetPlayerDiceSelection(availableDice);
        if (_shouldExit || playerDice == null) return false;

        availableDice.Remove(playerDice);
        Dice computerDice = _diceSelector.SelectComputerDice(availableDice);

        _diceList[0] = computerDice;
        _diceList[1] = playerDice;

        PrintSelections(computerDice, playerDice);
        return true;
    }

    private bool SelectComputerFirst(List<Dice> availableDice)
    {
        Dice computerDice = _diceSelector.SelectComputerDice(availableDice);
        availableDice.Remove(computerDice);

        Dice? playerDice = GetPlayerDiceSelection(availableDice);
        if (_shouldExit || playerDice == null) return false;

        _diceList[0] = computerDice;
        _diceList[1] = playerDice;

        PrintSelections(computerDice, playerDice);
        return true;
    }

    private Dice? GetPlayerDiceSelection(List<Dice> availableDice)
    {
        var selectedDice = _diceSelector.SelectPlayerDice(availableDice, _diceList);
        if (selectedDice == null)
        {
            _shouldExit = true;
            return null;
        }
        return selectedDice;
    }

    private void PrintSelections(Dice computerDice, Dice playerDice)
    {
        _printer.PrintLines($"\nComputer selected: {computerDice}",
                            $"You selected: {playerDice}");
    }

    private void PlayRound()
    {
        if (_shouldExit) return;

        _printer.PrintLines("\n=== Rolling Dice ===");

        int? computerRoll = _diceRoller.RollDice("Computer", _diceList[0]);
        int? playerRoll = _diceRoller.RollDice("Player", _diceList[1]);

        if (!computerRoll.HasValue || !playerRoll.HasValue)
        {
            ExitGame("Game exited by user.");
            return;
        }

        PrintResults(computerRoll.Value, playerRoll.Value);
    }

    private void PrintResults(int computerRoll, int playerRoll)
    {
        _printer.PrintLines($"\nComputer rolled: {computerRoll}",
                            $"You rolled: {playerRoll}");
        PrintGameOutcome(computerRoll, playerRoll);
    }

    private void PrintGameOutcome(int computerRoll, int playerRoll)
    {
        if (playerRoll > computerRoll)
            _printer.PrintLines("You win!");
        else if (playerRoll < computerRoll)
            _printer.PrintLines("Computer wins!");
        else
            _printer.PrintLines("It's a tie!");
    }

    private void ExitGame(string message)
    {
        _printer.PrintLines(message);
        _shouldExit = true;
    }
}
