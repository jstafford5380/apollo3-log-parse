namespace CataParser.Collectors.Damage;

public interface IDamageTable
{
    string SourceName { get; }

    int DamageDealt { get; }

    bool IsPlayer { get; }

    IReadOnlyCollection<IDamageEvent> DamageEvents { get; }
}
