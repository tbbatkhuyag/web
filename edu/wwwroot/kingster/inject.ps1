$dir = 'd:\www\iumc\iumc\edu\edu\wwwroot\im\max-themes.net\demos\kingster\kingster'
$files = Get-ChildItem -Path $dir -Filter '*.html' -File
foreach ($f in $files) {
    if ($f.Name -eq 'manager.html' -or $f.Name -eq 'grapesjs.html') { continue }
    $content = [System.IO.File]::ReadAllText($f.FullName)
    if (-not $content.Contains('dynamic-menu.js')) {
        $content = $content -replace '</body>', '<script src="js/dynamic-menu.js"></script></body>'
        [System.IO.File]::WriteAllText($f.FullName, $content)
    }
}
Write-Output "Done injection"
