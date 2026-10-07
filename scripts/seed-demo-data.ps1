<#
.SYNOPSIS
    Seeds realistic demo expenses into a running ExpenseTracker API.

.DESCRIPTION
    Logs in (registering the demo user first if needed), reads the user's categories,
    and creates expenses spread over the last few months through the public API.
    Everything goes through the normal endpoints, so validation and per-user
    ownership apply exactly as they would for a real client.

    The data is generated from a fixed random seed, so every run produces the same
    set of expenses. Running it twice creates the set twice.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File scripts/seed-demo-data.ps1

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File scripts/seed-demo-data.ps1 -Email bob@example.com -Count 15
#>
param(
    [string]$BaseUrl = "https://localhost:7093",
    [string]$Email = "alice@example.com",
    [string]$Password = "Passw0rd!",   # demo account for local development only
    [int]$Count = 40,
    [int]$DaysBack = 90
)

$ErrorActionPreference = "Stop"
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Invoke-Api {
    param([string]$Method, [string]$Path, $Body, [string]$Token)

    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

    $params = @{
        Uri         = "$BaseUrl$Path"
        Method      = $Method
        Headers     = $headers
        ContentType = "application/json"
    }
    if ($null -ne $Body) { $params["Body"] = ($Body | ConvertTo-Json -Depth 5) }

    Invoke-RestMethod @params
}

function Get-StatusCode($ErrorRecord) {
    if ($ErrorRecord.Exception.Response) { return [int]$ErrorRecord.Exception.Response.StatusCode }
    return $null
}

# -- 1. Log in (register first if the account doesn't exist yet) --------------
$credentials = @{ email = $Email; password = $Password }

try {
    $auth = Invoke-Api -Method Post -Path "/api/auth/login" -Body $credentials
    Write-Host "Logged in as $Email"
}
catch {
    if ((Get-StatusCode $_) -ne 401) { throw }

    Write-Host "Login failed, trying to register $Email ..."
    try {
        $auth = Invoke-Api -Method Post -Path "/api/auth/register" -Body $credentials
        Write-Host "Registered $Email"
    }
    catch {
        Write-Error ("Could not log in or register $Email. If the account is locked out, wait 5 minutes. " +
                     "Details: " + $_.ErrorDetails.Message)
        exit 1
    }
}

$token = $auth.accessToken

# -- 2. Map category names to ids ----------------------------------------------
$categories = @{}
foreach ($category in (Invoke-Api -Method Get -Path "/api/categories" -Token $token)) {
    $categories[$category.name] = $category.id
}

# Description, minimum amount, maximum amount (RM) for each default category
$templates = @{
    "Food"          = @(@("Nasi lemak", 5, 15), @("Roti canai and teh tarik", 4, 10), @("Chicken rice", 8, 16),
                        @("Mamak dinner", 12, 35), @("Groceries", 60, 250), @("Lunch with colleagues", 20, 60))
    "Transport"     = @(@("Grab ride", 8, 40), @("Petrol", 50, 120), @("Toll", 3, 20),
                        @("LRT reload", 20, 50), @("Parking", 3, 15))
    "Bills"         = @(@("Electricity bill", 80, 250), @("Water bill", 15, 45), @("Internet bill", 99, 199),
                        @("Phone bill", 30, 80))
    "Shopping"      = @(@("Online order", 25, 200), @("New shirt", 40, 150), @("Household items", 20, 90))
    "Entertainment" = @(@("Netflix subscription", 45, 55), @("Cinema tickets", 20, 50), @("Spotify subscription", 15, 25),
                        @("Bowling night", 30, 80))
    "Health"        = @(@("Pharmacy", 10, 60), @("Clinic visit", 40, 120), @("Gym membership", 100, 180))
}

$paymentMethods = @("Cash", "DebitCard", "CreditCard", "BankTransfer", "EWallet")

$usable = @($templates.Keys | Where-Object { $categories.ContainsKey($_) } | Sort-Object)
if ($usable.Count -eq 0) {
    Write-Error "None of the default categories (Food, Transport, ...) exist for $Email."
    exit 1
}

# -- 3. Create the expenses ----------------------------------------------------
$random = New-Object System.Random 42   # fixed seed: the same data every run
$today = (Get-Date).Date
$created = 0
$failed = 0

for ($i = 1; $i -le $Count; $i++) {
    $categoryName = $usable[$random.Next($usable.Count)]
    $options = $templates[$categoryName]
    $template = $options[$random.Next($options.Count)]

    $minimum = [double]$template[1]
    $maximum = [double]$template[2]
    $amount = [decimal][math]::Round($minimum + $random.NextDouble() * ($maximum - $minimum), 2)

    $date = $today.AddDays(-$random.Next($DaysBack + 1)).ToString("yyyy-MM-dd")

    $body = @{
        categoryId    = $categories[$categoryName]
        amount        = $amount
        description   = $template[0]
        date          = $date
        paymentMethod = $paymentMethods[$random.Next($paymentMethods.Count)]
        notes         = $(if ($random.Next(4) -eq 0) { "Seeded demo data" } else { $null })
    }

    try {
        Invoke-Api -Method Post -Path "/api/expenses" -Body $body -Token $token | Out-Null
        $created++
        Write-Host ("  [{0,2}/{1}] {2}  RM{3,7:N2}  {4,-14} {5}" -f $i, $Count, $date, $amount, $categoryName, $template[0])
    }
    catch {
        $failed++
        Write-Warning ("[{0}] Failed: {1}" -f $i, $_.ErrorDetails.Message)
    }
}

Write-Host ""
Write-Host "Done: $created created, $failed failed (user: $Email)"
