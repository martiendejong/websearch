import requests

api_key = "pk_74525428_P1UEETHS67964EXW4K4ZOPR1F1TWL0NI"
list_id = "901216309089"
headers = {'Authorization': api_key, 'Content-Type': 'application/json'}

tasks_data = [
    ("Core Abstractions - ISearchProvider interface", "ISearchProvider, SearchResult, SearchOptions models", 1),
    ("Infrastructure - Caching layer", "ISearchCache, InMemorySearchCache with TTL", 1),
    ("Infrastructure - Rate limiting", "TokenBucketRateLimiter per provider", 2),
    ("Google Search Provider - HTML scraping", "GoogleSearchProvider with AngleSharp", 1),
    ("Bing Search Provider - HTML scraping", "BingSearchProvider with AngleSharp", 2),
    ("DuckDuckGo Search Provider", "DuckDuckGoProvider implementation", 3),
    ("Retry logic and Circuit Breaker", "RetryPolicy with exponential backoff", 2),
    ("SearchProviderFactory and DI setup", "Factory pattern + DI extensions", 1),
    ("Search result normalization", "URL/snippet/title normalization", 2),
    ("Comprehensive README and documentation", "Installation, usage, API docs", 3),
    ("Integration tests with live searches", "End-to-end tests with real engines", 2),
    ("Sample console application", "Interactive demo app", 3),
]

print(f"Creating {len(tasks_data)} tasks...")

for name, desc, priority in tasks_data:
    task = {
        "name": name,
        "description": desc,
        "status": "backlog",
        "priority": priority
    }
    
    url = f"https://api.clickup.com/api/v2/list/{list_id}/task"
    r = requests.post(url, json=task, headers=headers)
    
    if r.status_code == 200:
        print(f"✓ {name}")
    else:
        print(f"✗ {name}: {r.status_code}")

print("Done!")
