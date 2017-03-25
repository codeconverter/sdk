param($CommandName)

    
$MetaData = New-Object System.Management.Automation.CommandMetaData (Get-Command $CommandName) 
$proxyBody = [System.Management.Automation.ProxyCommand]::Create($MetaData) 

"function $CommandName {
    $proxyBody 
}"
