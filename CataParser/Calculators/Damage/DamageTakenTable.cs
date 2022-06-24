using CataParser.Constants;

namespace CataParser.Calculators.Damage;

public class DamageTakenTable
{
    private List<DamageTakenTableEntry> _entries = new();
    
    public string UnitName { get; private set; }

    public bool IsPlayer { get; private set; }

    public IReadOnlyCollection<DamageTakenTableEntry> Events => _entries;

    public DamageTakenTable(LogEventBase e)
    {
        UnitName = e.DestinationName!;
        IsPlayer = e.DestinationFlags1.HasFlag(UnitFlags.TYPE_PLAYER | UnitFlags.CONTROL_PLAYER);
    }

    public void AddEntry(LogEventBase e)
    {
        var damage = e.Effect.GetDamageEffect();
        
        // not yet defined
        if (damage == null)
            return;

        var entry = new DamageTakenTableEntry(damage);
        _entries.Add(entry);
    }

    public Dictionary<string, int> ReportDamageBySource()
    {
        var report = new Dictionary<string, int>();
        foreach(var group in _entries.GroupBy(e => e.SpellId))
        {
            var spellName = group.First().SpellName;
            if (!report.ContainsKey(spellName))
                report.Add(spellName, 0);

            report[spellName] += group.Sum(g => g.DamageAmount);
        }

        return report
            .OrderByDescending(r => r.Value)
            .ToDictionary(k => k.Key, v => v.Value);
    }

    public int ReportDamageBySpellName(string spellName)
    {
        var entries = _entries.Where(e => e.SpellName == spellName);
        return entries.Sum(e => e.DamageAmount);
    }
}
