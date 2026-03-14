using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using WebSearch;
using WebSearch.Core;
using WebSearch.Infrastructure;

Console.WriteLine("=== WebSearch Library Demo ===\n");

// Initialize services
var cache = new InMemorySearchCache(new MemoryCache(new MemoryCacheOptions()));
var rateLimiter = new TokenBucketRateLimiter(requestsPerMinute: 10);
var factory = new SearchProviderFactory(cache, rateLimiter);

// Interactive loop
while (true)
{
    Console.Write("\nEnter search query (or 'exit' to quit): ");
    var query = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(query) || query.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Console.WriteLine("\nSelect provider:");
    Console.WriteLine("1. Google");
    Console.WriteLine("2. Bing");
    Console.WriteLine("3. DuckDuckGo");
    Console.WriteLine("4. All providers (comparison)");
    Console.Write("Choice: ");

    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                await SearchWithProvider(factory.Create(ProviderType.Google), query);
                break;
            case "2":
                await SearchWithProvider(factory.Create(ProviderType.Bing), query);
                break;
            case "3":
                await SearchWithProvider(factory.Create(ProviderType.DuckDuckGo), query);
                break;
            case "4":
                await SearchAllProviders(factory, query);
                break;
            default:
                Console.WriteLine("Invalid choice");
                break;
        }

        // Show cache statistics
        var stats = cache.GetStatistics();
        Console.WriteLine($"\n📊 Cache Stats: {stats.Hits} hits, {stats.Misses} misses ({stats.HitRate:P1} hit rate)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error: {ex.Message}");
    }
}

Console.WriteLine("\nGoodbye!");

static async Task SearchWithProvider(SearchService service, string query)
{
    Console.WriteLine($"\n🔍 Searching with {service.GetProviderName()}...");

    var sw = Stopwatch.StartNew();
    var results = await service.SearchAsync(query, new SearchOptions { MaxResults = 10 });
    sw.Stop();

    Console.WriteLine($"✓ Found {results.Length} results in {sw.ElapsedMilliseconds}ms\n");

    for (int i = 0; i < Math.Min(results.Length, 10); i++)
    {
        var result = results[i];
        Console.WriteLine($"{result.Rank}. {result.Title}");
        Console.WriteLine($"   {result.Url}");
        if (!string.IsNullOrEmpty(result.Snippet))
        {
            var snippet = result.Snippet.Length > 150
                ? result.Snippet.Substring(0, 150) + "..."
                : result.Snippet;
            Console.WriteLine($"   {snippet}");
        }
        Console.WriteLine();
    }
}

static async Task SearchAllProviders(SearchProviderFactory factory, string query)
{
    var providers = new[]
    {
        (Type: ProviderType.Google, Service: factory.Create(ProviderType.Google)),
        (Type: ProviderType.Bing, Service: factory.Create(ProviderType.Bing)),
        (Type: ProviderType.DuckDuckGo, Service: factory.Create(ProviderType.DuckDuckGo))
    };

    var tasks = providers.Select(async p =>
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var results = await p.Service.SearchAsync(query, new SearchOptions { MaxResults = 5 });
            sw.Stop();
            return (p.Type, Results: results, Time: sw.ElapsedMilliseconds, Error: (string?)null);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return (p.Type, Results: Array.Empty<SearchResult>(), Time: sw.ElapsedMilliseconds, Error: ex.Message);
        }
    });

    var allResults = await Task.WhenAll(tasks);

    Console.WriteLine("\n=== Multi-Provider Comparison ===\n");

    foreach (var (type, results, time, error) in allResults)
    {
        Console.WriteLine($"📍 {type} - {results.Length} results in {time}ms");
        if (error != null)
        {
            Console.WriteLine($"   ❌ Error: {error}");
        }
        else
        {
            foreach (var result in results.Take(3))
            {
                Console.WriteLine($"   • {result.Title}");
            }
        }
        Console.WriteLine();
    }
}
