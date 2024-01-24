
public class CoreGame
{
    #region Fields And Properties
    public List<Character> heroesParty;
    public Inventory heroesInventory;
    public List<List<Character>> monsterParties;
    public Inventory monsterInventory;
    private int _MonstersParties {get; set;}
    private readonly int _maxMonsterCount;
    private PlayerType playerTurn {get; set;}

    private bool AI {get; set;} = false;
    private bool AITWO {get; set;} = false;

    private bool GameWinner {get; set;}
    public object lockObject = new object();
    #endregion

    #region Constructor
    public CoreGame()
    {
        heroesParty = new List<Character>();
        playerTurn  = PlayerType.PlayerOne;
        GameWinner = false;

        List<Character> monsterParty = new List<Character>{
            new Skeleton(5, AttackType.Bone_Crunch)
        };

        List<Character>monsterPartyTwo = new List<Character>{
            new Skeleton(5, AttackType.Bone_Crunch),
            new Skeleton(5, AttackType.Bone_Crunch)
        };

        List<Character>monsterPartyBoss = new List<Character>{
            new BossMonster(15, AttackType.Unraveling_Attack)
        };

        monsterParties = new List<List<Character>>{
            monsterParty,
            monsterPartyTwo,
            monsterPartyBoss
        };

        _maxMonsterCount = monsterParties.Count;
        _MonstersParties = 0;

        heroesInventory = new Inventory();
        heroesInventory.items.Add(new HealthPotion());
        heroesInventory.items.Add(new HealthPotion());
        heroesInventory.items.Add(new HealthPotion());

        monsterInventory = new Inventory();
        monsterInventory.items.Add(new HealthPotion());

    }
    #endregion

    #region Methods
    public void Run()
    {
        heroesParty.Add(CreateMainPlayer());
        ChoosePlayer();

        while (!GameWinner)
        {
            Battle(); 
        }

        Console.ReadLine();
    }
    private Player CreateMainPlayer()
    {
        Console.WriteLine("Enter your character's name: ");
        string? name = Console.ReadLine();

        if(name != null)
        {
            return new Player(name, AttackType.Punch, 25, heroesInventory, this);
        }
        else
        {
            return new Player("TOG", AttackType.Punch, 25, heroesInventory, this);
        }
        
    }
    private bool GameWinnerChecker()
    {
        if(monsterParties.Count > 0 && heroesParty.Count <= 0 )
        {
            Console.WriteLine($"WINNER is Monsters!!");
            Console.WriteLine("The Player Lose!");

            return true;
        }

        if(monsterParties.FirstOrDefault<List<Character>>()?.Count == 0)
        {
            Console.WriteLine("Defeated a party of Monster");

            if(_MonstersParties < _maxMonsterCount)
            monsterParties.RemoveAt(0);

            if(monsterParties.Count == 0)
            {
                Console.WriteLine("All Enemy has been ELIMINATED!!!");

                return true;
            }
        }

        return false;
    }
    private void Battle()
    {
        Console.Clear();
        BattleStatus();
        // ai player
        
        if(AITWO == true && playerTurn == PlayerType.PlayerTwo)
        {
            AiAction();
        }
        else if(AITWO == false && playerTurn == PlayerType.PlayerTwo)
        {
            UserPlayerTwoAction();
        }

        if(AI == true && playerTurn == PlayerType.PlayerOne)
        {
            AiSecondAction();
        }
        else if(AI == false && playerTurn == PlayerType.PlayerOne)
        {
            UserPlayerOneAction();
        }
        Thread.Sleep(1000);
        Console.WriteLine();  
    }
    private void AiAction()
    {
        foreach (Character character in monsterParties[_MonstersParties])
        {
            CharacterCurrentTurns(character);
            character.SetAction(new Attack());
            character.Action(character, heroesParty.FirstOrDefault<Character>());
            heroesParty = RemoveDeadCharacter(heroesParty);
        }

        playerTurn = PlayerType.PlayerOne;
        GameWinner = GameWinnerChecker();
    }
    private void UserPlayerOneAction()
    {
        foreach (Character character in heroesParty)
        {
            lock(lockObject)
            {
            Task.Run(() => PlayerChooseAction(character)).Wait();
            CharacterCurrentTurns(character);
            character.Action(character, monsterParties?.FirstOrDefault<List<Character>>()?.FirstOrDefault<Character>());
            monsterParties[_MonstersParties] = RemoveDeadCharacter(monsterParties[_MonstersParties]);
            }
        }

        playerTurn = PlayerType.PlayerTwo;
        GameWinner = GameWinnerChecker();
    }
    // AI TWO
    private void AiSecondAction()
    {
        foreach(Character character in heroesParty)
        {
            CharacterCurrentTurns(character);
            character.SetAction(new Attack());
            character.Action(character, monsterParties?.FirstOrDefault<List<Character>>()?.FirstOrDefault<Character>());
            monsterParties[_MonstersParties] = RemoveDeadCharacter(monsterParties[_MonstersParties]);
        }

        playerTurn = PlayerType.PlayerTwo;
        GameWinner = GameWinnerChecker();
    }
    // Player TWO
    private void UserPlayerTwoAction()
    {
        foreach(Character character in monsterParties[_MonstersParties])
        {
             lock(lockObject)
            {
            Task.Run(() => PlayerChooseAction(character)).Wait();
            CharacterCurrentTurns(character);
            character.Action(character, heroesParty?.FirstOrDefault<Character>());
            heroesParty = RemoveDeadCharacter(heroesParty);
            }
        }

        playerTurn = PlayerType.PlayerOne;
        GameWinner = GameWinnerChecker();
    }
    private List<Character> RemoveDeadCharacter(List<Character> characters)
    {
        IEnumerable<Character> defeatedCharacter = from c in characters  where c.HP == 0 select c;

        foreach(Character player in defeatedCharacter)
        {
            Console.WriteLine($"{player.ToString()} has been defeated!");
        }

        IEnumerable<Character> aliveCharacter = from c in characters where c.HP > 0 select c; 

        return aliveCharacter.ToList<Character>();
    }
    private void PlayerChooseAction(Character character)
    {
        Console.WriteLine(@"Choose Action!!
        1. Skip
        2. Attack
        3. Use Item
        ");
        
        string? input = Console.ReadLine();

        IAction action = input switch
        {
            "1" => new Skip(),
            "2" => new Attack(),
            "3" => new UseItem(),
            _ => new Skip()
        };

        character.SetAction(action);
    }

    /// <summary>
    /// Select if player vs player
    /// Player vs AI
    /// AI vs AI
    /// </summary>
    private void ChoosePlayer()
    {
        Console.WriteLine(@"Select option:
        1. Player vs Player
        2. AI vs AI
        3. Player vs AI");

        string? input = Console.ReadLine();

        (AI, AITWO) = input switch
        {
            "1"     => (false, false),
            "2"    => (true, true),
            _       => (false, true)
        };     
    }
    private void BattleStatus()
    {
        ColoredText("==========================Battle==========================", ConsoleColor.Green);

        foreach(Character heroes in heroesParty)
        ColoredText($"{heroes.ToString()}             ({heroes.HP} / {heroes.MaxHP})", ConsoleColor.Yellow);

        ColoredText($"----------------------------VS----------------------------", ConsoleColor.Green);

        foreach(Character enemy in monsterParties[_MonstersParties])
        ColoredText($"                                          {enemy.ToString()} ({enemy.HP} / {enemy.MaxHP})   ", ConsoleColor.Yellow);

        ColoredText("==========================================================", ConsoleColor.Green);
    }

    private void ColoredText(string txt, ConsoleColor color, bool isAppend = false)
    {
        Console.ForegroundColor = color;

        if(isAppend){Console.Write(txt);} else{Console.WriteLine(txt);}

        Console.ResetColor();
    }
    private void Logger(string txt)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(txt);
        Console.ResetColor();
    }
    private void CharacterCurrentTurns(Character character)=>
    Console.WriteLine($"It is {character.ToString()?.ToUpper()}'s turn...");
    #endregion
}
