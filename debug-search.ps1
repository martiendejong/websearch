# Debug script to fetch and inspect Google SERP HTML
$query = "marcello valsuani"
$url = "https://www.google.com/search?q=" + [System.Web.HttpUtility]::UrlEncode($query)

$headers = @{
    "User-Agent" = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36"
    "Accept" = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
    "Accept-Language" = "en-US,en;q=0.9"
}

Write-Host "Fetching: $url"
$response = Invoke-WebRequest -Uri $url -Headers $headers -UseBasicParsing

$html = $response.Content
$html | Out-File "debug-serp.html" -Encoding UTF8

Write-Host "`nHTML saved to debug-serp.html ($($html.Length) bytes)"
Write-Host "`nSearching for common selectors..."

# Look for result containers
if ($html -match '<div[^>]*class="[^"]*\bg\b[^"]*"') { Write-Host "✓ Found div.g" } else { Write-Host "✗ No div.g" }
if ($html -match '<div[^>]*data-sokoban-container') { Write-Host "✓ Found data-sokoban-container" } else { Write-Host "✗ No data-sokoban-container" }
if ($html -match '<h3') { Write-Host "✓ Found h3 elements" } else { Write-Host "✗ No h3" }
if ($html -match 'class="[^"]*VwiC3b[^"]*"') { Write-Host "✓ Found VwiC3b snippet class" } else { Write-Host "✗ No VwiC3b" }

# Look for alternative patterns
Write-Host "`nLooking for alternative patterns..."
if ($html -match '<div[^>]*id="search"') { Write-Host "✓ Found #search container" }
if ($html -match '<div[^>]*id="rso"') { Write-Host "✓ Found #rso (results)" }
if ($html -match '<a[^>]*href="/url\?q=') { Write-Host "✓ Found /url?q= links" }
if ($html -match 'jsname="UWckNb"') { Write-Host "✓ Found jsname=UWckNb (modern pattern)" }

Write-Host "`nInspect debug-serp.html to see current structure!"
