
static class Constants
{
    public const int MaxWave = 3;
    public const int MaxKill = 1;
    public const int CountOfBoss = 3;
    public const int MonCountMax = 20;
    public const float AddHealth = 100;
}

public struct MonsterStatus //몬스터 능력치 저장용 구조체 
{
    public int Power { get; private set; }
    public int Hp { get; private set; }
    public int Exp { get; private set; }
    public int Gold { get; private set; }

    public MonsterStatus(int power, int hp, int exp, int gold)
    {
        Power = power;
        Hp = hp;
        Exp = exp;
        Gold = gold;
    }
}

public enum ITEM_SORT
{
    None,
    Sword,
    Hammer,
    Wand,
    Hp,
    Mp
}

public enum SCENE
{
    None,
    Main,
    Prologue,
    Play,
    Shop
}


public enum PLAYER_ATCION
{
    None,
    Attack,
    GetHit,
    Skill
}