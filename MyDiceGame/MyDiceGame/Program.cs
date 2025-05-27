using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ConsoleTables;


public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            RunGame(args);
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    private static void RunGame(string[] args)
    {
        var dependencies = InitializeDependencies(args);
        StartGameEngine(dependencies);
    }

    private static GameDependencies InitializeDependencies(string[] args)
    {
        var printer = CreatePrinter();
        var random = CreateRandomGenerator();
        var hmacGenerator = CreateHmacGenerator();
        var calculator = CreateProbabilityCalculator();
        var visualizer = CreateVisualizer(calculator, printer);
        var turnHandler = CreateTurnHandler(hmacGenerator, random, printer);
        var diceList = ParseDice(args);

        return new GameDependencies(
            diceList,
            printer,
            random,
            hmacGenerator,
            calculator,
            visualizer,
            turnHandler);
    }

    private static ConsolePrinter CreatePrinter() => new ConsolePrinter();
    private static FairRandomGenerator CreateRandomGenerator() => new FairRandomGenerator();
    private static HmacGenerator CreateHmacGenerator() => new HmacGenerator();
    private static ProbabilityCalculator CreateProbabilityCalculator() => new ProbabilityCalculator();

    private static ConsoleProbabilityVisualizer CreateVisualizer(
        IProbabilityCalculator calculator,
        IOutputPrinter printer)
    {
        return new ConsoleProbabilityVisualizer(calculator, printer);
    }

    private static HmacTurnHandler CreateTurnHandler(
        IHmacGenerator hmacGenerator,
        IFairRandomGenerator random,
        IOutputPrinter printer)
    {
        return new HmacTurnHandler(hmacGenerator, random, printer);
    }

    private static List<Dice> ParseDice(string[] args)
    {
        var diceParser = new DiceParser();
        return diceParser.ParseDice(args);
    }

    private static void StartGameEngine(GameDependencies dependencies)
    {
        var engine = CreateGameEngine(dependencies);
        engine.Start();
    }

    private static GameEngine CreateGameEngine(GameDependencies dependencies)
    {
        var firstPlayerDecider = CreateFirstPlayerDecider(dependencies);
        var diceSelector = CreateDiceSelector(dependencies);
        var diceRoller = CreateDiceRoller(dependencies);

        return new GameEngine(
            dependencies.DiceList,
            firstPlayerDecider,
            diceSelector,
            diceRoller,
            dependencies.Printer);
    }

    private static FirstPlayerDecider CreateFirstPlayerDecider(GameDependencies dependencies)
    {
        return new FirstPlayerDecider(
            dependencies.TurnHandler,
            dependencies.Visualizer,
            dependencies.DiceList,
            dependencies.Printer);
    }

    private static DiceSelector CreateDiceSelector(GameDependencies dependencies)
    {
        return new DiceSelector(
            dependencies.Random,
            dependencies.Calculator,
            dependencies.Printer,
            dependencies.Visualizer);
    }

    private static DiceRoller CreateDiceRoller(GameDependencies dependencies)
    {
        return new DiceRoller(
            dependencies.TurnHandler,
            dependencies.Printer,
            dependencies.Visualizer,
            dependencies.DiceList);
    }

    private static void HandleError(Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


   

