namespace CataParser.Encounters;

public static class BossTable
{
    private static Dictionary<ulong, Boss> _bosses = new();

    public static IReadOnlyDictionary<ulong, Boss> Bosses => _bosses;

    static BossTable()
    {
        _bosses.Add(0xF130CCB900000001, new Boss { Id = 0xF130CCB900000001, Name = "Ragnaros", TotalHealth = 0 });
    }

    public static Boss? Find(ulong id)
    {
        return _bosses.ContainsKey(id)
            ? _bosses[id]
            : null;
    }

    public static Boss? Find(string name)
    {
        var boss = _bosses.Values.SingleOrDefault(b => b.Name.Equals(name));
        return boss;
    }
}
