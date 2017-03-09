[DllImport("advapi32.dll", SetLastError = true)]
static extern bool AbortSystemShutdown(string lpMachineName);