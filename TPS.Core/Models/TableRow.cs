namespace TPS.Core.Models;

public class TableRow
{
    public string Name { get; set; }
    public int Price { get; set; }
    public int Availability { get; set; }
}

public class TableRowComparer : IEqualityComparer<TableRow>
{
    public bool Equals(TableRow x, TableRow y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Name == y.Name && x.Price == y.Price && x.Availability == y.Availability;
    }

    public int GetHashCode(TableRow obj)
    {
        return 0;
    }
}