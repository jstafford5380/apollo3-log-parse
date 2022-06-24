public class SwingDamage : DamageEffect
{
    private static string[] ValidPrefixes = new[]
    {
        EventPrefix.Swing
    };

    public SwingDamage(Effect effect)
    {
        if (!ValidPrefixes.Contains(effect.Action))
            throw new InvalidOperationException($"{effect.Action} is not swing damage.");

        SetFromSlice(effect.EffectDetails, 8);
    }
}
