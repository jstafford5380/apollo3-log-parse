using CataParser.Events;
using System.Text;

namespace CataParser.Collectors.Damage;

internal class DamageCollector : IParseCollector<ulong, DamageTable>
{
    private Dictionary<ulong, DamageTable> _allDamageSources = new();

    public IReadOnlyDictionary<ulong, DamageTable> Results => _allDamageSources;

    public IReadOnlyDictionary<ulong, DamageTable> PlayerDamage => _allDamageSources
        .Where(v => v.Value.IsPlayer)
        .ToDictionary(k => k.Key, v => v.Value);

    public IReadOnlyDictionary<string, IDictionary<string, int>> PlayerDamageTaken => 
        BuildPlayerDamageTaken().ToDictionary(k => k.Key, v => v.Value);

    public void Collect(LogEventBase e)
    {
        if (e.Effect.Result == EventSuffix.Summon)
        {
            RegisterMinion(e);
            return;
        }

        if (e.Effect.Result != EventSuffix.Damage)
            return;

        var damageTable = FindUnitTable(e);
        damageTable.Log(e);
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
        if (!_allDamageSources.ContainsKey(e.SourceId))
            _allDamageSources.Add(e.SourceId, new DamageTable(e));
    }

    private DamageTable FindUnitTable(LogEventBase e)
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
        return _allDamageSources[e.SourceId];
    }

    private DamageTable? FindSummoner(ulong unitId)
    {
        var match = _allDamageSources
            .SingleOrDefault(p => p.Value != null && p.Value.OwnsMinion(unitId));

        return match.Value;
    }

    /// <summary>
    /// Clean up any orphaned minion damage by reassociating to its summoner.
    /// This accounts for occasions where a summoned unit does damage before
    /// the summoner's damage table was created. (i.e. when the player summons
    /// a unit before engaging a target like popping wolves while running in).
    /// </summary>
    private void ReconcileMinionDamage()
    {
        foreach (var unit in _allDamageSources)
        {
            var summoner = FindSummoner(unit.Key);
            if (summoner == null)
                continue;

            summoner.Merge(unit.Value);
            _allDamageSources.Remove(unit.Key);
        }
    }

    public void Complete()
    {
        ReconcileMinionDamage();
        foreach (var entry in _allDamageSources.Where(u => u.Value.DamageEvents.Any()))
            entry.Value.CalculateDps();
    }

    private IDictionary<string, IDictionary<string, int>> BuildPlayerDamageTaken()
    {
        var damageReceivedByUnit = new Dictionary<string, IDictionary<string, int>>();

        var playersThatTookDamage = _allDamageSources
            .Values
            .Where(s => !s.IsPlayer)
            .SelectMany(s => s.DamageEvents)
            .Where(d => d.TargetIsPlayer)
            .GroupBy(d => d.TargetName);
            
        foreach(var player in playersThatTookDamage)
        {
            var damageSources = player
                .GroupBy(d => d.SpellName)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount))
                .OrderByDescending(k => k.Value)
                .ToDictionary(k => k.Key, v => v.Value);

            damageReceivedByUnit.Add(player.Key, damageSources);
        }

        return damageReceivedByUnit;
    }
}
