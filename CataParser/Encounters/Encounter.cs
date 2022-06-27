using CataParser.Events;

namespace CataParser.Encounters;

public class EventSlice
{
    private List<LogEventBase> _events = new();

    public string Name { get; }

    public DateTime Start => _events.First().Timestamp;

    public DateTime End => _events.Last().Timestamp;

    public IReadOnlyCollection<LogEventBase> Events => _events;

    public EventSlice(string name)
    {
        Name = name;
    }

    public void AddEvent(LogEventBase e) => _events.Add(e);    
}
