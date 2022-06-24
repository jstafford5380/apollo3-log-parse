using CsvHelper;

public class SpellDamage : DamageEffect
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int School { get; set; }

    private static string[] ValidPrefixes = new[]
    {
        EventPrefix.Spell,
        EventPrefix.Range,
        EventPrefix.SpellPeriodic,
        EventPrefix.SpellBuilding
    };

    public SpellDamage(Effect effect)
    {
        if (!ValidPrefixes.Contains(effect.Action))
            throw new InvalidOperationException($"{effect.Action}j is not spell damage.");
        
        // BUG: this ID doesn't seem to be correct
        Id = Convert.ToInt32(effect.EffectDetails.GetField(9), 16);
        Name = effect.EffectDetails.GetField(10);
        School = Convert.ToInt32(effect.EffectDetails.GetField(11), 16);

        SetFromSlice(effect.EffectDetails, 11);
    }
}
