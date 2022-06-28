using CataParser.Constants;
using CataParser.Events;

namespace CataParser.Collectors.Damage;

public class DamageTable : IDamageTable
{
    private List<IDamageEvent> _damageEvents = new();

    public IReadOnlyCollection<IDamageEvent> DamageEvents => _damageEvents;

    public ulong SourceId { get; private set; }

    public string SourceName { get; private set; }

    public int DamageDealt { get; private set; }

    public double Dps { get; private set; }

    public bool IsPlayer { get; private set; }

    public Dictionary<ulong, string> Summoned { get; } = new();

    public DamageTable(LogEventBase e)
    {
        SourceId = e.SourceId;
        SourceName = e.SourceName;
        IsPlayer = e.SourceFlags1.HasFlag(UnitFlags.TYPE_PLAYER | UnitFlags.CONTROL_PLAYER);
    }

    public void Log(LogEventBase e)
    {
        var damageInfo = e.Effect.GetDamageEffect();

        // if its null, its because that particular damage type is not
        // yet implemented
        if (damageInfo == null)
            return;        

        DamageDealt += damageInfo.Amount;
        _damageEvents.Add(new DamageDealtEntry(e));
    }

    public void Summon(ulong unitId, string name)
    {
        if (!Summoned.ContainsKey(unitId))
            Summoned.Add(unitId, name);
    }

    public void Merge(DamageTable table)
    {
        DamageDealt += table.DamageDealt;
        // TODO: update dps
    }

    public void CalculateDps()
    {
        var interval = TimeSpan.FromSeconds(1);
        Dps = _damageEvents
            .GroupBy(e => e.Timestamp.Ticks / interval.Ticks)
            .Average(g => g.Sum(e => e.Amount));

        Dps = Math.Round(Dps, 2);
    }

    public bool OwnsMinion(ulong unitId) => Summoned.ContainsKey(unitId);
}
