using T3.Interfaces;

namespace T3;

public class GameRules(string[] args) : IRules
{
    public int TotalGamers => 2;
    
    public int TotalThrows => 2;

    public string[] HeadsOrTails { get; } = ["0", "1"];
    
    private bool HasMinimumDice() => args.Length > 2;
    
    private bool AreAllDiceValid() => args.All(s => s.Split(",").Length == 6);
    
    private bool AreAllValuesIntegers() => args.All(s => s.Split(",").All(val => int.TryParse(val, out _)));
    
    private string GetMessage()
    {
        if (!HasMinimumDice())
        {
            return "At least three dice must be provided. Example: 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3";
        }

        if (!AreAllDiceValid())
        {
            return "Each dice must contain exactly six comma-separated integers. Example: 2,2,4,4,9,9";
        }

        if (!AreAllValuesIntegers())
        {
            return "All values must be integers. Example: 2,2,4,4,9,9 6,8,1,1,8,6";
        }

        return "Unknown error.";
    }
    
    public bool ValidateGameArguments(out string message)
    {
        if (!HasMinimumDice() || !AreAllDiceValid() || !AreAllValuesIntegers())
        {
            message = GetMessage();
            return true;
        }

        message = "";
        return false;
    }

    public bool ValidateInput(IEnumerable<string> values, string input, out string message)
    {
        var only = new List<string>(values){"x", "?"};
        var key = input.ToLower();
        
        message = $"Please press one of the following keys: { string.Join(", ", only.Select(x => $"'{x.ToString().ToUpper()}'")) }.";
        
        return only.Contains(key);
    }
}