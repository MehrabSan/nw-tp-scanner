using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using TPS.Core.Models;
using Process = FuzzierSharp.Process;

namespace TPS.Core;

public static class ItemResolver
{
    public static List<ItemModel> ItemsList;
    public static Dictionary<string, ItemModel> ItemsMap;

    public static Dictionary<string, Dictionary<string, ItemModel>> CategoryMap = new(StringComparer.OrdinalIgnoreCase);

    private static readonly ConcurrentDictionary<string, ItemModel> FuzzyItemsCache = new();

    public static void Initialize()
    {
        ItemsList = ApiClient.GetItems();

        ItemsMap = ItemsList.ToDictionary(i => i.Id);
        foreach (var itemModel in ItemsList)
        {
            CategoryMap.TryAdd(itemModel.TradingCategory, []);
            CategoryMap[itemModel.TradingCategory].TryAdd(itemModel.Name, itemModel);
        }
    }


    public static List<AuctionModel> GetAuctionModels(List<TableRow> rows, Category category)
    {
        while (category.Parent!=null)
        {
            category = category.Parent;
        }
        var dict = new ConcurrentDictionary<int, AuctionModel>();
        Parallel.For(0, rows.Count, (i) => { dict[i] = GetAuctionModel(rows[i], category); });

        var results = dict.OrderBy(e => e.Key).Select(e => e.Value).Where(v => v != null).ToList();

        var finalResults = new List<AuctionModel>();
        foreach (var auctionModel in results)
        {
            var item = ItemsMap[auctionModel.Id];
            if (item.MaxStackSize < auctionModel.Quantity)
            {
                if (item.MaxStackSize == 1)
                {
                    auctionModel.Quantity = item.MaxStackSize;
                    finalResults.Add(auctionModel);
                }
                else
                {
                    Logger.Log($"Invalid availability for {item.Name} {auctionModel.Quantity} > {item.MaxStackSize}");
                }
            }
            else
            {
                finalResults.Add(auctionModel);
            }
        }

        return finalResults;
    }

    private static AuctionModel GetAuctionModel(TableRow row, Category category)
    {
        if (row == null) return null;
        var namesMap = CategoryMap[category.Name];
        if (Overrides.TryGetValue(row.Name, out var overrideName) &&
            namesMap.TryGetValue(overrideName, out var itemModel))
        {
            return new AuctionModel
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                Price = row.Price,
                Quantity = row.Availability,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }

        if (namesMap.TryGetValue(row.Name, out itemModel))
        {
            return new AuctionModel
            {
                Id = itemModel.Id,
                Name = itemModel.Name,
                Price = row.Price,
                Quantity = row.Availability,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }

        if (row.Name.Length < 3) return null;

        if (FuzzyItemsCache.TryGetValue(row.Name, out var fuzzyItem))
        {
            return new AuctionModel
            {
                Id = fuzzyItem.Id,
                Name = fuzzyItem.Name,
                Price = row.Price,
                Quantity = row.Availability,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var matches = Process.ExtractTop(row.Name, namesMap.Keys, null, null, 5, 88).ToList();

        switch (matches.Count)
        {
            case 0:
                Logger.Debug($"No matches for \"{row.Name}\" - {stopwatch.ElapsedMilliseconds} ms.");
                return null;
            case 1:
                if (!namesMap.TryGetValue(matches[0].Value, out fuzzyItem)) return null;
                Logger.Debug($"Fuzzy match \"{row.Name}\" > \"{fuzzyItem.Name}\" Score: {matches[0].Score} - {stopwatch.ElapsedMilliseconds} ms.");

                return AddToFuzzyItems(row, fuzzyItem);
            default:
                Logger.Debug($"Multiple matches for \"{row.Name}\":  - {stopwatch.ElapsedMilliseconds} ms.");
                foreach (var match in matches)
                {
                    Logger.Debug($"\t\"{match.Value}\" Score: {match.Score}");
                }

                if (matches[0].Score - matches[1].Score >= 4)
                {
                    Logger.Debug($"\tAccepting \"{matches[0].Value}\"");
                    return !namesMap.TryGetValue(matches[0].Value, out fuzzyItem) ? null : AddToFuzzyItems(row, fuzzyItem);
                }

                var match0LengthDiff = Math.Abs(matches[0].Value.Length - row.Name.Length);
                var match1LengthDiff = Math.Abs(matches[1].Value.Length - row.Name.Length);
                if (match0LengthDiff < 2 && match1LengthDiff > 2)
                {
                    Logger.Debug($"\tAccepting \"{matches[0].Value}\"");
                    return !namesMap.TryGetValue(matches[0].Value, out fuzzyItem) ? null : AddToFuzzyItems(row, fuzzyItem);
                }

                if (match1LengthDiff < 2 && match0LengthDiff > 2)
                {
                    Logger.Debug($"\tAccepting \"{matches[1].Value}\"");
                    return !namesMap.TryGetValue(matches[1].Value, out fuzzyItem) ? null : AddToFuzzyItems(row, fuzzyItem);

                }

                if (row.Name.Length > 3)
                {
                    var last3 = row.Name[^3..];
                    if (char.IsDigit(last3[0]) && last3[1] == '/' && char.IsDigit(last3[2]))
                    {
                        var match = matches.FirstOrDefault(m => Math.Abs(m.Value.Length-row.Name.Length)<3 &&  m.Value.EndsWith(last3));
                        if (match != null)
                        {
                            Logger.Debug($"\tAccepting \"{match.Value}\"");
                            return !namesMap.TryGetValue(match.Value, out fuzzyItem) ? null : AddToFuzzyItems(row, fuzzyItem);
                        }
                    }
                }

                

                Logger.Debug("Results are too close, skipping");
                return null;

        }

    }

    private static AuctionModel AddToFuzzyItems(TableRow row, ItemModel item)
    {
        if (Math.Abs(row.Name.Length - item.Name.Length) > 8)
        {
            Logger.Log("The difference between the lengths of the match and the item is too large, skipping");
            return null;
        }

        if (FuzzyItemsCache.TryAdd(row.Name, item))
        {
            //File.WriteAllText(FuzzyItemsFile, JsonSerializer.Serialize(fuzzyItemsCache));
        }
        return new AuctionModel
        {
            Id = item.Id,
            Name = item.Name,
            Price = row.Price,
            Quantity = row.Availability,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }

    private static readonly Dictionary<string, string> Overrides = new Dictionary<string, string>
    {
        { "Ledestone", "Lodestone" },
        { "Water Mate", "Water Mote" },
    };


}