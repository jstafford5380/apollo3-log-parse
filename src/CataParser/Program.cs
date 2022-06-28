using CataParser;
using CataParser.Collectors;
using CataParser.Collectors.Damage;
using CataParser.Encounters;
using CataParser.Events;

ExternalVariableLoader.Load();

var dps = new DamageCollector();

using var parser = new LogParser(@"C:\Users\draep\OneDrive\wow_logs\20220627-g3\WoWCombatLog.txt");
var encounters = new EncounterParser(TimeSpan.FromSeconds(30));
encounters.ParseBossAttempts(parser);

var reportBuilder = ReportBuilder.FromEncounterParse(encounters);
reportBuilder.Use<DamageCollector>();

reportBuilder.Build();

var ragAttempts = reportBuilder.Reports["Alysrazor"];

for(var i = 1; i <= ragAttempts.Attempts.Count; i++)
{
    var damage = ragAttempts.GetCollector<DamageCollector>(i);

    Console.WriteLine($"DPS (Attempt {i}):");
    foreach(var item in damage.PlayerDamage.OrderByDescending(r => r.Value.Dps))
    {
        Console.WriteLine($"  {item.Value.SourceName}: {item.Value.Dps:#,#}");
    }

    Console.WriteLine();
    Console.WriteLine("Damage Taken:");

    foreach(var item in damage.PlayerDamageTaken)
    {
        Console.WriteLine($"  {item.Key}:");
        foreach(var source in item.Value)
        {
            Console.WriteLine($"    {source.Key}: {source.Value:#,#}");
        }
    }

    Console.WriteLine();
}

Console.ReadLine();