using CataParser;
using CataParser.Calculators;
using CataParser.Calculators.Damage;
using CataParser.Events;

using var parser = new LogParser(@"<file path>");
var dpsCalc = new DpsCalculator();
var damageTaken = new DamageTakenCalculator();

foreach(var entry in parser.Read())
{
    if (entry.Effect.Result == EventSuffix.Summon)
        dpsCalc.RegisterMinion(entry);

    if (entry.Effect.IsDamage)
    {
        dpsCalc.AddDamage(entry);
        damageTaken.Record(entry);
    }
}

dpsCalc.ReconcileMinionDamage();
dpsCalc.CalculateDps();

Console.WriteLine(dpsCalc.ReportPlayerDamage());

Console.WriteLine(damageTaken.ReportDamageTakenBySpellName("Lava Wave"));

Console.ReadLine();