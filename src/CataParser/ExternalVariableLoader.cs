using CataParser.Encounters;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace CataParser;

internal static class ExternalVariableLoader
{
    public static void Load()
    {
        LoadBosses();
    }

    private static void LoadBosses()
    {
        const string bossListingPath = "Variables/boss_listing.csv";
        if (!File.Exists(bossListingPath))
            throw new FileNotFoundException("Could not find boss listing.");

        using var stream = File.OpenText(bossListingPath);

        using var csvRead = new CsvReader(stream, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });
        csvRead.Context.RegisterClassMap<BossFileMap>();
        
        csvRead.Read();
        csvRead.ReadHeader();

        var bosses = new List<Boss>();
        while (csvRead.Read())
        {
            var boss = csvRead.GetRecord<Boss>();
            bosses.Add(boss);
        }

        BossTable.Load(bosses);
    }
}

public class BossFileMap : ClassMap<Boss>
{
    public BossFileMap()
    {
        Map(b => b.RaidName).Index(0);
        Map(b => b.Name).Index(1);
        Map(b => b.Health.Normal10).Index(2);
        Map(b => b.Health.Heroic10).Index(3);
        Map(b => b.Health.Normal25).Index(4);
        Map(b => b.Health.Heroic25).Index(5);
    }
}
