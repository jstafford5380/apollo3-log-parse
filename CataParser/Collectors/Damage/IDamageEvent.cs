namespace CataParser.Collectors.Damage;

public interface IDamageEvent
{
    DateTime Timestamp { get; }

    int Amount { get; }

    int SpellId { get; }

    string SpellName { get; }

    ulong SourceId { get; }

    string SourceName { get; }
    
    ulong TargetId { get; }

    string TargetName { get; } 

    bool SourceIsPlayer { get; }

    bool TargetIsPlayer { get; }
}
