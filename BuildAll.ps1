$scriptPath = Split-Path $MyInvocation.MyCommand.Path
Set-Location $scriptPath
$scriptPath
dotnet restore simulator.uwp.sln

dotnet build simulator.uwp.sln --configuration release --runtime x86
dotnet build simulator.uwp.sln --configuration release --runtime x64
dotnet build simulator.uwp.sln --configuration release --runtime ARM
dotnet build simulator.uwp.sln --configuration release --runtime Release 