namespace T3.Interfaces;

public interface IDiceProbability
{
    IList<Dice> Dices { get; set; }
    
    List<List<string>> Results { get; set; }
    
    void Calculate(IList<Dice> dices);
}