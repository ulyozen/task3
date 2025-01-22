using System.Globalization;
using T3.Interfaces;

namespace T3;

public class DiceProbability : IDiceProbability
{
    public IList<Dice> Dices { get; set; } = null!;
    
    public List<List<string>> Results { get; set; } = [];
    
    public void Calculate(IList<Dice> dices)
    {
        Dices = dices;

        foreach (var row in Dices)
        {
            var table = new List<string> { string.Join(",", row.Numbers) };

            foreach (var column in Dices)
            {
                var countNum = row.Numbers.SelectMany(_ => column.Numbers, (x, y) => int.Parse(x) > int.Parse(y)).Count(x => x);
                
                var result = Math.Round(countNum / Math.Pow(row.Numbers.Count, 2), 4);

                table.Add(!row.Numbers.SequenceEqual(column.Numbers) ? $"{result}" : $"- ({result})");
            }
            
            Results.Add(table);
        }
    }
}