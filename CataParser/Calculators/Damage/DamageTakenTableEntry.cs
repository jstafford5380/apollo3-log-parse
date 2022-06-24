namespace CataParser.Calculators.Damage;

public class DamageTakenTableEntry
{
    public int DamageAmount { get; private set; }

    public int SpellId { get; private set; }

    public string SpellName { get; private set; }

    public DamageTakenTableEntry(DamageEffect damage)
    {
        // spells and ranged
        if(damage is SpellDamage spell)
        {
            DamageAmount = spell.Amount;
            SpellId = spell.Id;
            SpellName = spell.Name;
        }
        else if(damage is SwingDamage swing)
        {
            DamageAmount = swing.Amount;
            SpellId = -1;
            SpellName = "Swing";
        }
        else
        {
            DamageAmount = damage.Amount;
            SpellId = -2;
            SpellName = "Unknown";
        }
    }
}
