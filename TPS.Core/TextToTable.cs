using TPS.Core.Models;

namespace TPS.Core;

public static class TextToTable
{
    public static List<TableRow> GetTable(List<string> names, List<int> prices, List<int> availabilities)
    {
        var results = new List<TableRow>();
        if (names.Count != prices.Count || names.Count != availabilities.Count)
        {
            Logger.Log($"Failed to map data. Names: {names.Count}, Prices: {prices.Count}, Availabilities: {availabilities.Count}");
            return results;
        }

        for (var i = 0; i < names.Count; i++)
        {
            var name = names[i];
            if (availabilities[i] == -1 || prices[i] == -1) continue;
            results.Add(new TableRow { Name = name, Availability = availabilities[i], Price = prices[i] });
        }

        return results;
    }

}