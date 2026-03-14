# WebSearch - Ultimate C# Web Search Library

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)

The ultimate C# web search library with pluggable provider architecture supporting multiple search engines (Google, Bing, DuckDuckGo, etc.) with intelligent result parsing, caching, rate limiting, and retry logic.

## Features

✨ **Multi-Engine Support**
- Google Search (HTML scraping)
- Bing Search (HTML scraping)
- DuckDuckGo Search
- Extensible architecture for adding more providers

🚀 **Performance & Resilience**
- Intelligent caching with configurable TTL
- Per-provider rate limiting (Token Bucket algorithm)
- User-agent rotation to avoid blocks
- Thread-safe implementation

🏗️ **Clean Architecture**
- Provider abstraction for easy extensibility
- Unified result model across all providers
- Dependency injection ready
- Async/await throughout

## Quick Start

```csharp
using WebSearch;
using WebSearch.Core;

// Create a provider factory
var factory = new SearchProviderFactory();

// Create a Google search service
var searchService = factory.Create(ProviderType.Google);

// Perform a search
var results = await searchService.SearchAsync("C# async programming");

// Display results
foreach (var result in results)
{
    Console.WriteLine($"{result.Rank}. {result.Title}");
    Console.WriteLine($"   {result.Url}");
}
```

## Installation

```bash
git clone https://github.com/martiendejong/websearch.git
cd websearch
dotnet build
dotnet run --project WebSearch.Samples
```

## Provider Comparison

| Provider | Speed | API Key | Rate Limits | Notes |
|----------|-------|---------|-------------|-------|
| Google | ⚡⚡ | No | ~10-20/min | Best quality |
| Bing | ⚡⚡⚡ | No | ~20-30/min | Good quality |
| DuckDuckGo | ⚡⚡⚡ | No | ~30+/min | Privacy-focused |

## Documentation

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed architecture documentation.

## License

MIT License - see LICENSE file for details

## Disclaimer

This library is for educational and research purposes. Respect robots.txt and Terms of Service when using.
