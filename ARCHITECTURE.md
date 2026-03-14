# WebSearch Library Architecture

## Overview

Ultimate C# web search library with pluggable provider architecture supporting multiple search engines (Google, Bing, DuckDuckGo, etc.) with intelligent result parsing, caching, rate limiting, and retry logic.

## Design Principles

1. **Provider Abstraction**: Clean interface for adding new search engines
2. **Result Normalization**: Unified SearchResult model across all providers
3. **Resilience**: Rate limiting, retry logic, fallback providers
4. **Performance**: In-memory caching with configurable TTL
5. **Extensibility**: Easy to add new providers without changing core

## Architecture Layers

### 1. Core Abstractions (`WebSearch.Core`)

```
ISearchProvider
├── SearchAsync(string query, SearchOptions options)
├── GetProviderName()
└── IsAvailable()

SearchResult
├── Title
├── Url
├── Snippet
├── Source (provider name)
├── Rank
└── Metadata (Dictionary<string, object>)

SearchOptions
├── MaxResults
├── Language
├── Region
├── SafeSearch
├── TimeRange
└── CustomParameters
```

### 2. Provider Implementations (`WebSearch.Providers`)

**Scraping-Based Providers:**
- `GoogleSearchProvider` - Uses AngleSharp for HTML parsing
- `BingSearchProvider` - Uses AngleSharp for HTML parsing  
- `DuckDuckGoProvider` - Current implementation
- `YahooSearchProvider` - Uses AngleSharp

**API-Based Providers (fallback/premium):**
- `GoogleCustomSearchProvider` - Requires API key
- `BingApiSearchProvider` - Requires Azure Cognitive Services key
- `SerpApiProvider` - Third-party SERP API

**Browser Automation (for JS-heavy sites):**
- `PlaywrightSearchProvider` - Uses Playwright for dynamic content

### 3. Infrastructure (`WebSearch.Infrastructure`)

**Caching:**
- `ISearchCache` interface
- `InMemorySearchCache` - Default implementation
- `DistributedSearchCache` - Redis/SQL support

**Rate Limiting:**
- `IRateLimiter` interface  
- `TokenBucketRateLimiter` - Per-provider rate limits
- `SlidingWindowRateLimiter` - Alternative implementation

**Retry Logic:**
- `RetryPolicy` - Exponential backoff with jitter
- `CircuitBreaker` - Prevents cascade failures

### 4. Factory & DI (`WebSearch`)

```
SearchProviderFactory
├── Create(ProviderType)
├── CreateWithFallback(ProviderType[])
└── CreateAll()

ServiceCollectionExtensions
├── AddWebSearch()
├── AddWebSearchProvider<T>()
└── AddWebSearchCache()
```

## Provider Implementation Strategy

### Phase 1: Google Scraping (PRIMARY)
- Use AngleSharp for fast HTML parsing
- Parse organic results from SERP
- Extract: title, URL, snippet, position
- Handle anti-scraping:
  - Rotating user agents
  - Random delays
  - Request headers mimicking browser
- Fallback to Google Custom Search API if blocked

### Phase 2: Bing Scraping (SECONDARY)
- Similar to Google but different selectors
- Generally more permissive than Google
- Can serve as primary fallback

### Phase 3: DuckDuckGo (TERTIARY)
- Existing implementation
- No tracking, no rate limiting
- Good for privacy-focused searches

### Phase 4: Premium APIs (OPTIONAL)
- Google Custom Search (100 free/day, then paid)
- Bing Web Search API (1000 free/month)
- SerpApi (unified SERP parsing service)

## Search Flow

```
User Request
    ↓
SearchProviderFactory.Create()
    ↓
Check Cache → HIT? → Return cached results
    ↓ MISS
Rate Limiter → ALLOWED?
    ↓ YES
Provider.SearchAsync()
    ↓
HTML/API Fetch → Parse → Normalize
    ↓
Cache Results (with TTL)
    ↓
Return SearchResult[]
    ↓ (on ERROR)
Retry with exponential backoff (3 attempts)
    ↓ (on FAILURE)
Try fallback provider (if configured)
```

## Configuration

```json
{
  "WebSearch": {
    "DefaultProvider": "Google",
    "FallbackChain": ["Google", "Bing", "DuckDuckGo"],
    "Cache": {
      "Enabled": true,
      "TTLSeconds": 3600,
      "MaxEntries": 10000
    },
    "RateLimit": {
      "Google": { "RequestsPerMinute": 10 },
      "Bing": { "RequestsPerMinute": 20 },
      "DuckDuckGo": { "RequestsPerMinute": 30 }
    },
    "Retry": {
      "MaxAttempts": 3,
      "InitialDelayMs": 1000,
      "MaxDelayMs": 30000
    },
    "Providers": {
      "Google": {
        "CustomSearchApiKey": "",  // Optional
        "CustomSearchEngineId": ""  // Optional
      },
      "Bing": {
        "SubscriptionKey": ""  // Optional
      }
    }
  }
}
```

## Usage Examples

```csharp
// Simple usage
var factory = new SearchProviderFactory();
var provider = factory.Create(ProviderType.Google);
var results = await provider.SearchAsync("C# async patterns");

// With DI
services.AddWebSearch(options => {
    options.DefaultProvider = ProviderType.Google;
    options.EnableCaching = true;
    options.FallbackProviders = new[] { ProviderType.Bing, ProviderType.DuckDuckGo };
});

var searchService = serviceProvider.GetRequiredService<ISearchService>();
var results = await searchService.SearchAsync("machine learning", new SearchOptions {
    MaxResults = 20,
    Language = "en",
    SafeSearch = true
});

// Multi-provider comparison
var multiProvider = factory.CreateAll();
var allResults = await Task.WhenAll(
    multiProvider.Select(p => p.SearchAsync("test query"))
);
```

## Anti-Scraping Strategies

1. **User Agent Rotation**: Mimic real browsers (Chrome, Firefox, Safari)
2. **Request Headers**: Accept, Accept-Language, Referer, etc.
3. **Rate Limiting**: Respect provider limits, add random jitter
4. **Session Management**: Cookie handling, persistent sessions
5. **Proxy Support**: Optional proxy rotation (user-provided)
6. **Error Detection**: Detect CAPTCHA/block pages, trigger fallback

## Testing Strategy

1. **Unit Tests**: Each provider in isolation with mocked HTTP
2. **Integration Tests**: Live searches with rate limiting
3. **Parser Tests**: HTML fixtures from real SERPs
4. **Cache Tests**: Verify TTL, eviction, consistency
5. **Rate Limit Tests**: Verify throttling behavior
6. **Fallback Tests**: Verify provider chain activation

## Performance Targets

- **Latency**: < 500ms for cached results, < 2s for live searches
- **Throughput**: 100+ searches/sec with caching
- **Memory**: < 100MB for 10K cached results
- **Reliability**: 99.9% success rate with fallback chain

## Security Considerations

- Never log API keys or credentials
- Sanitize user queries (prevent injection)
- Validate URLs before returning
- Rate limit per client (if used as API)
- Support HTTPS only

## Future Enhancements

1. **Semantic Search**: Integration with embedding models
2. **Result Deduplication**: Across multiple providers
3. **Relevance Scoring**: ML-based ranking
4. **Image/Video Search**: Specialized providers
5. **News Search**: Time-sensitive results
6. **Shopping Search**: Product-specific parsing
7. **Academic Search**: Google Scholar, PubMed, arXiv
8. **Local Search**: Location-based results

---

**Created**: 2026-03-14
**Author**: Jengo (Claude Agent)
**Version**: 1.0
