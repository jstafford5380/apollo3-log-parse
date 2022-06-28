namespace CataParser.Events;

public class LogParser : IDisposable
{
    private long LineNumber = 0;
    private StreamReader _stream;
    private bool _disposedValue;

    public LogParser(string logPath)
    {
        if (!File.Exists(logPath))
            throw new FileNotFoundException("Combat log not found", logPath);

        _stream = File.OpenText(logPath);
    }

    public bool Next(out LogEventBase logEvent)
    {
        logEvent = null;
        if (_stream.EndOfStream)
            return false;

        var line = _stream.ReadLine();
        if (line == null)
            return false;

        logEvent = LogEventBase.Parse(line, LineNumber);
        LineNumber++;
        return true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _stream.Dispose();
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
