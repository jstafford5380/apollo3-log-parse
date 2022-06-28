namespace CataParser.Encounters;

public class Boss
{
    public string RaidName { get; set; }

    public string Name { get; set; }

    public BossHealth Health { get; set; }
}

public class BossHealth
{
    public int Normal10 { get; set; }
    
    public int Normal25 { get; set; }
    
    public int Heroic10 { get; set; }

    public int Heroic25 { get; set; }
}
