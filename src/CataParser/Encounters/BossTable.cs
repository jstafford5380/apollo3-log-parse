namespace CataParser.Encounters;

public static class BossTable
{
    private static List<Boss> _bosses = new();

    public static IReadOnlyCollection<Boss> Bosses => _bosses;

    static BossTable()
    {        
    }

    public static void Load(IEnumerable<Boss> bosses)
    {
        _bosses = bosses.ToList();
    }

    public static Boss? Find(string name)
    {
        var boss = _bosses.SingleOrDefault(b => b.Name.Equals(name));
        return boss;
    }
}
