
using System.Text;

public class CoreGame
{
    #region Fields And Properties
    private Random random = new Random();
    public List<Character> heroesParty;
    public Inventory heroesInventory;
    public List<List<Character>> monsterParties;
    public List<Inventory> monsterInventories;
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
            new Skeleton(5, AttackType.Bone_Crunch, EquipmentGear.Dagger)
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
        heroesInventory.items.Add(new Sword());

        Inventory monsterInventory = new Inventory();
        monsterInventory.items.Add(new HealthPotion());

        Inventory monsterInventoryTwo = new Inventory();
        monsterInventoryTwo.items.Add(new HealthPotion());
        monsterInventoryTwo.items.Add(new Dagger());
        monsterInventoryTwo.items.Add(new Dagger());

        Inventory finalBossInventory = new Inventory();
        finalBossInventory.items.Add(new HealthPotion());

        monsterInventories = new List<Inventory>
        {
            monsterInventory,
            monsterInventoryTwo,
            
        };

    }
    #endregion

    #region Methods
    // Runs the game this is what we use.
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
    // create user player.
    private Player CreateMainPlayer()
    {
        Console.WriteLine("Enter your character's name: ");
        string name = Console.ReadLine();

        if(name != null)
        {
            return new Player(name, AttackType.Punch, 25, heroesInventory, this);
        }
        else
        {
            return new Player("TOG", AttackType.Punch, 25, heroesInventory, this);
        }
        
    }
    // check's game status.
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
            {
                monsterParties.RemoveAt(0);
                if(monsterInventories.Count != 0)
                {
                    GetDeadPartyItems(monsterInventories[0], heroesInventory);
                    monsterInventories.RemoveAt(0);
                }
                
            }

            if(monsterParties.Count == 0)
            {
                Console.WriteLine("All Enemy has been ELIMINATED!!!");

                return true;
            }
        }

        return false;
    }

    // this run where if player one turns or player two turn.
    private void Battle()
    {
        BattleStatus();
        // ai player
        InitiateBattle();
        Console.WriteLine();
        Thread.Sleep(2000);
        Console.Clear();
    }
    public void InitiateBattle()
    {
        var conditionsTuple = (AITWO, AI, playerTurn);

        switch (conditionsTuple)
        {
            case var t when t.Item1 && t.Item3 == PlayerType.PlayerTwo:
                AIaction();
                Thread.Sleep(1000);
                break;
            case var t when !t.Item1 && t.Item3 == PlayerType.PlayerTwo:
                UserPlayerTwoAction();
                break;
            case var t when t.Item2 && t.Item3 == PlayerType.PlayerOne:
                AISecondAction();
                Thread.Sleep(1000);
                break;
            case var t when !t.Item2 && t.Item3 == PlayerType.PlayerOne:
                UserPlayerOneAction();
                break;
        }
    }
    private void AIaction()
    {
        foreach (Character character in monsterParties[_MonstersParties])
        {
            CharacterCurrentTurns(character);
            IAction action = RandomChoiceAI(character);
            character.SetAction(action);
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
            PlayerChooseAction(character);
            CharacterCurrentTurns(character);
            character.Action(character, monsterParties?.FirstOrDefault<List<Character>>()?.FirstOrDefault<Character>());
            monsterParties[_MonstersParties] = RemoveDeadCharacter(monsterParties[_MonstersParties]);
        }

        playerTurn = PlayerType.PlayerTwo;
        GameWinner = GameWinnerChecker();
    }
    // AI TWO
    private void AISecondAction()
    {
        foreach(Character character in heroesParty)
        {
            CharacterCurrentTurns(character);
            IAction action = RandomChoiceAI(character);
            character.SetAction(action);
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
            PlayerChooseAction(character);
            CharacterCurrentTurns(character);
            character.Action(character, heroesParty?.FirstOrDefault<Character>());
            heroesParty = RemoveDeadCharacter(heroesParty);
        }

        playerTurn = PlayerType.PlayerOne;
        GameWinner = GameWinnerChecker();
    }
    /// <summary>
    /// Get all items on the dead party and transfer it to the heroes
    /// all Equip items also must be transferred.
    /// </summary>
    public void GetDeadPartyItems(Inventory sourceInventory, Inventory recievingTarget)
    {
        foreach (Item item in sourceInventory.items)
        {
            ColoredText($"{item.ToString()} is added to inventory the winning Party", ConsoleColor.Magenta);
            recievingTarget.items.Add(item);
        }
    }

    public void GetDeadCharacterEquipment(Character deadCharacter, Inventory recievingInventory)
    {
        if(deadCharacter.CharacterGear == EquipmentGear.Nothing)
            return;

        ColoredText($"{deadCharacter.ToString()} Dropped {deadCharacter.CharacterGear}", ConsoleColor.DarkMagenta);

        EquipmentGear deadCharacterGear = deadCharacter.CharacterGear;

        Item item = deadCharacterGear switch
        {
            EquipmentGear.Sword  =>  new Sword(),
            EquipmentGear.Dagger => new Dagger()
        };

        recievingInventory.items.Add(item);

    }
    // check's all hp of heroes and removed it from the list.
    private List<Character> RemoveDeadCharacter(List<Character> characters)
    {
        IEnumerable<Character> defeatedCharacter = from c in characters  where c.HP == 0 select c;

        foreach(Character player in defeatedCharacter)
        {
            Console.WriteLine($"{player.ToString()} has been defeated!");
            GetDeadCharacterEquipment(player, heroesInventory);
            Thread.Sleep(1500);
        }

        IEnumerable<Character> aliveCharacter = from c in characters where c.HP > 0 select c; 

        return aliveCharacter.ToList<Character>();
    }
    // choose what action does a player want.
    private void PlayerChooseAction(Character character)
    {
        Console.WriteLine(@"Choose Action!!
        1. Skip
        2. Attack
        3. Use Item
        4. Use Equipment
        ");
        
        string? input = Console.ReadLine();

        IAction action = input switch
        {
            "1" => new Skip(),
            "2" => new Attack(),
            "3" => new UseItem(),
            "4" => new UseEquipment(),
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
    public Inventory GetInventoryAI()
    {
        return monsterInventories.FirstOrDefault();
    }
    public IAction RandomChoiceAI(Character character)
    {
        Inventory inventoryAI = GetInventoryAI();

        if(random.NextDouble() <= 1.0) // 50%?
        {
            if(character.HP  == (int)(25.0 / 100 * character.MaxHP))
            {
                if (inventoryAI is not null)
                    foreach (Item item in inventoryAI.items)
                {
                    if(item is HealthPotion)
                    {
                        item.Use(character);
                        inventoryAI.items.Remove(item);
                        Console.WriteLine($"monster used {item.ToString()} and skip the turn");
                        return new Skip();
                    }
                }
            }
        }

        if(character.CharacterGear == EquipmentGear.Nothing)
        {
            if(inventoryAI is not null)
            foreach(Item item in inventoryAI.items)
            {
                if(item is Dagger)
                {
                    item.Use(character);
                    inventoryAI.items.Remove(item);
                    Console.WriteLine($"{character.ToString()} equipped {item.ToString()} and skipped the turn");
                    return new Skip();
                }
            }
        }
        
        if(character.CharacterGear != EquipmentGear.Nothing)
        if(random.NextDouble() < 0.25)
        {
            return new UseEquipment();
        }
        
        return new Attack();
    }
    // show's the status of every monster and make it look good.
    private void BattleStatus()
    {
        ColoredText("==========================Battle==========================", ConsoleColor.Green);

        foreach(Character heroes in heroesParty)
        {
            ColoredText($"{heroes.ToString()}          ", ConsoleColor.Cyan, true);
            CharacterHPBar(heroes);
            ColoredText($"       Gear: {heroes.CharacterGear}", ConsoleColor.White);
        }

        ColoredText($"----------------------------VS----------------------------", ConsoleColor.Green);

        foreach(Character enemy in monsterParties[_MonstersParties])
        {
            ColoredText($"        Gear: {enemy.CharacterGear}     ", ConsoleColor.White, true);
            CharacterHPBar(enemy);
            ColoredText($" {enemy.ToString()}" , ConsoleColor.DarkYellow);
        }
        
        ColoredText("==========================================================", ConsoleColor.Green);
    }
    private void CharacterHPBar(Character character)
    {
        if (character.MaxHP == 0)
        {
            return;
        }

        int bar = (int)(((float)character.HP / character.MaxHP) * 100);

        int filledBlocks = (int)Math.Round((float)bar / 10);

        StringBuilder HpBarBuilder = new StringBuilder("[");

        for (int index = 0; index < filledBlocks; index++)
        {
            HpBarBuilder.Append("█");
        }

        int hpSpace = Math.Abs(filledBlocks - 10);

        HpBarBuilder.Append($"{bar}%");

        for(int j = 0; j < hpSpace; j++ )
        {
            HpBarBuilder.Append("_");
        }
        HpBarBuilder.Append("]");

        ConsoleColor hpBarColor = PercentageColor(bar);

        ColoredText(HpBarBuilder.ToString(), hpBarColor, true);
    }

public ConsoleColor PercentageColor(int hpPercentage)
{
    if(hpPercentage > 80)
    return ConsoleColor.DarkBlue;

    if(hpPercentage > 50)
    return ConsoleColor.DarkGreen;

    if(hpPercentage > 25)
    return ConsoleColor.DarkYellow;

    else return ConsoleColor.DarkRed;
}
    /// <summary>
    /// colored the text
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="color"></param>
    /// <param name="isAppend"></param>
    private void ColoredText(string txt, ConsoleColor color, bool isAppend = false)
    {
        Console.ForegroundColor = color;

        if(isAppend){Console.Write(txt);} else{Console.WriteLine(txt);}

        Console.ResetColor();
    }

    /// <summary>
    /// instead of writing the console.writeline i use this to see the output.
    /// </summary>
    /// <param name="txt"></param>
    private void Logger(string txt)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(txt);
        Console.ResetColor();
    }
    /// <summary>
    /// this return who's turn is it
    /// </summary>
    /// <param name="character"></param>
    private void CharacterCurrentTurns(Character character)=>
    Console.WriteLine($"It is {character.ToString()?.ToUpper()}'s turn...");
    #endregion
}
