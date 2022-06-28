using CataParser.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CataParser.Collectors;

internal class Deaths : IParseCollector
{   

    public void Collect(LogEventBase e)
    {
        if (e.EventName != UnitDeathEntry.DeathEventName)
            return;
    }

    public void Complete()
    {
        throw new NotImplementedException();
    }
}

public class UnitDeathEntry
{
    public const string DeathEventName = "UNIT_DIED";

    public DateTime Timestamp { get; }

    public ulong UnitId { get; }

    public string UnitName { get; }

    public UnitDeathEntry(LogEventBase e)
    {
        if (e.EventName != UnitDeathEntry.DeathEventName)
            throw new ArgumentException($"{e.EventName} is not a death event!");

        Timestamp = e.Timestamp;
        UnitId = e.DestinationId;
        UnitName = e.DestinationName;
    }
}
