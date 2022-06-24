using System.Text;

namespace CataParser.Calculators.Damage;

internal class DamageTakenCalculator
{
    private Dictionary<long, DamageTakenTable> _unitPartitions = new();

    public void Record(LogEventBase e)
    {
        if (e.Effect.Result != EventSuffix.Damage)
            throw new ArgumentException($"{e.EventName} is not a damage event!");

        var damageTable = FindUnitTable(e);        
        damageTable.AddEntry(e);
    }

    private void EnsureTable(LogEventBase e)
    {
        if (!_unitPartitions.ContainsKey(e.DestinationId))
            _unitPartitions.Add(e.DestinationId, new DamageTakenTable(e));
    }

    private DamageTakenTable FindUnitTable(LogEventBase e)
    {
        EnsureTable(e);
        return _unitPartitions[e.DestinationId];
    }

    public string ReportDamageTaken()
    {
        var sb = new StringBuilder();
        foreach(var unit in _unitPartitions)
        {
            if (!unit.Value.IsPlayer)
                continue;

            sb.AppendLine(unit.Value.UnitName);
            var report = unit.Value.ReportDamageBySource();
            foreach(var item in report)
            {
                var line = $"{item.Key}: {item.Value}";
                sb.AppendLine($"  {line}");
            }
        }

        return sb.ToString();
    }

    public string ReportDamageTakenBySpellName(string spellName)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{spellName} damage:");

        var report = new Dictionary<string, int>();
        foreach (var unit in _unitPartitions)
        {
            if (!unit.Value.IsPlayer)
                continue;
                        
            var spellDamage = unit.Value.ReportDamageBySpellName(spellName);
            report.Add(unit.Value.UnitName, spellDamage);
            //sb.AppendLine($"  {unit.Value.UnitName}: {report:0,0}");
        }

        report = report
            .OrderByDescending(r => r.Value)
            .ToDictionary(k => k.Key, v => v.Value);

        foreach(var line in report)
        {
            sb.AppendLine($"  {line.Key}: {line.Value:#,#}");
        }

        return sb.ToString();
    }
}
