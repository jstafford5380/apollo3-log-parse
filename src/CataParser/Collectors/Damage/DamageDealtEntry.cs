using CataParser.Events;

namespace CataParser.Collectors.Damage;

public class DamageDealtEntry : DamageEntry
{
    public ulong SourceId { get; set; }

    public string SourceName { get; set; }

    public ulong TargetId { get; private set; }

    public string TargetName { get; private set; }

    public DamageDealtEntry(LogEventBase e) : base(e)
    {
        SourceId = e.SourceId;
        SourceName = e.SourceName;
        TargetId = e.DestinationId;
        TargetName = e.DestinationName;
    }

    public override string ToString() => $"{SourceName} dealt {Amount} damage to {TargetName}";
}
