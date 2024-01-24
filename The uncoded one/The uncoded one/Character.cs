
public abstract class Character
{
    public IAction? actions;

    public AttackType attackType;
    public  int HP {get; set;} = 1;
    public int MaxHP {get; set;} = 1;
    public int Damage {get; set;}
    public Inventory? inventory;
    public CoreGame coreGame;

    public void SetAction(IAction action)
    {
        if(action is not null)
        this.actions = action;
    }
    public virtual int DamageDealt()
    {
        return Damage;
    }
    public void Action(Character character, Character target)
    {
        actions?.Start(character, target);
    }
    public void ShowItem()
    {
        Console.WriteLine(inventory?.ToString());
    }
    
}

public class Inventory
{
    public List<Item> items = new List<Item>();

    public override string ToString()
    {
        string itemString = "Item: ";
        int index = 0;
        foreach(Item item in items)
        {
            itemString += $"[{index++}] {item.ToString() }";
        }

        return itemString;
    }
}

