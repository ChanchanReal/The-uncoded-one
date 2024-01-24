
public class Player : Character
{
    public string Name { get; protected set; } = "null";

    public Player(string name, AttackType attackType, int maxHP, Inventory inventoryItem, CoreGame coreGame, int damage = 1)
    {
        Name = name;
        this.attackType = attackType;
        MaxHP = maxHP;
        HP = MaxHP;
        Damage = damage;
        this.inventory = inventoryItem;
    }

    public override string ToString()
    {
        return Name.ToUpper();
    }
}
