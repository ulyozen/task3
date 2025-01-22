namespace T3;

public class Dice(IList<string> numbers)
{
    public IList<string> Numbers { get; private set; } = numbers.ToArray();
}