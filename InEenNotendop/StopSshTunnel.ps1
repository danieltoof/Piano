# Find the plink.exe process by name

$plinkProcess = Get-Process -Name plink -ErrorAction SilentlyContinue 

if ($plinkProcess) {
    # Check if the process was found
    Write-Host "Stopping plink process..."

    # Kill the process
    $plinkProcess | Stop-Process -Force
} else {
    Write-Host "plink process not found."
}
