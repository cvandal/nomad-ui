function Start-Build ()
{
    param (
        [Parameter(Mandatory=$true)]
        [string]$ImageName,
        [Parameter(Mandatory=$true)]
        [string]$ImageTag
    )

    $workingDir = Get-Location
    $app = "Nomad"

    # Cleanup
    Get-ChildItem -Path ".\$app\bin" -Recurse | Remove-Item -Recurse -Force
    Get-ChildItem -Path ".\$app\obj" -Recurse | Remove-Item -Recurse -Force
    Remove-Item -Path ".\$app\bin" -Force
    Remove-Item -Path ".\$app\obj" -Force

    # Restore packages, build the application, and publish the application
    Start-Process -FilePath "dotnet" -ArgumentList "restore" -NoNewWindow -PassThru -Wait
    Start-Process -FilePath "dotnet" -ArgumentList "build" -NoNewWindow -PassThru -Wait
    Start-Process -FilePath "dotnet" -ArgumentList "publish" -NoNewWindow -PassThru -Wait
    
    # Create and push the Docker image
    Set-Location -Path ".\Nomad\bin\Debug\netcoreapp1.1\publish\"
    $dockerfile = @"
FROM microsoft/dotnet

COPY . /app

WORKDIR /app

ENTRYPOINT ["dotnet", "Nomad.dll"]
"@
    New-Item -Name "Dockerfile" -Type File -Value $dockerfile -Force
    Start-Process -FilePath "docker" -ArgumentList "build -t $($ImageName):$($ImageTag) ." -NoNewWindow -PassThru -Wait
    Start-Process -FilePath "docker" -ArgumentList "build -t $($ImageName):latest ." -NoNewWindow -PassThru -Wait
    Start-Process -FilePath "docker" -ArgumentList "push $($ImageName):$($ImageTag)" -NoNewWindow -PassThru -Wait
    Start-Process -FilePath "docker" -ArgumentList "push $($ImageName):latest" -NoNewWindow -PassThru -Wait
    Set-Location -Path $workingDir
}

Start-Build
