namespace CataParser.Collectors;

public class ParseCollection : List<IParseCollector> 
{
    public ParseCollection(IEnumerable<IParseCollector> collectors)
    {
        AddRange(collectors);
    }
}
