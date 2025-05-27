public interface IProbabilityCalculator
{
    double CalculateWinProbability(Dice a, Dice b);
    double CalculateTieProbability(Dice dice);
}
