using ConsoleTableExt;
using T3.Interfaces;

namespace T3;

public class TableViewProb(IDiceProbability diceProb) : ITableGenerator
{
    public void Create()
    {
        var column = new List<string>() { "User dice v" };
        column.AddRange(diceProb.Dices.Select(dice => string.Join(",", dice.Numbers)));

        ConsoleTableBuilder
            .From(diceProb.Results)
            .WithTitle("", ConsoleColor.Yellow, ConsoleColor.DarkGray)
            .WithColumn(column)
            .WithTitle("Probability of the win for the user")
            .WithFormat(ConsoleTableBuilderFormat.Alternative)
            .ExportAndWriteLine();
    }
}