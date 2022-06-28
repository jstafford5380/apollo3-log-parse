using CataParser.Events;

namespace CataParser.Encounters;

public class EncounterParser
{
    private readonly TimeSpan _maxDowntime;
    private Dictionary<string, BossLog> _bossLog = new();

    public IReadOnlyDictionary<string, BossLog> BossLog => _bossLog;

    public EncounterParser(TimeSpan maxDowntime)
    {
        _maxDowntime = maxDowntime;
    }

    public void ParseBossAttempts(LogParser parser)
    {
        Dictionary<string, BossLog> encounters = new();

        while (parser.Next(out var e))
        {
            if (!IsBossEncounter(e.DestinationName, out var boss))
                continue;

            var encounter = StartEncounter(boss!, encounters);

            var lastEvent = e;
            var currentEvent = e;

            while (currentEvent != null && EncounterInProgress(encounter, currentEvent, lastEvent.Timestamp))
            {
                lastEvent = currentEvent;
                parser.Next(out currentEvent);
            }
        }

        _bossLog = encounters;
    }

    private bool EncounterInProgress(EventSlice encounter, LogEventBase e, DateTime lastEvent)
    {
        // this allows us to exclude events that take place during reset
        bool EncounterEnded(DateTime current, DateTime last) => current - last >= _maxDowntime;

        if (EncounterEnded(e.Timestamp, lastEvent))
            return false;

        encounter.AddEvent(e);
        return true;
    }

    private static EventSlice StartEncounter(Boss boss, Dictionary<string, BossLog> encounters)
    {
        if (!encounters.ContainsKey(boss.Name))
            encounters.Add(boss.Name, new BossLog(boss));

        var attempt = new EventSlice(boss.Name);
        encounters[boss.Name].RecordAttempt(attempt);

        return attempt;
    }

    private static bool IsBossEncounter(string targetName, out Boss? boss)
    {
        boss = BossTable.Find(targetName);
        return boss != null;
    }
}