using System.Text;

namespace CataParser.Calculators.Damage;

internal class DpsCalculator
{
    private Dictionary<long, DamageDealtTable> _unitPartitions = new();

    public void AddDamage(LogEventBase e)
    {
        if (e.Effect.Result != EventSuffix.Damage)
            throw new ArgumentException($"{e.EventName} is not a damage event!");

        var damageTable = FindUnitTable(e);
        damageTable.UpdateDamage(e);
    }

    /// <summary>
    /// Use this to associate a summoned unit to its summoner so that any damage
    /// will be credited to the summoner.
    /// </summary>
    /// <param name="e"></param>
    /// <exception cref="ArgumentException"></exception>
    public void RegisterMinion(LogEventBase e)
    {
        if (e.Effect.Result != EventSuffix.Summon)
            throw new ArgumentException($"{e.EventName} is not a summon!");

        var table = FindUnitTable(e);
        table.Summon(e.DestinationId, e.DestinationName!);
    }

    private void EnsureTable(LogEventBase e)
    {
        if (!_unitPartitions.ContainsKey(e.SourceId))
            _unitPartitions.Add(e.SourceId, new DamageDealtTable(e));
    }

    private DamageDealtTable FindUnitTable(LogEventBase e)
    {
        var summoner = FindSummoner(e.SourceId);

        // if this isn't null, that means this source is a minion so return
        // its summoner's damage table.        
        if (summoner != null)
            return summoner;

        // if it doesn't exist, then it wasn't registered, so treat it
        // like any other unit just for tracking purposes so maybe we
        // can clean it up at the end

        EnsureTable(e);

        // otherwise just find the table
        return _unitPartitions[e.SourceId];
    }

    private DamageDealtTable? FindSummoner(long unitId)
    {
        var match = _unitPartitions
            .SingleOrDefault(p => p.Value != null && p.Value.OwnsMinion(unitId));

        return match.Value;
    }

    /// <summary>
    /// Clean up any orphaned minion damage by reassociating to its summoner.
    /// This accounts for occasions where a summoned unit does damage before
    /// the summoner's damage table was created. (i.e. when the player summons
    /// a unit before engaging a target like popping wolves while running in).
    /// </summary>
    public void ReconcileMinionDamage()
    {
        foreach (var unit in _unitPartitions)
        {
            var summoner = FindSummoner(unit.Key);
            if (summoner == null)
                continue;

            summoner.Merge(unit.Value);
            _unitPartitions.Remove(unit.Key);
        }
    }

    public string ReportPlayerDamage()
    {
        var sb = new StringBuilder();
        foreach (var unit in _unitPartitions)
        {
            if (!unit.Value.IsPlayer)
                continue;

            var row = new[]
            {
                unit.Value.UnitName,
                unit.Value.TotalDamage.ToString(),
                unit.Value.Dps.ToString()
            };

            sb.AppendLine(string.Join(',', row));
        }

        return sb.ToString();
    }

    public void CalculateDps()
    {
        foreach (var entry in _unitPartitions.Where(u => u.Value.DamageEvents.Any()))
            entry.Value.CalculateDps();
    }
}
