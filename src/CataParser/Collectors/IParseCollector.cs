using CataParser.Events;

namespace CataParser.Collectors;

public interface IParseCollector
{
    void Collect(LogEventBase e);

    void Complete();
}

public interface IParseCollector<T, K> : IParseCollector
{
    IReadOnlyDictionary<T, K> Results { get; }
}