using System.Reflection.Emit;

public class LabelTable
{
    public Dictionary<string, int> MapLabel = new();
    public void Add(Token label)
    {
        if (MapLabel.ContainsKey(label.Value))
        {
            throw new RuntimeErrorException(label, $"Label {label.Value} is already defined");
        }
        MapLabel[label.Value] = label.Line;
    }
    public int GetLine(Token label)
    {
        if (MapLabel.TryGetValue(label.Value, out int line))
        {
            return line;
        }
        throw new RuntimeErrorException(label, $"Label {label.Value} doesn't exist");
    }
    public bool CheckLabel(Token label) => MapLabel.ContainsKey(label.Value);
}