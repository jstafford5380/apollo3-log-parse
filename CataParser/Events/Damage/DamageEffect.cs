using CsvHelper;

public class DamageEffect
{
    public int Amount { get; set; }

    public int Overkill { get; set; }

    public int School { get; set; }

    public int Resisted { get; set; }

    public int Blocked { get; set; }

    public int Absorbed { get; set; }

    public bool Critical { get; set; }

    public bool Glancing { get; set; }

    public bool Crushing { get; set; }

    public void SetFromSlice(CsvReader csv, int lastOffset)
    {
        Amount = Convert.ToInt32(csv.GetField(++lastOffset));
        Overkill = Convert.ToInt32(csv.GetField(++lastOffset));
        School = Convert.ToInt32(csv.GetField(++lastOffset), 16);
        Resisted = Convert.ToInt32(csv.GetField(++lastOffset), 16);
        Blocked = Convert.ToInt32(csv.GetField(++lastOffset), 16);
        Absorbed = Convert.ToInt32(csv.GetField(++lastOffset), 16);
        Critical = NilBool(csv.GetField(++lastOffset));
        Glancing = NilBool(csv.GetField(++lastOffset));
        Crushing = NilBool(csv.GetField(++lastOffset));
    }

    private static bool NilBool(string value) => value == "nil" ? false : true;
}
