param(
    [string]$BaseUrl = "http://localhost:5080",
    [string]$Mysql = "C:\Program Files\MySQL\MySQL Server 26.10\bin\mysql.exe"
)

$ErrorActionPreference = "Stop"
$script:suffix = Get-Date -Format "HHmmss"
$script:u1 = "trainer01_$suffix"
$script:u2 = "trainer02_$suffix"
$script:results = New-Object System.Collections.Generic.List[string]

function Invoke-Api {
    param([string]$Method, [string]$Path, $Body, [string]$Token)

    $headers = @{}
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

    $options = @{
        Uri = "$BaseUrl$Path"
        Method = $Method
        Headers = $headers
        UseBasicParsing = $true
        TimeoutSec = 20
    }

    if ($null -ne $Body) {
        $options["ContentType"] = "application/json"
        $options["Body"] = ($Body | ConvertTo-Json -Compress -Depth 6)
    }

    try {
        $response = Invoke-WebRequest @options
        return @{ Code = [int]$response.StatusCode; Body = $response.Content }
    }
    catch {
        $response = $_.Exception.Response
        if ($null -ne $response) {
            $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
            return @{ Code = [int]$response.StatusCode; Body = $reader.ReadToEnd() }
        }
        return @{ Code = 0; Body = $_.Exception.Message }
    }
}

function Check {
    param([string]$Id, [int]$Expected, $Result, [string]$Note)

    $ok = ($Result.Code -eq $Expected)
    $flag = if ($ok) { "PASS" } else { "FAIL" }
    $detail = if ($Note) { " - $Note" } else { "" }
    $script:results.Add("$Id $flag expected=$Expected actual=$($Result.Code)$detail")
}

$r1 = Invoke-Api POST "/api/auth/register" @{ username = $u1; password = "123456" }
Check "TC01" 201 $r1 "register $u1"

$r2 = Invoke-Api POST "/api/auth/register" @{ username = $u1; password = "123456" }
Check "TC02" 409 $r2 "duplicate username"

$r3 = Invoke-Api POST "/api/auth/register" @{ username = "short_$suffix"; password = "123" }
Check "TC03" 400 $r3 "password 3 chars"

$r4 = Invoke-Api POST "/api/auth/login" @{ username = $u1; password = "wrongpass" }
Check "TC04" 401 $r4 "wrong password"

$r5 = Invoke-Api POST "/api/auth/login" @{ username = $u1; password = "123456" }
$token = ""
if ($r5.Code -eq 200) {
    $parsed = $r5.Body | ConvertFrom-Json
    $token = $parsed.token
}
$tc05 = Check "TC05" 200 $r5 "login, token length=$($token.Length)"

$r6 = Invoke-Api POST "/api/characters" @{ name = "FireFox"; class = "Fire" }
Check "TC06" 401 $r6 "no token"

$tc07 = if ($token.Length -gt 20) { "TC07 PASS token available for Authorize" } else { "TC07 FAIL no token" }
$script:results.Add($tc07)

$r8 = Invoke-Api POST "/api/characters" @{ name = "FireFox"; class = "Fire" } $token
$petId = 0
if ($r8.Code -eq 201) {
    $parsed = $r8.Body | ConvertFrom-Json
    $petId = $parsed.id
}
Check "TC08" 201 $r8 "created pet id=$petId"

$r9 = Invoke-Api POST "/api/characters" @{ name = ""; class = "Water" } $token
Check "TC09" 400 $r9 "empty name"

$r10 = Invoke-Api PUT "/api/characters/$petId" @{ name = "FireFox"; class = "Fire"; level = 5; hp = 250 } $token
Check "TC10" 200 $r10 "level 5, hp 250"

$r11 = Invoke-Api PUT "/api/characters/$petId" @{ name = "FireFox"; class = "Fire"; level = 0; hp = 250 } $token
Check "TC11" 400 $r11 "level 0"

$r12reg = Invoke-Api POST "/api/auth/register" @{ username = $u2; password = "123456" }
$r12login = Invoke-Api POST "/api/auth/login" @{ username = $u2; password = "123456" }
$token2 = ""
if ($r12login.Code -eq 200) {
    $parsed = $r12login.Body | ConvertFrom-Json
    $token2 = $parsed.token
}
$r12pet = Invoke-Api POST "/api/characters" @{ name = "Aqua"; class = "Water" } $token2
$otherPetId = 0
if ($r12pet.Code -eq 201) {
    $parsed = $r12pet.Body | ConvertFrom-Json
    $otherPetId = $parsed.id
}
$r12 = Invoke-Api PUT "/api/characters/$otherPetId" @{ name = "Hacked"; class = "Fire"; level = 9; hp = 999 } $token
Check "TC12" 404 $r12 "other owner's pet id=$otherPetId"

$rGet = Invoke-Api GET "/api/characters" $null $token
$script:results.Add("EXTRA GET expected=200 actual=$($rGet.Code)")
$script:results | ForEach-Object { Write-Output $_ }

$all = $script:results | Where-Object { $_ -match "FAIL" }
if ($all) {
    Write-Output "ASM1 RESULT: FAIL"
    exit 1
}

Write-Output "ASM1 RESULT: ALL PASS"

if (Test-Path $Mysql) {
    $ErrorActionPreference = "Continue"
    Write-Output "--- MySQL check ---"
    & $Mysql -uroot -p123456 -N -e "SELECT Username, LEFT(PasswordHash, 6) FROM game_pet_adventure.Accounts ORDER BY Id DESC LIMIT 3;" 2>$null
    & $Mysql -uroot -p123456 -N -e "SELECT Id, Name, Class, Level, Hp, AccountId FROM game_pet_adventure.Characters ORDER BY Id DESC LIMIT 5;" 2>$null
}
