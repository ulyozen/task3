namespace T3.Interfaces;

public interface IRules
{
    public int TotalGamers { get; }
    
    public int TotalThrows { get; }
    
    public string[] HeadsOrTails { get; }
    
    bool ValidateGameArguments(out string message);
    
    bool ValidateInput(IEnumerable<string> values, string input, out string message);
}