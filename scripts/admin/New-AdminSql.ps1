param(
    [Parameter(Mandatory = $true)]
    [string] $Email,

    [string] $OutputPath = "scripts/admin/create-admin.local.sql"
)

$ErrorActionPreference = "Stop"

$templatePath = "scripts/admin/create-admin.sql.template"
$hashToolDirectory = ".tmp/admin-hash"
$hashToolProject = Join-Path $hashToolDirectory "AdminHash.csproj"
$hashToolProgram = Join-Path $hashToolDirectory "Program.cs"

if (-not (Test-Path $templatePath)) {
    throw "Template file not found: $templatePath"
}

$password = Read-Host "Admin password" -AsSecureString
$bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($password)

try {
    $plainPassword = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)

    New-Item -ItemType Directory -Force -Path $hashToolDirectory | Out-Null

    Set-Content -Path $hashToolProject -Encoding UTF8 -Value @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.2.0" />
  </ItemGroup>
</Project>
'@

    Set-Content -Path $hashToolProgram -Encoding UTF8 -Value @'
var password = Console.In.ReadToEnd().TrimEnd('\r', '\n');

if (string.IsNullOrWhiteSpace(password))
{
    Console.Error.WriteLine("Password is required.");
    return 1;
}

Console.Write(BCrypt.Net.BCrypt.HashPassword(password));
return 0;
'@

    dotnet restore $hashToolProject *> $null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to restore the temporary BCrypt hash tool."
    }

    $passwordHash = $plainPassword | dotnet run --project $hashToolProject --configuration Release --no-restore --nologo
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($passwordHash)) {
        throw "Failed to generate BCrypt password hash."
    }
}
finally {
    if ($bstr -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    }
}

$adminId = [guid]::NewGuid().ToString()
$normalizedEmail = $Email.Trim().ToLowerInvariant()

$sql = Get-Content $templatePath -Raw
$sql = $sql.Replace("{{ADMIN_ID}}", $adminId)
$sql = $sql.Replace("{{ADMIN_EMAIL}}", $normalizedEmail.Replace("'", "''"))
$sql = $sql.Replace("{{PASSWORD_HASH}}", $passwordHash.Replace("'", "''"))

$outputDirectory = Split-Path $OutputPath -Parent
if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) {
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
}

Set-Content -Path $OutputPath -Value $sql -Encoding UTF8

Write-Host "Admin SQL created: $OutputPath"
Write-Host "Admin email: $normalizedEmail"
