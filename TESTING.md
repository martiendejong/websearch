# Testing Notes

## Status

The library architecture and infrastructure are complete and working:
- ✅ Core abstractions (ISearchProvider, SearchResult, SearchOptions)
- ✅ Caching system (InMemorySearchCache)
- ✅ Rate limiting (TokenBucketRateLimiter)
- ✅ Factory pattern and DI setup
- ✅ Three provider implementations (Google, Bing, DuckDuckGo)

## Parser Status

**Note:** Search engine HTML structures change frequently. The parsers may need updates to match current SERP layouts.

### Current Parser Selectors

**Google:**
- Results: `div.g, div[data-sokoban-container]`
- Title: `h3`
- Link: `a` with href cleanup
- Snippet: `div.VwiC3b, div[data-content-feature], div.IsZvec`

**Bing:**
- Results: `li.b_algo`
- Title: `h2 a, h2`
- Link: `h2 a, a`
- Snippet: `div.b_caption p, p`

**DuckDuckGo:**
- Results: `div.result, div.web-result`
- Title: `a.result__a, h2.result__title a`
- Snippet: `a.result__snippet, div.result__snippet`

## Next Steps

1. **Create Browser Tests**: Use Playwright to fetch live SERPs and inspect current HTML structure
2. **Update Parsers**: Adjust selectors based on current SERP layout
3. **Integration Tests**: Automated tests that verify parsers work with real searches
4. **Parser Versioning**: Track SERP changes and maintain selector versions

## Manual Testing

To test manually:

```bash
# Run the sample app
cd E:\projects\websearch
dotnet run --project WebSearch.Samples

# Enter a query and select a provider
# If results are empty (0 found), the parser needs updating
```

## Debugging Parser Issues

1. Fetch a search page manually
2. Inspect the HTML structure in browser DevTools
3. Update selectors in provider classes
4. Test again

Example debug code:

```csharp
var html = await FetchSearchPageAsync(url, cancellationToken);
File.WriteAllText("debug-serp.html", html);  // Inspect this file
```

## Why Parsers May Break

- Search engines update their HTML frequently
- A/B testing creates variations
- Different regions may have different layouts
- Mobile vs desktop differences
- Personalization affects structure

## Solutions

1. **Multiple Selector Strategies**: Try several selectors in fallback order
2. **Regex Parsing**: For stable patterns in href attributes
3. **API Providers**: Use official APIs (Google Custom Search, Bing API) as fallbacks
4. **Playwright Provider**: Render JavaScript and extract from DOM
5. **Community Updates**: Accept PRs with updated selectors

---

**Last Updated:** 2026-03-14
**Author:** Jengo (Claude Agent)
