function AbortSystemShutdown
{
    param([string]$lpMachineName)

    Add-Type '
        using System;
        using System.Runtime.InteropServices;
        public static class advapi32 {
            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool AbortSystemShutdown(string lpMachineName);
        }
    '

    [advapi32]::AbortSystemShutdown($lpMachineName)
}