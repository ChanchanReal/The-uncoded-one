
// Item class we want this as a model for our item.
public class Item
{
    public ItemType usableItemType {get; set;}
    public int Damage {get; set;}
    public void Use(Character target)
    {
        if(usableItemType == ItemType.Heal)
        {
            target.HP += Damage;
            if(target.HP > target.MaxHP)
            target.HP = target.MaxHP;
        }

        if(usableItemType == ItemType.EquipAble)
        {
            target.EquipGear(this);
        }
    }
    public enum ItemType { Heal, Damage, EquipAble }
}
// this sword is an equipable means we can equip and use it
public class Sword : Item
{
    public Sword()
    {
        usableItemType = ItemType.EquipAble;
    }
}

public class Dagger : Item
{
    public Dagger()
    {
        usableItemType =ItemType.EquipAble;
    }
}
// heals the character who use it.
public class HealthPotion : Item
{
    public HealthPotion()
    {
        usableItemType = ItemType.Heal;
        Damage = 10;
    }

    public override string ToString()
    {
        return "Health Potion";
    }
}