function Method()
{
	Out-File -InputObject "My output test" -FilePath "supersecretfile.txt"
	Out-File -InputObject "My output test" -FilePath "supersecretfile.txt" -Append
}