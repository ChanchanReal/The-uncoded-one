// this is do nothing action basically nothing! 
public class Skip : IAction
{
    public void Start(Character character, Character target)
    {
        Console.WriteLine($"{character.ToString()} did NOTHING");
    }
}
// attack the target and display what kind of attack is used based on the property of the character
public class Attack : IAction
{
  public void Start(Character character, Character target)
  {
    if(target.HP != 0)
    target.HP -= character.DamageDealt();

    string attackName = character._AttackType.ToString().Replace("_", " ");

     Console.WriteLine($"{character.ToString()} used {attackName} on {target.ToString()} "+ "\n" +
     $"{character._AttackType} dealt {character.Damage} damage to {target.ToString()} \n" +
     $"{target.ToString()} is now at {target.HP} / { target.MaxHP} \n");

     Console.WriteLine();
  }   
}
// use item from the inventory
public class UseItem : IAction
{
  public void Start(Character character, Character target)
  {
    if(character._Inventory.items.Count == 0)
    {
      Console.WriteLine("There is no item in inventory");
      return;
    }

    int num = PromptUser(character);

    for(int i = 0; i < character._Inventory?.items.Count; i++)
    {
        if(num == i)
        {
          character._Inventory.items[i].Use(character);
          character._Inventory.items.RemoveAt(num);
          UpdateMainInventory(character, num);
          break;
        }
    }
  }
  private int PromptUser(Character character)
  {
    Console.BackgroundColor = ConsoleColor.Black;
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
    // updates the main inventory to make sure the item is removed from the main collection
  private void UpdateMainInventory( Character character, int itemPosition)
  {
    character._CoreGame?.heroesInventory?.items.RemoveAt(itemPosition);
  }
}
// use character equipment action.
public class UseEquipment : IAction
{
    public void Start(Character character, Character target)
    {
        if (character is not null) 
        {
            if (character.CharacterGear == EquipmentGear.Nothing)
            {
                Console.WriteLine("There is no gear equipped");
            }

            if (character.CharacterGear == EquipmentGear.Sword)
            {
                target.HP -= 2;
                Console.WriteLine("Slashed the target and deal 2 damage.");
            }

            if (character.CharacterGear == EquipmentGear.Dagger)
            {
                target.HP -= 1;
                Console.WriteLine($"{character.ToString()} stabbed {target.ToString()} and deal 1 damage," + "\n" +
                $"{target.ToString()} is now at ({target.HP } / {target.MaxHP})");
            }
        }
    }
}
public enum AttackType {Punch, Bone_Crunch, Unraveling_Attack}

