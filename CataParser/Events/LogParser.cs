namespace CataParser.Events;

public class LogParser : IDisposable
{
    private StreamReader _stream;
    private bool _disposedValue;

    public LogParser(string logPath)
    {
        if (!File.Exists(logPath))
            throw new FileNotFoundException("Combat log not found", logPath);

        _stream = File.OpenText(logPath);
    }

    public IEnumerable<LogEventBase> Read()
    {
        while (!_stream.EndOfStream)
        {
            var line = _stream.ReadLine();
            if (line == null)
                continue;

            var logEvent = LogEventBase.Parse(line);
            yield return logEvent;
        }
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
