public class Dice
{
    public IReadOnlyList<int> Faces { get; }
    public string Label { get; }

    public Dice(IEnumerable<int> faces, string label = "")
    {
        var facesList = faces?.ToList() ?? throw new ArgumentNullException(nameof(faces));
        if (facesList.Count == 0)
            throw new ArgumentException("Dice must have at least one face");
        Faces = facesList.AsReadOnly();
        Label = label;
    }

    public override string ToString() => $"[{string.Join(",", Faces)}]";
}