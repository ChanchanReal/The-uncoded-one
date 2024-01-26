
public class BossMonster : Character
{
    private Random rand = new Random();
    public BossMonster(int hp, AttackType attackType)
    {
        HP = hp;
        MaxHP = HP;
        this._AttackType = attackType;
    }

    public override int DamageDealt()
    {
        
        return Damage = rand.Next(3);
    }

    public override string ToString()
    {
        return "THE UNCODED ONE";
    }
}