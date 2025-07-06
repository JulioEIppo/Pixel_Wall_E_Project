using System.Collections.Generic;
using System.Reflection.Emit;
namespace PixelWallE
{
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
        public int GetLine(Token token, string labelName )
        {
            if (MapLabel.TryGetValue(labelName, out int line))
            {
                return line;
            }
            throw new RuntimeErrorException(token, $"Label {labelName} doesn't exist");
        }
        public bool CheckLabel(Token label) => MapLabel.ContainsKey(label.Value);
    }
}