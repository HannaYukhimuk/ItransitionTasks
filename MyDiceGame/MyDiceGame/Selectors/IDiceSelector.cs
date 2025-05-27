
public interface IDiceSelector
{
    Dice SelectComputerDice(List<Dice> availableDice);
    Dice? SelectPlayerDice(List<Dice> availableDice, List<Dice> allDice);
}
