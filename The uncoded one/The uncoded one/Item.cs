

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
    }
    public enum ItemType { Heal, Damage, Status }
}
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