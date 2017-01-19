$AndroidToolPath = "${env:ProgramFiles(x86)}\Android\android-sdk\tools\android" 
#$AndroidToolPath = "$env:localappdata\Android\android-sdk\tools\android"

environment:
  ANDROID_HOME: "${env:ProgramFiles(x86)}\Android\android-sdk" 
  JAVA_HOME: "C:\\Program Files\\Java\\jdk1.8.0"

Function Get-AndroidSDKs() { 
    $output = & $AndroidToolPath list sdk --all 
    $sdks = $output |% { 
        if ($_ -match '(?<index>\d+)- (?<sdk>.+), revision (?<revision>[\d\.]+)') { 
            $sdk = New-Object PSObject 
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Index -Value $Matches.index 
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Name -Value $Matches.sdk 
            Add-Member -InputObject $sdk -MemberType NoteProperty -Name Revision -Value $Matches.revision 
            $sdk 
        } 
    } 
    $sdks 
}

Function Install-AndroidSDK() { 
    [CmdletBinding()] 
    Param( 
        [Parameter(Mandatory=$true, Position=0)] 
        [PSObject[]]$sdks 
    )

    $sdkIndexes = $sdks |% { $_.Index } 
    $sdkIndexArgument = [string]::Join(',', $sdkIndexes) 
    Echo 'y' | & $AndroidToolPath update sdk -u -a -t $sdkIndexArgument 
}

install:
  - echo y | "%ANDROID_HOME%\tools\android.bat" update sdk -u -a -t tools
  - echo y | "%ANDROID_HOME%\tools\android.bat" update sdk -u -a -t platform-tools
  - echo y | "%ANDROID_HOME%\tools\android.bat" update sdk -u -a -t build-tools-25.0.1
  - echo y | "%ANDROID_HOME%\tools\android.bat" update sdk -u -a -t android-24

$sdks = Get-AndroidSDKs |? { $_.name -like 'sdk platform*API 24*' -or $_.name -like 'google apis*api 24' } 
Install-AndroidSDK -sdks $sdks