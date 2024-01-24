public class Skip : IAction
{
    public void Start(Character character, Character target)
    {
        Console.WriteLine($"{character.ToString()} did NOTHING");
    }
}

public class Attack : IAction
{
  public void Start(Character character, Character target)
  {
    if(target.HP != 0)
    target.HP -= character.DamageDealt();

    string attackName = character.attackType.ToString().Replace("_", " ");

     Console.WriteLine($"{character.ToString()} used {attackName} on {target.ToString()} "+ "\n" +
     $"{character.attackType} dealt {character.Damage} damage to {target.ToString()} \n" +
     $"{target.ToString()} is now at {target.HP} / { target.MaxHP}");

     Console.WriteLine();
  }   
}

public class UseItem : IAction
{
  public void Start(Character character, Character target)
  {
    int num = PromptUser(character);

    for(int i = 0; i < character.inventory?.items.Count; i++)
    {
        if(num == i)
        {
          character.inventory.items[i].Use(character);
          character.inventory.items.RemoveAt(num);
          UpdateMainInventory(character, num);
          break;
        }
    }
  }
  private int PromptUser(Character character)
  {
    Console.BackgroundColor = ConsoleColor.Cyan;
    Console.ForegroundColor = ConsoleColor.Gray;
    character.ShowItem();
    Console.ResetColor();

    Console.WriteLine("Choose which item you want to use");
    string? input = Console.ReadLine()?.Trim();
    bool result = int.TryParse(input, out int num);

    if(result)
    {
      return num;
    }

    return 0;
  }

  private void UpdateMainInventory( Character character, int itemPosition)
  {
    character.coreGame?.heroesInventory?.items.RemoveAt(itemPosition);
  }
}
public enum AttackType {Punch, Bone_Crunch, Unraveling_Attack}

