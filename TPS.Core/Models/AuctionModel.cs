namespace TPS.Core.Models;

public class AuctionModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public long Timestamp { get; set; }
}

public class AuctionRequest
{
    public string Server { get; set; }
    public string Category { get; set; }
    public string ScannerName { get; set; }
    public List<AuctionModel> Auctions { get; set; }
    public string Version { get; set; }
    public string UserId { get; set; }
}