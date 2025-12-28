namespace StaticSiege.Cards;

public sealed class CardInstance
{
    public CardDef Def { get; }
    public string InstanceId { get; } = System.Guid.NewGuid().ToString("N");

    public CardInstance(CardDef def)
    {
        Def = def;
    }
}

