public class DiceParser : IDiceParser
{
    private const int ExpectedFaceCount = 6;

    public List<Dice> ParseDice(string[] inputs)
    {
        ValidateInputs(inputs);
        return inputs.Select((input, index) => ParseSingleDice(input, index)).ToList();
    }

    private Dice ParseSingleDice(string input, int index)
    {
        ValidateDiceInput(input);
        var faces = ParseFaces(input, index);
        return CreateDice(faces, index);
    }

    private void ValidateInputs(string[] inputs)
    {
        if (inputs == null || inputs.Length == 0)
            throw new ArgumentException("Error: No dice provided.");
    }

    private void ValidateDiceInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Error: Empty dice configuration.");
    }

    private List<int> ParseFaces(string input, int diceIndex)
    {
        try
        {
            var faces = SplitAndCleanInput(input)
                        .Select(s => ParseFaceValue(s, diceIndex, input))
                        .ToList();

            ValidateFacesCount(faces, diceIndex, input);
            return faces;
        }
        catch (FormatException)
        {
            throw new ArgumentException($"Error: Invalid number format in dice {diceIndex + 1}: '{input}'");
        }
        catch (OverflowException)
        {
            throw new ArgumentException($"Error: Number too large in dice {diceIndex + 1}: '{input}'");
        }
    }

    private int ParseFaceValue(string value, int diceIndex, string input)
    {
        if (!int.TryParse(value, out var face))
            throw new ArgumentException($"Error: Value '{value}' in dice {diceIndex + 1} is not a valid integer. Full string: '{input}'");
        return face;
    }

    private IEnumerable<string> SplitAndCleanInput(string input)
    {
        return input.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s));
    }

    private void ValidateFacesCount(List<int> faces, int diceIndex, string input)
    {
        if (faces.Count != ExpectedFaceCount)
            throw new ArgumentException($"Error: Dice {diceIndex + 1} must have exactly {ExpectedFaceCount} faces. Found: {faces.Count}. Input: '{input}'");
    }

    private Dice CreateDice(List<int> faces, int index)
    {
        return new Dice(faces, $"D{index + 1}");
    }
}
