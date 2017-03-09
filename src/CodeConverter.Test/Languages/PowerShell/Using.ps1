function Method
{
	[AesManaged]$aes = $null;
	try
	{
		$aes = (New-Object -TypeName AesManaged)
		$aes.Padding = $PaddingMode.PKCS7
        $aes.KeySize = 128
        $aes.Key = $key
        $aes.IV = $IV
	}
	finally
	{
		$aes.Dispose()
	}
}