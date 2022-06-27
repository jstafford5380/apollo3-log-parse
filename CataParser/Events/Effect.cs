using CataParser.Events;
using CsvHelper;

public class Effect
{
    private readonly LogEventBase _baseEvent;

    public CsvReader EffectDetails { get; }

    public string EventName { get; set; }

    public string Action { get; set; }

    public string Result { get; set; }

    //public bool IsDamage => Result == EventSuffix.Damage;

    public bool IsHeal => Result == EventSuffix.Heal || Action == EventPrefix.SpellAbsorbed;

    public Effect(string @event, CsvReader csvReader, LogEventBase baseEvent)
    {
        EventName = @event;
        EffectDetails = csvReader;
        _baseEvent = baseEvent;

        var parts = @event.Split('_');
        Result = parts.Last();
        Action = @event.Substring(0, @event.IndexOf(Result)).Trim('_');        
    }

    public DamageEffect GetDamageEffect()
    {
        if (!IsDamage)
            throw new InvalidOperationException("This is not a damage effect.");

        DamageEffect damage = null;
        switch (Action)
        {
            case EventPrefix.Range:
            case EventPrefix.SpellPeriodic:
            case EventPrefix.Spell:
            case EventPrefix.SpellBuilding:
                damage = new SpellDamage(this);
                break;
            case EventPrefix.Swing:
                damage = new SwingDamage(this);
                break;
        }

        return damage;
    }
    
    public override string ToString() => $"{Action} = {Result}";

    public bool IsDamage => Result is EventSuffix.Damage;
}
