namespace CataParser.Collectors;

public class BossLogReport
{
    private int _attemptIndex;
    private readonly Dictionary<int, ParseCollection> _attempts = new();

    public string Name { get; }

    public int AttemptNumber => _attemptIndex;

    public IReadOnlyDictionary<int, ParseCollection> Attempts => _attempts;
    
    public BossLogReport(string name)
    {
        Name = name;
    }
    
    public void RecordAttempt(IEnumerable<IParseCollector> collectors)
    {
        _attemptIndex++;
        _attempts.Add(_attemptIndex, new ParseCollection(collectors));
    }

    public T? GetCollector<T>(int attemptNumber) where T : class, IParseCollector
    {
        if (!_attempts.ContainsKey(attemptNumber))
            return null;

        var collector = _attempts[attemptNumber].SingleOrDefault(a => a.GetType() == typeof(T)) as T;
        return collector;
    } 

    public override string ToString() => $"{Name} attempt {_attemptIndex}";
}