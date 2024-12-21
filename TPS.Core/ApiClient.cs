using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FuzzierSharp;
using TPS.Core.Models;

namespace TPS.Core;


public static class ApiClient
{
    private static List<ServerModel> Servers { get; set; }

    private static readonly HttpClient HttpClient = new();

    public static string RemoveNonLetterDigitSpace(string input)
    {
        return Regex.Replace(input, @"[^a-zA-Z0-9\s]", string.Empty).Trim();
    }

    public static ServerModel GetServer(string name)
    {

        name = RemoveNonLetterDigitSpace(name);
        if (name.StartsWith("Arise")) name = "Aries";
        if (name.StartsWith("Siran")) name = "Siren";
        var server = Servers.FirstOrDefault(s => s.Name == name);
        if (server != null) return server;
        server = Servers.FirstOrDefault(s => name.StartsWith(s.Name));
        if (server != null) return server;

        var matches = Process.ExtractSorted(name, Servers.Select(s => s.Name), null, null, 70).ToList();

        return matches.Count switch
        {
            0 => null,
            1 => Servers.First(s => s.Name == matches[0].Value),
            _ => Math.Abs(matches[0].Score - matches[1].Score) > 5
                ? Servers.First(s => s.Name == matches[0].Value)
                : null
        };
    }

    public static void GetServers()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://nwmpapi.gaming.tools/servers");
        request.Headers.UserAgent.ParseAdd("gaming.tools");
        var response = HttpClient.Send(request);
        if (!response.IsSuccessStatusCode) throw new TPSCoreException("Could not fetch the servers list");
        using var stream = response.Content.ReadAsStream();
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        Servers = JsonSerializer.Deserialize<List<ServerModel>>(json);

    }


    public static List<ItemModel> GetItems()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://scdn.gaming.tools/nwmp/data/items/en.json");
        request.Headers.UserAgent.ParseAdd("gaming.tools");
        var response = HttpClient.Send(request);
        if (!response.IsSuccessStatusCode) throw new TPSCoreException("Could not fetch the items list");
        using var stream = response.Content.ReadAsStream();
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
        var results = new List<ItemModel>();
        foreach (var (itemId, props) in data)
        {
            results.Add(new ItemModel
            {
                Id = itemId,
                Name = props[0],
                TradingCategory = props[2],
                MaxStackSize = int.TryParse(props[3], out var stackSize) ? stackSize: 1
            });
        }

        return results;
    }

    public static async Task Upload(string server, string category, List<AuctionModel> auctions)
    {
        var model = new AuctionRequest
        {
            Server = server,
            Category = category,
            Version = Scanner.Version,
            ScannerName = Scanner.Settings.ScannerName,
            Auctions = auctions.Where(a => a != null).ToList()
        };

        var json = JsonSerializer.Serialize(model);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://nwmpapi.gaming.tools/auctions/scanner");
        request.Headers.UserAgent.ParseAdd("gaming.tools");
        request.Content = new StringContent(json);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json, "utf8");

        var response = await HttpClient.SendAsync(request);
        Logger.Log($"Server responded with {response.StatusCode}");
    }
}

public class ServerModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("region")]
    public string Region { get; set; }
}