public class HealingEffect
{
    private readonly Effect _effect;

    public int Amount { get; set; }
    
    public int Overheal { get; set; }
    
    public int Absorbed { get; set; }

    public bool Critical { get; set; }

    public HealingEffect(Effect effect)
    {
        _effect = effect;
    }
}
