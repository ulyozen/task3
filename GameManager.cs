using T3.Interfaces;

namespace T3;

public class GameManager(
    IRules rules, 
    ISecureRNG rng, 
    ITrustHMAC hmac, 
    IDiceProbability diceProb,
    ITableGenerator table)
{
    private bool _isChosenByHuman;
    
    private readonly string _human = "Human";
    
    private readonly string _terminator = "Terminator";

    private IList<Dice> Dices { get; set; } = new List<Dice>();

    private IDictionary<string, Player> Players { get; } = new Dictionary<string, Player>();

    public void Run(string[] args)
    {
        if (rules.ValidateGameArguments(out var message))
        {
            Console.WriteLine(message);
            StopGame();
        }

        Dices = args
            .Select(arg => arg.Split(",").ToList())
            .Select(nums => new Dice(nums))
            .ToList();
        
        diceProb.Calculate(Dices);
        
        StartGame();
    }
    
    private void StartGame()
    {
        var isHumanFirst = ChooseFirstPlayer();
        
        PrepareForGame(isHumanFirst);
        
        PlayGame(isHumanFirst);

        DisplayGameResult();
    }
    
    private bool ChooseFirstPlayer()
    {
        var number = rng.GenerateNumber(rules.HeadsOrTails.Length);
        var (key, hash) = hmac.Compute(number);
        
        Console.WriteLine("Let's determine who makes the first move.");
        Console.WriteLine("I selected a random value in the range 0..1");
        Console.WriteLine($"(HMAC={ hash }).");
        Console.WriteLine("Try to guess my selection.");
        for (var i = 0; i < rules.HeadsOrTails.Length; i++)
        {
            Console.WriteLine($"{ i } - { rules.HeadsOrTails[i] }");
        }
        Console.WriteLine("X - exit\n? - help");

        HandleInputAndValidation(rules.HeadsOrTails, out var guess);

        Console.WriteLine($"Your selection: { guess }");
        Console.WriteLine($"My selection: { number } (KEY={ key }).");

        return number == guess;
    }

    private void PrepareForGame(bool isHumanFirst)
    {
        for (var i = 0; i < rules.TotalGamers; i++)
        {
            (isHumanFirst ? (Action)HumanSelectDice : TerminatorSelectDice)();
            
            isHumanFirst = !isHumanFirst;
        }
    }

    private void PlayGame(bool isHumanFirst)
    {
        for (var i = 0; i < rules.TotalThrows; i++)
        {
            (isHumanFirst ? (Action)HumanThrowDice : TerminatorThrowDice)();
            
            isHumanFirst = !isHumanFirst;
        }
    }

    private void DisplayGameResult()
    {
        Console.WriteLine(
            Players[_human].Score > Players[_terminator].Score 
                ? $"You win ({ Players[_human].Score } > { Players[_terminator].Score })!" 
                : $"I win ({ Players[_terminator].Score } > { Players[_human].Score })!"
        );
    }
    
    private void HumanSelectDice()
    {
        var availableChoice = Enumerable.Range(0, Dices.Count).Select(x => x.ToString()).ToList();
        if (!_isChosenByHuman)
        {
            Console.WriteLine("Choose your dice:");
            for (var i = 0; i < Dices.Count; i++)
            {
                Console.WriteLine($"{ i } - { string.Join(",", Dices[i].Numbers) }");
            }
            Console.WriteLine("X - exit\n? - help");
            
            HandleInputAndValidation(availableChoice, out var keyChar);

            var humanChoice = new Player(Dices, int.Parse(keyChar));
            Players.Add(_human, humanChoice);
            
            Console.WriteLine($"You made the first move and choose the { string.Join(",", humanChoice.SelectedDice.Numbers) } dice.");
            
            _isChosenByHuman = !_isChosenByHuman;
        }
        else
        {
            HandleInputAndValidation(availableChoice, out var keyChar);
            
            var humanChoice = new Player(Dices, int.Parse(keyChar));
            Players.Add(_human, humanChoice);
            
            Console.WriteLine($"Your selection: { keyChar }");
            Console.WriteLine($"You choose the {  string.Join(",", humanChoice.SelectedDice.Numbers) } dice.");
            Console.WriteLine("It's time for my throw.");
        }
    }

    private void HumanThrowDice()
    {
        var human = Players[_human];
        var terminatorThrow = rng.GenerateNumber(human.SelectedDice.Numbers.Count);
        var (key, hash) = hmac.Compute(terminatorThrow);
            
        Console.WriteLine($"I selected a random value in the range 0..5");
        Console.WriteLine($"(HMAC={ hash }).");
        Console.WriteLine("Add your number modulo 6.");
        Console.WriteLine("Choose your number:");
        for (var i = 0; i < human.SelectedDice.Numbers.Count; i++)
        {
            Console.WriteLine($"{ i } - { i }");
        }
        Console.WriteLine("X - exit\n? - help");

        var indexes = human.SelectedDice.Numbers.Select((_, index) => index.ToString()).ToList();
            
        HandleInputAndValidation(indexes, out var humanThrow);
            
        var mod = (int.Parse(humanThrow) + int.Parse(terminatorThrow)) % 6;

        Console.WriteLine($"Your selection: { humanThrow }");
        Console.WriteLine($"My number is { terminatorThrow } (KEY={ key }).");
        Console.WriteLine($"The result is { humanThrow } + { terminatorThrow } = { mod } (mod 6).");

        var number = human.RollDice(mod);
        human.Score = int.Parse(number);
            
        Console.WriteLine($"Your throw is {number}.");
    }
    
    private void TerminatorSelectDice()
    {
        var terminatorChoice = new Player(rng, Dices);
        Players.Add(_terminator, terminatorChoice);
        
        if (!_isChosenByHuman)
        {
            Console.WriteLine($"I made the first move and choose the { string.Join(",", terminatorChoice.SelectedDice.Numbers) } dice.");
            Console.WriteLine("Choose your dice:");
            for (var i = 0; i < Dices.Count; i++)
            {
                Console.WriteLine($"{ i } - { string.Join(",", Dices[i].Numbers) }");
            }
            Console.WriteLine("X - exit\n? - help");

            _isChosenByHuman = !_isChosenByHuman;
        }
        else
        {
            Console.WriteLine($"I choose the { string.Join(",", terminatorChoice.SelectedDice.Numbers) } dice:");
            Console.WriteLine("It's time for your throw.");
        }
    }

    private void TerminatorThrowDice()
    {
        var terminator = Players[_terminator];
        var terminatorThrow = rng.GenerateNumber(terminator.SelectedDice.Numbers.Count);
        var (key, hash) = hmac.Compute(terminatorThrow);
        
        Console.WriteLine($"I selected a random value in the range 0..5");
        Console.WriteLine($"(HMAC={ hash }).");
        Console.WriteLine("Add your number modulo 6.");
        for (var i = 0; i < terminator.SelectedDice.Numbers.Count; i++)
        {
            Console.WriteLine($"{ i } - { i }");
        }
        Console.WriteLine("X - exit\n? - help");
        
        var indexes = terminator.SelectedDice.Numbers.Select((_, index) => index.ToString()).ToList();
        
        HandleInputAndValidation(indexes, out var humanThrow);
            
        var mod = (int.Parse(humanThrow) + int.Parse(terminatorThrow)) % 6;

        Console.WriteLine($"Your selection: { humanThrow }");
        Console.WriteLine($"My number is { terminatorThrow } (KEY={ key }).");
        Console.WriteLine($"The result is { humanThrow } + { terminatorThrow } = { mod } (mod 6).");
        
        var number = terminator.RollDice(mod);
        terminator.Score = int.Parse(number);
            
        Console.WriteLine($"My throw is {number}.");
        Console.WriteLine("It's time for your throw.");
    }
    
    private void HandleInputAndValidation(IList<string> values, out string key)
    {
        var input = Console.ReadKey(intercept: true).KeyChar.ToString().ToLower();
        
        while (!rules.ValidateInput(values, input, out var message) || input == "?")
        {
            if (input == "?") table.Create();
            
            Console.WriteLine($"Message: {message}");

            input = Console.ReadKey(intercept: true).KeyChar.ToString().ToLower();
        }
        
        if (input == "x") StopGame();
        
        key = input;
    }
    
    private static void StopGame()
    {
        Environment.Exit(0);
    }
}