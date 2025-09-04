using System.Text.Json;
using TPS.Core.Models;

namespace TPS.Core;

public static class Scanner
{
    private static bool _needToCheckSorting = true;
    public static string Version = null;
    public static Settings Settings;
    public static void Initialize(Settings settings)
    {
        Settings = settings;

        if (Directory.Exists("images")) Directory.Delete("images", true);
        Directory.CreateDirectory("images");
        Directory.CreateDirectory("auctions");
        Directory.CreateDirectory("logs");
        ControlModule2.Initialize();
        Logger.Flush();
        GameWindow.Initialize();
        Logger.Flush();
    }

    public static async Task StartScan(CancellationToken cancellationToken)
    {
        _needToCheckSorting = true;
        var categoriesToScan = Settings.CategoriesToScan.ToHashSet();
        await GameWindow.BringToFront();

        try
        {
            var serverName = await GameWindow.GetServerName();
            foreach (var category in Categories.Root)
            {
                if (!categoriesToScan.Contains(category.Name)) continue;
                await ScanCategory(category, serverName, cancellationToken);

            }


        }

        catch (Exception e)
        {
            Logger.Log("An error occured:", true);
            throw;
        }

    }

    private static async Task ScanCategory(Category rootCategory, string serverName,
        CancellationToken cancellationToken)
    {
        GameWindow.maskThreshold = 10;
        GameWindow.threshold = 10;
        GameWindow.RequiredConfidence = 0.96f;
        var categories = rootCategory.Children.Count > 0 ? rootCategory.Children : [rootCategory];
        var retryCount = 0;
        for (var i = 0; i < categories.Count; i++)
        {
            if (cancellationToken.IsCancellationRequested) throw new TPSExitException();
            var category = categories[i];
            var firstRun = true;
            var foundPageInfo = false;
            PageInfo pageInfo = null;
            while (!foundPageInfo)
            {

                await GameWindow.Reset();
                if (!firstRun)
                {
                    ControlModule2.Press("a");
                    await Task.Delay(250);
                    ControlModule2.Release("a");
                    await Task.Delay(50);
                    ControlModule2.Press("d");
                    await Task.Delay(250);
                    ControlModule2.Release("d");
                }

                firstRun = false;
                await GameWindow.OpenTradingPost();
                if (category.Parent != null) await GameWindow.SwitchToCategory(category.Parent);
                await GameWindow.SwitchToCategory(category);
                //await GameWindow.SortByPrice();
                foundPageInfo = GameWindow.TryGetPageInfo(out pageInfo);
                if (!foundPageInfo)
                {

                    Logger.Log("Could not read the page info, retrying.", true);
                    retryCount++;
                    if (retryCount > 4)
                    {
                        Logger.Log("Skipping category", true);
                        break;
                    }
                }
                Logger.Flush();
            }

            if (!foundPageInfo)
            {
                continue;
            }

            if (_needToCheckSorting)
            {
                // Check if sorted by price
                var sortingTries = 0;
                var correctSorting = false;
                Logger.Log("Checking if the TP is sorted by price", true);
                while (!correctSorting)
                {
                    if (cancellationToken.IsCancellationRequested) throw new TPSExitException();
                    try
                    {
                        var checkAuctions = await GameWindow.ScanPage();
                        var parsedAuctions = ItemResolver.GetAuctionModels(checkAuctions, category)
                            .Where(a => a.Price != 0).ToList();
                        //var parsedAuctions = checkAuctions.Where(a => a != null).Select(ItemResolver.GetAuctionModel).Where(a => a != null && a.Price != 0).ToList();
                        var wrongSorting = false;
                        for (var k = 1; k < parsedAuctions.Count; k++)
                        {
                            if (parsedAuctions[k].Price >= parsedAuctions[k - 1].Price) continue;
                            Logger.Log(
                                $"{parsedAuctions[k].Price} is not bigger than {parsedAuctions[k - 1].Price}, changing sort.",
                                true);
                            wrongSorting = true;
                            break;
                        }

                        if (wrongSorting)
                        {
                            sortingTries++;
                            if (sortingTries > 6)
                            {
                                Logger.Log(
                                    "Could not determine sort order, please try to position your character differently and try again",
                                    true);
                                throw new TPSExitException();
                            }

                            await GameWindow.SortByPrice();
                        }
                        else
                        {
                            correctSorting = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log(e.Message);
                    }


                }

                await ControlModule2.WheelUp(15);
                _needToCheckSorting = false;
            }

            List<AuctionModel> auctions = null;
            try
            {
                auctions = await GetAuctions(pageInfo, category, cancellationToken);
            }
            catch (TPSCoreException e)
            {
                switch (e.Message)
                {
                    case "Refresh":
                        {
                            retryCount++;
                            if (retryCount >= 2)
                            {
                                Logger.Log("Refresh loop detected, tried 2 times, skipping the category", true);
                                continue;
                            }
                            i--;
                            //GameWindow.SlowDownMode = true;
                            continue;
                        }
                    case "Restart":
                        {
                            retryCount++;
                            if (retryCount >= 3)
                            {
                                Logger.Log("Tried 3 times, skipping the category", true);
                                continue;
                            }
                            i--;
                            //GameWindow.SlowDownMode = true;
                            continue;
                        }
                    default:
                        throw;
                }
            }
            if (cancellationToken.IsCancellationRequested) throw new TPSExitException();
            var fileName = Path.Combine("auctions", $"{serverName}-{category.Name}.json");

            await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(auctions), cancellationToken);
            Logger.Flush();
            if (cancellationToken.IsCancellationRequested) throw new TPSExitException();
            if (auctions.Count < 20) continue;
        //  //   await ApiClient.Upload(serverName, category.Name, auctions);
//
        }
    }

    private static async Task<List<AuctionModel>> GetAuctions(PageInfo pageInfo, Category category,
        CancellationToken cancellationToken)
    {
        var allAuctions = new Dictionary<int, List<AuctionModel>>();

        var pageCount = pageInfo.PageCount;
        var currentPage = pageInfo.CurrentPage;

        while (currentPage <= pageCount)
        {
            if (cancellationToken.IsCancellationRequested) throw new TPSExitException();
            if (GameWindow.IsRefresh())
            {
                if (pageCount * 0.8f < currentPage)
                {
                    Logger.Log("Possible paging bug detected, 80% of the category completed, accepting the category", true);
                    break;
                }
                Logger.Log("Refresh detected, restarting the category", true);
                throw new TPSCoreException("Refresh");
            }

            if (GameWindow.TryGetPageInfo(out pageInfo))
            {
                if (currentPage != pageInfo.CurrentPage)
                {
                    if (Math.Abs(currentPage - pageInfo.CurrentPage) < 2)
                    {
                        Logger.Log($"Total page count changed to {pageInfo.PageCount}", true);
                        currentPage = pageInfo.CurrentPage;
                    }
                    else if (pageInfo.CurrentPage.ToString().Contains(currentPage.ToString()))
                    {
                        // ignore
                    }
                    else
                    {
                        Logger.Log("Page numbers doesn't match, restarting the category", true);
                        throw new TPSCoreException("Restart");
                    }
                }

                if (pageCount != pageInfo.PageCount)
                {
                    if (Math.Abs(pageCount - pageInfo.PageCount) < 2)
                    {
                        Logger.Log($"Total page count changed to {pageInfo.PageCount}", true);
                        pageCount = pageInfo.PageCount;
                    }
                    else if (pageInfo.PageCount.ToString().Contains(pageCount.ToString()))
                    {
                        // ignore
                    }
                    else
                    {
                        Logger.Log("Page numbers doesn't match, restarting the category", true);
                        throw new TPSCoreException("Restart");
                    }
                }
            }
            Logger.Log($"Scanning page {currentPage} of {pageCount}", true);
            if (!GameWindow.IsFocused())
            {
                Logger.Log("Game is not focused, waiting.", true);
                await Task.Delay(2000, cancellationToken);
                continue;
            }
            var auctions = await GameWindow.ScanPage(pageCount == currentPage);
            var parsedAuctions = ItemResolver.GetAuctionModels(auctions, category);
            var checkedAuctions = CheckValidity(parsedAuctions);
            if (currentPage < pageCount && checkedAuctions.Count < 18)
            {
                Logger.Log($"Valid auctions count: {checkedAuctions.Count}, retrying current page");
                await ControlModule2.WheelUp(15);
                var newAuctions = await GameWindow.ScanPage(pageCount == currentPage, true);
                var newParsedAuctions = ItemResolver.GetAuctionModels(newAuctions, category);
                var newCheckedAuctions = CheckValidity(newParsedAuctions);
                if (newCheckedAuctions.Count > checkedAuctions.Count) checkedAuctions = newCheckedAuctions;
            }
            //var parsedAuctions = auctions.Select(ItemResolver.GetAuctionModel).Where(a => a != null).ToList();
            allAuctions[currentPage] = checkedAuctions;

            Logger.Log($"Page {currentPage} valid auctions count: {checkedAuctions.Count}", true);
            await GameWindow.NextPage(currentPage % 5 == 0);
            currentPage++;
            if (currentPage > pageCount) break;

        }
        Logger.Flush();
        return allAuctions.Values.SelectMany(a => a).ToList();
    }

    private static List<AuctionModel> CheckValidity(List<AuctionModel> auctions)
    {
        var results = new List<AuctionModel>();
        auctions = auctions.Where(a => a.Price > 0 && a.Quantity > 0).ToList();
        if (auctions.Count == 0) return auctions;
        results.Add(auctions[0]);
        for (var i = 1; i < auctions.Count; i++)
        {
            if (auctions[i].Price >= auctions[i - 1].Price) results.Add(auctions[i]);
            else
            {
                Logger.Debug($"{auctions[i].Name} price {auctions[i].Price} is smaller than previous {auctions[i - 1].Price}, skipping");
            }
        }

        return results;
    }

}
