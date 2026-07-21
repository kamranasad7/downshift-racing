$ErrorActionPreference = "Stop"
$proj = $PSScriptRoot
$editorRoot = "C:\Program Files\Unity\Hub\Editor"
$unity = Get-ChildItem $editorRoot -Directory |
    Where-Object Name -like "6000.3.*" |
    Sort-Object Name -Descending | Select-Object -First 1
if (-not $unity) { throw "No Unity 6000.3.x found under $editorRoot" }
$results = Join-Path $proj "TestResults\playmode.xml"
New-Item -ItemType Directory -Force (Split-Path $results) | Out-Null
& "$($unity.FullName)\Editor\Unity.exe" -batchmode -projectPath $proj `
    -runTests -testPlatform PlayMode -testResults $results `
    -logFile (Join-Path $proj "TestResults\playmode.log") | Out-Null
[xml]$xml = Get-Content $results
$run = $xml."test-run"
Write-Host ("Tests: {0}  Passed: {1}  Failed: {2}" -f $run.total, $run.passed, $run.failed)
if ([int]$run.failed -gt 0) {
    $xml.SelectNodes("//test-case[@result='Failed']") | ForEach-Object {
        Write-Host ("FAILED: " + $_.fullname)
        Write-Host ($_.failure.message."#cdata-section")
    }
    exit 1
}
