
public class ConsolePrinter : IOutputPrinter
{
    public void PrintLines(params string[] lines)
    {
        foreach (var line in lines)
            Console.WriteLine(line);
    }

    public void Prompt(string message)
    {
        Console.Write(message);
    }

    public void ShowMenu(Dictionary<string, string> options, string prompt = "Your selection: ")
    {
        foreach (var kvp in options)
            Console.WriteLine($"{kvp.Key} - {kvp.Value}");
        Console.Write(prompt);
    }
}
