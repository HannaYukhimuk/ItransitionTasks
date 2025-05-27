public interface IOutputPrinter
{
    void PrintLines(params string[] lines);
    void Prompt(string message);
    void ShowMenu(Dictionary<string, string> options, string prompt = "Your selection: ");
}