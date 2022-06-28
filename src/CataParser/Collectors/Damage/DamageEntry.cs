using CataParser.Constants;
using CataParser.Events;

namespace CataParser.Collectors.Damage;

public abstract class DamageEntry : IDamageEvent
{
    public DateTime Timestamp { get; private set; }

    public int Amount { get; private set; }

    public int SpellId { get; private set; }

    public string SpellName { get; private set; }

    public ulong SourceId { get; set; }

    public string SourceName { get; set; }

    public ulong TargetId { get; set; }

    public string TargetName { get; set; }

    public bool SourceIsPlayer { get; }

    public bool TargetIsPlayer { get; }

    public DamageEntry(LogEventBase e)
    {
        var damage = e.Effect.GetDamageEffect();
        if (damage == null)
            throw new ArgumentException($"{e.EventName} is not a damage event!");

        Timestamp = e.Timestamp;
        SourceId = e.SourceId;
        SourceName = e.SourceName;
        TargetId = e.DestinationId;
        TargetName = e.DestinationName;
        SourceIsPlayer = e.SourceFlags1.HasFlag(UnitFlags.TYPE_PLAYER | UnitFlags.CONTROL_PLAYER);
        TargetIsPlayer = e.DestinationFlags1.HasFlag(UnitFlags.TYPE_PLAYER | UnitFlags.CONTROL_PLAYER);

        // spells and ranged
        if (damage is SpellDamage spell)
        {
            Amount = spell.Amount;
            SpellId = spell.Id;
            SpellName = spell.Name;
        }
        else if (damage is SwingDamage swing)
        {
            Amount = swing.Amount;
            SpellId = -1;
            SpellName = "Swing";
        }
        else
        {
            Amount = damage.Amount;
            SpellId = -2;
            SpellName = "Unknown";
        }
    }
}
