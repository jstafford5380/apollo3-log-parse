namespace CataParser.Encounters;

public class BossLog
{
    private List<EventSlice> _bossAttempts = new();

    public string Name { get; }

    public IReadOnlyCollection<EventSlice> Attempts => _bossAttempts;

    public BossLog(Boss boss)
    {
        Name = boss.Name;
    }

    public void RecordAttempt(EventSlice attempt) => _bossAttempts.Add(attempt);
}
