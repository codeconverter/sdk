function Get-Process {
    [CmdletBinding(DefaultParameterSetName='Name', HelpUri='http://go.microsoft.com/fwlink/?LinkID=113324', RemotingCapability='SupportedByCommand')]
param(
    [Parameter(ParameterSetName='NameWithUserName', Position=0, ValueFromPipelineByPropertyName=$true)]
    [Parameter(ParameterSetName='Name', Position=0, ValueFromPipelineByPropertyName=$true)]
    [Alias('ProcessName')]
    [ValidateNotNullOrEmpty()]
    [string[]]
    ${Name},

    [Parameter(ParameterSetName='Id', Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
    [Parameter(ParameterSetName='IdWithUserName', Mandatory=$true, ValueFromPipelineByPropertyName=$true)]
    [Alias('PID')]
    [int[]]
    ${Id},

    [Parameter(ParameterSetName='InputObject', Mandatory=$true, ValueFromPipeline=$true)]
    [Parameter(ParameterSetName='InputObjectWithUserName', Mandatory=$true, ValueFromPipeline=$true)]
    [System.Diagnostics.Process[]]
    ${InputObject},

    [Parameter(ParameterSetName='IdWithUserName', Mandatory=$true)]
    [Parameter(ParameterSetName='NameWithUserName', Mandatory=$true)]
    [Parameter(ParameterSetName='InputObjectWithUserName', Mandatory=$true)]
    [switch]
    ${IncludeUserName},

    [Parameter(ParameterSetName='Id', ValueFromPipelineByPropertyName=$true)]
    [Parameter(ParameterSetName='Name', ValueFromPipelineByPropertyName=$true)]
    [Parameter(ParameterSetName='InputObject', ValueFromPipelineByPropertyName=$true)]
    [Alias('Cn')]
    [ValidateNotNullOrEmpty()]
    [string[]]
    ${ComputerName},

    [Parameter(ParameterSetName='Name')]
    [Parameter(ParameterSetName='Id')]
    [Parameter(ParameterSetName='InputObject')]
    [ValidateNotNull()]
    [switch]
    ${Module},

    [Parameter(ParameterSetName='Name')]
    [Parameter(ParameterSetName='Id')]
    [Parameter(ParameterSetName='InputObject')]
    [Alias('FV','FVI')]
    [ValidateNotNull()]
    [switch]
    ${FileVersionInfo})

begin
{
    try {
        $outBuffer = $null
        if ($PSBoundParameters.TryGetValue('OutBuffer', [ref]$outBuffer))
        {
            $PSBoundParameters['OutBuffer'] = 1
        }
        $wrappedCmd = $ExecutionContext.InvokeCommand.GetCommand('Microsoft.PowerShell.Management\Get-Process', [System.Management.Automation.CommandTypes]::Cmdlet)
        $scriptCmd = {& $wrappedCmd @PSBoundParameters }
        $steppablePipeline = $scriptCmd.GetSteppablePipeline($myInvocation.CommandOrigin)
        $steppablePipeline.Begin($PSCmdlet)
    } catch {
        throw
    }
}

process
{
    try {
        $steppablePipeline.Process($_)
    } catch {
        throw
    }
}

end
{
    try {
        $steppablePipeline.End()
    } catch {
        throw
    }
}
<#

.ForwardHelpTargetName Microsoft.PowerShell.Management\Get-Process
.ForwardHelpCategory Cmdlet

#>
 
}