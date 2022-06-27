using CataParser.Constants;
using CataParser.Ext;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace CataParser.Events;

/// <summary>
/// All logs begin with this structure which describes what happened, who did it, who they did it
/// to, and flags for both of those parties. The "Effect" is different depending on the event.
/// </summary>
public class LogEventBase : IDisposable
{
    private CsvReader _csv;
    private bool _disposedValue;

    public long Index { get; private set; }

    public DateTime Timestamp { get; private set; }

    public string EventName { get; private set; }

    public ulong SourceId { get; private set; }

    public string SourceName { get; private set; }

    public UnitFlags SourceFlags1 { get; private set; }

    public UnitFlags SourceFlags2 { get; private set; }

    public ulong DestinationId { get; private set; }

    public string DestinationName { get; private set; }

    public UnitFlags DestinationFlags1 { get; private set; }

    public UnitFlags DestinationFlags2 { get; private set; }

    public Effect Effect { get; private set; }

    private LogEventBase() { }

    public static LogEventBase Parse(string logEntry, long index)
    {
        var entry = new LogEventBase { Index = index };

        var parts = logEntry.Split("  ");
        entry.Timestamp = DateTime.ParseExact(parts[0], "M/dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(parts[1]));
        using var sr = new StreamReader(ms);
        entry._csv = new CsvReader(sr, new CsvConfiguration(CultureInfo.InvariantCulture));
        entry._csv.Read();

        entry.EventName = entry._csv.GetField(0);
        entry.SourceId = Convert.ToUInt64(entry._csv.GetField(1), 16);
        entry.SourceName = entry._csv.GetField(2);

        // TODO: model these
        entry.SourceFlags1 = entry._csv.GetField(3).ToUnitFlags();
        entry.SourceFlags2 = entry._csv.GetField(4).ToUnitFlags();

        entry.DestinationId = Convert.ToUInt64(entry._csv.GetField(5), 16);
        entry.DestinationName = entry._csv.GetField(6) == "nil" ? "Undefined" : entry._csv.GetField(6);

        // TODO: model these
        entry.DestinationFlags1 = entry._csv.GetField(7).ToUnitFlags();
        entry.DestinationFlags2 = entry._csv.GetField(8).ToUnitFlags();

        entry.Effect = new Effect(entry.EventName, entry._csv, entry);

        return entry;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _csv.Dispose();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}