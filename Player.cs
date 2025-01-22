using T3.Interfaces;

namespace T3;

public class Player
{
    public Dice SelectedDice { get; }
    
    public int Score { get; set; }
    
    public Player(IList<Dice> dices, int choose)
    {
        SelectedDice = dices[choose];
        
        dices.RemoveAt(choose);
    }
    
    public Player(ISecureRNG rng, IList<Dice> dices)
    {
        var index = int.Parse(rng.GenerateNumber(dices.Count));
        
        SelectedDice = dices[index];
        
        dices.RemoveAt(index);
    }
    
    public string RollDice(int choose)
    {
        return SelectedDice.Numbers[choose];
    }
}