param (
    [string]$CodeGenLog = $(if ($env:PFLCodeGenLog) { $env:PFLCodeGenLog } else { "" })
)
function Main {
    function IsPackageInstalled {
        Param ([string]$pName)
        Process {
            $result = choco list -lo | Where-object { $_.ToLower().StartsWith($pName.ToLower()) }
            if ($null -ne $result) {
                $parts = $result.Split(' ');
                return $parts[1] -eq $this.Version;
            }
            return $false;
        }
    }

    function GenerateCSharpModels {
        Param ([string]$packageName, [string]$yamlfilepath, [string]$dest)
        Process {
            Write-Output "Package name: $packageName"
            Write-Output "Yaml path: $yamlfilepath"
            Write-Output "File destination: $dest"
            $tempModelsPath = ".\src\$packageName\Models\"
            if ((Test-Path -Path $tempModelsPath) -eq $false) {
                New-Item -ItemType Directory -Path $tempModelsPath
            }
            java -jar .\swagger-codegen-cli.jar generate -i "$yamlfilepath" -l aspnetcore -Dmodels --model-package "Models" -DpackageName="$packageName" -o ".\src\"
            copy-item -Path ".\src\$packageName\Models\*" -Destination $dest -Recurse
            Remove-Item -Path ".\src\" -Recurse
        }
    }

    function GenerateTypescriptFetchModels {
        Param ([string]$yamlfilepath, [string]$dest)
        Process {
            java -jar .\swagger-codegen-cli.jar generate --model-package "Models" -i "$yamlfilepath" -l typescript-fetch -o "$dest"
            Remove-Item -Path "$dest\.swagger-*" -Recurse
            Remove-Item -Path "$dest\git_push.sh" -Recurse
            Remove-Item -Path "$dest\.gitignore" -Recurse
        }
    }

    if (IsPackageInstalled("java") -ne $true) {
        choco install jdk8
    }

    #########################################
    #### Add Model generation calls here ####

    # Googler API
    GenerateCSharpModels "Googler" "..\Googler\Googler\Controllers\googlerApi.yaml" "..\Googler\Googler\Models"
    # GenerateTypescriptFetchModels "..\Googler\Googler\Controllers\googlerApi.yaml" ".\reactapp\src\services\googler-api"
    ########################################
}

if ($CodeGenLog -ne "") {
    Main *>&1 | ForEach-Object {
        Write-Host $_   # This goes to the terminal immediatly, escaping this pipeline
        Write-Output $_ # This continues down the pipeline
    } | Out-File -Encoding UTF8 $CodeGenLog
}
else {
    Main
}
