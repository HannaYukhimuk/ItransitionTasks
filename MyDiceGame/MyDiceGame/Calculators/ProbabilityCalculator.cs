public class ProbabilityCalculator : IProbabilityCalculator
{
    public double CalculateWinProbability(Dice a, Dice b)
    {
        var (wins, ties) = CountWinsAndTies(a, b);
        int total = a.Faces.Count * b.Faces.Count;
        return CalculateFinalProbability(wins, ties, total);
    }

    public double CalculateTieProbability(Dice dice)
    {
        int ties = CountTies(dice);
        int total = dice.Faces.Count * dice.Faces.Count;
        return (double)ties / total;
    }

    private (int wins, int ties) CountWinsAndTies(Dice a, Dice b)
    {
        int wins = 0, ties = 0;
        foreach (int faceA in a.Faces)
            foreach (int faceB in b.Faces)
                UpdateCounters(faceA, faceB, ref wins, ref ties);
        return (wins, ties);
    }

    private void UpdateCounters(int faceA, int faceB, ref int wins, ref int ties)
    {
        if (faceA > faceB) wins++;
        else if (faceA == faceB) ties++;
    }

    private double CalculateFinalProbability(int wins, int ties, int total)
    {
        return (double)wins / total + (double)ties / total / 2;
    }

    private int CountTies(Dice dice)
    {
        int ties = 0;
        foreach (var faceA in dice.Faces)
            foreach (var faceB in dice.Faces)
                if (faceA == faceB) ties++;
        return ties;
    }
}