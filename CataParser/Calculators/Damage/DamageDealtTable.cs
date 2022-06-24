using CataParser.Constants;

namespace CataParser.Calculators.Damage;

public class DamageDealtTable
{
    public List<(DateTime TimeStamp, int DamageAmount)> DamageEvents { get; private set; } = new();

    public string UnitName { get; private set; }

    public int TotalDamage { get; private set; }

    public double Dps { get; private set; }

    public bool IsPlayer { get; private set; }

    public Dictionary<long, string> Summoned { get; } = new();

    public DamageDealtTable(LogEventBase e)
    {
        UnitName = e.SourceName;
        IsPlayer = e.SourceFlags1.HasFlag(UnitFlags.TYPE_PLAYER | UnitFlags.CONTROL_PLAYER);
    }

    public void UpdateDamage(LogEventBase e)
    {
        var damageInfo = e.Effect.GetDamageEffect();

        // if its null, its because that particular damage type is not
        // yet implemented
        if (damageInfo == null)
            return;

        TotalDamage += damageInfo.Amount;
        DamageEvents.Add((e.Timestamp, damageInfo.Amount));
    }

    public void Summon(long unitId, string name)
    {
        if (!Summoned.ContainsKey(unitId))
            Summoned.Add(unitId, name);
    }

    public void Merge(DamageDealtTable table)
    {
        TotalDamage += table.TotalDamage;
        // TODO: update dps
    }

    public void CalculateDps()
    {
        var interval = TimeSpan.FromSeconds(1);
        Dps = DamageEvents
            .GroupBy(e => e.TimeStamp.Ticks / interval.Ticks)
            .Average(g => g.Sum(e => e.DamageAmount));

        Dps = Math.Round(Dps, 2);
    }

    public bool OwnsMinion(long unitId) => Summoned.ContainsKey(unitId);
}
