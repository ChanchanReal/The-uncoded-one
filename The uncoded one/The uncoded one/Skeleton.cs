
public class Skeleton : Character
{
    private Random rand = new Random();
    public Skeleton(int hp, AttackType attackType)
    {
        attackType = AttackType.Bone_Crunch;
        MaxHP = hp;
        HP = MaxHP;
    }

    public Skeleton(int hp, AttackType attackType, EquipmentGear equipment)
    {
        attackType = AttackType.Bone_Crunch;
        MaxHP = hp;
        HP = MaxHP;
        CharacterGear = equipment;
    }

    public override int DamageDealt()
    {
        return Damage = rand.Next(2);
    }
    public override string ToString()
    {
        return "SKELETON";
    }
}
