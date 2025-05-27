using System;
using ConsoleTables;

public class ConsoleProbabilityVisualizer : IProbabilityVisualizer
{
    private readonly IProbabilityCalculator _calculator;
    private readonly IOutputPrinter _printer;

    public ConsoleProbabilityVisualizer(IProbabilityCalculator calculator, IOutputPrinter printer)
    {
        _calculator = calculator;
        _printer = printer;
    }

    public void ShowProbabilities(List<Dice> diceList)
    {
        PrintTableHeader();
        var table = InitializeTable(diceList);
        FillTableData(table, diceList);
        PrintTableAndDetails(table, diceList);
    }

    private void PrintTableHeader()
    {
        _printer.PrintLines("\n=== Probability Table ===",
            "Values show probability that row dice beats column dice",
            "Diagonal shows probability of tie");
    }

    private ConsoleTable InitializeTable(List<Dice> diceList)
    {
        var table = new ConsoleTable(new ConsoleTableOptions
        {
            EnableCount = false,
            NumberAlignment = Alignment.Right
        });

        var headers = CreateTableHeaders(diceList);
        table.AddColumn(headers.ToArray());
        return table;
    }

    private List<string> CreateTableHeaders(List<Dice> diceList)
    {
        var headers = new List<string> { "Dice \\ vs →" };
        headers.AddRange(diceList.Select(d => d.Label));
        return headers;
    }

    private void FillTableData(ConsoleTable table, List<Dice> diceList)
    {
        for (int i = 0; i < diceList.Count; i++)
        {
            var row = CreateTableRow(diceList, i);
            table.AddRow(row.ToArray());
        }
    }

    private List<object> CreateTableRow(List<Dice> diceList, int rowIndex)
    {
        var row = new List<object> { diceList[rowIndex].Label };

        for (int j = 0; j < diceList.Count; j++)
        {
            row.Add(GetCellValue(diceList, rowIndex, j));
        }

        return row;
    }

    private string GetCellValue(List<Dice> diceList, int i, int j)
    {
        return i == j
            ? $"{_calculator.CalculateTieProbability(diceList[i]):0.0000}"
            : $"{_calculator.CalculateWinProbability(diceList[i], diceList[j]):0.0000}";
    }

    private void PrintTableAndDetails(ConsoleTable table, List<Dice> diceList)
    {
        table.Write(Format.Minimal);
        PrintDiceDetails(diceList);
    }

    private void PrintDiceDetails(List<Dice> diceList)
    {
        _printer.PrintLines("\nDice details:");
        foreach (var dice in diceList)
        {
            _printer.PrintLines($"{dice.Label}: {dice}");
        }
    }
}