using CataParser.Encounters;
using CataParser.Events;

namespace CataParser.Collectors;

internal class ReportBuilder
{
    private IReadOnlyDictionary<string, BossLog> _fightLog = new Dictionary<string, BossLog>();
    private Dictionary<string, BossLogReport> _reports = new();
    private readonly List<Type> _collectors = new();
    

    public IReadOnlyDictionary<string, BossLogReport> Reports => _reports;

    private ReportBuilder() { }

    public static ReportBuilder FromEncounterParse(EncounterParser parser)
    {
        var builder = new ReportBuilder
        {
            _fightLog = parser.BossLog
        };

        return builder;
    }

    public ReportBuilder Use<T>() where T : IParseCollector
    {
        var asType = typeof(T);
        if (_collectors.Contains(asType))
            return this;

        _collectors.Add(asType);
        return this;
    }

    public void Build()
    {
        var reports = new Dictionary<string, BossLogReport>();
        foreach(var (name, log) in _fightLog)
        {
            var logReport = new BossLogReport(name);

            foreach(var slice in log.Attempts)   
            {
                if (!reports.ContainsKey(logReport.Name))
                    reports.Add(logReport.Name, logReport);

                var collectors = CreateCollectors();
                RunCollectors(logReport, collectors, slice);
                                
            }
        }

        _reports = reports;
    }

    private static void RunCollectors(
        BossLogReport report,
        IEnumerable<IParseCollector> collectors, 
        EventSlice attemptSlice)
    {
        foreach (var e in attemptSlice.Events)
        {            
            foreach(var collector in collectors)
            {
                collector.Collect(e);                
            }            
        }

        foreach (var collector in collectors)
            collector.Complete();

        report.RecordAttempt(collectors);
    }

    private List<IParseCollector> CreateCollectors()
    {
        var collectors = new List<IParseCollector>();
        foreach(var collector in _collectors)
        {
            var instance = (IParseCollector) Activator.CreateInstance(collector)!;
            collectors.Add(instance);
        }

        return collectors;
    }
}
