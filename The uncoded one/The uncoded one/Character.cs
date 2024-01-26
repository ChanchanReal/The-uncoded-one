
public abstract class Character
{
    public IAction? _Actions;

    public AttackType _AttackType;
    public int HP { get; set; } = 1;
    public int MaxHP { get; set; } = 1;
    public int Damage { get; set; }

    public EquipmentGear CharacterGear { get; set; } = EquipmentGear.Nothing;
    public Inventory? _Inventory;
    public CoreGame _CoreGame;

    public void SetAction(IAction action)
    {
        if(action is not null)
        this._Actions = action;
    }
    public virtual int DamageDealt()
    {
        return Damage;
    }
    public void Action(Character character, Character target)
    {
        _Actions?.Start(character, target);
    }
    public void ShowItem()
    {
        Console.WriteLine(_Inventory?.ToString());
    }

    public void EquipGear(Item equipment)
    {
        if(CharacterGear != null)
        {
            Console.WriteLine($"{CharacterGear.ToString()} is been Unequipped. ");
            
            if(equipment.ToString() == "Sword") 
            {
                CharacterGear = EquipmentGear.Sword;
            }

            if (equipment.ToString() == "Dagger")
            {
                CharacterGear = EquipmentGear.Dagger;
            }
            Console.WriteLine($"{equipment.ToString()} is been EQUIPPED \n");
        }
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

public enum EquipmentGear { Sword , Dagger, Nothing}
